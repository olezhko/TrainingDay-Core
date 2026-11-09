using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Globalization;
using TrainingDay.Common.Communication;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Services.Extensions;
using TrainingDay.Web.Services.Firebase;
using TrainingDay.Web.Services.Rabbit;
using TrainingDay.Web.Services.UserTokens;

namespace TrainingDay.Web.Services.Notification;

public class ScopedProcessingService(
    TrainingDayContext context,
    IStringLocalizer localizer,
    IMessageProducer messagePublisher,
    IFirebaseService firebaseService,
    IUserTokenManager userTokenManager) : IScopedProcessingService
{
    private static readonly TimeSpan WeightNotifyTime = new(8, 0, 0);
    private static readonly TimeSpan WorkoutNotifyTime = new(11, 0, 0);
    private static readonly TimeSpan WeightPeriod = TimeSpan.FromDays(7);
    private static readonly TimeSpan WorkoutPeriod = TimeSpan.FromDays(3);
    private static readonly TimeSpan DefaultZone = new(3, 0, 0);

    public async Task DoWork(DateTime fromUtc, DateTime toUtc, CancellationToken stoppingToken)
    {
        // Zones are few, so pick the ones whose send time was reached and only load tokens from those zones.
        var zones = await context.MobileTokens.AsNoTracking()
            .Select(item => item.Zone)
            .Distinct()
            .ToListAsync(stoppingToken);

        var weightZones = zones.Where(zone => IsTimeReached(fromUtc, toUtc, ParseOrDefault(zone), WeightNotifyTime)).ToList();
        if (weightZones.Count > 0)
        {
            await SendWeightNotificationAsync(weightZones, toUtc, stoppingToken);
        }

        var workoutZones = zones.Where(zone => IsTimeReached(fromUtc, toUtc, ParseOrDefault(zone), WorkoutNotifyTime)).ToList();
        if (workoutZones.Count > 0)
        {
            await SendWorkoutNotificationAsync(workoutZones, toUtc, stoppingToken);
        }
    }

    private async Task SendWeightNotificationAsync(List<string> zones, DateTime dateTimeNow, CancellationToken stoppingToken)
    {
        var lastAllowed = dateTimeNow - WeightPeriod;
        var items = await context.MobileTokens.AsNoTracking()
            .Where(item => zones.Contains(item.Zone) && item.LastBodyControlDateTime <= lastAllowed)
            .ToListAsync(stoppingToken);

        foreach (var contextMobileToken in items)
        {
            await SendAsync(contextMobileToken, "WeightMessage", PushNotificationItem.WeightType, PushNotificationItem.BodyControlPage);
        }
    }

    private async Task SendWorkoutNotificationAsync(List<string> zones, DateTime dateTimeNow, CancellationToken stoppingToken)
    {
        var lastAllowed = dateTimeNow - WorkoutPeriod;
        var items = await context.MobileTokens.AsNoTracking()
            .Where(item => zones.Contains(item.Zone) && item.LastWorkoutDateTime <= lastAllowed)
            .ToListAsync(stoppingToken);

        foreach (var contextMobileToken in items)
        {
            await SendAsync(contextMobileToken, "WorkoutMessage", PushNotificationItem.WorkoutType, PushNotificationItem.TrainingsPage);
        }
    }

    private async Task SendAsync(MobileToken contextMobileToken, string messageKey, string type, string page)
    {
        var locale = CultureInfo.GetCultureInfo(contextMobileToken.Language).TwoLetterISOLanguageName;
        var message = localizer[locale, messageKey];
        if (message == null)
        {
            return;
        }

        var item = new RabbitMessage
        {
            Message = message,
            Token = contextMobileToken.Token,
            Type = type,
            Title = "TrainingDay",
            Data = new PushNotificationItem()
            {
                Step1 = new PushNotificationItem.PushNotificationActionItem(
                    PushNotificationItem.PushNotificationDataAction.OpenPage,
                    page),
            }
        };
        //_messagePublisher.SendMessage(item);

        var exc = await firebaseService.SendMessage(item.Token, item.Title, item.Message, item.Type, item.Data);

        // Only drop tokens Firebase says are gone; transient errors (quota, unavailable) must not remove them.
        if (exc?.MessagingErrorCode is MessagingErrorCode.Unregistered or MessagingErrorCode.SenderIdMismatch)
        {
            await userTokenManager.RemoveNotExistToken(contextMobileToken);
        }
    }

    // The app sends signed offsets ("+03:00"); unsigned values ("03:00") may still be stored from older versions.
    public static TimeSpan ParseOrDefault(string zone)
    {
        if (DateTimeExtensions.TryParseZone(zone, out var result) || TimeSpan.TryParse(zone, out result))
        {
            return result;
        }

        return DefaultZone;
    }

    /// <summary>
    /// True when <paramref name="timeOfDay"/> in the given UTC offset fell within (fromUtc, toUtc].
    /// The window must be shorter than a day.
    /// </summary>
    public static bool IsTimeReached(DateTime fromUtc, DateTime toUtc, TimeSpan zone, TimeSpan timeOfDay)
    {
        var localTo = toUtc + zone;
        var target = localTo.Date + timeOfDay;
        if (target > localTo)
        {
            target = target.AddDays(-1);
        }

        return target > fromUtc + zone;
    }
}
