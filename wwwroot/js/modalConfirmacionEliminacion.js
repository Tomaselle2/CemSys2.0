function confirmarEliminacion(idForm, mensaje) {
    const form = document.getElementById(idForm);
    if (form) {
        const modal = new bootstrap.Modal(document.getElementById("modalConfirmacionEliminacion"));
        document.getElementById("mensajeConfirmacion").innerText = mensaje;
        document.getElementById("modalConfirmacionEliminacion").dataset.formId = idForm;
        modal.show();
    }
}

function cerrarModal() {
    const modalElement = document.getElementById("modalConfirmacionEliminacion");
    const modal = bootstrap.Modal.getInstance(modalElement);
    if (modal) modal.hide();
}

function enviarFormulario() {
    const formId = document.getElementById("modalConfirmacionEliminacion").dataset.formId;
    const form = document.getElementById(formId);
    if (form.checkValidity()) {
        form.submit();
    } else {
        form.reportValidity();
        cerrarModal();
    }
}
