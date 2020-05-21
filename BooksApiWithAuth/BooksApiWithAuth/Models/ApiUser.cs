using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace BooksApiWithAuth.Models
{
    public class ApiUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("FirstName")]
        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [BsonElement("LastName")]
        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [BsonElement("Username")]
        [JsonProperty("Username")]
        public string Username { get; set; }

        [BsonElement("Email")]
        [JsonProperty("Email")]
        public string Email { get; set; }

        [BsonElement("PasswordHash")]
        [JsonProperty("PasswordHash")]
        public string PasswordHash { get; set; }

        [BsonElement("PasswordSalt")]
        [JsonProperty("PasswordSalt")]
        public string PasswordSalt { get; set; }

        [BsonElement("Role")]
        [JsonProperty("Role")]
        public UserRoles Role { get; set; }

        public static UserRoles GetUserRoleFromInt(int value)
        {
            switch (value)
            {
                case 1:
                    return UserRoles.unassigned;

                case 2:
                    return UserRoles.libraryAdmin;

                case 3:
                    return UserRoles.libraryStaff;

                case 4:
                    return UserRoles.libraryMember;

                default:
                    return UserRoles.noRole;
            }
        }

        public static string GetUserRoleString(UserRoles role)
        {
            switch (role)
            {
                case UserRoles.noRole:
                    return "NoRole";

                case UserRoles.unassigned:
                    return "Unassigned";

                case UserRoles.libraryAdmin:
                    return "LibraryAdmin";

                case UserRoles.libraryStaff:
                    return "LibraryStaff";

                case UserRoles.libraryMember:
                    return "LibraryMember";

                default:
                    return "NoRole";
            }
        }
    }

    public enum UserRoles
    {
        noRole = 0,
        unassigned = 1,
        libraryAdmin = 2,
        libraryStaff = 3,
        libraryMember = 4
    }
}