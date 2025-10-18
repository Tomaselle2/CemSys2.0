
// Variables globales

let contribuyenteSeleccionado = null;
let estadoModal = 'buscar'; // 'buscar' o 'registrar'

// Elementos del DOM
const agregarContribuyenteButton = document.getElementById('agregarContribuyenteButton');
const formContribuyente = document.getElementById('formContribuyente');
const dniInput = document.getElementById('dniInput');
const sexoInput = document.getElementById('sexoInput');
const nombreInput = document.getElementById('nombreInput');
const apellidoInput = document.getElementById('apellidoInput');
const camposAdicionales = document.getElementById('camposAdicionales');
const btnAccionContribuyente = document.getElementById('btnAccionContribuyente');
const contribuyenteDisplay = document.getElementById('contribuyenteDisplay');
const contribuyenteIcon = document.getElementById('contribuyenteIcon');
const errorMessage = document.getElementById('errorMessage');
const idContribuyenteHidden = document.getElementById('idContribuyenteHidden');

let listaDetalles = []; //para los detalles de la factura


//------------------------------------------------//logica de carga de recibo, spiner-------------------------
//--------------------------------------------------------------------------------------------------------
function handleFormSubmit(event) {
    // Primero verificar si el formulario es válido
    const form = document.getElementById('formRecibo');

    // Usar la validación nativa del navegador
    if (!form.checkValidity()) {
        // Si no es válido, mostrar mensajes de validación y detener
        form.reportValidity();
        return false;
    }

    // Si llegamos aquí, la validación pasó
    const btn = document.getElementById('btnCargarRecibo');
    const btnText = btn.querySelector('.btn-text');
    const spinner = btn.querySelector('.spinner-border');
    const loadingText = btn.querySelector('.loading-text');

    // Deshabilitar el botón
    btn.disabled = true;

    // Ocultar texto original y mostrar spinner + texto de carga
    btnText.classList.add('d-none');
    spinner.classList.remove('d-none');
    loadingText.classList.remove('d-none');

    // Permitir que el formulario se envíe
    return true;
}

//-------------------------------------------------//logica de agregar contribuyente----------------------------
//---------------------------------------------------------------------------------------------------------
// Abrir modal de contribuynete
if (agregarContribuyenteButton) {
    agregarContribuyenteButton.addEventListener('click', function (event) {
        event.preventDefault();

        if (contribuyenteSeleccionado) {
            // Si ya hay un contribuyente seleccionado, limpiar selección
            limpiarSeleccionContribuyente();

        } else {
            // Abrir modal para buscar/agregar contribuyente
            resetearModal();
            $('#ContribuyenteModal').modal('show');
        }
    });

}

// Submit del formulario del modal
formContribuyente.addEventListener('submit', function (event) {
    event.preventDefault();

    if (!validarFormulario()) {
        return;
    }

    if (estadoModal === 'buscar') {
        buscarContribuyente();
    } else if (estadoModal === 'registrar') {
        registrarContribuyente();
    }
});

// Función para validar formulario
function validarFormulario() {
    let esValido = true;

    // Limpiar clases de validación anteriores
    formContribuyente.querySelectorAll('.is-invalid').forEach(el => el.classList.remove('is-invalid'));

    // Validar DNI
    if (!dniInput.value.trim() || dniInput.value.length < 1) {
        dniInput.classList.add('is-invalid');
        esValido = false;
    }

    // Validar sexo
    if (!sexoInput.value) {
        sexoInput.classList.add('is-invalid');
        esValido = false;
    }

    // Si estamos en modo registrar, validar nombre y apellido
    if (estadoModal === 'registrar') {
        if (!nombreInput.value.trim() || nombreInput.value.length < 2) {
            nombreInput.classList.add('is-invalid');
            esValido = false;
        }

        if (!apellidoInput.value.trim() || apellidoInput.value.length < 2) {
            apellidoInput.classList.add('is-invalid');
            esValido = false;
        }
    }

    return esValido;
}

