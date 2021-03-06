﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using market.Helpers;
using market.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace market.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        //dependency Injection

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signManager;
        private readonly AppSettings _appSettings;


        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signManager, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _signManager = signManager;
            _appSettings = appSettings.Value;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> register([FromBody] RegisterViewModel formdata ){
            // Will Hold all The errors related to Registration
            List<string> errorList = new List<string> {};

            var user = new IdentityUser
            {
                Email = formdata.Email,
                UserName = formdata.Username,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, formdata.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");

                return Ok(new {username = user.UserName ,email = user.Email, status = 1,message ="Registeration Succesful"});
                
            }

            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    errorList.Add(error.Description);
                }
            }
            return BadRequest(new JsonResult(errorList));
        }

        // Login Method 
        [HttpPost("[action]")]
        public async Task<IActionResult> login([FromBody]LoginViewModel formdata)
        {
            // find a user
            var user = await _userManager.FindByNameAsync(formdata.Username);

            // find a Role
            var roles = await _userManager.GetRolesAsync(user);

            //defining Key
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));

            // Expire time defining
            var tokenExpirytime =  Convert.ToDouble(_appSettings.ExpireTime)+60;
           
            if (user != null && await _userManager.CheckPasswordAsync(user,formdata.Password))
            {
                // Confirmation Email

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDiscriptor = new SecurityTokenDescriptor{
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, formdata.Username),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                        new Claim("LoggedOn", DateTime.Now.ToString()),

                    }),
                    
                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _appSettings.Site,
                    Audience = _appSettings.Audience,
                    Expires = DateTime.Now.AddMinutes(tokenExpirytime),
             
                    IssuedAt =DateTime.Now
                    
                };


                // generate Token
                var token = tokenHandler.CreateToken(tokenDiscriptor);
                return Ok(new { token =tokenHandler.WriteToken(token), expiration = token.ValidTo, username = user.UserName ,userRole = roles.FirstOrDefault()});



            }

            // login error 
            ModelState.AddModelError("", "Username/password are not found !");
            return Unauthorized(new { LoginError = "Please check the login credentials - invalid username / password" });
        }
    }
}