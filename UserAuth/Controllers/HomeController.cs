using BE;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace UserAuth.Controllers
{
    public class HomeController : Controller
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public HomeController()
        {
        }

        public IActionResult Index()
        {
            Logger.Info("Return Homepage from HomeController::Index");
            return View();
        }

        public IActionResult Privacy()
        {
            Logger.Info("Return Privacy page from HomeController::Privacy");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            Logger.Info("Return Error page from HomeController::Error");

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
