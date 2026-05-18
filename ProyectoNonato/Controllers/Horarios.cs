using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoNonato.Utilidades;

namespace ProyectoNonato.Controllers
{
    public class HorariosController : Controller
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
                    string query = "SELECT * FROM HORARIOS";
                    SqlDataAdapter da = new SqlDataAdapter(query, cn);
                    da.Fill(dt);
                }
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado: Tu rol actual no tiene permisos para ver la planeación de horarios.";
            }
            return View(dt);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string dia, string horaInicio, string horaFin, string edificio, string aula, string nombreGrupo, string nivel, string nombreMateria, string cedulaProfesional)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    string query = @"INSERT INTO HORARIOS (Dia, HoraInicio, HoraFin, Edificio, Aula, NombreGrupo, Nivel, NombreMateria, CedulaProfesional) 
                                     VALUES (@dia, @inicio, @fin, @edif, @aula, @grupo, @nivel, @materia, @cedula)";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@dia", dia);
                    cmd.Parameters.AddWithValue("@inicio", horaInicio);
                    cmd.Parameters.AddWithValue("@fin", horaFin);
                    cmd.Parameters.AddWithValue("@edif", string.IsNullOrEmpty(edificio) ? DBNull.Value : (object)edificio);
                    cmd.Parameters.AddWithValue("@aula", string.IsNullOrEmpty(aula) ? DBNull.Value : (object)aula);
                    cmd.Parameters.AddWithValue("@grupo", nombreGrupo);
                    cmd.Parameters.AddWithValue("@nivel", nivel);
                    cmd.Parameters.AddWithValue("@materia", nombreMateria);
                    cmd.Parameters.AddWithValue("@cedula", cedulaProfesional);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado: No tienes privilegios para agendar horarios.";
                return View();
            }
        }

        public IActionResult Delete(string dia, string horaInicio, string nombreGrupo, string nivel, string nombreMateria)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    string query = "DELETE FROM HORARIOS WHERE Dia = @dia AND HoraInicio = @inicio AND NombreGrupo = @grupo AND Nivel = @nivel AND NombreMateria = @materia";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@dia", dia);
                    cmd.Parameters.AddWithValue("@inicio", horaInicio);
                    cmd.Parameters.AddWithValue("@grupo", nombreGrupo);
                    cmd.Parameters.AddWithValue("@nivel", nivel);
                    cmd.Parameters.AddWithValue("@materia", nombreMateria);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado: No tienes privilegios para eliminar horarios.";
                return RedirectToAction("Index");
            }
        }
    }
}