using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KalumAuthManagement.Models
{
    public class UserInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        [JsonPropertyName("normalizedUserName")]
        [Required(ErrorMessage = "Es necesario asignar un nombre al usuario")]
        public string NormalizedUserName { get; set; }
        [JsonPropertyName("email")]
        [Required(ErrorMessage = "Es necesario asignar un correo al usuario")]
        public string Email { get; set; }
        [JsonPropertyName("password")]
        [Required(ErrorMessage = "Es necesario asignar una contraseña al usuario")]
        public string Password { get; set; }
        [JsonPropertyName("roles")]
        [Required(ErrorMessage = "Es necesario asignar un rol al usuario")]
        public List<string> Roles { get; set; }
    }
}