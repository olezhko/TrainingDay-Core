using TrainingDay.Common.Communication;

namespace TrainingDay.Web.Services.Rabbit
{
    public class RabbitMessage
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public string Token { get; set; }
        public PushNotificationItem Data { get; set; }
    }
}
