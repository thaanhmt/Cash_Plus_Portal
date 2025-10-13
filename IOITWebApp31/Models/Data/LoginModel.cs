namespace IOITWebApp31.Models
{
    public class LoginModel
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class FacebookSetting
    {
        public string FaceAppId { get; set; }
        public string FaceAppSecret { get; set; }
        public string URL_AUTHOR { get; set; }
        public string URL_GET_TOKEN { get; set; }
        public string URL_GET_USERINFO { get; set; }
        public string URL_GET_BUSINESSAPP { get; set; }
        public string url_return { get; set; }
    }

    public class GoogleSetting
    {
        public string clientId { get; set; }
        public string secretKey { get; set; }
        public string urlReturn { get; set; }
    }
}