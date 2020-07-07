using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareStream.LoggerService;
using CareStream.Models;
using CareStream.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CareStream.WebApp.Controllers
{
    public class UserAttributeController : BaseController
    {
        private readonly ILoggerManager _logger;
        private readonly IUserAttributeService _userAttributeService;

        public UserAttributeController(IUserAttributeService userAttributeService, ILoggerManager logger)
        {
            _userAttributeService = userAttributeService;
            _logger = logger;
        }

        public async Task<IActionResult> List()
        {
            var userAttributes = await _userAttributeService.GetUserAttribute();
            BuildViewUserAttributes();
            userAttributes.TargetObjects = new List<String>()
                {
                    "User"
                };
            return View(userAttributes);
        }

        public async Task<IActionResult> Create(UserAttributeModel model)
        {

            await _userAttributeService.UpsertUserAttributes(model);
            return RedirectToAction("List");
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(List<ExtensionModel> models)
        //{
        //    //   await _userAttributeService.UpsertUserAttributes(models.First());

        //    BuildViewUserAttributes();

        //    TempData["UserAttrs"] = models;

        //    return View();
        //}

        private void BuildViewUserAttributes()
        {
            var dataTypes = new SelectList(new List<SelectListItem>());
            var itemList = new List<SelectListItem>();
            var groupTypeListItem = new SelectListItem
            {
                Text = "String",
                Value = "String"
            };

            itemList.Add(groupTypeListItem);
            groupTypeListItem = new SelectListItem
            {
                Text = "Boolean",
                Value = "Boolean"
            };

            itemList.Add(groupTypeListItem);
            groupTypeListItem = new SelectListItem
            {
                Text = "Integer",
                Value = "Integer"
            };

            itemList.Add(groupTypeListItem);


            dataTypes = new SelectList(itemList, "Value", "Text");
            ViewData["DataType"] = dataTypes;
        }
    }
}