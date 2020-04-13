fetch('/simulation/start')
    .then((response) => {
        return response.text();
    })
    .then((data) => {
        d3.select("#simulation_status").text(data);
        console.log(data);
        if (data == "started") {
            fetchRound(0);
        }
    });

function fetchRound(roundNum) {
    fetch('/simulation/round/' + roundNum)
        .then((response) => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.text();
        })
        .then((statusText) => {
            console.log(statusText);
            if (statusText == "") {
                return null;
            }
            var status = JSON.parse(statusText);
            console.log(status);
            d3.select("#round_num").text(status.roundNum);
            d3.select("#sim_table").html("");
            d3.select("#sim_table").selectAll("tr")
                .data(status.spaceArr)
                .enter().append("tr")
                .selectAll("td")
                .data(function (d, i, j) { return d; }).enter().append("td")
                .text(function (d, i, j) {
                    console.log(d);
                    return d;
                });

            fetchRound(roundNum + 1);
        });
}
