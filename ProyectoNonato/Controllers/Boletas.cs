using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoNonato.Utilidades;

namespace ProyectoNonato.Controllers
{
    public class BoletasController : Controller
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
                    string query = "SELECT * FROM BOLETAS";
                    SqlDataAdapter da = new SqlDataAdapter(query, cn);
                    da.Fill(dt);
                }
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado: Tu rol actual no tiene permisos para ver las boletas de calificaciones.";
            }
            return View(dt);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(int idBoleta, string periodo, int boletaAlumno)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    string query = @"INSERT INTO BOLETAS (ID_Boleta, Periodo, Boleta) 
                                     VALUES (@id, @periodo, @boleta)";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@id", idBoleta);
                    cmd.Parameters.AddWithValue("@periodo", periodo);
                    cmd.Parameters.AddWithValue("@boleta", boletaAlumno);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado: No tienes permisos para generar boletas.";
                return View();
            }
        }

        public IActionResult Update(int id)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    string query = "SELECT * FROM BOLETAS WHERE ID_Boleta = @id";
                    SqlDataAdapter da = new SqlDataAdapter(query, cn);
                    da.SelectCommand.Parameters.AddWithValue("@id", id);
                    da.Fill(dt);
                }
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado a la información de la boleta.";
            }
            return View(dt);
        }

        public IActionResult Delete(int id)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    string query = "DELETE FROM BOLETAS WHERE ID_Boleta = @id";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado: No tienes permisos para eliminar boletas.";
                return RedirectToAction("Index");
            }
        }
    }
}