// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {

    const carousel = document.getElementById("performersCarousel");
    const prevButton = document.querySelector(".performer-prev");
    const nextButton = document.querySelector(".performer-next");

    if (!carousel || !prevButton || !nextButton) {
        return;
    }

    function getScrollAmount() {
        return carousel.clientWidth;
    }

    function updateButtons() {

        const maxScrollLeft =
            carousel.scrollWidth - carousel.clientWidth;

        // At the beginning
        if (carousel.scrollLeft <= 5) {
            prevButton.style.visibility = "hidden";
        } else {
            prevButton.style.visibility = "visible";
        }

        // At the end
        if (carousel.scrollLeft >= maxScrollLeft - 5) {
            nextButton.style.visibility = "hidden";
        } else {
            nextButton.style.visibility = "visible";
        }
    }


    nextButton.addEventListener("click", function () {

        carousel.scrollTo({
            left: carousel.scrollWidth - carousel.clientWidth,
            behavior: "smooth"
        });

    });


    prevButton.addEventListener("click", function () {

        carousel.scrollTo({
            left: 0,
            behavior: "smooth"
        });

    });


    carousel.addEventListener("scroll", function () {
        updateButtons();
    });


    // Set correct arrow visibility when page loads
    updateButtons();

});

// =========================================
// MS ACADEMY PWA INSTALL
// =========================================

let deferredInstallPrompt = null;

window.addEventListener("beforeinstallprompt", event => {
    event.preventDefault();
    deferredInstallPrompt = event;

    const installButton = document.getElementById("installAppBtn");

    if (installButton) {
        installButton.classList.remove("d-none");
    }
});

document.addEventListener("click", async event => {
    const installButton = event.target.closest("#installAppBtn");

    if (!installButton || !deferredInstallPrompt) {
        return;
    }

    deferredInstallPrompt.prompt();

    const { outcome } = await deferredInstallPrompt.userChoice;

    console.log("PWA install result:", outcome);

    deferredInstallPrompt = null;
    installButton.classList.add("d-none");
});

window.addEventListener("appinstalled", () => {
    const installButton = document.getElementById("installAppBtn");

    if (installButton) {
        installButton.classList.add("d-none");
    }

    deferredInstallPrompt = null;
});