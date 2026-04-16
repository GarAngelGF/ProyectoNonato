using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoNonato.Utilidades;

namespace ProyectoNonato.Controllers
{
    // Restricción: Solo el Administrador puede gestionar el catálogo de materias
    [Authorize(Roles = "Admin")]
    public class MateriasController : Controller
    {
        // 1. LISTADO DE MATERIAS
        public IActionResult Index()
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = "SELECT * FROM MATERIAS";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                da.Fill(dt);
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
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = @"INSERT INTO MATERIAS (NombreMateria, Area, NivelAplicable) 
                                 VALUES (@nombre, @area, @nivel)";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@nombre", nombreMateria);

                // Manejo de valores nulos si no se especifican el área o el nivel
                cmd.Parameters.AddWithValue("@area", string.IsNullOrEmpty(area) ? DBNull.Value : area);
                cmd.Parameters.AddWithValue("@nivel", string.IsNullOrEmpty(nivelAplicable) ? DBNull.Value : nivelAplicable);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // 4. ACTUALIZAR MATERIA (GET)
        // Recibe un string ya que la llave primaria es NombreMateria
        public IActionResult Update(string nombreMateria)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = "SELECT * FROM MATERIAS WHERE NombreMateria = @nombre";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                da.SelectCommand.Parameters.AddWithValue("@nombre", nombreMateria);
                da.Fill(dt);
            }
            return View(dt);
        }

        // 5. ELIMINAR MATERIA
        public IActionResult Delete(string nombreMateria)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = "DELETE FROM MATERIAS WHERE NombreMateria = @nombre";