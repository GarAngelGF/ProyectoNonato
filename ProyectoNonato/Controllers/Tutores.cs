using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoNonato.Utilidades;

namespace ProyectoNonato.Controllers
{
    public class TutoresController : Controller
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
                    string query = "SELECT * FROM TUTORES";
                    SqlDataAdapter da = new SqlDataAdapter(query, cn);
                    da.Fill(dt);
                }
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado: Tu rol actual no tiene permisos para ver los tutores.";
            }
            return View(dt);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string identificacionOficial, string nombre, string apellidoPaterno, string apellidoMaterno, string parentesco, string telefono, string calle, string numero, string colonia, string alcaldia, string codigoPostal)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    string query = @"INSERT INTO TUTORES (IdentificacionOficial, Nombre, ApellidoPaterno, ApellidoMaterno, Parentesco, Telefono, Calle, Numero, Colonia, Alcaldia, CodigoPostal) 
                                     VALUES (@id, @nom, @ap, @am, @par, @tel, @cal, @num, @col, @alc, @cp)";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@id", identificacionOficial);
                    cmd.Parameters.AddWithValue("@nom", nombre);
                    cmd.Parameters.AddWithValue("@ap", apellidoPaterno);
                    cmd.Parameters.AddWithValue("@am", string.IsNullOrEmpty(apellidoMaterno) ? DBNull.Value : (object)apellidoMaterno);
                    cmd.Parameters.AddWithValue("@par", string.IsNullOrEmpty(parentesco) ? DBNull.Value : (object)parentesco);
                    cmd.Parameters.AddWithValue("@tel", string.IsNullOrEmpty(telefono) ? DBNull.Value : (object)telefono);
                    cmd.Parameters.AddWithValue("@cal", string.IsNullOrEmpty(calle) ? DBNull.Value : (object)calle);
                    cmd.Parameters.AddWithValue("@num", string.IsNullOrEmpty(numero) ? DBNull.Value : (object)numero);
                    cmd.Parameters.AddWithValue("@col", string.IsNullOrEmpty(colonia) ? DBNull.Value : (object)colonia);
                    cmd.Parameters.AddWithValue("@alc", string.IsNullOrEmpty(alcaldia) ? DBNull.Value : (object)alcaldia);
                    cmd.Parameters.AddWithValue("@cp", string.IsNullOrEmpty(codigoPostal) ? DBNull.Value : (object)codigoPostal);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado: No tienes permisos para registrar tutores.";
                return View();
            }
        }

        public IActionResult Update(string id)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    string query = "SELECT * FROM TUTORES WHERE IdentificacionOficial = @id";
                    SqlDataAdapter da = new SqlDataAdapter(query, cn);
                    da.SelectCommand.Parameters.AddWithValue("@id", id);
                    da.Fill(dt);
                }
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado a la información del tutor.";
            }
            return View(dt);
        }

        public IActionResult Delete(string id)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    string query = "DELETE FROM TUTORES WHERE IdentificacionOficial = @id";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch (SqlException)
            {
                ViewBag.Error = "Acceso denegado: No tienes permisos para eliminar tutores.";
                return RedirectToAction("Index");
            }
        }
    }
}