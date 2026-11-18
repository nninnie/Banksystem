/* ==========================================================
   HELPERS
   ========================================================== */

// Number with spaces: 51 096
const formatNumber = (n) => Number(n).toLocaleString("sv-SE");

// Money with 2 decimals: 51 096,10
const formatMoney = (n) =>
    Number(n).toLocaleString("sv-SE", { minimumFractionDigits: 2, maximumFractionDigits: 2 });

// Escapes text so that it is shown as text, never as HTML
const escapeHtml = (value) =>
    String(value ?? "")
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#39;");


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

    // Click on the button opens/closes the menu
    launcherBtn.addEventListener("click", function (e) {
        e.preventDefault();
        e.stopPropagation();

        launcher.classList.toggle("hidden");
    });

    // Click inside the menu keeps it open
    launcher.addEventListener("click", function (e) {
        e.stopPropagation();
    });

    // Click anywhere else closes it
    document.addEventListener("click", function () {
        launcher.classList.add("hidden");
    });
});


/* ==========================================================
   DASHBOARD - CUSTOMERS PER COUNTRY
   ========================================================== */
document.addEventListener("DOMContentLoaded", function () {

    const canvas = document.getElementById("customersChart");

    // Only runs on the dashboard, where the chart and Chart.js are loaded
    if (!canvas || typeof Chart === "undefined") return;

    const values = JSON.parse(canvas.dataset.values);
    const labels = JSON.parse(canvas.dataset.labels);
    const flagEls = canvas
        .closest(".dashboard-card")
        .querySelectorAll(".chart-flags .flag-item");

    Chart.defaults.global.defaultFontFamily = "Inter, system-ui, sans-serif";

    // Draws bars with rounded tops and the value above each bar
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

                // The bar
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

                // The value above the bar
                ctx.save();
                ctx.fillStyle = "#F8FAFC";
                ctx.font = "600 12px Inter, sans-serif";
                ctx.textAlign = "center";
                ctx.fillText(formatNumber(values[i]), m.x, top - 10);
                ctx.restore();
            });
        },

        // Puts each flag under its bar
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
                backgroundColor: "rgba(0,0,0,0)", // the plugin draws the bars
                borderWidth: 0,
                barPercentage: 0.8,
                categoryPercentage: 0.7
            }]
        },

        options: {
            responsive: true,
            maintainAspectRatio: false,
            layout: { padding: { top: 28 } },
            scales: {
                yAxes: [{
                    ticks: {
                        fontSize: 10,
                        padding: 2,
                        maxTicksLimit: 5,
                        display: window.innerWidth > 576
                    }
                }]
            },
            legend: { display: false },

            tooltips: {
                callbacks: {
                    label: function (item) {
                        return " " + formatNumber(item.yLabel) + " customers";
                    }
                }
            }
        }
    });
});


/* ==========================================================
   START PAGE - COUNTRY MAP
   ========================================================== */
document.addEventListener("DOMContentLoaded", function () {

    const mapEl = document.getElementById("countryMap");

    // Only runs on the start page, where the map and its library are loaded
    if (!mapEl || typeof jQuery === "undefined" || !jQuery.fn.vectorMap) return;

    const countries = JSON.parse(mapEl.dataset.countries);

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
        `Customers: ${formatNumber(c.customers)}<br>` +
        `Accounts: ${formatNumber(c.accounts)}<br>` +
        `Balance: ${formatNumber(c.balance)} ${c.currency}`;

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

    const map = jQuery(mapEl).vectorMap("get", "mapObject");

    // Locks the map on touch screens
    mapEl.addEventListener("touchmove", function (e) {
        e.stopPropagation();
    }, { capture: true });

    // Starts on the Nordics also when the map was drawn in another size
    map.updateSize();
    map.setFocus(focus);

    // Keeps the Nordics in the middle when the window changes size
    let resizeTimer;

    window.addEventListener("resize", function () {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function () {
            map.updateSize();
            map.setFocus(focus);
        }, 150); // waits until the user has stopped resizing
    });
});


/* ==========================================================
   ACCOUNT PAGE - SHOW MORE TRANSACTIONS (AJAX)
   ========================================================== */
