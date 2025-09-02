window.isIOS = () => {
    const userAgent = navigator.userAgent || navigator.vendor || window.opera;
    // Check for iPhone or iPad
    return /iPad|iPhone|iPod/.test(userAgent) && !window.MSStream;
};


//window.registerServiceWorkerMessageHandler = (dotNetHelper) => {

//    if (window.isIOS()) {
//        console.log("iOS device detected. Service Worker handler will not be registered.");
//        return;
//    }

//    console.log('registerServiceWorkerMessageHandler');
//    navigator.serviceWorker.addEventListener('message', event => {
//        try {
//            console.log('message received ' + event.data?.type);
//            if (event.data?.type === 'push-notification' && dotNetHelper) {
//                dotNetHelper.invokeMethodAsync('HandlePushNotification', event.data?.data || {})
//                    .then(res => console.log('invoked HandlePushNotification - ' + res))
//                    .catch(err => console.error('Error invoking HandlePushNotification:', err));
//            }
//        } catch (error) {
//            console.error('Unhandled error in message handler:', error);
//        }
//    });

//};
window.registerServiceWorkerMessageHandler = (dotNetHelper) => {

    console.log('registerServiceWorkerMessageHandler');
    navigator.serviceWorker.addEventListener('message', event => {
        console.log('message received ' + event.data.type);
        if (event.data.type === 'push-notification') {
            dotNetHelper.invokeMethodAsync('HandlePushNotification', event.data.data)
                .then(res => console.log('invoked HandlePushNotification - ' + res))
                .catch(err => console.error('Error invoking HandlePushNotification:', err));
        }
    });
};


window.sayHello1 = (dotNetHelper) => {
    return dotNetHelper.invokeMethodAsync('GetHelloMessage');
};