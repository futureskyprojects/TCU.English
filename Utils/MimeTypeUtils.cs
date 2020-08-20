using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TCU.English.Utils
{
    public static class MimeTypeUtils
    {

        public static class Image
        {
            // https://stackoverflow.com/questions/11063900/determine-if-uploaded-file-is-image-any-format-on-mvc
            public static bool CheckFileExtension(string filename)
            {
                var postedFileExtension = Path.GetExtension(filename);
                if (!string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            public static bool CheckContentType(string contentType)
            {
                if (!string.Equals(contentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(contentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(contentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(contentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(contentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(contentType, "image/png", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                else
                    return true;
            }
        }
    }
}
