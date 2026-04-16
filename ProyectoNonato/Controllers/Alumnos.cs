using Microsoft.AspNetCore.Mvc;

namespace ProyectoNonato.Controllers
{
    public class Alumnos : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
