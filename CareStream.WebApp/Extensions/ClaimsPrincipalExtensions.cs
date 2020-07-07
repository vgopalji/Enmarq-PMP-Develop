using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CareStream.WebApp.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserFullName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.GivenName) + " " + principal.FindFirstValue(ClaimTypes.Surname);
        }
        public static string GetEmail(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue("emails");
        }

        public static string GetUserId(this ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
