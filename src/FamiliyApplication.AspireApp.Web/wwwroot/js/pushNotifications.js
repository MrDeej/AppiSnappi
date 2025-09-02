(function () {
    // Note: Replace with your own key pair before deploying
    const applicationServerPublicKey = 'BFGwCw64gOVp43JLs8OnOcGhuwkM1SRMdWsurzIedhAjOVlfOfZdzCS4Di_z_1Aei8csIxr7SmFrtvQvvLxTUnw';

    window.blazorPushNotifications = {
        requestSubscription: async () => {
            const worker = await navigator.serviceWorker.getRegistration();
            const existingSubscription = await worker.pushManager.getSubscription();

            if (existingSubscription != null)
                return {
                    url: existingSubscription.endpoint,
                    p256dh: arrayBufferToBase64(existingSubscription.getKey('p256dh')),
                    auth: arrayBufferToBase64(existingSubscription.getKey('auth'))
                }

            const newSubscription = await subscribe(worker);
            if (newSubscription) {
                return {
                    url: newSubscription.endpoint,
                    p256dh: arrayBufferToBase64(newSubscription.getKey('p256dh')),
                    auth: arrayBufferToBase64(newSubscription.getKey('auth'))
                };
            }

        },
        checkNotificationPermission: () => {
            const permission = Notification.permission;
            switch (permission) {
                case "default":
                    return "default"; // User has not granted or denied permissions.
                case "granted":
                    return "granted"; // User has allowed notifications.
                case "denied":
                    return "denied"; // User has blocked notifications.
                default:
                    return "unknown"; // Fallback for unexpected states.
            }
        }
    };

    window.blazorPushNotification = {
        showCustomNotificationPrompt: async () => {
            const promptContainer = document.createElement('div');
            promptContainer.style.position = 'fixed';
            promptContainer.style.bottom = '20px';
            promptContainer.style.right = '20px';
            promptContainer.style.padding = '15px';
            promptContainer.style.background = '#f1f1f1';
            promptContainer.style.border = '1px solid #ccc';
            promptContainer.style.borderRadius = '5px';
            promptContainer.style.boxShadow = '0 4px 6px rgba(0,0,0,0.1)';
            promptContainer.innerHTML = `
        <p>Stay updated! Enable push notifications to receive important updates.</p>
        <button id="enableNotifications" style="margin-right: 10px;">Enable Notifications</button>
        <button id="dismissPrompt">Dismiss</button>
    `;

            document.body.appendChild(promptContainer);

            document.getElementById('enableNotifications').addEventListener('click', async () => {
                await window.blazorPushNotifications.requestSubscription();
                document.body.removeChild(promptContainer);
            });

            document.getElementById('dismissPrompt').addEventListener('click', () => {
                document.body.removeChild(promptContainer);
            });

        }
    };

    async function subscribe(worker) {
        try {
            return await worker.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: applicationServerPublicKey
            });
        } catch (error) {
            if (error.name === 'NotAllowedError') {
                return null;
            }
            throw error;
        }
    }

    function arrayBufferToBase64(buffer) {
        // https://stackoverflow.com/a/9458996
        var binary = '';
        var bytes = new Uint8Array(buffer);
        var len = bytes.byteLength;
        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    }


})();

