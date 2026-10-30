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

    const pages = {
        dashboard: "/Admin/Dashboard",

        customers: "/Admin/Customers",
        customer: "/Admin/Customers",

        country: "/Country",
        countries: "/Country",

        login: "/Login",
        logout: "/Logout",

        home: "/",
        index: "/"
    };

    searchInput.addEventListener("keydown", function (e) {

        if (e.key !== "Enter") return;

        e.preventDefault();

        const term = this.value.trim().toLowerCase();

        for (const key in pages) {

            if (term.includes(key)) {
                window.location.href = pages[key];
                return;
            }
        }

        alert("No matching page found.");
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




/* ==========================================================
   CUSTOMER CARD - TOTAL BALANCE
   ========================================================== */


document.addEventListener("DOMContentLoaded", () => {
    const canvas = document.getElementById("balanceChart");

    if (!canvas) return;

    const ctx = canvas.getContext("2d");

    function drawChart() {
        const rect = canvas.getBoundingClientRect();

        if (rect.width === 0 || rect.height === 0) return;

        const dpr = window.devicePixelRatio || 1;

        canvas.width = rect.width * dpr;
        canvas.height = rect.height * dpr;

        ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
        ctx.clearRect(0, 0, rect.width, rect.height);

        const width = rect.width;
        const height = rect.height;

        // Kurvans punkter
        const points = [
            [0.00, 0.78],
            [0.12, 0.64],
            [0.23, 0.70],
            [0.36, 0.40],
            [0.48, 0.52],
            [0.60, 0.25],
            [0.72, 0.38],
            [0.84, 0.15],
            [1.00, 0.28]
        ].map(([x, y]) => ({
            x: x * width,
            y: y * height
        }));

        // Funktion för att rita den mjuka kurvan
        function drawCurve() {
            ctx.beginPath();
            ctx.moveTo(points[0].x, points[0].y);

            for (let i = 0; i < points.length - 1; i++) {
                const current = points[i];
                const next = points[i + 1];

                const midX = (current.x + next.x) / 2;
                const midY = (current.y + next.y) / 2;

                ctx.quadraticCurveTo(
                    current.x,
                    current.y,
                    midX,
                    midY
                );
            }

            const last = points[points.length - 1];
            ctx.lineTo(last.x, last.y);
        }

        const last = points[points.length - 1];

        // Grön gradient under kurvan
        drawCurve();
        ctx.lineTo(width, height);
        ctx.lineTo(0, height);
        ctx.closePath();

        const fillGradient = ctx.createLinearGradient(
            0, 0, 0, height
        );

        fillGradient.addColorStop(
            0, "rgba(32, 184, 121, 0.24)"
        );
        fillGradient.addColorStop(
            1, "rgba(32, 184, 121, 0)"
        );

        ctx.fillStyle = fillGradient;
        ctx.fill();

        // Grön till guld på själva linjen
        drawCurve();

        const lineGradient = ctx.createLinearGradient(
            0, 0, width, 0
        );

        lineGradient.addColorStop(0, "#16865B");
        lineGradient.addColorStop(1, "#F2D675");

        ctx.strokeStyle = lineGradient;
        ctx.lineWidth = 2;
        ctx.lineCap = "round";
        ctx.lineJoin = "round";
        ctx.stroke();
    }

    drawChart();

    // Rita om kurvan när fönstrets storlek ändras
    window.addEventListener("resize", drawChart);
});


/* ==========================================================
   START PAGE - COUNTRY MAP
   ========================================================== */
document.addEventListener("DOMContentLoaded", function () {

    const mapEl = document.getElementById("countryMap");

    // Only runs on the start page, where the map and its library are loaded
    if (!mapEl || typeof jQuery === "undefined" || !jQuery.fn.vectorMap) return;

    const countries = JSON.parse(mapEl.dataset.countries);
    const fmt = (n) => Number(n).toLocaleString("sv-SE");

    // Marker position per country (the capital city)
    const capitals = {
        SE: [59.33, 18.07],
        NO: [59.91, 10.75],
        DK: [55.68, 12.57],
        FI: [60.17, 24.94]
    };

    // Look up a country by its code, e.g. byCode["SE"]
    const byCode = {};
    countries.forEach(c => byCode[c.code] = c);

    // Countries the bank has customers in are highlighted
    const activeRegions = {};
    countries.forEach(c => activeRegions[c.code] = "active");

    const markers = countries
        .filter(c => capitals[c.code])
        .map(c => ({ latLng: capitals[c.code], name: c.name, code: c.code }));

    // Tooltip text for one country
    const tipText = (c) =>
        `<strong>${c.name}</strong><br>` +
        `Customers: ${fmt(c.customers)}<br>` +
        `Accounts: ${fmt(c.accounts)}<br>` +
        `Balance: ${fmt(c.balance)} SEK`;

    // Goes to the Top 10 page for a country
    const openCountry = (c) =>
        window.location.href = "/Country?country=" + encodeURIComponent(c.name);

    // A fixed view centred on the Nordics. Used both when the map loads
    // and when the window is resized, so the Nordics always stay in the middle.
    const focus = { lat: 63.5, lng: 17, scale: 5.5, animate: false };

    jQuery(mapEl).vectorMap({
        map: "world_mill_en",
        backgroundColor: "transparent",

        // A still view, zoomed in on the Nordics (see focus above)
        focusOn: focus,
        zoomButtons: false,
        zoomOnScroll: false,
        panOnDrag: false,

        // Other countries are barely visible, the bank's countries stand out
        regionStyle: {
            initial: { fill: "#10221B", stroke: "#2A4A3D", "stroke-width": 0.4 },
            hover: { "fill-opacity": 1 }
        },
        series: {
            regions: [{ values: activeRegions, scale: { active: "#2FA574" } }]
        },

        // Small gold dots on the capitals
        markerStyle: {
            initial: { fill: "#F2D675", stroke: "#04120C", "stroke-width": 1.5, r: 5 },
            hover: { fill: "#FFE9A8", cursor: "pointer" }
        },
        markers: markers,

        // Tooltips only for the bank's countries
        onRegionTipShow: function (e, tip, code) {
            const c = byCode[code];
            if (c) tip.html(tipText(c));
            else e.preventDefault();
        },
        onMarkerTipShow: function (e, tip, index) {
            tip.html(tipText(byCode[markers[index].code]));
        },

        // Click a country or a dot -> Top 10 page
        onRegionClick: function (e, code) {
            if (byCode[code]) openCountry(byCode[code]);
        },
        onMarkerClick: function (e, index) {
            openCountry(byCode[markers[index].code]);
        }
    });

    // Keep the Nordics in the middle when the window changes size
    const map = jQuery(mapEl).vectorMap("get", "mapObject");
    let resizeTimer;

    window.addEventListener("resize", function () {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function () {
            map.updateSize();
            map.setFocus(focus);
        }, 150); // waits until the user has stopped resizing
    });
});