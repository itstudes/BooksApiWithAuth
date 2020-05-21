using Newtonsoft.Json;

namespace BooksApiWithAuth.Models
{
    public class LoginModel
    {
        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("PasswordRaw")]
        public string PasswordRaw { get; set; }
    }
}