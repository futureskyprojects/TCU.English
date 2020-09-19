using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TCU.English.Utils
{
    // https://stackoverflow.com/questions/36358751/how-do-you-enforce-lowercase-routing-in-asp-net-core
    public class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        public string TransformOutbound(object value)
        {
            if (value == null)
                return null;
            // Slugify value
            string result = Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1-$2").ToLower();
            result = Regex.Replace(result.ToString(), "([a-zA-Z])([0-9])", "$1-$2").ToLower();
            result = Regex.Replace(result.ToString(), "([0-9])([a-zA-Z])", "$1-$2").ToLower();
            return result;
        }
    }
}
