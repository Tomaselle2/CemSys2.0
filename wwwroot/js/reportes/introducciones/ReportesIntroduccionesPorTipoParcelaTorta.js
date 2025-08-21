
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
                    'rgba(175, 238, 238, 0.7)', // Pale Blue #AFEEEE
                    'rgba(135, 206, 235, 0.7)', // Sky Blue #87CEEB
                    'rgba(127, 255, 212, 0.7)', // Aquamarine #7FFFD4
                    'rgba(0, 255, 255, 0.7)',   // Aqua Blue #00FFFF
                    'rgba(0, 191, 255, 0.7)',   // Cyan #00BFFF
                    'rgba(0, 127, 255, 0.7)',   // Azure #007FFF
                    'rgba(0, 0, 255, 0.7)',     // Deep Blue #0000FF
                    'rgba(64, 224, 208, 0.7)',  // Turquoise #40E0D0
                    'rgba(0, 206, 209, 0.7)',   // Deep Turquoise #00CED1
                    'rgba(0, 128, 128, 0.7)',   // Teal #008080
                    'rgba(0, 136, 139, 0.7)',   // Dark Cyan #008B8B
                    'rgba(0, 123, 167, 0.7)',   // Cerulean #007BA7
                    'rgba(46, 184, 87, 0.7)',   // Sea Green #2EB857
                    'rgba(0, 91, 125, 0.7)',    // Deep Sea Blue #005B7D
                    'rgba(70, 130, 180, 0.7)',  // Steel Blue #4682B4
                    'rgba(65, 105, 225, 0.7)',  // Royal Blue #4169E1
                    'rgba(0, 71, 171, 0.7)',    // Cobalt Blue #0047AB
                    'rgba(79, 66, 133, 0.7)',   // Ocean Blue #4F4285
                    'rgba(75, 0, 130, 0.7)',    // Indigo #4B0082
                    'rgba(0, 0, 128, 0.7)',     // Navy Blue #000080
                    'rgba(25, 25, 112, 0.7)',   // Midnight Blue #191970
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
