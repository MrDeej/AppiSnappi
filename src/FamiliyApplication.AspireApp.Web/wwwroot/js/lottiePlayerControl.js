window.lottiePlayerControl = {
    setDirection: function (element, direction) {
        console.log("Setting direction:", direction);
        element.setDirection(direction);
    },
    play: function (element) {
        console.log("Calling play on lottie-player", {
            isPaused: element.getLottie()?.isPaused,
            isLoaded: element.getLottie()?.isLoaded,
            currentFrame: element.getLottie()?.currentFrame
        });
        element.play();
    },
    pause: function (element) {
        console.log("Calling pause on lottie-player", {
            isPaused: element.getLottie()?.isPaused,
            isLoaded: element.getLottie()?.isLoaded
        });
        element.pause();
    },
    addClickListener: function (element, dotnetRef, methodName) {
        element.addEventListener('click', async () => {
            console.log("Lottie-player clicked");
            await dotnetRef.invokeMethodAsync(methodName);
        });
    },
    isInitialized: function (element) {
        const initialized = !!element && typeof element.play === 'function';
        console.log("Is lottie-player initialized:", initialized);
        return initialized;
    },
    isPaused: function (element) {
        const isPaused = element.getLottie()?.isPaused || false;
        console.log("Is lottie-player paused:", isPaused);
        return isPaused;
    },
    isLoaded: function (element) {
        const isLoaded = element.getLottie()?.isLoaded || false;
        console.log("Is lottie-player loaded:", isLoaded);
        return isLoaded;
    },
    resetAndPlay: function (element) {
        console.log("Resetting and playing lottie-player");
        element.stop(); // Reset to start
        element.play();
    }
};