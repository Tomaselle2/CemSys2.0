
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
