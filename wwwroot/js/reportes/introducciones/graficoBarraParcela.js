
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
