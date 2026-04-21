
// dashboard-chart.js
// Example Dashboard Charts using Chart.js

// Bar Chart
function loadBarChart() {

    const ctx = document.getElementById('barChart').getContext('2d');

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
            datasets: [{
                label: 'Student Fees',
                data: [1200, 1900, 3000, 2500, 2200, 3100],
                backgroundColor: [
                    'rgba(75, 192, 192, 0.6)'
                ]
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    display: true
                }
            }
        }
    });

}


//// Pie Chart
//// ✅ Attendance Pie Chart
//document.addEventListener("DOMContentLoaded", function () {

//    fetch('/StudentAttendance/GetAttendanceAmountByStatus')

//        .then(response => response.json())

//        .then(data => {

//            var status = [];
//            var total = [];
//            var colors = [];

//            data.forEach(item => {

//                status.push(item.status);
//                total.push(item.total);

//                if (item.status === "Present")
//                    colors.push("#006400");   // Green

//                else if (item.status === "Absent")
//                    colors.push("#8B0000");   // Red

//                else
//                    colors.push("#B8860B");   // Yellow

//            });

//            // ✅ IMPORTANT FIX canvas id
//            var ctx = document.getElementById('canvas-pieChart');

//            if (!ctx) return;

//            new Chart(ctx, {

//                type: 'pie',

//                data: {

//                    labels: status,

//                    datasets: [{

//                        data: total,

//                        backgroundColor: colors

//                    }]
//                },

//                options: {

//                    responsive: true,

//                    plugins: {

//                        legend: {

//                            position: 'bottom'

//                        }

//                    }

//                }

//            });

//        });

//});



function loadFeesChart() {

    fetch('/Fees/GetFeesChartData')

        .then(response => response.json())

        .then(data => {

            const ctx = document.getElementById('canvas-lineCharts');

            if (!ctx) return;

            new Chart(ctx, {

                type: 'line',

                data: {

                    labels: data.labels,

                    datasets: [{

                        label: 'Monthly Fees ₹',

                        data: data.values,

                        borderColor: '#198754',

                        backgroundColor: 'rgba(25,135,84,0.2)',

                        fill: true,

                        tension: 0.4,

                        pointBackgroundColor: '#198754',

                        pointRadius: 5

                    }]
                },

                options: {

                    responsive: true,

                    scales: {

                        y: {

                            beginAtZero: true,

                            title: {

                                display: true,

                                text: 'Fees Amount ₹'
                            }
                        },

                        x: {

                            title: {

                                display: true,

                                text: 'Month'
                            }
                        }
                    }
                }

            });

        });

}
//bar charts

document.addEventListener("DOMContentLoaded", function () {

    fetch('/Fees/GetFeesAmountByStatus')

        .then(response => response.json())

        .then(data => {

            var status = [];
            var amount = [];
            var colors = [];

            data.forEach(item => {

                status.push(item.status);
                amount.push(item.totalAmount);

                if (item.status === "Paid")
                    colors.push("#006400");   // Dark Green
                else
                    colors.push("#B8860B");   // Dark Yellow

            });

            var ctx = document.getElementById('canvas-barCharts');

            new Chart(ctx, {

                type: 'bar',

                data: {

                    labels: status,

                    datasets: [{

                        label: 'Fees Amount ₹',

                        data: amount,

                        backgroundColor: colors

                    }]
                },

                options: {

                    responsive: true,

                    scales: {

                        y: {
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'Amount ₹'
                            }
                        },

                        x: {
                            title: {
                                display: true,
                                text: 'Fees Status'
                            }
                        }

                    }

                }

            });

        });

});


document.addEventListener("DOMContentLoaded", function () {

    fetch('/Student/GetGenderCount')

        .then(response => response.json())

        .then(data => {

            var total = data.male + data.female;

            var malePercent = ((data.male / total) * 100).toFixed(0);

            var femalePercent = ((data.female / total) * 100).toFixed(0);


            // Male
            document.getElementById("maleBar").style.width = malePercent + "%";

            document.getElementById("maleText").innerText =
                data.male + " Students (" + malePercent + "%)";


            // Female
            document.getElementById("femaleBar").style.width = femalePercent + "%";

            document.getElementById("femaleText").innerText =
                data.female + " Students (" + femalePercent + "%)";

        });

});


//visitors 
document.addEventListener("DOMContentLoaded", function () {

    fetch('/Visitor/GetVisitorStats')

        .then(response => response.json())

        .then(data => {

            document.getElementById("totalVisitor").innerText = data.total;

            document.getElementById("todayVisitor").innerText = data.today;

            document.getElementById("insideVisitor").innerText = data.inside;

            document.getElementById("outsideVisitor").innerText = data.outside;

        });

});

//bottom card 

document.addEventListener("DOMContentLoaded", function () {

    fetch('/Student/GetStudentCount')
        .then(res => res.json())
        .then(data => {

            document.getElementById("studentCount").innerText = data;

        });


    fetch('/Warden/GetWardenCount')
        .then(res => res.json())
        .then(data => {

            document.getElementById("wardenCount").innerText = data;

        });


    fetch('/Visitor/GetVisitorCount')
        .then(res => res.json())
        .then(data => {

            document.getElementById("visitorCount").innerText = data;

        });

});




//// Load all charts
//window.onload = function () {

//    if (document.getElementById('barChart')) {
//        loadBarChart();
//    }
    

//    if (document.getElementById('canvas-pieChart')) {
//        loadPieChart();
//    }

//};
