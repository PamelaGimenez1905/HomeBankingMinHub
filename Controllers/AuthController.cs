using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HomeBankingMindHub.Migrations;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IClientRepository _clientRepository;
        public AuthController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ClientLoginDTO client)
        {
            try
            {   //Verificamos que se encuentre dentro de la base de datos
                Client user = _clientRepository.FindByEmail(client.Email);
                if (user == null || !String.Equals(user.Password, client.Password))
                    return Unauthorized();
                //Creamos las claims
                var claims = new List<Claim>

                {   //Si el email contiene @admin.com es Admin y sino Client
                    new Claim(user.Email.Contains("@admin.com") ? "Admin" : "Client", user.Email),
                   

                }; //Atenticación del usuario mediante cookie que contiene su información que se va a enviar al cliente
                    var claimsIdentity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                        );

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return Ok();

                
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {   //Cierra sesión eliminando la cookie de autenticación
            try
            {
                await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

