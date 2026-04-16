using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoNonato.Utilidades;
using System;

namespace ProyectoNonato.Controllers
{
    // Restricción: Solo el Administrador puede gestionar las inscripciones de los alumnos
    [Authorize(Roles = "Admin")]
    public class InscripcionesController : Controller
    {
        // 1. LISTADO GENERAL DE INSCRIPCIONES
        public IActionResult Index()
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                // Unimos con la tabla ALUMNOS para mostrar el nombre del estudiante además de su boleta
                string query = @"SELECT I.*, A.Nombre + ' ' + A.ApellidoPaterno AS NombreAlumno 
                                 FROM INSCRIPCIONES I
                                 JOIN ALUMNOS A ON I.Boleta = A.Boleta";

                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                da.Fill(dt);
            }
            return View(dt);
        }

        // 2. CREAR INSCRIPCIÓN (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. CREAR INSCRIPCIÓN (POST)
        [HttpPost]
        public IActionResult Create(int boleta, string nombreGrupo, string nivel, string ciclo, DateTime fechaInscripcion, string estatus, bool documentosCompletos)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = @"INSERT INTO INSCRIPCIONES (Boleta, NombreGrupo, Nivel, Ciclo, FechaInscripcion, Estatus, DocumentosCompletos) 
                                 VALUES (@boleta, @grupo, @nivel, @ciclo, @fecha, @estatus, @docs)";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@boleta", boleta);
                cmd.Parameters.AddWithValue("@grupo", nombreGrupo);
                cmd.Parameters.AddWithValue("@nivel", nivel);
                cmd.Parameters.AddWithValue("@ciclo", ciclo);

                // Si la fecha no se envía, asignamos nulo o la fecha actual según convenga
                cmd.Parameters.AddWithValue("@fecha", fechaInscripcion == default(DateTime) ? DBNull.Value : fechaInscripcion);
                cmd.Parameters.AddWithValue("@estatus", string.IsNullOrEmpty(estatus) ? "Inscrito" : estatus);

                // En SQL Server, un booleano de C# (true/false) se mapea directamente al tipo BIT (1/0)
                cmd.Parameters.AddWithValue("@docs", documentosCompletos);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // 4. ACTUALIZAR INSCRIPCIÓN (GET)
        // Requiere los 4 campos de la llave primaria compuesta para localizar el registro exacto
        public IActionResult Update(int boleta, string grupo, string nivel, string ciclo)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = @"SELECT * FROM INSCRIPCIONES 
                                 WHERE Boleta = @boleta AND NombreGrupo = @grupo AND Nivel = @nivel AND Ciclo = @ciclo";

                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                da.SelectCommand.Parameters.AddWithValue("@boleta", boleta);
                da.SelectCommand.Parameters.AddWithValue("@grupo", grupo);
                da.SelectCommand.Parameters.AddWithValue("@nivel", nivel);
                da.SelectCommand.Parameters.AddWithValue("@ciclo", ciclo);
                da.Fill(dt);
            }
            return View(dt);
        }

        // 5. ELIMINAR INSCRIPCIÓN
        // Requiere los 4 campos de la llave primaria compuesta
        public IActionResult Delete(int boleta, string grupo, string nivel, string ciclo)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = @"DELETE FROM INSCRIPCIONES 
                                 WHERE Boleta = @boleta AND NombreGrupo = @grupo AND Nivel = @nivel AND Ciclo = @ciclo";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@boleta", boleta);
                cmd.Parameters.AddWithValue("@grupo", grupo);
                cmd.Parameters.AddWithValue("@nivel", nivel);
                cmd.Parameters.AddWithValue("@ciclo", ciclo);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }
    }
}