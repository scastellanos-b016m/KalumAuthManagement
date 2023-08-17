using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using KalumAuthManagement.DTOs;
using KalumAuthManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace KalumAuthManagement.Controllers
{
    [ApiController]
    [Route("kalum-Auth/v1/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> SignInManager;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly IConfiguration Configuration;
        public CuentasController(IConfiguration _Configuration, SignInManager<ApplicationUser> _SignInManager, UserManager<ApplicationUser> _UserManager)
        {
            this.SignInManager = _SignInManager;
            this.UserManager = _UserManager;
            this.Configuration = _Configuration;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] LoginDTO value)
        {
            var login = await SignInManager.PasswordSignInAsync(value.UserName, value.Password, isPersistent: false, lockoutOnFailure: false);
            if (login.Succeeded)
            {
                var usuario = await UserManager.FindByNameAsync(value.UserName);
                var roles = await UserManager.GetRolesAsync(usuario);
                return BuildToken(usuario, roles);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "el login es invalido");
                return BadRequest(ModelState);
            }
        }

        [HttpPost("finish-register")]
        public async Task<ActionResult<UserToken>> FinishRegister([FromBody] FinishRegisterDTO request)
        {
            var user = await this.UserManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                user.IdentificationId = request.IdentificationId;
                await this.UserManager.UpdateAsync(user);
                await UserManager.RemoveFromRolesAsync(user, new string[] { "ROLE_USER" });
                await UserManager.AddToRoleAsync(user, "ROLE_CANDIDATE");
                return StatusCode(201, BuildToken(user, new string[] { "ROLE_CANDIDATE" }));
            }
            else
            {
                var response = new AccountResponseDTO()
                {
                    StatusCode = 404,
                    Message = $"No existe el correo {request.Email}"
                };
                return StatusCode(404, response);
            }
        }

        [HttpPost("finish-register-alumno")]
        public async Task<ActionResult<UserToken>> FinishRegisterAlumno([FromBody] FinishRegisterDTO request)
        {
            var user = await this.UserManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                user.IdentificationId = request.IdentificationId;
                await this.UserManager.UpdateAsync(user);
                await UserManager.RemoveFromRolesAsync(user, new string[] { "ROLE_CANDIDATE" });
                await UserManager.AddToRoleAsync(user, "ROLE_STUDENT");
                return StatusCode(201, BuildToken(user, new string[] { "ROLE_STUDENT" }));
            }
            else
            {
                var response = new AccountResponseDTO()
                {
                    StatusCode = 404,
                    Message = $"No existe el correo {request.Email}"
                };
                return StatusCode(404, response);
            }
        }

        [HttpPost("Crear")]
        public async Task<ActionResult<UserToken>> Create([FromBody] UserInfo value)
        {
            var userInfo = new ApplicationUser
            {
                UserName = value.UserName,
                Email = value.Email
            };

            var newUser = await UserManager.CreateAsync(userInfo, value.Password);

            if (newUser.Succeeded)
            {
                // value.Roles.Add("ROLE_USER");
                await UserManager.AddToRoleAsync(userInfo, value.Roles.ElementAt(0));
                return BuildToken(userInfo, value.Roles);
                // return BuildToken(userInfo, value.Roles != null ? value.Roles : new List<string>());
            }
            else
            {
                return BadRequest("La información enviada no es correcta");
            }
        }

        private UserToken BuildToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Email),
                new Claim("api", "kalumAuth"),
                new Claim("username", user.UserName),
                new Claim("email", user.Email),
                new Claim("identificationId", user.IdentificationId != null ? user.IdentificationId : "0"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            foreach (var rol in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["JWT:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(1);
            JwtSecurityToken token = new JwtSecurityToken
            (
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );
            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}