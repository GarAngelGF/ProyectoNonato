using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoNonato.Utilidades;
using System;

namespace ProyectoNonato.Controllers
{
    // Solo el Administrador tiene permiso para gestionar las calificaciones detalladas
    [Authorize(Roles = "Admin")]
    public class DetalleBoletaController : Controller
    {
        // 1. LISTADO GENERAL
        public IActionResult Index()
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                // Consulta que trae el detalle y el nombre de la materia para mejor visualización
                string query = "SELECT * FROM DETALLE_BOLETA";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                da.Fill(dt);
            }
            return View(dt);
        }

        // 2. AGREGAR CALIFICACIÓN (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. AGREGAR CALIFICACIÓN (POST)
        [HttpPost]
        public IActionResult Create(int boleta, string nombreGrupo, string nivel, string ciclo, string nombreMateria, decimal calificacion, string observaciones)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = @"INSERT INTO DETALLE_BOLETA (Boleta, NombreGrupo, Nivel, Ciclo, NombreMateria, Calificacion, ObservacionesMateria) 
                                 VALUES (@boleta, @grupo, @nivel, @ciclo, @materia, @calif, @obs)";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@boleta", boleta);
                cmd.Parameters.AddWithValue("@grupo", nombreGrupo);
                cmd.Parameters.AddWithValue("@nivel", nivel);
                cmd.Parameters.AddWithValue("@ciclo", ciclo);
                cmd.Parameters.AddWithValue("@materia", nombreMateria);
                cmd.Parameters.AddWithValue("@calif", calificacion);
                cmd.Parameters.AddWithValue("@obs", string.IsNullOrEmpty(observaciones) ? DBNull.Value : observaciones);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // 4. ACTUALIZAR CALIFICACIÓN (GET)
        // Se requieren los 5 parámetros de la PK compuesta para identificar la calificación única
        public IActionResult Update(int boleta, string grupo, string nivel, string ciclo, string materia)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = @"SELECT * FROM DETALLE_BOLETA 
                                 WHERE Boleta = @boleta AND NombreGrupo = @grupo AND Nivel = @nivel 
                                 AND Ciclo = @ciclo AND NombreMateria = @materia";

                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                da.SelectCommand.Parameters.AddWithValue("@boleta", boleta);
                da.SelectCommand.Parameters.AddWithValue("@grupo", grupo);
                da.SelectCommand.Parameters.AddWithValue("@nivel", nivel);
                da.SelectCommand.Parameters.AddWithValue("@ciclo", ciclo);
                da.SelectCommand.Parameters.AddWithValue("@materia", materia);
                da.Fill(dt);
            }
            return View(dt);
        }

        // 5. ELIMINAR REGISTRO
        public IActionResult Delete(int boleta, string grupo, string nivel, string ciclo, string materia)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = @"DELETE FROM DETALLE_BOLETA 
                                 WHERE Boleta = @boleta AND NombreGrupo = @grupo AND Nivel = @nivel 
                                 AND Ciclo = @ciclo AND NombreMateria = @materia";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@boleta", boleta);
                cmd.Parameters.AddWithValue("@grupo", grupo);
                cmd.Parameters.AddWithValue("@nivel", nivel);
                cmd.Parameters.AddWithValue("@ciclo", ciclo);
                cmd.Parameters.AddWithValue("@materia", materia);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }
    }
}