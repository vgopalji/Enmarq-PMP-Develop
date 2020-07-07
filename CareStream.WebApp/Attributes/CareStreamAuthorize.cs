using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CareStream.WebApp.Attributes
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CareStreamAuthorizeAttribute : TypeFilterAttribute
    {
        public CareStreamAuthorizeAttribute(string claimType, string claimValue) : base(typeof(CareStreamAuthorizeFilter))
        {
            Arguments = new object[] { new Claim(claimType, claimValue) };
        }
    }

    public class CareStreamAuthorizeFilter : IAuthorizationFilter
    {
        readonly Claim _claim;

        public CareStreamAuthorizeFilter(Claim claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string claimValue = null;
            switch (_claim.Type)
            {
                case "nameidentifier":
                    claimValue= context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                    break;
                case "emails":
                    claimValue = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                    break;
                default:
                    break;
            }

            if (string.IsNullOrWhiteSpace(claimValue))
                context.Result = new RedirectResult("~/Error/Error");
        }
    }
}
