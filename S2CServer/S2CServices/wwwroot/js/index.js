var currentSimId;

var simConfig = {
    space: {
        rows: 20,
        columns: 20,
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
        {
            type: "simple.boundandwall",
            params: {}
        },
    ]
}

var cmdsCount = 0;

function agentLabelFromId(agentId) {
    return String.fromCharCode(64 + (agentId - 1) / 2)
}

var agentTypes = {
    "simple": "Simple Random Agent",
    "simple.bound": "Random Agent w/bound check",
    "simple.boundandwall": "Random Agent w/bound & wall check"
}

$("#configModal").on("shown.bs.modal", function (e) {
    $('#numRows').val(simConfig.space.rows);
    $('#numCols').val(simConfig.space.columns);
    $('#dirtProb').val(simConfig.space.dirtProbability);
    $('#wallProb').val(simConfig.space.wallProbability);

    $('#numAgents').val(simConfig.agents.length);
    for (var i = 0; i < simConfig.agents.length; i++) {
        var formElemId = 'agentType' + i;
        var typeSelect = $('<select>').attr('id', formElemId)
            .attr('class', 'form-control');
        for (var type in agentTypes) {
            //console.log(type);
            //var opt = $('option').attr('value', type).val(agentTypes[type]);
            //console.log(opt);
            typeSelect.append(new Option(type, agentTypes[type]));
        }
        $('#agentConfig').append($('<label>')
            .attr('for', formElemId)
            .text("Agent #" + i));
        $('#agentConfig').append(typeSelect);
    }
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
        .attr("class", "w-100 h-100 text-center");
    var thead = table.append('thead')
        .attr("class", "bg-secondary");
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
        .text(function (d) {
            return d.value;
        });

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
            if (statusText == "") {
                return null;
            }
            var status = JSON.parse(statusText);
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

            // see https://stackoverflow.com/questions/46629188/multidimensional-array-for-d3
            // for explanation of this section
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
                    if (d == 1) return "black";
                    if (d == 2) return "red";
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

            var cmdDiv = d3.select("#agent_commands");
            var $container = $("#agent_commands");
            for (var count = 0; count < status.commands.length; count++) {
                var element = status.commands[count];
                cmdDiv.append("div")
                    .attr('id', 'agent_command_' + cmdsCount)
                    .text(agentLabelFromId(element.agentId) + ": " +
                        element.name.toUpperCase() + " [" + element.location.row + ", " + element.location.column + "]");

                // see https://stackoverflow.com/questions/2905867/how-to-scroll-to-specific-item-using-jquery
                var $scrollTo = $('#agent_command_' + cmdsCount);
                $container.scrollTop(
                    $scrollTo.offset().top - $container.offset().top + $container.scrollTop()
                );
                cmdsCount += 1;
            };

            for (var i = 0; i < status.agentStatistics.length; i++) {
                var v = status.agentStatistics[i];
                v["agentLabel"] = agentLabelFromId(v["agentId"]);
            }
            tabulateScores(status.agentStatistics,
                ["agentLabel", "clean", "moves", "efficiency", "errorRate"],
                ["#", "Clean", "Move", "Eff %", "Err %"]);
            fetchRound(id, roundNum + 1);
        });
}
