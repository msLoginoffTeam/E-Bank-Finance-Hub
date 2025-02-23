namespace UserApi.Data.DTOs.Responses
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public TokenResponse(string AccessToken, string RefreshToken)
        {
            this.AccessToken = AccessToken;
            this.RefreshToken = RefreshToken;
        }
    }
}
