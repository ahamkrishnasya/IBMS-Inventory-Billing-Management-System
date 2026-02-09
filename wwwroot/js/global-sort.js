// global-sort.js
(() => {
    let sortDirections = {};

    window.sortTable = function (columnIndex) {
        const table = document.getElementById("dataTable");
        if (!table) return;

        const tbody = table.tBodies[0];
        const rows = Array.from(tbody.rows);

        const asc = !sortDirections[columnIndex];
        sortDirections[columnIndex] = asc;

        rows.sort((a, b) => {
            let x = a.cells[columnIndex].innerText.trim().toLowerCase();
            let y = b.cells[columnIndex].innerText.trim().toLowerCase();

            const xNum = parseFloat(x);
            const yNum = parseFloat(y);

            if (!isNaN(xNum) && !isNaN(yNum)) {
                return asc ? xNum - yNum : yNum - xNum;
            }

            return asc
                ? x.localeCompare(y)
                : y.localeCompare(x);
        });

        rows.forEach(row => tbody.appendChild(row));
    };
})();
