window.downloadFile = (fileName, contentType, base64Data) => {
    const byteCharacters = atob(base64Data);
    const byteNumbers = new Array(byteCharacters.length);

    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }

    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: contentType });
    const url = URL.createObjectURL(blob);

    const link = document.createElement("a");
    link.href = url;
    link.download = fileName;
    link.style.display = "none";

    document.body.appendChild(link);
    link.click();
    link.remove();

    URL.revokeObjectURL(url);
};

window.shareReportImage = async (fileName, base64Data) => {
    const byteCharacters = atob(base64Data);
    const byteNumbers = new Array(byteCharacters.length);

    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }

    const byteArray = new Uint8Array(byteNumbers);

    const blob = new Blob(
        [byteArray],
        { type: "image/png" }
    );

    const file = new File(
        [blob],
        fileName,
        { type: "image/png" }
    );

    if (
        navigator.share &&
        navigator.canShare &&
        navigator.canShare({ files: [file] })
    ) {
        await navigator.share({
            title: "Class Report",
            text: "Class Report",
            files: [file]
        });

        return true;
    }

    const url = URL.createObjectURL(blob);

    const link = document.createElement("a");
    link.href = url;
    link.download = fileName;
    link.style.display = "none";

    document.body.appendChild(link);
    link.click();
    link.remove();

    URL.revokeObjectURL(url);

    return false;
};

/* =========================================
   MS ACADEMY PWA INSTALL
   ========================================= */

let deferredInstallPrompt = null;

window.addEventListener("beforeinstallprompt", event => {
    event.preventDefault();
    deferredInstallPrompt = event;

    const installButton = document.getElementById("installAppBtn");

    if (installButton) {
        installButton.style.display = "block";
    }
});

window.installAcademyApp = async function () {
    if (!deferredInstallPrompt) {
        return;
    }

    deferredInstallPrompt.prompt();

    const { outcome } = await deferredInstallPrompt.userChoice;

    console.log("PWA install result:", outcome);

    deferredInstallPrompt = null;

    const installButton = document.getElementById("installAppBtn");

    if (installButton) {
        installButton.style.display = "none";
    }
};

window.addEventListener("appinstalled", () => {
    const installButton = document.getElementById("installAppBtn");

    if (installButton) {
        installButton.style.display = "none";
    }

    deferredInstallPrompt = null;
});

window.isIOSDevice = function () {
    return /iPad|iPhone|iPod/.test(navigator.userAgent) ||
        (navigator.platform === "MacIntel" && navigator.maxTouchPoints > 1);
};

window.isPWAInstalled = function () {
    return window.matchMedia("(display-mode: standalone)").matches ||
        window.navigator.standalone === true;
};
window.showIOSInstallInstructions = function () {
    const instructions = document.getElementById("iosInstallInstructions");

    if (instructions) {
        instructions.style.display = "block";
    }
};
window.hideInstallButton = function () {
    const installButton = document.getElementById("installAppBtn");

    if (installButton) {
        installButton.style.display = "none";
    }
};