using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CareStream.WebApp.Attributes;
using CareStream.WebApp.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CareStream.WebApp.Controllers
{
    [CareStreamAuthorize("nameidentifier", "")]
    public class BaseController : Controller
    {
        public void ShowSuccessMessage(string message)
        {
            TempData["UserMessage"] = message;
            TempData["UserMsgColor"] = "green";
        }

        public void ShowErrorMessage(string message)
        {
            TempData["UserMessage"] = message;
            TempData["UserMsgColor"] = "red";
        }

        public object GetSuccessMessage(string message)
        {
            return new { Message = message, MsgColor = "green" };
        }

        public object GetErrorMessage(string message)
        {
            return new { Message = message, MsgColor = "red" };
        }

        public string GetUserId()
        {
            return HttpContext.User.GetUserId();
        }
    }
}
