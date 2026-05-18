using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoNonato.Utilidades;

namespace ProyectoNonato.Controllers
{
    public class DetalleBoletaController : Controller
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
                    string query = "SELECT * FROM DETALLE_BOLETA";
                    SqlDataAdapter da = new SqlDataAdapter(query, cn);
                    da.Fill(dt);
                }
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado: Tu rol actual no tiene permisos para ver el desglose de calificaciones.";
            }
            return View(dt);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(int idBoleta, string nombreMateria, decimal calificacion)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    string query = @"INSERT INTO DETALLE_BOLETA (ID_Boleta, NombreMateria, Calificacion) 
                                     VALUES (@id, @materia, @calif)";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@id", idBoleta);
                    cmd.Parameters.AddWithValue("@materia", nombreMateria);
                    cmd.Parameters.AddWithValue("@calif", calificacion);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado: No tienes permisos para asentar calificaciones.";
                return View();
            }
        }

        public IActionResult Delete(int idBoleta, string nombreMateria)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    string query = "DELETE FROM DETALLE_BOLETA WHERE ID_Boleta = @id AND NombreMateria = @materia";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@id", idBoleta);
                    cmd.Parameters.AddWithValue("@materia", nombreMateria);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado: No tienes permisos para eliminar calificaciones.";
                return RedirectToAction("Index");
            }
        }
    }
}