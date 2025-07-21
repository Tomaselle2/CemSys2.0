var chartPorMes = null;


function BarrasIntroduccionesMes(data) {

    if (chartPorMes !== null) {
        chartPorMes.destroy();
    }

    if (data.length === 0) {
        alert("No hay datos para mostrar.");
        $('#contenedorBtnPdf').hide();
        return;
    }
    const datosMes = data.filter(x => x.cantidad > 0);
    const ctx = document.getElementById('graficoPorMes').getContext('2d');

    const etiquetas = datosMes.map(x => `${x.mes}/${x.año}`);
    const cantidades = datosMes.map(x => x.cantidad);


    $('#graficoPorMes').closest('.contenedor-reporte').show();

    


    chartPorMes = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: etiquetas,
            datasets: [{
                label: 'Cantidad de Introducciones',
                data: cantidades,
                backgroundColor: 'yellow', // Nombre de color simple
                borderColor: 'black',
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
                x: { ticks: { font: { size: 20 } } },
                y: {
                    beginAtZero: true,
                    ticks: { stepSize: 1 }
                }
            }
        }
    });

    chartPorMes.update();


}



