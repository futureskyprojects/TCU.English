using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace TCU.English
{
    public static class NotificationComponent
    {
        /// <summary>
        /// Render các thông báo nếu có ra trang HTML
        /// </summary>
        public static HtmlString ShowNotifications(this IHtmlHelper<dynamic> helper)
        {

            // Khởi tạo biến chứa danh sách thông báo
            List<string> notifications = new List<string>();

            // Lấy danh sách các thông báo đang có
            if (helper.TempData[nameof(Notification)] != null)
                notifications.AddRange((string[])helper.TempData[nameof(Notification)]);

            // Nếu trống thì bỏ qua
            if (notifications == null || notifications.Count <= 0)
                return HtmlString.Empty;

            // Không thì cho hiển thị
            return new HtmlString("<script>\r\n" +
                string.Join("\r\n", notifications) +
                "\r\n</script>");
        }

        private static List<string> CollectNotification(this Controller controller)
        {
            // Khởi tạo biến chứa danh sách thông báo
            List<string> notifications = new List<string>();

            // Kiểm tra mục TempData xem có thông báo nào còn được lưu trữ không
            if (controller.TempData[nameof(Notification)] != null)
                notifications.AddRange((string[])controller.TempData[nameof(Notification)]);

            // Xóa bỏ các thông báo đã có
            controller.TempData[nameof(Notification)] = null;

            // Trả về kết quả
            return notifications;
        }

        /// <summary>
        /// Thêm một thông báo ở mức độ tùy chỉnh cao
        /// </summary>
        public static void AddNotification(this Controller controller, params string[] Notifications)
        {
            if (Notifications == null || Notifications.Length <= 0)
                throw new Exception("Không thể để nội dung thông báo rỗng được");

            // Lấy danh sách thông báo đã có
            List<string> notifications = CollectNotification(controller) ?? new List<string>();

            // Thêm thông báo mới vào
            notifications.AddRange(Notifications);

            // Lưu lại dữ liệu
            controller.TempData[nameof(Notification)] = notifications.ToArray();
        }

        /// <summary>
        /// Tạo một thông báo kiểu SUCCESS
        /// </summary>
        public static Controller NotifySuccess(this Controller controller, string message)
        {
            AddNotification(controller, Notification.Success(message));
            return controller;
        }

        /// <summary>
        /// Tạo một thông báo kiểu WARNING
        /// </summary>
        public static Controller NotifyWarning(this Controller controller, string message)
        {
            AddNotification(controller, Notification.Warning(message));
            return controller;
        }

        /// <summary>
        /// Tạo một thông báo kiểu ERROR
        /// </summary>
        public static Controller NotifyError(this Controller controller, string message)
        {
            AddNotification(controller, Notification.Error(message));
            return controller;
        }

        /// <summary>
        /// Tạo một thông báo kiểu INFO
        /// </summary>
        public static Controller NotifyInfo(this Controller controller, string message)
        {
            AddNotification(controller, Notification.Info(message));
            return controller;
        }
    }
}
