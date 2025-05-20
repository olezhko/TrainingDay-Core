namespace TrainingDay.Common.Communication
{
    /// <summary>
    /// Models for connect user to Firebase Token
    /// </summary>
    public class MobileUserToken
    {
        public string Token { get; set; }
        public Guid UserId { get; set; }
    }
}
