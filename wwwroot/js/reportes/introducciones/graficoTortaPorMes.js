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
        'rgba(255, 99, 132, 0.7)',
        'rgba(54, 162, 235, 0.7)',
        'rgba(255, 206, 86, 0.7)',
        'rgba(75, 192, 192, 0.7)',
        'rgba(153, 102, 255, 0.7)',
        'rgba(255, 159, 64, 0.7)',
        'rgba(199, 199, 199, 0.7)',
        'rgba(83, 102, 255, 0.7)',
        'rgba(255, 99, 255, 0.7)',
        'rgba(99, 255, 132, 0.7)',
        'rgba(255, 132, 99, 0.7)',
        'rgba(132, 99, 255, 0.7)'
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