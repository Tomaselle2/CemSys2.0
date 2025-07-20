var chartTorta = null;

// Modificación en la función GraficoTorta
function GraficoTorta(data) {
    if (data.length === 0) {
        alert("No hay datos para mostrar.");
        $('#contenedorBtnPdf').hide();
        return;
    }

    const ctxTorta = document.getElementById('graficoTortaParcela').getContext('2d');
    if (window.chartTorta) window.chartTorta.destroy();

    // Calcular el total para los porcentajes
    const total = data.reduce((sum, item) => sum + item.cantidadPorTipo, 0);

    window.chartTorta = new Chart(ctxTorta, {
        type: 'pie',
        data: {
            labels: data.map(x => x.tipoParcela),
            datasets: [{
                data: data.map(x => x.cantidadPorTipo),
                backgroundColor: [
                    'rgba(255, 99, 132, 0.7)',
                    'rgba(54, 162, 235, 0.7)',
                    'rgba(255, 206, 86, 0.7)',
                    'rgba(75, 192, 192, 0.7)',
                    'rgba(153, 102, 255, 0.7)',
                    'rgba(255, 159, 64, 0.7)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: 'Distribución por tipo de parcela',
                    font: { size: 24 }
                },
                legend: {
                    position: 'right',
                    labels: { font: { size: 20 } }
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            const label = context.label || '';
                            const value = context.raw || 0;
                            const percentage = Math.round((value / total) * 100);
                            return `${label}: ${value} (${percentage}%)`;
                        }
                    }
                },
                datalabels: {
                    color: '#000',
                    font: {
                        weight: 'bold',
                        size: 30
                    },
                    formatter: (value) => {
                        const percentage = (value / total) * 100;
                        return percentage % 1 === 0 ?
                            `${percentage.toFixed(0)}%` :  // Muestra "67%"
                            `${percentage.toFixed(2)}%`;   // Muestra "66.67%"
                    },
                    anchor: 'center',
                    align: 'center',
                    offset: 0
                }
            }
        },
        plugins: [ChartDataLabels] // Registra el plugin
    });

    $('.contenedor-reporte').show();
}
