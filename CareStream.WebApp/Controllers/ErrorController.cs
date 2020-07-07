using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CareStream.WebApp.Controllers
{
    public class ErrorController : Controller
    {

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {

            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found";
                    break;
                case 401:
                    ViewBag.ErrorMessage = "You are not Authenticated / Authorized for the requested resource.";
                    break;
            }
            return View("NotFound");
        }

        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            ViewBag.exceptionPath = exceptionDetails.Path;
            ViewBag.exceptionMessage = exceptionDetails.Error.Message;
            ViewBag.exceptionSource = exceptionDetails.Error.Source;

            return View();
        }
    }
}