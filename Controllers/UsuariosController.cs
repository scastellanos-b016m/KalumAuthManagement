using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KalumAuthManagement.DBContexts;
using KalumAuthManagement.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KalumAuthManagement.Controllers
{
    [ApiController]
    [Route("kalum-Auth/v1/usuarios")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsuariosController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly ApplicationDbContext ApplicationDbContext;
        public UsuariosController(UserManager<ApplicationUser> _UserManager, ApplicationDbContext _ApplicationDbContext)
        {
            this.UserManager = _UserManager;
            this.ApplicationDbContext = _ApplicationDbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<ApplicationUser>>> Get()
        {
            List<ApplicationUser> users = await UserManager.Users.ToListAsync();
            return users;
        }

        [HttpGet("search/{id}", Name = "GetUsuario")]
        public async Task<ActionResult<ApplicationUser>> Get(string id)
        {
            var usuario = await UserManager.FindByIdAsync(id);
            if (usuario != null)
            {
                return usuario;
            }
            return NotFound();
        }
    }
}