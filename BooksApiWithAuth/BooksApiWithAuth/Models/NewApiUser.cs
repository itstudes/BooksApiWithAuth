using Newtonsoft.Json;

namespace BooksApiWithAuth.Models
{
    public class NewApiUser
    {
        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("PasswordRaw")]
        public string PasswordRaw { get; set; }

        [JsonProperty("Role")]
        public UserRoles Role { get; set; }
    }
}