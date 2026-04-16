using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using ProyectoNonato.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProyectoNonato.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Welcome");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string usuario, string password)
        {
            string rol = "";

            if (usuario == "admin" && password == "admin123") rol = "Admin";
            else if (usuario == "consultor" && password == "user123") rol = "Consultor";
            else if (usuario == "gestor" && password == "prof123") rol = "Gestor";

            if (!string.IsNullOrEmpty(rol))
            {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, usuario),
                    new Claim(ClaimTypes.Role, rol)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Welcome");
            }

            ViewBag.Error = "Credenciales incorrectas";
            return View();
        }

        public IActionResult Welcome()
        {
            return View();
        }

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }
    }
}