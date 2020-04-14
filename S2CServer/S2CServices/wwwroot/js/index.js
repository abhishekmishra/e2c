var simConfig = {
    space: {
        rows: 5,
        columns: 5,
        dirtProbability: 0.3,
        wallProbability: 0.1
    },
    agents: [
        {
            type: "simple"
        }
    ]
}

function new_sim() {
    fetch('/simulation/new/default')
        .then((response) => {
            return response.text();
        })
        .then((data) => {
            var id = data;
            fetch('/simulation/' + id + '/start')
                .then((response) => {
                    return response.text();
                })
                .then((data) => {
                    d3.select("#simulation_status").text(data);
                    //console.log(data);
                    if (data == "started") {
                        fetchRound(id, 0);
                    }
                }).then(() => {
                    fetch('/simulation/' + id + '/config')
                    .then((response) => {
                        return response.json();
                    }).then((data) => {
                        console.log(data);
                    });
                });
        });
}

function fetchRound(id, roundNum) {
    fetch('/simulation/' + id + '/round/' + roundNum)
        .then((response) => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.text();
        })
        .then((statusText) => {
            //console.log(statusText);
            if (statusText == "") {
                return null;
            }
            var status = JSON.parse(statusText);
            //console.log(status);
            d3.select("#round_num").text(status.roundNum);
            d3.select("#sim").html("");

            var w = 400, h = 400;

            var svgCtr = d3.select("#sim").append("svg")
                .attr("width", w)
                .attr("height", h);

            var rows = status.spaceArr.length;
            var cols = status.spaceArr[0].length;

            var rw = w / cols;
            var rh = h / rows;
            for (var i = 0; i < rows; i++) {
                for (var j = 0; j < cols; j++) {
                    var fill = "orange";
                    //console.log(status.spaceArr[i][j]);
                    switch (status.spaceArr[i][j]) {
                        case 0: fill = "red";
                            break;
                        case 1: fill = "brown";
                            break;
                        case 2: fill = "grey";
                            break;
                    }
                    svgCtr.append("rect")
                        .attr("x", j * rw)
                        .attr("y", i * rh)
                        .attr("width", rw)
                        .attr("height", rw)
                        .attr("fill", fill);
                }
            }
            //svgCtr.data(status.spaceArr)
            //    .enter().append("tr")
            //    .selectAll("td")
            //    .data(function (d, i, j) { return d; }).enter().append("td")
            //    .text(function (d, i, j) {
            //        console.log(d);
            //        return d;
            //    });

            fetchRound(id, roundNum + 1);
        });
}
