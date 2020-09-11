using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TCU.English.Models;

namespace TCU.English.Utils
{
    public static class JsonUtils
    {
        public static string ToJson(this object objectData)
        {
            return JsonConvert.SerializeObject(objectData);
        }
        public static T ConvertTo<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
