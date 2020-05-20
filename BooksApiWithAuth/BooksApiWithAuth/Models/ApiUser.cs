using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                    return UserRoles.author;
                case 5:
                    return UserRoles.proofReader;
                case 6:
                    return UserRoles.publisher;
                case 20:
                    return UserRoles.standardMembershipReader;
                case 21:
                    return UserRoles.specialMembershipReader;
                case 22:
                    return UserRoles.exclusiveMembershipReader;
                default:
                    return UserRoles.noRole;
            }
        }
    }

    public enum UserRoles
    {
        //roles 0 - 19 are for staff/general
        noRole = 0,
        unassigned = 1,
        libraryAdmin = 2,
        libraryStaff = 3,
        author = 4,
        proofReader = 5,
        publisher = 6,
        //roles 20 - 39 are for members
        standardMembershipReader = 20,
        specialMembershipReader = 21,
        exclusiveMembershipReader = 22
    }

    
}
