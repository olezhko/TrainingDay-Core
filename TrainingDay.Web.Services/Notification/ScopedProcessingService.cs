using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections;
using System.Globalization;
using TrainingDay.Common;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Services.Firebase;
using TrainingDay.Web.Services.Rabbit;
using TrainingDay.Web.Services.UserTokens;

namespace TrainingDay.Web.Services.Notification;

public class ScopedProcessingService : IScopedProcessingService
{
    private readonly TrainingDayContext _context;
    private IStringLocalizer _localizer;
    private readonly IMessageProducer _messagePublisher;
    private readonly IFirebaseService _firebaseService;
    private IUserTokenManager _userTokenManager;

    public ScopedProcessingService(TrainingDayContext context, IStringLocalizer localizer,
        IMessageProducer messagePublisher, IFirebaseService firebaseService, IUserTokenManager userTokenManager)
    {
        _localizer = localizer;
        _context = context;
        _messagePublisher = messagePublisher;
        _firebaseService = firebaseService;
        _userTokenManager = userTokenManager;
    }

    public async Task DoWork(CancellationToken stoppingToken)
    {
        var dateTimeNow = DateTime.UtcNow;

        await SendWorkoutNotification(dateTimeNow, stoppingToken);
        await SendWeightNotification(dateTimeNow, stoppingToken);
        await SendAlarmNotification(dateTimeNow, stoppingToken);
    }

    private async Task SendWeightNotification(DateTime dateTimeNow, CancellationToken stoppingToken)
    {
        var itemsList = await _context.MobileTokens.AsNoTracking().ToListAsync(cancellationToken: stoppingToken);
        var items = itemsList.Where(item => CheckTime(dateTimeNow, item.Zone, TimeSpan.Parse("08:00:00")));
        foreach (var contextMobileToken in items)
        {
            var locale = CultureInfo.GetCultureInfo(contextMobileToken.Language).TwoLetterISOLanguageName;

            CheckTokenByTime(contextMobileToken, dateTimeNow, out var actIsWeightNotifySend, out _);
            if (actIsWeightNotifySend)
            {
                var weightMessage = _localizer[locale, "WeightMessage"];
                if (weightMessage != null)
                {
                    var item = new RabbitMessage();
                    item.Message = weightMessage;
                    item.Token = contextMobileToken.Token;
                    item.Type = PushNotificationData.WeightType;
                    item.Title = "TrainingDay";
                    item.Data = new PushNotificationData()
                    {
                        Step1 = new PushNotificationData.PushNotificationActionItem(
                            PushNotificationData.PushNotificationDataAction.OpenPage,
                            PushNotificationData.BodyControlPage),
                    };
                    //_messagePublisher.SendMessage(item);

                    var exc = await _firebaseService.SendMessage(item.Token, item.Title, item.Message, item.Type, item.Data);
                    if (exc != null)
                    {
                        await _userTokenManager.RemoveNotExistToken(contextMobileToken);
                    }
                }
            }
        }
    }

    private async Task SendWorkoutNotification(DateTime dateTimeNow, CancellationToken stoppingToken)
    {
        var itemsList = await _context.MobileTokens.AsNoTracking().ToListAsync(cancellationToken: stoppingToken);
        var items = itemsList.Where(item => CheckTime(dateTimeNow, item.Zone, TimeSpan.Parse("11:00:00")));
        foreach (var contextMobileToken in items)
        {
            var locale = CultureInfo.GetCultureInfo(contextMobileToken.Language).TwoLetterISOLanguageName;

            CheckTokenByTime(contextMobileToken, dateTimeNow, out _, out var actIsWorkoutNotifySend);
            if (actIsWorkoutNotifySend)
            {
                var workoutMessage = _localizer[locale, "WorkoutMessage"];
                if (workoutMessage != null)
                {
                    var item = new RabbitMessage();
                    item.Message = workoutMessage;
                    item.Token = contextMobileToken.Token;
                    item.Type = PushNotificationData.WorkoutType;
                    item.Title = "TrainingDay";
                    item.Data = new PushNotificationData()
                    {
                        Step1 = new PushNotificationData.PushNotificationActionItem(
                            PushNotificationData.PushNotificationDataAction.OpenPage,
                            PushNotificationData.TrainingsPage),
                    };
                    //_messagePublisher.SendMessage(item);

                    var exc = await _firebaseService.SendMessage(item.Token, item.Title, item.Message, item.Type, item.Data);
                    if (exc != null)
                    {
                        await _userTokenManager.RemoveNotExistToken(contextMobileToken);
                    }
                }
            }
        }
    }

    private async Task SendAlarmNotification(DateTime dateTimeNow, CancellationToken stoppingToken)
    {
        var alarms = await _context.UserAlarm.ToListAsync(cancellationToken: stoppingToken);
        foreach (var userAlarm in alarms)
        {
            try
            {
                MobileToken token;
                if (userAlarm.User.UserMobileToken is not null)
                {
                    var userToken = await _context.UserTokens.FirstOrDefaultAsync(a => a.UserId == userAlarm.UserId, cancellationToken: stoppingToken);
                    if (userToken == null)
                    {
                        continue;
                    }

                    token = await _context.MobileTokens.FirstOrDefaultAsync(item => item.Id == userToken.TokenId, cancellationToken: stoppingToken);
                    if (token == null)
                    {
                        continue;
                    }
                }
                else
                {
                    token = await _context.MobileTokens.SingleOrDefaultAsync(item => item.Id == userAlarm.User.UserMobileToken.TokenId, cancellationToken: stoppingToken);
                }

                var locale = CultureInfo.GetCultureInfo(token.Language).TwoLetterISOLanguageName;
                var checkDay = CheckDay(userAlarm.Days, dateTimeNow);
                if (checkDay)
                {
                    var time = CheckTime(dateTimeNow, "00:00:00", userAlarm.TimeOffset.TimeOfDay);
                    if (time)
                    {
                        var alarmMessage = _localizer[locale, "AlarmMessageFormat"];
                        if (alarmMessage != null)
                        {
                            var item = new RabbitMessage();
                            item.Message = alarmMessage;
                            item.Token = token.Token;
                            item.Title = "TrainingDay";
                            item.Type = PushNotificationData.AlarmType;
                            item.Data = new PushNotificationData()
                            {
                                Step1 = new PushNotificationData.PushNotificationActionItem(PushNotificationData.PushNotificationDataAction.OpenPage, PushNotificationData.TrainingsPage),
                                Step2 = new PushNotificationData.PushNotificationActionItem(PushNotificationData.PushNotificationDataAction.OpenItem, userAlarm.DatabaseId),
                            };
                            //_messagePublisher.SendMessage(item);

                            var exc = await _firebaseService.SendMessage(item.Token, item.Title, item.Message, item.Type, item.Data);
                            if (exc != null)
                            {
                                await _userTokenManager.RemoveNotExistToken(token);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    private static bool CheckDay(in int userAlarmDays, DateTime dateTime)
    {
        BitArray arr = new BitArray(new[] { userAlarmDays });
        return arr.Get(ConvertToSimple(dateTime.DayOfWeek));
    }

    public static int ConvertToSimple(DayOfWeek day)
    {
        switch (day)
        {
            case DayOfWeek.Friday:
                return 4;
            case DayOfWeek.Monday:
                return 0;
            case DayOfWeek.Saturday:
                return 5;
            case DayOfWeek.Sunday:
                return 6;
            case DayOfWeek.Thursday:
                return 3;
            case DayOfWeek.Tuesday:
                return 1;
            case DayOfWeek.Wednesday:
                return 2;
        }

        return 0;
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
