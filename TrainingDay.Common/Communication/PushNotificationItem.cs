namespace TrainingDay.Common.Communication
{
    public class PushNotificationItem
    {
        public const string AlarmType = "alarm";
        public const string WeightType = "weight";
        public const string WorkoutType = "workout";
        public const string BlogType = "blog";

        public const string TrainingsPage = "trainings";
        public const string BodyControlPage = "body";
        public const string BlogsPage = "blogs";
        public enum PushNotificationDataAction
        {
            None = 0,
            OpenPage,
            OpenItem,
        }

        public class PushNotificationActionItem
        {
            public PushNotificationDataAction ActionType {
                get;
                set;
            }

            public object Argument { get; set; }

            public PushNotificationActionItem(PushNotificationDataAction actionType, object argument)
            {
                Argument = argument;
                ActionType = actionType;
            }
        }

        public PushNotificationActionItem Step1 { get; set; }
        public PushNotificationActionItem Step2 { get; set; }
    }
}
