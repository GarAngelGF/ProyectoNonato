using Microsoft.AspNetCore.Mvc;

namespace ProyectoNonato.Controllers
{
    public class Materias : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
