window.setDialogBackground = () => {
    console.log("Attempting to set dialog background...");

    const maxAttempts = 50; // Try for 5 seconds (50 * 100ms)
    let attempts = 0;

    const trySetBackground = () => {
        // Broader selector to find any fluent-dialog
        const dialog = document.querySelector('fluent-dialog');
        if (dialog) {
            console.log("Dialog found:", dialog);
            const shadow = dialog.shadowRoot;
            if (shadow) {
                console.log("Shadow root found:", shadow);
                const control = shadow.querySelector('div[part="control"]');
                if (control) {
                    console.log("Control part found, setting background...");
                    control.style.background = '#e0f2e9'; // Light green to match app
                    control.style.borderRadius = '20px';
                } else {
                    console.log("Control part not found in shadow root.");
                }
                const header = shadow.querySelector('div[part="header"]');
                if (header) {
                    header.style.background = 'transparent';
                    header.style.color = '#fff';
                    header.style.textShadow = '1px 1px 2px rgba(0, 0, 0, 0.3)';
                }
                const footer = shadow.querySelector('div[part="footer"]');
                if (footer) {
                    footer.style.background = 'transparent';
                    footer.style.color = '#fff';
                }
            } else {
                console.log("Shadow root not found for dialog.");
            }
        } else {
            attempts++;
            if (attempts < maxAttempts) {
                console.log(`Dialog not found, attempt ${attempts}/${maxAttempts}. Retrying in 100ms...`);
                console.log("Current DOM state:", document.body.innerHTML);
                setTimeout(trySetBackground, 100);
            } else {
                console.log("Failed to find dialog after maximum attempts.");
                console.log("Final DOM state:", document.body.innerHTML);
            }
        }
    };

    trySetBackground();
};