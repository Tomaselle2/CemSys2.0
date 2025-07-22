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
        'rgba(169, 169, 169, 0.7)' // Color para "Otros"
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