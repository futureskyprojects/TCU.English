namespace TCU.English
{
    public class Notification
    {
        public int TimeOut { get; set; }
        public string Message { get; set; }
        public string Herf { get; set; }

        public static string Generate(string message, string type, int timeout, string href, string iconBefore, string theme = "metroui", string layout = "bottomRight")
        {
            return
                    "new Noty({" +
                    $"theme: '{theme}'," +
                    $"timeout: {timeout}," +
                    $"layout: '{layout}'," +
                    $"text: '{iconBefore}&nbsp;&nbsp;&nbsp;&nbsp;{message}'," +
                    $"type: '{type}'," +
                    "}).show();";
        }

        public static string Success(string message, int timeout = 3000, string href = "")
        {
            return Generate(message, nameof(Success).ToLower(), timeout, href, "<em class=\\'fa fa-check\\'></em>");
        }
        public static string Error(string message, int timeout = 3000, string href = "")
        {
            return Generate(message, nameof(Error).ToLower(), timeout, href, "<em class=\\'fa fa-times-circle\\'></em>");
        }
        public static string Warning(string message, int timeout = 3000, string href = "")
        {
            return Generate(message, nameof(Warning).ToLower(), timeout, href, "<em class=\\'icon-warning22\\'></em>");
        }
        public static string Info(string message, int timeout = 3000, string href = "")
        {
            return Generate(message, nameof(Info).ToLower(), timeout, href, "<em class=\\'fa fa-info\\'></em>");
        }
    }
}
