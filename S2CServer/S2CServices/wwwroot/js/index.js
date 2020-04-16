﻿var currentSimId;

var simConfig = {
    space: {
        rows: 10,
        columns: 10,
        dirtProbability: 0.3,
        wallProbability: 0.1
    },
    agents: [
        {
            type: "simple",
            params: {}
        },
        {
            type: "simple.bound",
            params: {}
        },
    ]
}

var cmdsCount = 0;

$("#configModal").on("shown.bs.modal", function (e) {
    $('#numRows').val(simConfig.space.rows);
    $('#numCols').val(simConfig.space.columns);
    $('#dirtProb').val(simConfig.space.dirtProbability);
    $('#wallProb').val(simConfig.space.wallProbability);
});

$("#configModalSave").click((e) => {
    simConfig.space.rows = parseInt($('#numRows').val());
    simConfig.space.columns = parseInt($('#numCols').val());
    simConfig.space.dirtProbability = parseFloat($('#dirtProb').val());
    simConfig.space.wallProbability = parseFloat($('#wallProb').val());
    $("#configModal").modal("toggle");
});

function abort_sim() {
    cmdsCount = 0;
    $("#agent_commands").html("");
    fetch('/simulation/' + currentSimId + '/abort')
        .then((response) => {
            return response.text();
        })
        .then((data) => {
            console.log("Aborted sim " + currentSimId + " " + data);
        });
}

function tabulateScores(data, columns, colnames) {
    $("#score_table").html("");
    var table = d3.select('#score_table').append('table')
        .attr("class", "table table-dark");
    var thead = table.append('thead');
    var tbody = table.append('tbody');

    // append the header row
    thead.append('tr')
        .selectAll('th')
        .data(colnames).enter()
        .append('th')
        .text(function (column) { return column; });

    // create a row for each object in the data
    var rows = tbody.selectAll('tr')
        .data(data)
        .enter()
        .append('tr');

    // create a cell in each row for each column
    var cells = rows.selectAll('td')
        .data(function (row) {
            return columns.map(function (column) {
                return { column: column, value: row[column] };
            });
        })
        .enter()
        .append('td')
        .text(function (d) { return d.value; });

    return table;
}


function new_sim() {
    cmdsCount = 0;
    $("#agent_commands").html("");
    fetch('/simulation/new', {
        method: 'POST', // or 'PUT'
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(simConfig)
    })
        .then((response) => {
            return response.text();
        })
        .then((data) => {
            var id = data;
            currentSimId = id;
            $("sim_id").val(id);
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
                            simConfig = data;
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

            var local = d3.local();

            var rects = svgCtr.append("g")
                .selectAll("g")
                .data(status.spaceArr)
                .enter()
                .append("g") //removing
                .selectAll("rect")
                .data(function (d, i, j) {
                    local.set(this, i);
                    return d;
                })
                .enter()
                .append("rect")
                .attr("x", function (d, i, j) { return i * rw })
                .attr("y", function (d, i, j) { return local.get(this) * rh; })
                .attr("width", function (d, i, j) { return rw })
                .attr("height", function (d, i, j) { return rh })
                .attr("fill", function (d, i, j) {
                    if (d == 0) return "whitesmoke";
                    if (d == 1) return "brown";
                    if (d == 2) return "grey";
                });

            var texts = svgCtr.append("g")
                .selectAll("g")
                .data(status.agentSpaceArr)
                .enter()
                .append("g") //removing
                .selectAll("text")
                .data(function (d, i, j) {
                    local.set(this, i);
                    return d;
                })
                .enter()
                .append("text")
                .attr("x", function (d, i, j) { return (i + 0.5) * rw })
                .attr("y", function (d, i, j) { return (local.get(this) + 1) * rh; })
                .attr("fill", function (d, i, j) { return "black" })
                .attr("text-anchor", function (d, i, j) { return "middle" })
                .attr("font-size", function (d, i, j) { return rh + "px" })
                .text(function (d, i, j) {
                    if (d == 0) {
                        return '';
                    } else {
                        return String.fromCharCode(64 + (d - 1) / 2);
                    }
                });

            console.log(texts);
            //for (var i = 0; i < rows; i++) {
            //    for (var j = 0; j < cols; j++) {
            //        //        var fill = "orange";
            //        //        //console.log(status.spaceArr[i][j]);
            //        //        switch (status.spaceArr[i][j]) {
            //        //            case 0: fill = "whitesmoke";
            //        //                break;
            //        //            case 1: fill = "brown";
            //        //                break;
            //        //            case 2: fill = "grey";
            //        //                break;
            //        //        }
            //        //        svgCtr.append("rect")
            //        //            .attr("x", j * rw)
            //        //            .attr("y", i * rh)
            //        //            .attr("width", rw)
            //        //            .attr("height", rh)
            //        //            .attr("fill", fill);

            //        if (status.agentSpaceArr[i][j] > 0) {
            //            var agentId = status.agentSpaceArr[i][j];
            //            rects = rects.append("text")
            //                .attr("x", function (d, i, j) { return (j + 0.5) * rw })
            //                .attr("y", function (d, i, j) { return (i + 1) * rh })
            //                .attr("fill", function (d, i, j) { return "black" })
            //                .attr("text-anchor", function (d, i, j) { return "middle" })
            //                .attr("font-size", function (d, i, j) { return rh + "px" })
            //                .text(String.fromCharCode(64 + (agentId - 1) / 2));
            //        }
            //    }
            //}

            //var cmdDiv = d3.select("#agent_commands");
            //var $container = $("#agent_commands");
            //for (var count = 0; count < status.commands.length; count++) {
            //    var element = status.commands[count];
            //    cmdDiv.append("div")
            //        .attr('id', 'agent_command_' + cmdsCount)
            //        .text("id#" + element.agentId + " #:" + cmdsCount + " command:" + element.name)
            //    //console.log(element);
            //    //console.log(cmdsCount);

            //    // see https://stackoverflow.com/questions/2905867/how-to-scroll-to-specific-item-using-jquery
            //    var $scrollTo = $('#agent_command_' + cmdsCount);
            //    $container.scrollTop(
            //        $scrollTo.offset().top - $container.offset().top + $container.scrollTop()
            //    );
            //    cmdsCount += 1;
            //};
            tabulateScores(status.agentStatistics,
                ["agentId", "clean", "moves", "efficiency", "errorRate"],
                ["#", "Clean", "Move", "Eff %", "Err %"]);
            fetchRound(id, roundNum + 1);
        });
}
