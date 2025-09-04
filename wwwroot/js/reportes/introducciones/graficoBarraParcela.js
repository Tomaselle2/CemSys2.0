
var chartBarraParcela = null;

function mostrarListaTiposParcela(dataLista, total) {
    if (!dataLista || dataLista.length === 0) {
        alert("No hay datos para mostrar.");
        $('#contenedorBtnPdf').hide();
        return;
    }

    // Mostrar el contenedor ANTES
    const contenedor = $('#graficoBarraParcela').closest('.contenedor-reporte');
    contenedor.show();

    // Esperar a que el DOM pinte el canvas antes de crear el gráfico
    requestAnimationFrame(() => {

        const ctxBarraParcela = document.getElementById('graficoBarraParcela')?.getContext('2d');
        if (!ctxBarraParcela) {
            console.error("No se encontró el canvas");
            return;
        }

        if (chartBarraParcela) {
            chartBarraParcela.destroy();
        }

        const labels = dataLista.map(item => item.tipo);

        const datos = dataLista.map(item => item.cantidad);

        const colores = [

            'rgba(0, 71, 171, 0.7)',     // Cobalt Blue
            'rgba(79, 66, 133, 0.7)',    // Ocean Blue
            'rgba(160, 150, 200, 0.7)',  // Muted Lilac
            'rgba(153, 102, 204, 0.7)',  // Deep Lilac
            'rgba(156, 79, 172, 0.7)',   // Plum Lilac
            'rgba(204, 0, 204, 0.7)',     // Vivid Lilac
            'rgba(175, 238, 238, 0.7)',  // Pale Blue
            'rgba(127, 255, 212, 0.7)',  // Aquamarine
            'rgba(64, 224, 208, 0.7)',   // Turquoise
            'rgba(0, 206, 209, 0.7)',    // Deep Turquoise
            'rgba(0, 191, 255, 0.7)',    // Cyan
            'rgba(65, 105, 225, 0.7)',   // Royal Blue
            
           

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
                        font: { size: 24 }
                    },
                    legend: { display: false },
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
                    x: { ticks: { font: { size: 20 } } },
                    y: { beginAtZero: true, ticks: { stepSize: 1, font: { size: 12 } } }
                }
            }
        });

        // 🔑 Importante: forzar resize una vez creado
        chartBarraParcela.resize();
    });
}
