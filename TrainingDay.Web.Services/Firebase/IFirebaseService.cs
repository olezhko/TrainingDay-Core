using FirebaseAdmin.Messaging;
using TrainingDay.Common;

namespace TrainingDay.Web.Services.Firebase
{
    public interface IFirebaseService
    {
        Task<FirebaseMessagingException> SendMessage(string token, string title, string body, string type, PushNotificationData pushData);
        Task SendGroupMessage(List<string> tokens, string title, string body, string type, PushNotificationData pushData);
    }
}
