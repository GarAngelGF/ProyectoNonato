using Microsoft.AspNetCore.Mvc;

namespace ProyectoNonato.Controllers
{
    public class Boletas : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