// Función para buscar contribuyente via AJAX
function buscarContribuyente() {
    mostrarCargando(true);
    ocultarError();

    const datos = {
        Dni: parseInt(dniInput.value),
        Sexo: sexoInput.value
    };

    fetch('/Personas/BuscarContribuyente', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        body: JSON.stringify(datos)
    })
        .then(response => response.json())
        .then(data => {
            mostrarCargando(false);

            if (data.success) {
                if (data.contribuyente) {
                    // Contribuyente encontrado
                    seleccionarContribuyente(data.contribuyente);
                    $('#ContribuyenteModal').modal('hide');
                } else {
                    // Contribuyente no encontrado, mostrar campos para registrar
                    cambiarAModoRegistrar();
                }
            } else {
                mostrarError(data.message || 'Error al buscar contribuyente');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            mostrarCargando(false);
            mostrarError('Error de conexión');
        });
}

// Función para registrar nuevo contribuyente
function registrarContribuyente() {
    mostrarCargando(true);
    ocultarError();

    const datos = {
        Dni: parseInt(dniInput.value),
        Sexo: sexoInput.value,
        Nombre: nombreInput.value.trim(),
        Apellido: apellidoInput.value.trim()
    };

    fetch('/Personas/RegistrarContribuyente', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        body: JSON.stringify(datos)
    })
        .then(response => response.json())
        .then(data => {
            mostrarCargando(false);

            if (data.success) {
                seleccionarContribuyente(data.contribuyente);
                $('#ContribuyenteModal').modal('hide');
            } else {
                mostrarError(data.message || 'Error al registrar contribuyente');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            mostrarCargando(false);
            mostrarError('Error de conexión');
        });
}

// Función para seleccionar contribuyente
function seleccionarContribuyente(contribuyente) {

    // Si es contribuyente de decreto, marcar el checkbox
    if (contribuyente.dni === "00000000") {
        document.getElementById('decreto').checked = true;
    }

    contribuyenteSeleccionado = contribuyente;
    contribuyenteDisplay.value = `${contribuyente.nombre} ${contribuyente.apellido}`;

    // Actualizar todos los campos hidden
    document.getElementById('idContribuyenteHidden').value = contribuyente.id;
    document.getElementById('dniHidden').value = contribuyente.dni;
    document.getElementById('sexoHidden').value = contribuyente.sexo;
    document.getElementById('nombreHidden').value = contribuyente.nombre;
    document.getElementById('apellidoHidden').value = contribuyente.apellido;

    // Cambiar botón a modo "quitar"
    agregarContribuyenteButton.className = 'btn btn-danger';
    contribuyenteIcon.className = 'bi bi-person-dash';
}

// Función para limpiar selección de contribuyente
function limpiarSeleccionContribuyente() {
    if (contribuyenteSeleccionado && contribuyenteSeleccionado.dni === "00000000") {
        document.getElementById('decreto').checked = false;
        apareceCamposCheck();
        ocultarCampoArchivoMonto();
    }

    contribuyenteSeleccionado = null;
    contribuyenteDisplay.value = '';

    // Limpiar todos los campos hidden
    document.getElementById('idContribuyenteHidden').value = '';
    document.getElementById('dniHidden').value = '';
    document.getElementById('sexoHidden').value = '';
    document.getElementById('nombreHidden').value = '';
    document.getElementById('apellidoHidden').value = '';

    // Restaurar botón a modo "agregar"
    agregarContribuyenteButton.className = 'btn btn-success';
    contribuyenteIcon.className = 'bi bi-person-plus';
}

// Función para cambiar a modo registrar
function cambiarAModoRegistrar() {
    estadoModal = 'registrar';
    camposAdicionales.style.display = 'block';
    btnAccionContribuyente.textContent = 'Registrar';

    // Hacer campos nombre y apellido requeridos
    nombreInput.setAttribute('required', '');
    apellidoInput.setAttribute('required', '');
}

// Función para resetear modal
function resetearModal() {
    estadoModal = 'buscar';
    formContribuyente.reset();
    formContribuyente.classList.remove('was-validated');
    camposAdicionales.style.display = 'none';
    btnAccionContribuyente.textContent = 'Buscar';
    ocultarError();

    // Quitar required de campos nombre y apellido
    nombreInput.removeAttribute('required');
    apellidoInput.removeAttribute('required');

    // Limpiar clases de validación
    formContribuyente.querySelectorAll('.is-invalid').forEach(el => el.classList.remove('is-invalid'));
}

