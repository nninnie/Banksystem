/* ==========================================================
   APP-LAUNCHER
   ========================================================== */
document.addEventListener("DOMContentLoaded", function () {
    const launcher = document.getElementById("app-launcher");
    const launcherBtn = document.getElementById("app-launcher-btn");

    if (!launcher || !launcherBtn) {
        console.error("App-launcher: element saknas.");
        return;
    }

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
                window.location.href = "/Admin/Dashboard";
                break;
            case "customers":
                window.location.href = "/Admin/Customers";
                break;
            case "users":
            case "user admin":
                window.location.href = "/Admin/UserAdmin";
                break;
            default:
                alert("No matching page found.");
        }
    });
});


/* ==========================================================
   DASHBOARD - CUSTOMER PER COUNTRY 
   ========================================================== */

document.addEventListener("DOMContentLoaded", function () {

    const canvas = document.getElementById("customersChart");
    if (!canvas || typeof Chart === "undefined") return;

    const values = JSON.parse(canvas.dataset.values);
    const labels = JSON.parse(canvas.dataset.labels);
    const flagEls = canvas
        .closest(".dashboard-card")
        .querySelectorAll(".chart-flags .flag-item");

    const fmt = (n) => Number(n).toLocaleString("sv-SE");

    Chart.defaults.global.defaultFontFamily = "Inter, system-ui, sans-serif";

    // Ritar staplar med rundade hörn upptill + värde ovanför
    const roundedBars = {
        afterDatasetsDraw: function (chart) {
            const ctx = chart.ctx;
            const area = chart.chartArea;
            const meta = chart.getDatasetMeta(0);

            const gradient = ctx.createLinearGradient(0, area.bottom, 0, area.top);
            gradient.addColorStop(0, "rgba(14,74,58,.35)");
            gradient.addColorStop(1, "#2FA574");

            meta.data.forEach(function (bar, i) {
                const m = bar._model;
                const w = m.width;
                const x = m.x - w / 2;
                const top = m.y;
                const bottom = m.base;
                const r = Math.min(10, w / 2, (bottom - top) / 2);

                ctx.save();
                ctx.beginPath();
                ctx.moveTo(x, bottom);
                ctx.lineTo(x, top + r);
                ctx.quadraticCurveTo(x, top, x + r, top);
                ctx.lineTo(x + w - r, top);
                ctx.quadraticCurveTo(x + w, top, x + w, top + r);
                ctx.lineTo(x + w, bottom);
                ctx.closePath();
                ctx.fillStyle = gradient;
                ctx.fill();
                ctx.restore();

                ctx.save();
                ctx.fillStyle = "#F8FAFC";
                ctx.font = "600 12px Inter, sans-serif";
                ctx.textAlign = "center";
                ctx.fillText(fmt(values[i]), m.x, top - 10);
                ctx.restore();
            });
        },

        // Flaggor centrerade under varje stapel
        afterDraw: function (chart) {
            chart.getDatasetMeta(0).data.forEach(function (bar, i) {
                if (flagEls[i]) flagEls[i].style.left = bar._model.x + "px";
            });
        }
    };

    new Chart(canvas, {
        type: "bar",
        plugins: [roundedBars],

        data: {
            labels: Array(labels.length).fill(""),
            datasets: [{
                label: "",
                data: values,
                backgroundColor: "rgba(0,0,0,0)",
                borderWidth: 0,
                barPercentage: 0.8,
                categoryPercentage: 0.7
            }]
        },

        options: {
            responsive: true,
            maintainAspectRatio: false,
            layout: { padding: { top: 28 } },
            legend: { display: false },

            tooltips: {
                callbacks: {
                    label: function (item) {
                        return " " + fmt(item.yLabel) + " customers";
                    }
                }
            },


        }
    });
});