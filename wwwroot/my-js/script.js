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

    // Stop if the chart isn't on this page (script.js loads on every page)
    if (!canvas || typeof Chart === "undefined") return;

    const names = JSON.parse(canvas.dataset.labels);
    const values = JSON.parse(canvas.dataset.values);

    // Show the count in the legend, e.g. "Sweden (1234)"
    const labels = names.map((name, i) => `${name} (${values[i]})`);

    // Read the theme colors from CSS so the chart always matches the site
    const css = getComputedStyle(document.documentElement);
    const gold = css.getPropertyValue("--gold").trim();
    const muted = css.getPropertyValue("--muted").trim();
    const card = css.getPropertyValue("--card").trim();

    // Draws the total number of customers in the middle of the doughnut
    const centerText = {
        beforeDraw: function (chart) {
            // chart.ctx = Chart.js v3/v4, chart.chart.ctx = v2
            const ctx = chart.ctx || chart.chart.ctx;
            const area = chart.chartArea;
            if (!area) return;

            const x = (area.left + area.right) / 2;
            const y = (area.top + area.bottom) / 2;
            const total = chart.data.datasets[0].data.reduce((a, b) => a + b, 0);

            ctx.save();
            ctx.textAlign = "center";
            ctx.textBaseline = "middle";

            ctx.fillStyle = gold;
            ctx.font = "700 28px sans-serif";
            ctx.fillText(total, x, y - 8);

            ctx.fillStyle = muted;
            ctx.font = "12px sans-serif";
            ctx.fillText("customers", x, y + 16);
            ctx.restore();
        }
    };

    new Chart(canvas, {
        type: "doughnut",
        plugins: [centerText],
        data: {
            labels: labels,
            datasets: [{
                data: values,
                backgroundColor: [gold, "#4CAF50", muted, "#2E7D5B", "#B8893B"],
                borderColor: card,
                borderWidth: 2
            }]
        },
        options: {
            maintainAspectRatio: false,

            // v2 syntax (Corona's bundled Chart.js)
            cutoutPercentage: 70,
            legend: {
                position: "bottom",
                labels: { fontColor: muted }
            },

            // v3/v4 syntax (ignored by v2)
            cutout: "70%",
            plugins: {
                legend: {
                    position: "bottom",
                    labels: { color: muted }
                }
            }
        }
    });
});