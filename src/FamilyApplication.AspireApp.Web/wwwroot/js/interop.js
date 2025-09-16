window.setCssVariables = (vars) => {
    Object.entries(vars).forEach(([key, value]) => {
        document.documentElement.style.setProperty(`--${key}`, value);
    });
};