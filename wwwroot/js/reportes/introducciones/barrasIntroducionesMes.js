let chart = null;

$(document).ready(function () {

    // ✅ Activar/desactivar fechas según opción
    $('input[name="opcion"]').change(function () {
        const opcion = $('input[name="opcion"]:checked').val();
        $('input[name="desdeFecha"], input[name="hastaFecha"]').prop('disabled', opcion === 'todas');
    }).change();

    // ✅ Cargar gráfico al hacer click
    $('#btnBuscar').click(function (e) {
        e.preventDefault();

        const opcion = $('input[name="opcion"]:checked').val();
        const desdeFecha = $('input[name="desdeFecha"]').val();
        const hastaFecha = $('input[name="hastaFecha"]').val();

        $.ajax({
            url: urlBase + '/Introduccion/ReporteGeneralIntroducciones',
            type: 'GET',
            data: { opcion, desdeFecha, hastaFecha },
            success: function (response) {
                if (response.success) {
                    BarrasIntroduccionesMes(response.data);
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert('Error al consultar los datos');
            }
        });
    });
});

function BarrasIntroduccionesMes(data) {
    if (data.length === 0) {
        alert("No hay datos para mostrar.");
        document.getElementById('contenedorBtnPdf').style.display = 'none';
        return;
    }

    const datosMes = data.filter(x => x.cantidad > 0);

    if (datosMes.length === 0) {
        alert("No hay datos para mostrar.");
        document.getElementById('contenedorBtnPdf').style.display = 'none';
        return;
    }

    const etiquetas = datosMes.map(x => `${x.mes}/${x.año}`);
    const cantidades = datosMes.map(x => x.cantidad);

    const ctx = document.getElementById('graficoPorMes').getContext('2d');

    if (chart) {
        chart.destroy();
    }

    chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: etiquetas,
            datasets: [{
                label: 'Cantidad de Introducciones',
                data: cantidades,
                backgroundColor: 'rgba(54, 162, 235, 0.6)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { display: false },
                title: {
                    display: true,
                    text: 'Cantidad de introducciones por mes',
                    font: { size: 24 }
                }
            },
            scales: {
                x: { ticks: { font: { size: 15 } } },
                y: {
                    beginAtZero: true,
                    ticks: { stepSize: 1 }
                }
            }
        }
    });

    document.querySelectorAll('.contenedor-btn-descargar').forEach(div => {
        div.style.display = 'flex';
    });
}

