var chartTorta = null;
$(document).ready(function () {
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
                    // Gráfico de barras (por mes)
                    BarrasIntroduccionesMes(response.dataBarra);

                    // Gráfico de torta (por tipo de parcela)
                    GraficoTorta(response.dataTorta);

                    // Nuevo gráfico de lista (por tipo de parcela)
                    mostrarListaTiposParcela(response.dataLista, response.total);

                    // Actualizar fechas en todos los formularios PDF
                    $('input[name="fechaDesde"]').val(response.fechaDesde);
                    $('input[name="fechaHasta"]').val(response.fechaHasta);

                    // Mostrar todos los contenedores
                    $('.contenedor-grafico').show();
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

let chartBarraParcela = null; // Variable global para el gráfico de barras

function mostrarListaTiposParcela(dataLista, total) {
    if (!dataLista || dataLista.length === 0) {
        alert("No hay datos para mostrar.");
        $('#contenedorBtnPdf').hide();
        return;
    }

    const ctxBarraParcela = document.getElementById('graficoBarraParcela')?.getContext('2d');
    if (!ctxBarraParcela) {
        console.error("No se encontró el elemento canvas para el gráfico de barras");
        return;
    }

    // Destruir gráfico anterior si existe
    if (chartBarraParcela) {
        chartBarraParcela.destroy();
    }

    // Preparar datos para el gráfico
    const labels = dataLista.map(item => item.tipo);
    const datos = dataLista.map(item => item.cantidad);
    const colores = [
        'rgba(255, 99, 132, 0.7)',
        'rgba(54, 162, 235, 0.7)',
        'rgba(255, 206, 86, 0.7)',
        'rgba(75, 192, 192, 0.7)',
        'rgba(153, 102, 255, 0.7)'
    ];

    chartBarraParcela = new Chart(ctxBarraParcela, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Cantidad',
                data: datos,
                backgroundColor: colores.slice(0, labels.length),
                borderColor: colores.map(c => c.replace('0.7', '1')).slice(0, labels.length),
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: `Cantidad de introducciones por tipo de parcela (Total: ${total})`,
                    font: { size: 18 } // Tamaño reducido para mejor adaptación
                },
                legend: {
                    display: false
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            const item = dataLista.find(x => x.tipo === context.label);
                            return `${context.label}: ${context.raw} (${item?.porcentaje || 0}%)`;
                        }
                    }
                }
            },
            scales: {
                x: {
                    ticks: {
                        font: {
                            size: 14 // Tamaño óptimo para etiquetas
                        }
                    }
                },
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 1,
                        font: {
                            size: 12
                        }
                    }
                }
            }
        }
    });

    $('.contenedor-btn-descargar').show();
}