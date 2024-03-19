namespace TrainingDay.Web.Services.Rabbit
{
    public interface IMessageProducer
    {
        void SendMessage<T>(T message);
    }
}
