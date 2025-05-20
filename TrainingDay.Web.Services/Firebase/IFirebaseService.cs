using FirebaseAdmin.Messaging;
using TrainingDay.Common.Communication;

namespace TrainingDay.Web.Services.Firebase
{
    public interface IFirebaseService
    {
        Task<FirebaseMessagingException> SendMessage(string token, string title, string body, string type, PushNotificationItem pushData);
        Task SendGroupMessage(List<string> tokens, string title, string body, string type, PushNotificationItem pushData);
    }
}
