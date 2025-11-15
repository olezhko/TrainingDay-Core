using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Globalization;
using TrainingDay.Common.Communication;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;
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
    public async Task DoWork(CancellationToken stoppingToken)
    {
        var dateTimeNow = DateTime.UtcNow;

        await SendWorkoutNotificationAsync(dateTimeNow, stoppingToken);
        await SendWeightNotificationAsync(dateTimeNow, stoppingToken);
    }

    private async Task SendWeightNotificationAsync(DateTime dateTimeNow, CancellationToken stoppingToken)
    {
        var itemsList = await context.MobileTokens.AsNoTracking().ToListAsync(cancellationToken: stoppingToken);
        var items = itemsList.Where(item => CheckTime(dateTimeNow, item.Zone, TimeSpan.Parse("08:00:00")));
        foreach (var contextMobileToken in items)
        {
            var locale = CultureInfo.GetCultureInfo(contextMobileToken.Language).TwoLetterISOLanguageName;

            CheckTokenByTime(contextMobileToken, dateTimeNow, out var actIsWeightNotifySend, out _);
            if (actIsWeightNotifySend)
            {
                var weightMessage = localizer[locale, "WeightMessage"];
                if (weightMessage != null)
                {
                    var item = new RabbitMessage
                    {
                        Message = weightMessage,
                        Token = contextMobileToken.Token,
                        Type = PushNotificationItem.WeightType,
                        Title = "TrainingDay",
                        Data = new PushNotificationItem()
                        {
                            Step1 = new PushNotificationItem.PushNotificationActionItem(
                                PushNotificationItem.PushNotificationDataAction.OpenPage,
                                PushNotificationItem.BodyControlPage),
                        }
                    };
                    //_messagePublisher.SendMessage(item);

                    var exc = await firebaseService.SendMessage(item.Token, item.Title, item.Message, item.Type, item.Data);
                    if (exc != null)
                    {
                        await userTokenManager.RemoveNotExistToken(contextMobileToken);
                    }
                }
            }
        }
    }

    private async Task SendWorkoutNotificationAsync(DateTime dateTimeNow, CancellationToken stoppingToken)
    {
        var itemsList = await context.MobileTokens.AsNoTracking().ToListAsync(cancellationToken: stoppingToken);
        var items = itemsList.Where(item => CheckTime(dateTimeNow, item.Zone, TimeSpan.Parse("11:00:00")));
        foreach (var contextMobileToken in items)
        {
            var locale = CultureInfo.GetCultureInfo(contextMobileToken.Language).TwoLetterISOLanguageName;

            CheckTokenByTime(contextMobileToken, dateTimeNow, out _, out var actIsWorkoutNotifySend);
            if (actIsWorkoutNotifySend)
            {
                var workoutMessage = localizer[locale, "WorkoutMessage"];
                if (workoutMessage != null)
                {
                    var item = new RabbitMessage
                    {
                        Message = workoutMessage,
                        Token = contextMobileToken.Token,
                        Type = PushNotificationItem.WorkoutType,
                        Title = "TrainingDay",
                        Data = new PushNotificationItem()
                        {
                            Step1 = new PushNotificationItem.PushNotificationActionItem(
                                PushNotificationItem.PushNotificationDataAction.OpenPage,
                                PushNotificationItem.TrainingsPage),
                        }
                    };
                    //_messagePublisher.SendMessage(item);

                    var exc = await firebaseService.SendMessage(item.Token, item.Title, item.Message, item.Type, item.Data);
                    if (exc != null)
                    {
                        await userTokenManager.RemoveNotExistToken(contextMobileToken);
                    }
                }
            }
        }
    }

    public static void CheckTokenByTime(MobileToken token, DateTime utcNow, out bool isWeightNotifySend, out bool isWorkoutNotifySend)
    {
        isWeightNotifySend = CheckIsTimePeriodPast(utcNow, token.LastBodyControlDateTime, TimeSpan.FromDays(7));
        isWorkoutNotifySend = CheckIsTimePeriodPast(utcNow, token.LastWorkoutDateTime, TimeSpan.FromDays(3));
    }

    public static bool CheckTime(DateTime currentUtcTime, string zone, TimeSpan timeofDay)
    {
        var userActualTime = currentUtcTime + TimeSpan.Parse(zone);
        var expectedTime = currentUtcTime.Date + timeofDay;

        return userActualTime.TimeOfDay - expectedTime.TimeOfDay <= TimeSpan.FromSeconds(2)
               && userActualTime.TimeOfDay - expectedTime.TimeOfDay >= TimeSpan.FromSeconds(0);
    }

    public static bool CheckIsTimePeriodPast(DateTime nowUtc, DateTime lastRecDateTimeUtc, TimeSpan daysPeriod)
    {
        return nowUtc - lastRecDateTimeUtc >= daysPeriod;
    }
}
