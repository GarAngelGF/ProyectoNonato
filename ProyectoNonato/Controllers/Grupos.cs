using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.Data.SqlClient;
using ProyectoNonato.Utilidades;

namespace ProyectoNonato.Controllers
{
    // El acceso general para ver el listado se permite a Admin y Consultor
    [Authorize(Roles = "Admin,Consultor")]
    public class GruposController : Controller
    {
        // 1. LISTADO DE GRUPOS
        // Accesible para el administrador y el usuario que solo consulta grupos
        public IActionResult Index()
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = "SELECT * FROM GRUPOS";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                da.Fill(dt);
            }
            return View(dt);
        }

        // 2. CREAR GRUPO (GET)
        // Solo el administrador tiene permisos de escritura
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // 3. CREAR GRUPO (POST)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(string nombreGrupo, string nivel, int capacidad)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = "INSERT INTO GRUPOS (NombreGrupo, Nivel, CapacidadMaxima) VALUES (@nombre, @nivel, @capacidad)";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@nombre", nombreGrupo);
                cmd.Parameters.AddWithValue("@nivel", nivel);
                cmd.Parameters.AddWithValue("@capacidad", capacidad);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // 4. ACTUALIZAR GRUPO (GET)
        // Requiere la llave primaria compuesta (NombreGrupo y Nivel)
        [Authorize(Roles = "Admin")]
        public IActionResult Update(string nombre, string nivel)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = "SELECT * FROM GRUPOS WHERE NombreGrupo = @nombre AND Nivel = @nivel";
                SqlDataAdapter da = new SqlDataAdapter(query, cn);
                da.SelectCommand.Parameters.AddWithValue("@nombre", nombre);
                da.SelectCommand.Parameters.AddWithValue("@nivel", nivel);
                da.Fill(dt);
            }
            return View(dt);
        }

        // 5. ELIMINAR GRUPO
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string nombre, string nivel)
        {
            using (SqlConnection cn = new SqlConnection(Conexion.CadenaSQL))
            {
                string query = "DELETE FROM GRUPOS WHERE NombreGrupo = @nombre AND Nivel = @nivel";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@nivel", nivel);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }
    }
}