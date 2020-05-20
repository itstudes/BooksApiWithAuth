using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using BooksApiWithAuth.Models;
using BooksApiWithAuth.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BooksApiWithAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApiUserService _userService;

        public UsersController(ApiUserService userService)
        {
            _userService = userService;
        }

        #region public routes

        //[AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]NewApiUser newApiUser)
        {
            try
            {
                //create user
                _userService.Create(newApiUser);
                return Ok();
            }
            catch (Exception ex)
            {
                //return exception if there was one
                return BadRequest(new { message = ex.Message });
            }
        }

        //[AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]LoginModel newLogin)
        {
            try
            {
                //validation
                if (newLogin == null)
                    throw new ArgumentNullException();

                //get user data if authenticated
                ApiUser userFromDb = null;
                string tokenString = "";
                if (string.IsNullOrEmpty(newLogin.Username) == false)
                {
                    (userFromDb, tokenString) = _userService.Authenticate(newLogin.PasswordRaw, username: newLogin.Username);
                }
                else if (string.IsNullOrEmpty(newLogin.Email) == false)
                {
                    (userFromDb, tokenString) = _userService.Authenticate(newLogin.PasswordRaw, email: newLogin.Email);
                }
                else
                    throw new ArgumentNullException();

                //confirm user data is not null
                if (userFromDb == null || string.IsNullOrEmpty(tokenString))
                    throw new ApplicationException("User credentials could not be found");

                return Ok(new
                {
                    Id = userFromDb.Id,
                    FirstName = userFromDb.FirstName,
                    LastName = userFromDb.LastName,
                    Username = userFromDb.Username,
                    Token = tokenString
                });
            }
            catch (Exception ex)
            {
                //return exception if there was one
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region authorized routes

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var users = _userService.GetAll();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }           
        }

        [HttpGet]
        public IActionResult GetById(string? id)
        {
            try
            {
                var user = _userService.GetById(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetByUsername(string? username)
        {
            try
            {
                var user = _userService.GetByUsername(username);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetByEmail(string? email)
        {
            try
            {
                var user = _userService.GetByUsername(email);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Search(string? firstName, string? lastName, int? role)
        {
            try
            {
                List<ApiUser> users = null;
                if (role.HasValue)
                {
                    UserRoles roleForQuery = ApiUser.GetUserRoleFromInt(role.Value);
                    users = _userService.Search(firstName, lastName, roleForQuery);
                }
                else
                {
                    users = _userService.Search(firstName, lastName);
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        public IActionResult Delete(string id)
        {
            try
            {
                _userService.Remove(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });

            }            
        }

        #endregion
    }
}