using BooksApiWithAuth.Helpers;
using BooksApiWithAuth.Models;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace BooksApiWithAuth.Services
{
    public class ApiUserService
    {
        private readonly IMongoCollection<ApiUser> _users;
        private readonly string _jwtSecret;
        private IAuthenticationHelper authHelper;

        public ApiUserService(IBookstoreDatabaseSettings dbSettings, IBookstoreApplicationSettings appSettings)
        {
            var client = new MongoClient(dbSettings.ConnectionString);
            var database = client.GetDatabase(dbSettings.DatabaseName);

            _users = database.GetCollection<ApiUser>(dbSettings.UsersCollectionName);

            _jwtSecret = appSettings.JwtTokenSecret;

            authHelper = new AuthenticationHelper();
        }

        public (ApiUser user, string token) Authenticate(string password, string username = "", string email = "")
        {
            string tokenString = "";

            //check for nulls
            if ((string.IsNullOrEmpty(username) && string.IsNullOrEmpty(email))
                || string.IsNullOrEmpty(password))
                throw new ArgumentNullException("A null value for username/email and/or password was parsed");

            //get the user on username
            ApiUser returnUser = null;
            if (string.IsNullOrEmpty(username) != true)
            {
                returnUser = _users.Find(x => x.Username == username).FirstOrDefault();
            }
            //get the user on email
            else if (string.IsNullOrEmpty(email) != true)
            {
                returnUser = _users.Find(x => x.Email == email).FirstOrDefault();
            }

            if (returnUser == null)
                throw new ApplicationException("A user with the provided credentials does not exist");

            //validate the password
            if (authHelper.ValidatePassword(returnUser, password)==false)
                throw new ApplicationException("The password parsed is incorrect");

            //create jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    //add a claim with username and role
                    new Claim(ClaimTypes.NameIdentifier, returnUser.Username),
                    new Claim(ClaimTypes.Role, returnUser.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            tokenString = tokenHandler.WriteToken(token);

            return (returnUser, tokenString);
        }

        public List<ApiUser> GetAll() =>
            _users.Find(user => true).ToList();

        public ApiUser GetById(string id) =>
            _users.Find<ApiUser>(user => user.Id == id).FirstOrDefault();

        public ApiUser GetByUsername(string username) =>
            _users.Find<ApiUser>(user => user.Username == username).FirstOrDefault();

        public ApiUser GetByEmail(string email) =>
            _users.Find<ApiUser>(user => user.Email == email).FirstOrDefault();

        public List<ApiUser> Search(string firstName = "", string lastName = "", UserRoles role = UserRoles.noRole)
        {
            List<ApiUser> returnUsers = _users.Find(user => true).ToList();

            if(string.IsNullOrEmpty(firstName) != true)
            {
                List<ApiUser> filteredByFirstName = returnUsers.Where(x => x.FirstName == firstName).ToList();
                returnUsers = filteredByFirstName;
            }

            if (string.IsNullOrEmpty(lastName) != true)
            {
                List<ApiUser> filteredByLastName = returnUsers.Where(x => x.LastName == lastName).ToList();
                returnUsers = filteredByLastName;
            }

            if (role != UserRoles.noRole)
            {
                List<ApiUser> filteredByRole = returnUsers.Where(x => x.Role == role).ToList();
                returnUsers = filteredByRole;
            }

            return returnUsers;
        }

        public void Create(NewApiUser newApiUser)
        {
            //validate inputs
            if(newApiUser == null)
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(newApiUser.Email))
                throw new ApplicationException("An Email address is required");

            if (string.IsNullOrEmpty(newApiUser.Username))
                throw new ApplicationException("A username is required");

            if (string.IsNullOrEmpty(newApiUser.PasswordRaw))
                throw new ApplicationException("A password is required");

            //check username and email
            ApiUser existingUser = _users.Find(x => x.Username == newApiUser.Username || x.Email == newApiUser.Email).FirstOrDefault();
            if(existingUser != null)
                throw new ApplicationException("A user already exists with the provided username/email");

            //create ApiUser
            ApiUser newUser = new ApiUser()
            {
                FirstName = newApiUser.FirstName,
                LastName = newApiUser.LastName,
                Role = newApiUser.Role,
                Email = newApiUser.Email,
                Username = newApiUser.Username
            };

            //add password salt and hash
            newUser.PasswordSalt = authHelper.GetPasswordSalt();
            newUser.PasswordHash = authHelper.GetPasswordHashed(newApiUser.PasswordRaw, newUser.PasswordSalt);

            //add to db
            _users.InsertOne(newUser);
        }

        public void Update(string id, ApiUser newUserInfo) =>
            _users.ReplaceOne(user => user.Id == id, newUserInfo);

        public void Remove(ApiUser userToDelete) =>
            _users.DeleteOne(user => user.Id == userToDelete.Id);

        public void Remove(string id) =>
            _users.DeleteOne(user => user.Id == id);
    }
}
