using BooksApiWithAuth.Models;
using System.Security.Cryptography;
using System.Text;

namespace BooksApiWithAuth.Helpers
{
    public class AuthenticationHelper : IAuthenticationHelper
    {
        public AuthenticationHelper()
        {
            //nothing here
        }

        //public functions
        public bool ValidatePassword(ApiUser user, string password)
        {
            //get user salt value
            string userKnownSalt = user.PasswordSalt;

            //get passwordHash with submitted password
            string passwordAttempt = this.GetPasswordHashed(password, userKnownSalt);

            //compare the passwordAttempt with the saved password
            bool success = string.Equals(passwordAttempt, user.PasswordHash);

            return success;
        }

        public string GetPasswordSalt()
        {
            bool success = false;

            //generate random 256byte salt
            byte[] dataBytes = new byte[256];
            RNGCryptoServiceProvider cryptographicRandomNumberGenerator = new RNGCryptoServiceProvider();
            cryptographicRandomNumberGenerator.GetBytes(dataBytes);
            string randomDataString = Encoding.Unicode.GetString(dataBytes);

            //hash random salt data using SHA256
            string saltHash = this.ComputeSHA256(randomDataString);

            return saltHash;
        }

        public string GetPasswordHashed(string passwordRaw, string passwordSalt)
        {
            string passwordHash = "";

            //add salt
            string passwordWithSalt = passwordSalt + passwordRaw;

            //get SHA256
            passwordHash = this.ComputeSHA256(passwordWithSalt);

            return passwordHash;
        }

        //private functions
        private string ComputeSHA256(string rawData)
        {
            string returnData = "";

            // Create a SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                returnData = builder.ToString();
            }

            return returnData;
        }
    }

    public interface IAuthenticationHelper
    {
        bool ValidatePassword(ApiUser user, string password);

        string GetPasswordSalt();

        string GetPasswordHashed(string passwordRaw, string passwordSalt);
    }
}