// Funciones auxiliares
function mostrarCargando(mostrar) {
    if (mostrar) {
        btnAccionContribuyente.disabled = true;
        btnAccionContribuyente.innerHTML = '<span class="spinner-border spinner-border-sm" role="status"></span> Procesando...';
    } else {
        btnAccionContribuyente.disabled = false;
        btnAccionContribuyente.textContent = estadoModal === 'buscar' ? 'Buscar' : 'Registrar';
    }
}

function mostrarError(mensaje) {
    errorMessage.textContent = mensaje;
    errorMessage.style.display = 'block';
}

function ocultarError() {
    errorMessage.style.display = 'none';
}

function ocultaCamposCheck() { //cuando se preciona el decreto
    const elementos = document.querySelectorAll('.elemento-ocultar-check');
    elementos.forEach(el => el.style.display = 'none');
    document.getElementById('detalleFacturaTable').style.display = 'none';
    listaDetalles = [];
    document.getElementById("conceptoSelect").selectedIndex = 0;
    document.getElementById("precioInput").value = "";
    document.getElementById('btn-enviar-factura').textContent = 'Emitir decreto';
    renderTabla();

    //aparece el input de archivo y monto
}

function mostrarCampoArchivoMonto() {
    document.getElementById('montoDecreto').style.display = 'block';
    document.getElementById('archivoDecreto').style.display = 'block';
}

function ocultarCampoArchivoMonto() {
    document.getElementById('montoDecreto').style.display = 'none';
    document.getElementById('archivoDecreto').style.display = 'none';
}

function apareceCamposCheck() { //cuando se deschequea el decreto
    const elementos = document.querySelectorAll('.elemento-ocultar-check');
    elementos.forEach(el => el.style.display = 'block');
    document.getElementById('detalleFacturaTable').style.display = 'table';
    document.getElementById('btn-enviar-factura').textContent = 'Emitir factura';
    //se oculta el input de archivo y monto

}

// Función para manejar cambios en el checkbox Decreto
function handleDecretoChange(checkbox) {
    if (checkbox.checked) {
        // Buscar contribuyente con DNI 00000000
        //funcion de ocultar campos
        ocultaCamposCheck();
        mostrarCampoArchivoMonto();
        buscarContribuyenteDecreto();
    } else {
        // Deseleccionar contribuyente si existe
        if (contribuyenteSeleccionado && contribuyenteSeleccionado.dni === "00000000") {
            limpiarSeleccionContribuyente();
            apareceCamposCheck();
            ocultarCampoArchivoMonto();
        }
    }
}

//para cuando vuelve de un error y esta decreto seleccionado
if (document.getElementById('decreto')) {
    document.getElementById('decreto').addEventListener('DOMContentLoaded', handleDecretoChange(document.getElementById('decreto')));
}

// Función para buscar contribuyente de decreto
function buscarContribuyenteDecreto() {
    mostrarCargando(true);
    ocultarError();

    const datos = {
        Dni: 0, // Se enviará como 0 para identificarlo como decreto
        Sexo: "otro" // Sexo por defecto
    };

    fetch('/Personas/BuscarContribuyenteDecreto', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        body: JSON.stringify(datos)
    })
        .then(response => response.json())
        .then(data => {
            mostrarCargando(false);

            if (data.success) {
                if (data.contribuyente) {
                    seleccionarContribuyente(data.contribuyente);
                } else {
                    // Registrar si no existe
                    registrarContribuyenteDecreto();
                }
            } else {
                mostrarError(data.message || 'Error al buscar contribuyente de decreto');
                document.getElementById('decreto').checked = false;
            }
        })
        .catch(error => {
            console.error('Error:', error);
            mostrarCargando(false);
            mostrarError('Error de conexión');
            document.getElementById('decreto').checked = false;
        });
}

// Función para registrar contribuyente de decreto
function registrarContribuyenteDecreto() {
    mostrarCargando(true);

    const datos = {
        Dni: 00000000, // Identificador de decreto
        Sexo: "otro",
        Nombre: "",
        Apellido: "Municipalidad Colonia Tirolesa"
    };

    fetch('/Personas/RegistrarContribuyenteDecreto', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        body: JSON.stringify(datos)
    })
        .then(response => response.json())
        .then(data => {
            mostrarCargando(false);

            if (data.success) {
                seleccionarContribuyente(data.contribuyente);
            } else {
                mostrarError(data.message || 'Error al registrar contribuyente de decreto');
                document.getElementById('decreto').checked = false;
            }
        })
        .catch(error => {
            console.error('Error:', error);
            mostrarCargando(false);
            mostrarError('Error de conexión');
            document.getElementById('decreto').checked = false;
        });
}

