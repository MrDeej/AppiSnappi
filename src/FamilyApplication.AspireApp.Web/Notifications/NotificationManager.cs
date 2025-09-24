using BlazorServerCommon.Notifications;
using FamilyApplication.AspireApp.Web.CosmosDb.Notification;
using FamilyApplication.AspireApp.Web.CosmosDb.User;
using FamilyApplication.AspireApp.Web.Databuffer;
using FamilyApplication.AspireApp.Web.Sessions;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net;
using WebPush;

namespace FamilyApplication.AspireApp.Web.Notifications
{
    public class NotificationManager : NotificationManagerBase
    {
        private readonly string vapidPublicSecret;
        private readonly string vapidPrivateSecret;
        private readonly GlobalVm _globalVm;
        private readonly IJSRuntime _JSRuntime;
        private readonly SessionManager _sessionManager;
        private readonly IToastService _toastService;

        public NotificationManager(GlobalVm globalVm, IConfiguration configuration, IJSRuntime jsruntime, SessionManager sessionManager, IToastService toastService) : base()
        {
            // Load secrets from configuration
            vapidPublicSecret = configuration["Vapid:PublicSecret"]!;
            vapidPrivateSecret = configuration["Vapid:PrivateSecret"]!;
            _globalVm = globalVm;
            _JSRuntime = jsruntime;
            _sessionManager = sessionManager;
            _toastService = toastService;
            if (string.IsNullOrEmpty(vapidPublicSecret) || string.IsNullOrEmpty(vapidPrivateSecret))
            {
                throw new InvalidOperationException("VAPID secrets are not configured properly.");
            }

        }

        public async Task RequestNotificationSubscription(UserDtoDataService userDtoDataService)
        {

            var subscription = await _JSRuntime.InvokeAsync<NotificationSubscription?>("blazorPushNotifications.requestSubscription");
            var myUser = _sessionManager.GetMyUserDto();

            var allOk = true;
            if (subscription is not null && subscription != myUser.NotificationSubscription)
            {

                allOk = false;
                myUser.NotificationSubscription = subscription;
                myUser.DisableNotifications = false;
                await userDtoDataService.Save(myUser, CancellationToken.None);
                _toastService.ShowToast(ToastIntent.Info, "Varsler er aktivert");
            }
            if (myUser.DisableNotifications == true)
            {
                allOk = false;
                myUser.DisableNotifications = false;
                await userDtoDataService.Save(myUser, CancellationToken.None);
                _toastService.ShowToast(ToastIntent.Info, "Varsler er aktivert");
            }

            if (allOk)
                _toastService.ShowToast(ToastIntent.Info, "Varsler var allerede aktivert");

        }

        public async Task<string> CheckNotificationPermissionAsync()
        {
            return await _JSRuntime.InvokeAsync<string>("window.blazorPushNotifications.checkNotificationPermission");
        }

        public async Task WebPushNotify(List<NotificationDto> list, UserDtoDataService userDtoDataService, CancellationToken token)
        {
            var users = (from userDto in _globalVm.UserDtos
                         where userDto.NotificationSubscription != null && userDto.DisableNotifications != true
                         join notification in list on userDto.Id equals notification.UserId
                         into joined
                         select new
                         {
                             user = userDto,
                             notifications = joined
                         }).ToArray();

            foreach (var user in users)
            {
                foreach (var notification in user.notifications)
                    await SendToUserAsync(notification, user.user, userDtoDataService, token);
            }

        }

        private async Task SendToUserAsync(NotificationDto dto, UserDto user, UserDtoDataService userDtoDataservice, CancellationToken token)
        {
            if (user.NotificationSubscription == null)
                return;

            var pushSubscription = new PushSubscription(user.NotificationSubscription.Url, user.NotificationSubscription.P256dh, user.NotificationSubscription.Auth);
            var vapidDetails = new VapidDetails("mailto:mreirik83@gmail.com", vapidPublicSecret, vapidPrivateSecret);
            var webPushClient = new WebPushClient();

            var payload = System.Text.Json.JsonSerializer.Serialize(dto);

            try
            {
                await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails, token);
            }
            catch (WebPushException ex)
            {
                if (ex.StatusCode == HttpStatusCode.Gone || ex.Message.Contains("Subscription is no longer valid"))
                {
                    await userDtoDataservice.DisableEnableNotification(user, "System");  // Implement this method as needed
                }
                else
                {
                    throw;
                }
            }
        }


    }
}
