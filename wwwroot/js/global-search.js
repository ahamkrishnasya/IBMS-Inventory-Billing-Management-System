document.addEventListener("DOMContentLoaded", function () {

    const searchInput = document.getElementById("tableSearch");
    const table = document.getElementById("dataTable");

    if (!searchInput || !table) return;

    searchInput.addEventListener("keyup", function () {
        const filter = searchInput.value.toLowerCase();
        const rows = table.tBodies[0].rows;

        for (let row of rows) {
            const text = row.innerText.toLowerCase();
            row.style.display = text.includes(filter) ? "" : "none";
        }
    });
});