//logica de precio del desplegable-----------------------------------------------------------------------------------------------------------------------------------------------


if (listaDetalleFactura && listaDetalleFactura.length > 0) {
    listaDetalleFactura.forEach(d => {
        const concepto = listaConceptosTarifaria.find(c => c.ConceptoTarifariaId === d.ConceptoTarifariaId);

        listaDetalles.push({
            PrecioId: d.PrecioId,
            ConceptoTarifariaId: d.ConceptoTarifariaId,
            PrecioUnitario: d.PrecioUnitario,
            Cantidad: d.Cantidad,
            TipoConceptoFacturaId: d.TipoConceptoFacturaId ?? 0,
            NombreConcepto: concepto ? concepto.NombreConcepto : ""
        });
    });
}

// renderizar la tabla con lo que ya vino del backend
renderTabla();

// Mostrar precio al seleccionar
if (document.getElementById("conceptoSelect")) {
    document.getElementById("conceptoSelect").addEventListener("change", function () {
        var selectedOption = this.options[this.selectedIndex];
        var precio = selectedOption.getAttribute("data-precio");
        document.getElementById("precioInput").value = precio ? `$ ${precio}` : "";
    });
}


function agregarConceptoDetalle() {
    let select = document.getElementById("conceptoSelect");
    let option = select.options[select.selectedIndex];

    if (!option.value) {
        alert("Debe seleccionar un concepto");
        return;
    }

    // validar duplicado por PrecioId (value del select)
    let precioId = option.value;
    if (listaDetalles.some(d => d.PrecioId == precioId)) {
        alert("Este concepto ya fue agregado");
        return;
    }

    let detalle = {
        PrecioId: precioId,
        ConceptoTarifariaId: option.getAttribute("data-conceptoid"),
        PrecioUnitario: parseFloat(option.getAttribute("data-precio")),
        Cantidad: 1, // siempre fijo en 1
        TipoConceptoFacturaId: option.getAttribute("data-tipoconcepto"),
        NombreConcepto: option.text
    };

    listaDetalles.push(detalle);
    renderTabla();

    // resetear el select y precio
    select.selectedIndex = 0;
    document.getElementById("precioInput").value = "";
}

// Agregar item
if (document.getElementById("agregarBtn")) {
    document.getElementById("agregarBtn").addEventListener("click", agregarConceptoDetalle);
}

// Renderizar tabla + totales
function renderTabla() {
    let tbody = document.querySelector("#detalleFacturaTable tbody");

    if (!tbody) {
        return;
    }

    tbody.innerHTML = "";

    let subtotal = 0;

    listaDetalles.forEach((d, index) => {
        let row = document.createElement("tr");

        subtotal += d.PrecioUnitario; // siempre cantidad 1

        row.innerHTML = `
                       <td>${d.NombreConcepto}</td>
                       <td>$${d.PrecioUnitario.toFixed(2)}</td>
                       <td><button type="button" class="btn btn-danger btn-sm" onclick="eliminarDetalle(${index})">X</button></td>
                   `;
        tbody.appendChild(row);
    });

    let fondo = subtotal * porcentajeFondo;
    let total = subtotal + fondo;

    document.getElementById("fondoSalud").textContent = fondo.toFixed(2);
    document.getElementById("totalFactura").textContent = total.toFixed(2);

    // Generar inputs ocultos para enviar al backend
    let hiddenDiv = document.getElementById("detalleHiddenInputs");
    hiddenDiv.innerHTML = "";
    listaDetalles.forEach((d, i) => {
        hiddenDiv.innerHTML += `
                       <input type="hidden" name="ListaDetalleFactura[${i}].ConceptoTarifariaId" value="${d.ConceptoTarifariaId}" />
                       <input type="hidden" name="ListaDetalleFactura[${i}].PrecioUnitario" value="${d.PrecioUnitario}" />
                       <input type="hidden" name="ListaDetalleFactura[${i}].Cantidad" value="1" />
                       <input type="hidden" name="ListaDetalleFactura[${i}].TipoConceptoFacturaId" value="${d.TipoConceptoFacturaId}" />
                       <input type="hidden" name="ListaDetalleFactura[${i}].PrecioId" value="${d.PrecioId}" />
                   `;
    });
}

