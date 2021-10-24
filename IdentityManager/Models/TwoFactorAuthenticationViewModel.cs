namespace IdentityManager.Models
{
    public class TwoFactorAuthenticationViewModel
    {
        //used to login
        public string Code { get; set; }

        //used to register / signup
        public string Token { get; set; }

        public string QRCodeUrl { get; set; }
    }
}
