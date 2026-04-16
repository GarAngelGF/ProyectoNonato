using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoNonato.Utilidades;
using ProyectoNonato.Models;

namespace ProyectoNonato.Controllers
{
    // Restricción de acceso: Solo usuarios con rol Admin pueden gestionar alumnos
    [Authorize(Roles = "Admin")]
    public class AlumnosController : Controller
    {
        // 1. VISTA PRINCIPAL (LISTADO)
        public IActionResult Index()
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                // Consulta que une Alumnos con Tutores para mostrar información completa
                string query = "SELECT A.*, T.Nombre + ' ' + T.ApellidoPaterno AS NombreTutor FROM ALUMNOS A JOIN TUTORES T ON A.IdentificacionTutor = T.IdentificacionOficial";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                da.Fill(dt);
            }
            return View(dt);
        }

        // 2. CREAR ALUMNO (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. CREAR ALUMNO (POST)
        [HttpPost]
        public IActionResult Create(int boleta, string nombre, string apPaterno, string apMaterno, DateTime fechaNac, string genero, string calle, string numero, string colonia, string alcaldia, string cp, string tipoSangre, string identificacionTutor)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = @"INSERT INTO ALUMNOS (Boleta, Nombre, ApellidoPaterno, ApellidoMaterno, FechaNacimiento, Genero, Calle, Numero, Colonia, Alcaldia, CodigoPostal, FechaIngreso, TipoSangre, Estatus, IdentificacionTutor) 
                                VALUES (@boleta, @nombre, @apPaterno, @apMaterno, @fechaNac, @genero, @calle, @numero, @colonia, @alcaldia, @cp, GETDATE(), @tipoSangre, 'Activo', @idTutor)";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@boleta", boleta);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@apPaterno", apPaterno);
                cmd.Parameters.AddWithValue("@apMaterno", apMaterno);
                cmd.Parameters.AddWithValue("@fechaNac", fechaNac);
                cmd.Parameters.AddWithValue("@genero", genero);
                cmd.Parameters.AddWithValue("@calle", calle);
                cmd.Parameters.AddWithValue("@numero", numero);
                cmd.Parameters.AddWithValue("@colonia", colonia);
                cmd.Parameters.AddWithValue("@alcaldia", alcaldia);
                cmd.Parameters.AddWithValue("@cp", cp);
                cmd.Parameters.AddWithValue("@tipoSangre", tipoSangre);
                cmd.Parameters.AddWithValue("@idTutor", identificacionTutor);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // 4. ACTUALIZAR ALUMNO (GET)
        public IActionResult Update(int id)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = "SELECT * FROM ALUMNOS WHERE Boleta = @boleta";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                da.SelectCommand.Parameters.AddWithValue("@boleta", id);
                da.Fill(dt);
            }
            return View(dt);
        }

        // 5. ELIMINAR ALUMNO
        public IActionResult Delete(int id)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = "DELETE FROM ALUMNOS WHERE Boleta = @boleta";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@boleta", id);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }
    }
}