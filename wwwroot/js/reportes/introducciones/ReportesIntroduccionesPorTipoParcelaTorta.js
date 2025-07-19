let chartTorta = null;

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
                GraficoTorta(response.data);
            } else {
                alert(response.message);
            }
        },
        error: function () {
            alert('Error al consultar los datos');
        }
    });
});

function GraficoTorta(data) {
    if (data.length === 0) {
        alert("No hay datos para mostrar.");
        $('#contenedorBtnPdf').hide();
        return;
    }

    const datosTipo = data.filter(x => x.cantidadPorTipo > 0);

    if (datosTipo.length === 0) {
        alert("No hay datos para mostrar.");
        $('#contenedorBtnPdf').hide();
        return;
    }

    const labels = datosTipo.map(x => x.tipoParcela);
    const cantidades = datosTipo.map(x => x.cantidadPorTipo);

    const ctxTorta = document.getElementById('graficoTortaParcela').getContext('2d');
    if (chartTorta) chartTorta.destroy();

    chartTorta = new Chart(ctxTorta, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                data: cantidades,
                backgroundColor: [
                    'rgba(255, 99, 132, 0.7)',
                    'rgba(255, 206, 86, 0.7)',
                    'rgba(75, 192, 192, 0.7)'
                ],
                borderColor: [
                    'rgba(255, 99, 132, 1)',
                    'rgba(255, 206, 86, 1)',
                    'rgba(75, 192, 192, 1)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: 'Porcentaje de introducciones por tipo de parcela',
                    font: { size: 24 }
                },
                datalabels: {
                    color: '#000',
                    font: { weight: 'bold', size: 30 },
                    formatter: (value, context) => {
                        const sum = cantidades.reduce((a, b) => a + b, 0);
                        return ((value * 100) / sum).toFixed(2) + '%';
                    }
                },
                legend: {
                    position: 'top',
                    labels: { font: { size: 18 } }
                }
            }
        },
        plugins: [ChartDataLabels]
    });

    document.querySelectorAll('.contenedor-btn-descargar').forEach(div => {
        div.style.display = 'flex';
    }); }

