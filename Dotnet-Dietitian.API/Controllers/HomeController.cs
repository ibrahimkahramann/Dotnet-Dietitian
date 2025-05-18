using Microsoft.AspNetCore.Mvc;

namespace Dotnet_Dietitian.API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult About()
        {
            return View();
        }
        
        public IActionResult Contact()
        {
            return View();
        }
        
        public IActionResult Privacy()
        {
            return View();
        }
    }
}