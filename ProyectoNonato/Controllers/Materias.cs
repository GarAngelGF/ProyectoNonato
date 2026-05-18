using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoNonato.Utilidades;

namespace ProyectoNonato.Controllers
{
    public class MateriasController : Controller
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
                    string query = "SELECT * FROM MATERIAS";
                    SqlDataAdapter da = new SqlDataAdapter(query, cn);
                    da.Fill(dt);
                }
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado: Tu rol actual no tiene permisos para ver las materias.";
            }
            return View(dt);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string nombreMateria, string area, string nivelAplicable)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    string query = @"INSERT INTO MATERIAS (NombreMateria, Area, NivelAplicable) 
                                     VALUES (@nombre, @area, @nivel)";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@nombre", nombreMateria);
                    cmd.Parameters.AddWithValue("@area", string.IsNullOrEmpty(area) ? DBNull.Value : (object)area);
                    cmd.Parameters.AddWithValue("@nivel", string.IsNullOrEmpty(nivelAplicable) ? DBNull.Value : (object)nivelAplicable);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado: No tienes permisos para registrar materias.";
                return View();
            }
        }

        public IActionResult Update(string nombreMateria)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    string query = "SELECT * FROM MATERIAS WHERE NombreMateria = @nombre";
                    SqlDataAdapter da = new SqlDataAdapter(query, cn);
                    da.SelectCommand.Parameters.AddWithValue("@nombre", nombreMateria);
                    da.Fill(dt);
                }
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado a la información de la materia.";
            }
            return View(dt);
        }

        public IActionResult Delete(string nombreMateria)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    string query = "DELETE FROM MATERIAS WHERE NombreMateria = @nombre";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@nombre", nombreMateria);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado: No tienes permisos para eliminar materias.";
                return RedirectToAction("Index");
            }
        }
    }
}