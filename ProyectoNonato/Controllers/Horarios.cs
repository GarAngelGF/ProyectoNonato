using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoNonato.Utilidades;
using System;

namespace ProyectoNonato.Controllers
{
    // Restricción: Solo el Administrador puede gestionar la infraestructura de horarios
    [Authorize(Roles = "Admin")]
    public class HorariosController : Controller
    {
        private string GetConnectionString()
        {
            var user = User.Identity?.Name ?? "";
            var pass = User.Claims.FirstOrDefault(c => c.Type == "UserPass")?.Value ?? "";
            return Conexion.GenerarCadenaDinamica(user, pass);
        }

        // 1. LISTADO GENERAL DE HORARIOS
        public IActionResult Index()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    // Consulta que une tablas para mostrar nombres en lugar de solo IDs/Cédulas
                    string query = @"SELECT H.*, M.NombreMateria, P.Nombre + ' ' + P.ApellidoPaterno AS Profesor
                                     FROM HORARIOS H
                                     JOIN MATERIAS M ON H.NombreMateria = M.NombreMateria
                                     JOIN PROFESORES P ON H.CedulaProfesional = P.CedulaProfesional";

                    SqlDataAdapter da = new SqlDataAdapter(query, cn);
                    da.Fill(dt);
                }
            }
            catch (SqlException ex)
            {
                ViewBag.Error = "Acceso denegado a la información de horarios. Verifique sus permisos.";
            }
            return View(dt);
        }

        // 2. CREAR HORARIO (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. CREAR HORARIO (POST)
        [HttpPost]
        public IActionResult Create(string dia, TimeSpan horaInicio, TimeSpan horaFin, string edificio, string aula, string grupo, string nivel, string materia, string cedula)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    string query = @"INSERT INTO HORARIOS (Dia, HoraInicio, HoraFin, Edificio, Aula, NombreGrupo, Nivel, NombreMateria, CedulaProfesional) 
                                     VALUES (@dia, @inicio, @fin, @edificio, @aula, @grupo, @nivel, @materia, @cedula)";

                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@dia", dia);
                    cmd.Parameters.AddWithValue("@inicio", horaInicio);
                    cmd.Parameters.AddWithValue("@fin", horaFin);
                    cmd.Parameters.AddWithValue("@edificio", edificio);
                    cmd.Parameters.AddWithValue("@aula", aula);
                    cmd.Parameters.AddWithValue("@grupo", grupo);
                    cmd.Parameters.AddWithValue("@nivel", nivel);
                    cmd.Parameters.AddWithValue("@materia", materia);
                    cmd.Parameters.AddWithValue("@cedula", cedula);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch (SqlException ex)
            {
                ViewBag.Error = "No tiene permisos para crear horarios.";
                return View();
            }
        }

        // 4. ACTUALIZAR HORARIO (GET)
        // Se utiliza la llave primaria compuesta (Dia y HoraInicio)
        public IActionResult Update(string dia, TimeSpan inicio)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    string query = "SELECT * FROM HORARIOS WHERE Dia = @dia AND HoraInicio = @inicio";
                    SqlDataAdapter da = new SqlDataAdapter(query, cn);
                    da.SelectCommand.Parameters.AddWithValue("@dia", dia);
                    da.SelectCommand.Parameters.AddWithValue("@inicio", inicio);
                    da.Fill(dt);
                }
            }
            catch (SqlException ex)
            {
                ViewBag.Error = "Acceso denegado al horario.";
            }
            return View(dt);
        }

        // 5. ELIMINAR HORARIO
        public IActionResult Delete(string dia, TimeSpan inicio)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(GetConnectionString()))
                {
                    string query = "DELETE FROM HORARIOS WHERE Dia = @dia AND HoraInicio = @inicio";
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Parameters.AddWithValue("@dia", dia);
                    cmd.Parameters.AddWithValue("@inicio", inicio);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("Index");
            }
            catch (SqlException ex)
            {
                ViewBag.Error = "No tiene permisos para eliminar horarios.";
                return RedirectToAction("Index");
            }
        }
    }
}