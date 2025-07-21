var chartBarrasEmpleados = null;

function GraficoBarrasEmpleados(data) {
    if (data.length === 0) {
        alert("No hay datos para mostrar.");
        $('#contenedorBtnPdf').hide();
        return;
    }

    // Destruir el gráfico anterior si existe
    if (chartBarrasEmpleados !== null) {
        chartBarrasEmpleados.destroy();
    }

    const ctxEmpleados = document.getElementById('graficoBarrasEmpleados').getContext('2d');

    // Extraer labels y datos del response
    const labelsEmpleados = data.map(item => item.nombreEmpleado); // Asegúrate que el JSON trae este campo
    const valoresEmpleados = data.map(item => item.cantidad); // Asegúrate que el JSON trae este campo

    chartBarrasEmpleados = new Chart(ctxEmpleados, {
        type: 'bar',
        data: {
            labels: labelsEmpleados,
            datasets: [{
                label: 'Introducciones',
                data: valoresEmpleados,
                backgroundColor: 'violet',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1
            }]
        },
        options: {
            indexAxis: 'y', // Barras horizontales
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: 'Cantidad de introducciones por empleado',
                    font: { size: 24 }
                },
                legend: {
                    display: false
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return ` ${context.dataset.label}: ${context.raw}`;
                        }
                    }
                }
            },
            scales: {
                x: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 1
                    },
                    title: {
                        display: true,
                        text: 'Cantidad de Introducciones',
                        font: { size: 20 }
                    }
                },
                y: {
                    title: {
                        display: true,
                        text: 'Empleados',
                        font: { size: 20 }
                    },
                    ticks: {
                        font: {
                            size: 16
                        }
                    }
                }
            }
        }
    });

    $('.contenedor-reporte').show();
}
