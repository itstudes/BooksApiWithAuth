using BooksApiWithAuth.Models;
using BooksApiWithAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BooksApiWithAuth.Controllers
{
    [Authorize]
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

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] NewApiUser newApiUser)
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

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] LoginModel newLogin)
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

        #endregion public routes

        #region authorized routes

        [HttpGet]
        [Authorize(Policy = "LibraryTeam")]
        public IActionResult Get(string? id, string? username, string? email)
        {
            try
            {
                //get all
                if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(username) && string.IsNullOrEmpty(email))
                    return Ok(_userService.GetAll());
                //get by id
                else if (string.IsNullOrEmpty(id) == false && string.IsNullOrEmpty(username) && string.IsNullOrEmpty(email))
                    return Ok(_userService.GetById(id));
                //get by username
                else if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(username) == false && string.IsNullOrEmpty(email))
                    return Ok(_userService.GetByUsername(username));
                //get by email
                else if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(username) && string.IsNullOrEmpty(email) == false)
                    return Ok(_userService.GetByEmail(email));
                else
                    throw new ApplicationException("Cannot find user based on 2 or more key properties");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        [Authorize(Policy = "LibraryTeam")]
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
        [Authorize(Roles = "LibraryAdmin")]
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

        #endregion authorized routes
    }
}