var chartTortaEmpleados = null;

function GraficoTortaEmpleados(data) {
    if (data.length === 0) {
        console.log("No hay datos de empleados para mostrar.");
        $('#contenedorBtnPdfTortaEmpleados').hide();
        return;
    }

    const ctx = document.getElementById('graficoTortaEmpleados').getContext('2d');

    // Destruir el gráfico anterior si existe
    if (window.chartTortaEmpleados) {
        window.chartTortaEmpleados.destroy();
    }

    // Calcular el total para los porcentajes
    const total = data.reduce((sum, item) => sum + item.cantidad, 0);

    // Ordenar los datos por cantidad (de mayor a menor)
    const datosOrdenados = [...data].sort((a, b) => b.cantidad - a.cantidad);

    // Limitar a los primeros 10 empleados para mejor visualización
    const datosMostrar = datosOrdenados.slice(0, 10);
    const otros = datosOrdenados.slice(10);
    const totalOtros = otros.reduce((sum, item) => sum + item.cantidad, 0);

    // Si hay "otros", agregamos al final
    if (totalOtros > 0) {
        datosMostrar.push({
            nombreEmpleado: "Otros empleados",
            cantidad: totalOtros
        });
    }

    // Configurar colores
    const colores = [

        'rgba(0, 91, 125, 0.7)',     // Deep Sea Blue
        'rgba(70, 130, 180, 0.7)',   // Steel Blue
        'rgba(135, 206, 235, 0.7)',  // Sky Blue
        'rgba(160, 150, 200, 0.7)',  // Muted Lilac
        'rgba(153, 102, 204, 0.7)',  // Deep Lilac
        'rgba(127, 255, 212, 0.7)',  // Aquamarine
        'rgba(0, 255, 255, 0.7)',    // Aqua Blue
        'rgba(0, 206, 209, 0.7)',    // Deep Turquoise
        'rgba(64, 224, 208, 0.7)',   // Turquoise
        'rgba(0, 191, 255, 0.7)',    // Cyan
        'rgba(0, 128, 128, 0.7)',    // Teal
        'rgba(46, 184, 87, 0.7)',    // Sea Green
     


    ];

    window.chartTortaEmpleados = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: datosMostrar.map(item => item.nombreEmpleado),
            datasets: [{
                data: datosMostrar.map(x => x.cantidad),
                backgroundColor: colores,
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: 'Distribución de introducciones por empleado',
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
                        size: 20
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