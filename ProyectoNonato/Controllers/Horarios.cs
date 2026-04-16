using Microsoft.AspNetCore.Mvc;

namespace ProyectoNonato.Controllers
{
    public class Horarios : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
