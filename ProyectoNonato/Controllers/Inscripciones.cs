using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoNonato.Utilidades;

namespace ProyectoNonato.Controllers
{
    public class InscripcionesController : Controller
    {
        private string GetConnectionString()
        {
            var builder = new SqlConnectionStringBuilder(Conexion.CadenaSQLBase);
            var claimsIdentity = User.Identity as System.Security.Claims.ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst("DbUser");
            var passwordClaim = claimsIdentity?.FindFirst("DbPassword");

            if (userIdClaim != null && passwordClaim != null)
            {
                builder.UserID = userIdClaim.Value;
                builder.Password = passwordClaim.Value;
            }
            return builder.ConnectionString;
        }

        public IActionResult Index()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    string query = "SELECT * FROM INSCRIPCIONES";
                    SqlDataAdapter da = new SqlDataAdapter(query, cn);
                    da.Fill(dt);
                }
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado: Tu rol actual no tiene permisos para ver las inscripciones.";
            }
            return View(dt);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(int boleta, string nombreGrupo, string nivel)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    string query = @"INSERT INTO INSCRIPCIONES (Boleta, NombreGrupo, Nivel) 
                                     VALUES (@boleta, @grupo, @nivel)";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@boleta", boleta);
                    cmd.Parameters.AddWithValue("@grupo", nombreGrupo);
                    cmd.Parameters.AddWithValue("@nivel", nivel);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado: No tienes permisos para registrar inscripciones.";
                return View();
            }
        }

        public IActionResult Delete(int boleta, string nombreGrupo, string nivel)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    string query = "DELETE FROM INSCRIPCIONES WHERE Boleta = @boleta AND NombreGrupo = @grupo AND Nivel = @nivel";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@boleta", boleta);
                    cmd.Parameters.AddWithValue("@grupo", nombreGrupo);
                    cmd.Parameters.AddWithValue("@nivel", nivel);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado: No tienes permisos para eliminar inscripciones.";
                return RedirectToAction("Index");
            }
        }
    }
}