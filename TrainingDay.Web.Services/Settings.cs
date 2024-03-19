namespace TrainingDay.Web.Services
{
	public class EmailSettings
	{
		public String PrimaryDomain { get; set; }
		public int PrimaryPort { get; set; }
		public String UsernameEmail { get; set; }
		public String UsernamePassword { get; set; }
	}

	public class YouTubeSettings
	{
		public string Key { get; set; }
		public string AppName { get; set; }
	}

	public class FirebaseSettings
	{
		public string Key { get; set; }
		public string SenderId { get; set; }
	}

	public class GoogleAuthSettings
	{
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
	}

    public class AppleAuthSettings
    {
        public string AppleClientId { get; set; }
        public string AppleKeyId { get; set; }
        public string AppleTeamId { get; set; }
    }


    public class ApiSettings
	{
		public YouTubeSettings YouTube { get; set; }
		public FirebaseSettings Firebase { get; set; }
		public GoogleAuthSettings GoogleAuth { get; set; }
        public AppleAuthSettings AppleAuth { get; set; }
    }
}
