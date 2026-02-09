document.addEventListener("DOMContentLoaded", function () {

    const deleteModal = document.getElementById("globalDeleteModal");

    if (!deleteModal) return;

    deleteModal.addEventListener("show.bs.modal", function (event) {

        const button = event.relatedTarget;

        const id = button.getAttribute("data-id");
        const name = button.getAttribute("data-name");
        const action = button.getAttribute("data-action");

        document.getElementById("deleteItemId").value = id;
        document.getElementById("deleteItemName").textContent = name;

        document.getElementById("deleteForm").setAttribute("action", action);
    });

});