document.addEventListener("DOMContentLoaded", function () {

    const table = document.getElementById("transactions-table");
    const button = document.getElementById("show-more-btn");

    // Only runs on the account page
    if (!table || !button) return;

    // Currency of the account's country, set on the table by Account.cshtml
    const currency = table.dataset.currency;

    // Which page of 20 transactions to fetch next
    let pageNo = 1;

    // Fetches the next 20 transactions from OnGetShowMoreAsync
    function showMore() {
        fetch(table.dataset.url + "&pageNo=" + pageNo)
            .then((response) => {
                if (!response.ok) throw new Error("Server error " + response.status);
                return response.json();
            })
            .then((json) => {
                pageNo = pageNo + 1;
                json.transactions.forEach(drawElements);

                // Updates "Showing X of N"
                const shown = table.querySelector("tbody").rows.length;
                document.getElementById("show-more-info").textContent =
                    `Showing ${shown} of ${table.dataset.total}`;

                // Hides the button when there are no more transactions
                if (json.transactions.length < 20) {
                    button.style.display = "none";
                }
            })
            .catch((error) => {
                console.error("Could not load transactions:", error);
            });
    }

    // Adds one row at the end of the table
    function drawElements(element) {
        const amountClass = element.amount < 0 ? "negative" : "positive";
        // Red edge for money out, green for money in (mobile cards)
        const rowClass = element.amount < 0 ? "tx-out" : "tx-in";

        table.querySelector("tbody").insertAdjacentHTML("beforeend",
            `<tr class="${rowClass}">
                <td data-label="Date">${new Date(element.date).toLocaleDateString("sv-SE")}</td>
                <td data-label="Type">${escapeHtml(element.type)}</td>
                <td data-label="Operation">${escapeHtml(element.operation) || "–"}</td>
                <td class="currency ${amountClass}" data-label="Amount">${formatMoney(element.amount)} ${currency}</td>
                <td class="currency" data-label="Balance">${formatMoney(element.balance)} ${currency}</td>
            </tr>`);
    }

    button.addEventListener("click", showMore);
    showMore(); // loads the first 20
});


/* ==========================================================
   TRANSFER - SHOW RECEIVER NAME (Confirmation of Payee, AJAX)
   ========================================================== */
document.addEventListener("DOMContentLoaded", function () {
    const input = document.querySelector("[data-recipient-url]");
    const info = document.getElementById("recipientInfo");

    // Only runs on the transfer page
    if (!input || !info) return;

    let timer;

    input.addEventListener("input", function () {
        // Clears the old answer on every key press
        clearTimeout(timer);
        info.textContent = "";
        info.className = "recipient-info";
        input.dataset.recipientName = "";

        const accountId = input.value.trim();
        if (!accountId) return;

        // Waits until the cashier stops typing, then asks the server once
        timer = setTimeout(async function () {
            try {
                const url = `${input.dataset.recipientUrl}&accountId=${encodeURIComponent(accountId)}`;
                const response = await fetch(url);
                const data = await response.json();

                if (data.found) {
                    info.textContent = `Receiver: ${data.name}`;
                    info.classList.add("found");
                    input.dataset.recipientName = data.name;
                } else {
                    info.textContent = "The account does not exist";
                    info.classList.add("not-found");
                }
            } catch (error) {
                console.error("Could not look up the receiver:", error);
            }
        }, 400);
    });
});


/* ==========================================================
   CONFIRM BEFORE POST (transfer and withdraw)
   Runs after client-side validation. The server still validates everything.
   ========================================================== */
document.addEventListener("DOMContentLoaded", function () {
    const modalEl = document.getElementById("confirmModal");

    // Only runs on pages with the confirm dialog
    if (!modalEl) return;

    const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
    const message = document.getElementById("confirmMessage");
    let formToSend = null;

    document.querySelectorAll("form[data-confirm]").forEach(function (form) {
        form.addEventListener("submit", function (e) {
            // Already confirmed - let it through
            if (form.dataset.confirmed === "true") return;

            // Validation first: invalid form = no dialog, the errors are shown instead
            if (window.jQuery && jQuery(form).valid && !jQuery(form).valid()) return;

            // Transfer: no dialog for an unknown receiver or the same account
            if (form.dataset.confirm === "transfer") {
                const toId = form.querySelector("[name='Input.ToAccountId']").value.trim();
                const recipientInfo = document.getElementById("recipientInfo");

                if (toId === form.dataset.from && recipientInfo) {
                    recipientInfo.textContent = "You can't transfer to the same account.";
                    recipientInfo.className = "recipient-info not-found";
                }

                if (toId === form.dataset.from || (recipientInfo && recipientInfo.classList.contains("not-found"))) {
                    e.preventDefault();
                    return;
                }
            }

            e.preventDefault();

            const amount = form.querySelector("[name='Input.Amount']").value;
            const currency = form.dataset.currency;

            if (form.dataset.confirm === "transfer") {
                const to = form.querySelector("[name='Input.ToAccountId']");
                const name = to.dataset.recipientName ? ` (${to.dataset.recipientName})` : "";
                message.textContent =
                    `Transfer ${amount} ${currency} from account ${form.dataset.from} to account ${to.value}${name}?`;
            } else {
                message.textContent =
                    `Withdraw ${amount} ${currency} from account ${form.dataset.from}?`;
            }

            formToSend = form;
            modal.show();
        });
    });

    // OK in the dialog sends the form for real
    document.getElementById("confirmOk").addEventListener("click", function () {
        if (!formToSend) return;
        formToSend.dataset.confirmed = "true";
        modal.hide();
        formToSend.requestSubmit();
    });
});




/* ==========================================================
   ACCOUNT QUICK ACTIONS (Customer page)
   ========================================================== */
document.addEventListener("DOMContentLoaded", function () {

    // The whole account row is clickable. Clicking a shortcut button (Deposit, Withdraw,
    // Transfer) must only use that button, so we stop the click from reaching the row.
    document.querySelectorAll(".account-quick-actions a").forEach(function (link) {
        link.addEventListener("click", function (e) {
            e.stopPropagation();
        });
    });
});