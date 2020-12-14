using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BookStore_API.Contracts;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace BookStore_API.Controllers
{
    /// <summary>
    ///     This is a test controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : LoggingController
    {
        private readonly IConfiguration config;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;

        public UsersController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager,
            ILoggerService logger, IConfiguration config) : base(logger)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.config = config;
        }

        /// <summary>
        ///     User login endpoint
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            try
            {
                string username = userDTO.Username;
                string password = userDTO.Password;
                SignInResult result = await this.signInManager.PasswordSignInAsync(username, password, false, false);

                this.LogInfo($"Login attempt from user {username}");

                if (result.Succeeded)
                {
                    this.LogInfo($"{username} successfully authenticated");
                    IdentityUser user = await this.userManager.FindByNameAsync(username);
                    string tokenString = await this.GenerateJSONWebToken(user);
                    return this.Ok(new {token = tokenString});
                }

                this.LogInfo($"{username} Not authenticated");
                return this.Unauthorized(userDTO);
            }
            catch (Exception e)
            {
                return this.InternalError(e);
            }
        }

        private async Task<string> GenerateJSONWebToken(IdentityUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            IList<string> roles = await this.userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r)));
            var token = new JwtSecurityToken(
                this.config["Jwt:Issuer"],
                this.config["Jwt:Issuer"],
                claims, null,
                DateTime.Now.AddMinutes(5),
                credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}