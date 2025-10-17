namespace WebAPI_Manasa.Models.Controllers
{
    public class RefreshTokenModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}

