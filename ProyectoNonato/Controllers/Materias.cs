using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoNonato.Utilidades;

namespace ProyectoNonato.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MateriasController : Controller
    {
        private string GetConnectionString()
        {
            var user = User.Identity?.Name ?? "";
            var pass = User.Claims.FirstOrDefault(c => c.Type == "UserPass")?.Value ?? "";
            return Conexion.GenerarCadenaDinamica(user, pass);
        }

        // 1. LISTADO DE MATERIAS
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
            catch (SqlException ex)
            {
                ViewBag.Error = "Acceso denegado a la información de materias. Verifique sus permisos.";
            }
            return View(dt);
        }

        // 2. CREAR MATERIA (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. CREAR MATERIA (POST)
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
            catch (SqlException ex)
            {
                ViewBag.Error = "No tiene permisos para crear materias.";
                return View();
            }
        }

        // 4. ACTUALIZAR MATERIA (GET)
        // Recibe un string ya que la llave primaria es NombreMateria
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
            catch (SqlException ex)
            {
                ViewBag.Error = "Acceso denegado a la información de la materia.";
            }
            return View(dt);
        }

        // 5. ELIMINAR MATERIA
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
            catch (SqlException ex)
            {
                ViewBag.Error = "No tiene permisos para eliminar la materia.";
                return RedirectToAction("Index");
            }
        }
    }
}