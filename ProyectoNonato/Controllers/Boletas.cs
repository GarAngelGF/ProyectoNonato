using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoNonato.Utilidades;
using System;

namespace ProyectoNonato.Controllers
{
    // Restricción: Solo el Administrador puede ver y gestionar las boletas
    [Authorize(Roles = "Admin")]
    public class BoletasController : Controller
    {
        // 1. VISTA PRINCIPAL (LISTADO)
        public IActionResult Index()
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                // Unimos con ALUMNOS para mostrar el nombre completo del estudiante en la tabla
                string query = @"SELECT B.*, A.Nombre + ' ' + A.ApellidoPaterno AS NombreAlumno 
                                 FROM BOLETAS B 
                                 JOIN ALUMNOS A ON B.Boleta = A.Boleta";

                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                da.Fill(dt);
            }
            return View(dt);
        }

        // 2. CREAR BOLETA (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. CREAR BOLETA (POST)
        [HttpPost]
        public IActionResult Create(int boleta, string nombreGrupo, string nivel, string ciclo, DateTime fechaEmision, string observaciones, decimal promedio, int faltas)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = @"INSERT INTO BOLETAS (Boleta, NombreGrupo, Nivel, Ciclo, FechaEmision, ObservacionesGenerales, PromedioGeneral, Faltas) 
                                 VALUES (@boleta, @nombreGrupo, @nivel, @ciclo, @fechaEmision, @observaciones, @promedio, @faltas)";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@boleta", boleta);
                cmd.Parameters.AddWithValue("@nombreGrupo", nombreGrupo);
                cmd.Parameters.AddWithValue("@nivel", nivel);
                cmd.Parameters.AddWithValue("@ciclo", ciclo);

                // Manejo de nulos si las fechas u observaciones no son obligatorias
                cmd.Parameters.AddWithValue("@fechaEmision", fechaEmision == default(DateTime) ? DBNull.Value : fechaEmision);
                cmd.Parameters.AddWithValue("@observaciones", string.IsNullOrEmpty(observaciones) ? DBNull.Value : observaciones);
                cmd.Parameters.AddWithValue("@promedio", promedio);
                cmd.Parameters.AddWithValue("@faltas", faltas);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // 4. ACTUALIZAR BOLETA (GET)
        // Se requieren los 4 campos de la llave primaria compuesta para encontrar el registro exacto
        public IActionResult Update(int boleta, string grupo, string nivel, string ciclo)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = @"SELECT * FROM BOLETAS 
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

        // 5. ELIMINAR BOLETA
        // Al igual que el Update, el Delete necesita la llave primaria compuesta completa
        public IActionResult Delete(int boleta, string grupo, string nivel, string ciclo)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = @"DELETE FROM BOLETAS 
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