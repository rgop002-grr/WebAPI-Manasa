namespace WebAPI_Manasa
{

    // ✅ Helper class to store active session
    public class UserSession
    {
            public string Username { get; set; }
            public string Role { get; set; }
            public string RefreshToken { get; set; }
        
    }
}
