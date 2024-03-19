using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TrainingDay.Common;

namespace TrainingDay.Web.Services.Firebase
{
	public class FirebaseService : IFirebaseService
    {
        private ILogger<FirebaseService> _logger;

		public FirebaseService(ILogger<FirebaseService> logger)
        {
			_logger = logger;
        }

        public async Task<FirebaseMessagingException> SendMessage(string token, string title, string body, string type, PushNotificationData pushData = null)
        {
	        var data = new Dictionary<string, string>
	        {
		        {nameof(title), title},
		        {nameof(body), body},
		        {nameof(type), type},
                {nameof(pushData), JsonConvert.SerializeObject(pushData)}
            };

            try
            {
                var message = new Message()
                {
                    Notification = new Notification()
                    {
                        Title = title,
                        Body = body,
                    },
                    Token = token,
                    Data = data,
                };
                await FirebaseMessaging.DefaultInstance.SendAsync(message);

                _logger.LogInformation($"SendNotification {token} {type} {body}");
            }
            catch (FirebaseMessagingException ex)
            {
                return ex;
            }
			catch (Exception exp)
            {
                _logger.LogError(exp.ToString());
            }

            return null;
        }

        public async Task SendGroupMessage(List<string> tokens, string title, string body, string type, PushNotificationData pushData)
        {
	        var data = new Dictionary<string, string>
	        {
		        {nameof(title), title},
		        {nameof(body), body},
		        {nameof(type), type},
                {nameof(pushData), JsonConvert.SerializeObject(pushData)}
            };

	        try
	        {
		        var message = new MulticastMessage()
		        {
			        Notification = new Notification()
			        {
				        Title = title,
				        Body = body,
			        },
			        Tokens = tokens,
			        Data = data,
		        };
		        await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);

		        _logger.LogInformation($"SendNotification Group {type} {body}");
	        }
	        catch (Exception exp)
	        {
		        _logger.LogError(exp.ToString());
	        }
		}
    }
}
