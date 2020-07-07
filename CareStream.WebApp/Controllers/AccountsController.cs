using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareStream.Models;
using CareStream.Utility;
using Microsoft.AspNetCore.Mvc;
using CareStream.LoggerService;
using Microsoft.Graph;
using CareStream.WebApp.Extensions;

namespace CareStream.WebApp.Controllers
{
    public class AccountsController : Controller
    {
        private readonly ILoggerManager _logger;
        private readonly IUserService _userService;

        public AccountsController(IUserService userService, ILoggerManager logger)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var user = Ok(await _userService.GetUser(this.User.GetEmail()));
            return View(user.Value);
        }
    }
}
