
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

/* ==========================================================
   SEARCH – Global navbar search (navigate between pages)
   ========================================================== */

document.addEventListener("DOMContentLoaded", () => {
    const searchInput = document.getElementById("globalSearch");

    if (!searchInput) return;

    searchInput.addEventListener("keydown", (e) => {
        if (e.key !== "Enter") return;

        e.preventDefault();
        const value = searchInput.value.trim().toLowerCase();

        switch (value) {
            case "dashboard":
                window.location.href = "/Dashboard";
                break;
            case "customers":
                window.location.href = "/Customers";
                break;
            case "accounts":
                window.location.href = "/Accounts";
                break;
            case "transactions":
                window.location.href = "/Transactions";
                break;
            case "reports":
                window.location.href = "/Reports";
                break;
            case "settings":
                window.location.href = "/Settings";
                break;
            default:
                alert("No matching page found.");
        }
    });
});