// Report functions
window.downloadFile = function (filename, base64Content) {
    const link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + base64Content;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};

window.renderMotionChart = function (labels, data) {
    const ctx = document.getElementById('motionChart');
    if (!ctx) return;

    // Destroy existing chart if any
    if (window.motionChartInstance) {
        window.motionChartInstance.destroy();
    }

    window.motionChartInstance = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Số lần phát hiện chuyển động',
                data: data,
                backgroundColor: 'rgba(54, 162, 235, 0.5)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: true,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 1
                    }
                }
            },
            plugins: {
                legend: {
                    display: true,
                    position: 'top'
                },
                title: {
                    display: true,
                    text: 'So sánh tần suất chuyển động giữa các trạm'
                }
            }
        }
    });
};
