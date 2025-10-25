namespace ITNBaja.Models
{
    public class AdminCredentialsOptions : List<AdminUser>
    {
    }
    
    public class AdminUser
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}