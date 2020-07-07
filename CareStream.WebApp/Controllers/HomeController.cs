using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CareStream.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("UsersList", "Users");
            }
            else
            {
                return RedirectToAction("SignIn", "Account", new { area = "AzureADB2C" });
            }
        }
    }
}