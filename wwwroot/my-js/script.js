
/* ==========================================================
   APP-LAUNCHER – öppna/stäng rutnätsmenyn
   ========================================================== */
document.addEventListener("DOMContentLoaded", function () {
    const launcher = document.getElementById("app-launcher");
    const launcherBtn = document.getElementById("app-launcher-btn");

    if (!launcher || !launcherBtn) {
        console.error("App-launcher: element saknas.");
        return;
    }

    // Gör hela ikonen klickbar
    launcherBtn.style.cursor = "pointer";

    launcherBtn.addEventListener("click", function (e) {
        e.preventDefault();
        e.stopPropagation();

        launcher.classList.toggle("hidden");
    });

    launcher.addEventListener("click", function (e) {
        e.stopPropagation();
    });

    document.addEventListener("click", function () {
        launcher.classList.add("hidden");
    });
});
