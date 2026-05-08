using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoNonato.Utilidades;
using System;

namespace ProyectoNonato.Controllers
{
    // Solo el Administrador tiene acceso a la gestión de tutores
    [Authorize(Roles = "Admin")]
    public class TutoresController : Controller
    {
        private string GetConnectionString()
        {
            var user = User.Identity?.Name ?? "";
            var pass = User.Claims.FirstOrDefault(c => c.Type == "UserPass")?.Value ?? "";
            return Conexion.GenerarCadenaDinamica(user, pass);
        }

        // 1. LISTADO DE TUTORES
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
            catch (SqlException ex)
            {
                ViewBag.Error = "Acceso denegado a la información de tutores. Verifique sus permisos.";
            }
            return View(dt);
        }

        // 2. REGISTRAR TUTOR (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. REGISTRAR TUTOR (POST)
        [HttpPost]
        public IActionResult Create(string idOficial, string nombre, string apPaterno, string apMaterno, string parentesco, string telefono, string calle, string numero, string colonia, string alcaldia, string cp, string lugarTrabajo, string telTrabajo)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    string query = @"INSERT INTO TUTORES (IdentificacionOficial, Nombre, ApellidoPaterno, ApellidoMaterno, Parentesco, Telefono, Calle, Numero, Colonia, Alcaldia, CodigoPostal, LugarTrabajo, TelefonoTrabajo) 
                                     VALUES (@id, @nom, @pat, @mat, @par, @tel, @calle, @num, @col, @alc, @cp, @trab, @telTrab)";

                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@id", idOficial);
                    cmd.Parameters.AddWithValue("@nom", nombre);
                    cmd.Parameters.AddWithValue("@pat", apPaterno);

                    // Manejo de valores opcionales para evitar errores en Azure SQL
                    cmd.Parameters.AddWithValue("@mat", string.IsNullOrEmpty(apMaterno) ? DBNull.Value : (object)apMaterno);
                    cmd.Parameters.AddWithValue("@par", string.IsNullOrEmpty(parentesco) ? DBNull.Value : (object)parentesco);
                    cmd.Parameters.AddWithValue("@tel", string.IsNullOrEmpty(telefono) ? DBNull.Value : (object)telefono);
                    cmd.Parameters.AddWithValue("@calle", string.IsNullOrEmpty(calle) ? DBNull.Value : (object)calle);
                    cmd.Parameters.AddWithValue("@num", string.IsNullOrEmpty(numero) ? DBNull.Value : (object)numero);
                    cmd.Parameters.AddWithValue("@col", string.IsNullOrEmpty(colonia) ? DBNull.Value : (object)colonia);
                    cmd.Parameters.AddWithValue("@alc", string.IsNullOrEmpty(alcaldia) ? DBNull.Value : (object)alcaldia);
                    cmd.Parameters.AddWithValue("@cp", string.IsNullOrEmpty(cp) ? DBNull.Value : (object)cp);
                    cmd.Parameters.AddWithValue("@trab", string.IsNullOrEmpty(lugarTrabajo) ? DBNull.Value : (object)lugarTrabajo);
                    cmd.Parameters.AddWithValue("@telTrab", string.IsNullOrEmpty(telTrabajo) ? DBNull.Value : (object)telTrabajo);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch (SqlException ex)
            {
                ViewBag.Error = "No tiene permisos para crear tutores.";
                return View();
            }
        }

        // 4. ACTUALIZAR TUTOR (GET)
        // La identificación oficial es un string (VARCHAR 20)
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
            catch (SqlException ex)
            {
                ViewBag.Error = "Acceso denegado a la información del tutor.";
            }
            return View(dt);
        }

        // 5. ELIMINAR TUTOR
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
            }
            catch (SqlException ex)
            {
                // Manejo de error si el tutor tiene alumnos asignados (Integridad Referencial o permisos)
                ViewBag.Error = "No se puede eliminar el tutor porque tiene alumnos asociados o falta de permisos.";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}