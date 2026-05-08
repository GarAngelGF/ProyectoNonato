using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using ProyectoNonato.Models;
using ProyectoNonato.Utilidades; 
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System;

namespace ProyectoNonato.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Main");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string usuario, string password)
        {
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Debe ingresar usuario y contraseña";
                return View();
            }

            // 1. Construimos dinámicamente la cadena de conexión
            string cadenaConexion = Utilidades.Conexion.GenerarCadenaDinamica(usuario, password);

            bool credencialesValidas = false;
            string rol = "Usuario";

            // 2. Intentamos conectar a la base de datos con esas credenciales
            try
            {
                using (SqlConnection conn = new SqlConnection(cadenaConexion))
                {
                    await conn.OpenAsync();
                    credencialesValidas = true; 

                    // Consulta de rol de forma dinámica en la base de datos
                    using (SqlCommand cmd = new SqlCommand("SELECT RolApp FROM AppRoles WHERE NombreUsuario = @usuario", conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@usuario", usuario));

                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            rol = result.ToString();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                ViewBag.Error = "Credenciales incorrectas o acceso denegado por la base de datos";
                return View();
            }

            // 3. Si es válido, asignamos el rol en la aplicación web
            if (credencialesValidas)
            {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, usuario),
                    new Claim(ClaimTypes.Role, rol),
                    new Claim("UserPass", password) // Guardamos el password temporalmente (ver consideraciones de seguridad)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Main");
            }

            return View();
        }

        public IActionResult Main()
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