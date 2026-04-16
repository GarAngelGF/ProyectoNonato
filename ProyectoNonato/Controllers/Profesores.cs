using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoNonato.Utilidades;
using System;

namespace ProyectoNonato.Controllers
{
    // Acceso permitido tanto al Administrador como al Gestor de Profesores
    [Authorize(Roles = "Admin,Gestor")]
    public class ProfesoresController : Controller
    {
        // 1. LISTADO DE PROFESORES
        public IActionResult Index()
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = "SELECT * FROM PROFESORES";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                da.Fill(dt);
            }
            return View(dt);
        }

        // 2. CREAR PROFESOR (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. CREAR PROFESOR (POST)
        [HttpPost]
        public IActionResult Create(string cedula, string nombre, string apPaterno, string apMaterno, string telefono, string calle, string numero, string colonia, string alcaldia, string cp, DateTime fechaIngreso, int horasBase, string estatus)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = @"INSERT INTO PROFESORES (CedulaProfesional, Nombre, ApellidoPaterno, ApellidoMaterno, Telefono, Calle, Numero, Colonia, Alcaldia, CodigoPostal, FechaIngreso, HorasBase, Estatus) 
                                 VALUES (@cedula, @nombre, @apPaterno, @apMaterno, @tel, @calle, @num, @col, @alc, @cp, @fecha, @horas, @estatus)";

                SqlCommand cmd = new SqlCommand(query, cn);

                // Campos obligatorios
                cmd.Parameters.AddWithValue("@cedula", cedula);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@apPaterno", apPaterno);

                // Campos opcionales (Manejo de nulos para Azure SQL)
                cmd.Parameters.AddWithValue("@apMaterno", string.IsNullOrEmpty(apMaterno) ? DBNull.Value : apMaterno);
                cmd.Parameters.AddWithValue("@tel", string.IsNullOrEmpty(telefono) ? DBNull.Value : telefono);
                cmd.Parameters.AddWithValue("@calle", string.IsNullOrEmpty(calle) ? DBNull.Value : calle);
                cmd.Parameters.AddWithValue("@num", string.IsNullOrEmpty(numero) ? DBNull.Value : numero);
                cmd.Parameters.AddWithValue("@col", string.IsNullOrEmpty(colonia) ? DBNull.Value : colonia);
                cmd.Parameters.AddWithValue("@alc", string.IsNullOrEmpty(alcaldia) ? DBNull.Value : alcaldia);
                cmd.Parameters.AddWithValue("@cp", string.IsNullOrEmpty(cp) ? DBNull.Value : cp);
                cmd.Parameters.AddWithValue("@fecha", fechaIngreso == default(DateTime) ? DBNull.Value : fechaIngreso);
                cmd.Parameters.AddWithValue("@horas", horasBase);
                cmd.Parameters.AddWithValue("@estatus", string.IsNullOrEmpty(estatus) ? "Activo" : estatus);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // 4. ACTUALIZAR PROFESOR (GET)
        // Se utiliza la Cédula Profesional (string) como identificador único
        public IActionResult Update(string cedula)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = "SELECT * FROM PROFESORES WHERE CedulaProfesional = @cedula";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                da.SelectCommand.Parameters.AddWithValue("@cedula", cedula);
                da.Fill(dt);
            }
            return View(dt);
        }

        // 5. ELIMINAR PROFESOR
        public IActionResult Delete(string cedula)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = "DELETE FROM PROFESORES WHERE CedulaProfesional = @cedula";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@cedula", cedula);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }
    }
}