// Eliminar detalle
function eliminarDetalle(index) {
    listaDetalles.splice(index, 1);
    renderTabla();
}


//---------------------------------------------------------------------------------------------------------------------------
//-------------------Logica de anular factura---------------------------------------------------------------------------------
document.querySelectorAll('.anularFactura').forEach(btn => {
    btn.addEventListener('click', function () {
        // Obtener el ID de factura del atributo data-facturaid
        const facturaId = this.getAttribute('data-facturaid');

        // Asignar el valor al input hidden del modal
        document.getElementById('facturaIdAnulacion').value = facturaId;

        // Mostrar el modal con Bootstrap 5
        const modal = new bootstrap.Modal(document.getElementById('ModalAnularFactura'));
        modal.show();
    });
});

//------------------------------ver el recivo/archivo----------------------------------------------------------------
//------------------------------------------------------------------------------------------------------------------------
function verRecibo(archivoId) {
    const iframe = document.getElementById("reciboViewer");
    const img = document.getElementById("reciboImage");

    // Ocultar ambos al principio
    iframe.style.display = "block";
    img.style.display = "none";

    iframe.src = `/Archivos/VerArchivo?archivoId=${archivoId}`;

    new bootstrap.Modal(document.getElementById("modalVerRecibo")).show();
}


//---------------------------------------Abrir el panel del historial de la factura--------------------------------------
//------------------------------------------------------------------------------------------------------------------
document.addEventListener('DOMContentLoaded', function () {

    const botonesHistorial = document.querySelectorAll('.btn-historial');
    const tituloFactura = document.getElementById('tituloFactura');
    const contenedor = document.getElementById('historialContenido');

    botonesHistorial.forEach(btn => {
        btn.addEventListener('click', function () {
            const facturaId = this.getAttribute('data-facturaid');

            // Mostrar mensaje de carga
            tituloFactura.textContent = `Factura: ${facturaId}`;
            contenedor.innerHTML = `
                            <div class="text-center text-muted">
                                <i class="bi bi-hourglass-split"></i> Cargando historial...
                            </div>`;

            // Llamada AJAX con fetch
            fetch(`/Cajero/ObtenerHistorialFactura?facturaId=${facturaId}`)
                .then(response => {
                    if (!response.ok) throw new Error('Error en la respuesta del servidor');
                    return response.json();
                })
                .then(data => {
                    if (!data.success) {
                        contenedor.innerHTML = `<div class="alert alert-info">${data.mensaje}</div>`;
                        return;
                    }

                    let html = '<div class="timeline">';
                    data.historial.forEach(item => {
                        html += `
                                        <div class="timeline-item mb-3">
                                            <div class="icon bg-success text-white">
                                                <i class="bi bi-check-lg"></i>
                                            </div>
                                            <div class="content">
                                                <p class="mb-0 fw-bold">${item.estadoNombre}</p>
                                                <small class="text-muted">
                                                    <i class="bi bi-clock"></i> ${item.fecha}
                                                </small>
                                            </div>
                                        </div>
                                    `;
                    });
                    html += '</div>';
                    contenedor.innerHTML = html;
                })
                .catch(error => {
                    contenedor.innerHTML = `<div class="alert alert-danger">${error.message}</div>`;
                });
        });
    });

});


//-------------------------------------// Lógica para editar un ARCHIVO---------------------------------
//----------------------------------------------------------------------------------------------------
document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll('.btnEditarArchivo').forEach(btn => {
        btn.addEventListener('click', function () {
            const id = this.dataset.id;
            const concepto = this.dataset.concepto;
            const archivo = this.dataset.archivo;

            document.getElementById('idArchivoActual').value = id;
            document.getElementById('conceptoReciboEdicion').value = concepto;
            document.getElementById('archivoActualRecibo').value = archivo || "Sin archivo";

            $('#EditarReciboModal').modal('show');
        });
    });
});