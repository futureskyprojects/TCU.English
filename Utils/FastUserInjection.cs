using System;
using System.Linq;
using System.Security.Claims;

namespace TCU.English.Utils
{
    public static class FastUserInjection
    {
        public static string ExtractValue(this ClaimsPrincipal claims, string claimTypes)
        {
            try
            {
                return claims.Claims.Where(it => it.Type == claimTypes).FirstOrDefault()?.Value ?? "";
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
