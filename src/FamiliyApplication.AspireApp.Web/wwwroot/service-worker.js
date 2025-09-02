// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).


const cacheNamePrefix = 'offline-cache-';
const SW_VERSION = 'v1.0.0.5' 
self.addEventListener('fetch', () => { });
self.addEventListener('message', event => {
    if (event.data === 'GET_VERSION') {
        event.ports[0].postMessage(SW_VERSION);
    }
});

self.addEventListener('push', event => {
    const payload = event.data.json();
    console.log(payload);
    console.log("Webpush received - " + payload.Url + " - " + payload.Message);
    // Extract fields from NotificationDto
    const title = payload.Title || 'AppiSnappi Notification'; // Use Title if available, otherwise fallback
    const body = payload.Text || 'New notification'; // Use Text as the body
    const url = payload.Url || 'https://appisnappi.net/home'; // Fallback URL if Url is not in payload



    event.waitUntil(
        self.registration.showNotification(title, {
            body: body,
            icon: 'icons/familyapplication-icon.png',
            vibrate: [100, 50, 100],
            data: { url: url || '/'}
        })
    );

    // Notify the client (Blazor app) about the push notification
    event.waitUntil(
        self.clients.matchAll({ type: 'window', includeUncontrolled: true }).then(clients => {
            for (const client of clients) {
                client.postMessage({
                    type: 'push-notification',
                    data: payload
                });
            }
        })
    );
});

self.addEventListener('notificationclick', event => {
    event.notification.close();
    const urlToOpen = event.notification.data.url || 'https://appisnappi.net/home'; // Use the stored URL
    event.waitUntil(clients.openWindow(urlToOpen));
});

self.addEventListener('install', async event => {
    console.log('Installing service worker...');
    self.skipWaiting();
});