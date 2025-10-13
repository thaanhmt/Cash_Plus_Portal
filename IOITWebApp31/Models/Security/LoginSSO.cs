namespace IOITWebApp31.Models.Security
{
    public class LoginSSO
    {
    }

    public class TokenResponse
    {
        public string access_token { get; set; }
        public string session_state { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public int refresh_expires_in { get; set; }
        public string refresh_token { get; set; }
    }

    public class IntrospectionResponse
    {
        public bool active { get; set; }
        public string scope { get; set; }
        public int exp { get; set; }
        public string client_id { get; set; }
        public string sub { get; set; }
        public string preferred_username { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string session_state { get; set; }
    }

}
