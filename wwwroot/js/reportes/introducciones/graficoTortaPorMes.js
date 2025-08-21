var chartTortaPorMes = null;

function GraficoTortaPorMes(data) {
    if (data.length === 0) {
        console.log("No hay datos para mostrar el gráfico por mes.");
        $('#contenedorBtnPdfTortaMes').hide();
        return;
    }

    const ctx = document.getElementById('graficoTortaPorMes').getContext('2d');

    // Destruir el gráfico anterior si existe
    if (window.chartTortaPorMes) {
        window.chartTortaPorMes.destroy();
    }

    // Calcular el total para los porcentajes
    const total = data.reduce((sum, item) => sum + item.cantidad, 0);

    // Nombres de los meses
    const nombresMeses = [
        'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
        'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
    ];

    // Ordenar los datos por año y mes
    const datosOrdenados = [...data].sort((a, b) => {
        if (a.año !== b.año) return a.año - b.año;
        return a.mes - b.mes;
    });

    // Crear etiquetas con formato "Mes Año"
    const labels = datosOrdenados.map(item =>
        `${nombresMeses[item.mes - 1]} ${item.año}`
    );

    // Colores para el gráfico
    const backgroundColors = [
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
    ];

    window.chartTortaPorMes = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                data: datosOrdenados.map(x => x.cantidad),
                backgroundColor: backgroundColors,
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: 'Distribución de introducciones por mes',
                    font: { size: 24 }
                },
                legend: {
                    position: 'right',
                    labels: {
                        font: { size: 14 },
                        boxWidth: 20,
                        padding: 20
                    }
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
                        size: 16
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
        plugins: [ChartDataLabels]
    });

    // Mostrar el contenedor
    $('.contenedor-reporte').show();

    
}