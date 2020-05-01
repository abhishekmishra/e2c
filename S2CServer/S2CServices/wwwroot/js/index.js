var currentSimId;

var simConfig = {
    space: {
        rows: 20,
        columns: 20,
        dirtProbability: 0.3,
        wallProbability: 0.0
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
        {
            type: "spiral",
            params: {}
        },        //{
        //    type: "http",
        //    params: {
        //        "url": "http://127.0.0.1:5000/"
        //    }
        //},
    ]
}

var cmdsCount = 0;

function agentLabelFromId(agentId) {
    return String.fromCharCode(64 + (agentId - 1) / 2)
}

var agentTypes = {
    "simple": "Simple Random Agent",
    "simple.bound": "Bound Checker Agent",
    "simple.boundandwall": "Bound/Wall Checker Agent",
    "http": "HTTP Proxy Agent"
}

var modalAgentArr;

$("#configModal").on("shown.bs.modal", function (e) {
    $('#numRows').val(simConfig.space.rows);
    $('#numCols').val(simConfig.space.columns);
    $('#dirtProb').val(simConfig.space.dirtProbability);
    $('#wallProb').val(simConfig.space.wallProbability);

    //deep clone array
    modalAgentArr = JSON.parse(JSON.stringify(simConfig.agents));

    initAgentConfigInModal();
});

$('#configModalAddAgent').click((e) => {
    var numAgentsVal = parseInt($('#numAgents').text());
    $('#numAgents').text(numAgentsVal + 1);

    modalAgentArr.push({
        type: "simple",
        params: {}
    });

    if (numAgentsVal + 1 == 5) {
        $('#configModalAddAgent').prop('disabled', true);
    }
    initAgentConfigInModal();
});

$("#configModalSave").click((e) => {
    simConfig.space.rows = parseInt($('#numRows').val());
    simConfig.space.columns = parseInt($('#numCols').val());
    simConfig.space.dirtProbability = parseFloat($('#dirtProb').val());
    simConfig.space.wallProbability = parseFloat($('#wallProb').val());
    simConfig.agents = modalAgentArr;
    $("#configModal").modal("toggle");
});

function initAgentConfigInModal() {
    $('#numAgents').text(modalAgentArr.length);
    $('#agentConfig').html('');
    for (var i = 0; i < modalAgentArr.length; i++) {
        var typeSelect = createAgentSelect(i);
        typeSelect.val(modalAgentArr[i].type);
        if (modalAgentArr[i].type == "http") {
            var urlElemId = 'agentType_url_' + i;
            $('#' + urlElemId).val(modalAgentArr[i].params['url']).attr('type', 'text');
        }
    }
}

function createAgentSelect(i) {
    var formElemId = 'agentType_' + i;
    var labelElemId = 'agentType_label_' + i;
    var buttonElemId = 'agentType_btn_' + i;
    var urlElemId = 'agentType_url_' + i;
    var typeSelect = $('<select>').attr('id', formElemId)
        .attr('class', 'form-control')
        .attr('agent_id', i);
    for (var type in agentTypes) {
        typeSelect.append(new Option(agentTypes[type], type));
    }
    $('#agentConfig').append($('<label>')
        .attr('for', formElemId)
        .attr('id', labelElemId)
        .text("Agent #" + i));
    $('#agentConfig').append($('<button>')
        .attr('class', 'btn btn-danger btn-sm ml-2')
        .attr('id', buttonElemId)
        .text("Del"));
    $('#agentConfig').append(typeSelect);
    $('#agentConfig').append($('<input>').attr('type', 'hidden')
        .attr('class', 'form-control')
        .attr('id', urlElemId));
    
    $('#' + buttonElemId).click((e) => {
        removeRow(i);
    });
    $('#' + formElemId).change((e) => {
        //console.log($('#' + formElemId).val());
        var aid = parseInt($('#' + formElemId).attr('agent_id'));
        var urlElemId = 'agentType_url_' + aid;
        if ($('#' + formElemId).val() == "http") {
            $('#' + urlElemId).attr('type', 'text');
            modalAgentArr[aid].params['url'] = $('#' + urlElemId).val();
        } else {
            $('#' + urlElemId).attr('type', 'hidden');
            delete modalAgentArr[aid].params['url'];
        }
        modalAgentArr[aid].type = $('#' + formElemId).val();
    });
    return typeSelect;
}

function removeRow(i) {
    var numAgentsVal = parseInt($('#numAgents').text());
    modalAgentArr.splice(i, 1);
    $('#numAgents').text(numAgentsVal - 1);
    $('#configModalAddAgent').prop('disabled', false);
    initAgentConfigInModal();
}

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
