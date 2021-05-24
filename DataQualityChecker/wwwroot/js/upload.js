$(document).ready(function () {

    var urlParams = new URLSearchParams(window.location.search);
    var sessionID = urlParams.get('sessionID');

    //Upload part

    var HeaderColumnEnums = {
        Header: 0,
        ColumnType: 1,
        Unique: 2,
        AllowNull: 3,
        Format: 4,
        Options: 5,
        Ranges: 6,
    };

    var ErrorCodes = {
        Null: 0,
        //Starts with
        WrongType: 1,
        Duplicate: 2,
        WrongFormat: 3,
        WrongOptions: 4,
        WrongRanges: 5,
    };

  var headerTableRowData = "<td>" +
          "<select class='form-control column-type'>" +
              "<option value='Text'>Text</option>" +
              "<option value='Boolean'>Boolean</option>" +
              "<option value='Integer'>Integer</option>" +
              "<option value='Float'>Float</option>" +
              "<option value='DateTime'>DateTime</option>" +
              "<option value='Time'>Time</option>" +
              "<option value='Char'>Char</option>" +
          "</select >" +
        "</td>" +
        "<td>" +
            "<input type='checkbox' class='unique'>" + 
        "</td>" +
        "<td>" + 
            "<input type='checkbox' class='allow-null'>" +
        "</td>" +
        "<td>" +
            "<input type='text' class='form-control data-format' maxlength='50'>" +
        "</td>" +
      "<td>" +
        '<div class="d-flex flex-row">' +
            '<div class="input-group custom-input">' +
            '<input type="text" class="form-control" maxlength="50">' +
                '<div class="input-group-append">' +
                    '<span class="input-group-text"><a class="add-option-btn"><i class="fa fa-plus"></i></a></span>' +
                '</div>' +
            '</div>' +
            '<div class="dropdown">' +
                '<button class="btn btn-secondary dropdown-toggle" type="button" id="dropdownMenuButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"></button>' +
                '<div class="dropdown-menu options-menu" aria-labelledby="dropdownMenuButton">' +
            '</div>' +
          '</div>' +
        '</div>' +
      "</td>" +
        "<td>" +
            '<div class="d-flex flex-row">' +
                '<div class="input-group">' +
                    '<input type="number" class="form-control range-numeric start-range" placeholder="0">' +
                    '<div class="range-delimiter"><b> - </b></div>' +
                    '<input type="number" class="form-control range-numeric end-range" placeholder="0">' +
                    '<button class="btn btn-secondary add-range-btn"><i class="fa fa-plus"></i></button>' +
                '</div>' +
                '<div class="dropdown">' +
                    '<button class="btn btn-secondary dropdown-toggle" type="button" id="dropdownMenuButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"></button>' +
                    '<div class="dropdown-menu ranges-menu" aria-labelledby="dropdownMenuButton">' +
                    '</div>' +
                '</div>' +
            '</div>' +
      "</td>"
      ;

    $(document).on('change', ".column-type", function (e) {
        var targetSelect = $(this);
        var input = targetSelect.closest("tr").find("td:eq(" + HeaderColumnEnums.Format + ") input[type='text']");

        if (targetSelect.val() != "Text" && targetSelect.val() != "DateTime" && targetSelect.val() != "Time") {
            input.val("");
            input.prop("disabled", true);
        }
        else {
            input.prop("disabled", false);
        }

        var ranges = targetSelect.closest("tr").find("td:eq(" + HeaderColumnEnums.Ranges + ") input[type='number']");
        if (targetSelect.val() != "Text" && targetSelect.val() != "Integer" && targetSelect.val() != "Float") {
            ranges.prop("disabled", true);
        }
        else {
            ranges.prop("disabled", false);
        }
    });

    $(document).on('change', "#urlInput", function (e) {
        if ($(this).val().length && $(this).val().length > 0) {
            $('#files').prop('disabled', true);
        } else {
            $('#files').prop('disabled', false);
        }
    });

    $(document).on('change', "#files", function (e) {
        var files = $("#files").prop('files');

        if (files.length != 0) {
            $('#urlInput').prop('disabled', true);
        } else {
            $('#urlInput').prop('disabled', false);
        }
    });


    var settingsData = [];

    $("#headersSettingsButton").on("click", function (e) {
        e.preventDefault();

        $('#headersTable tbody tr').each(function () {
            var colID = $(this).attr("col-id");
            var columnType = $(this).find("td:eq(" + HeaderColumnEnums.ColumnType + ") :selected").text();
            var unique = $(this).find("td:eq(" + HeaderColumnEnums.Unique + ") input[type='checkbox']").prop('checked');
            var allowNull = $(this).find("td:eq(" + HeaderColumnEnums.AllowNull + ") input[type='checkbox']").prop('checked');
            var format = $(this).find("td:eq(" + HeaderColumnEnums.Format + ") input[type='text']").val();

            var options = [];
            var optionsFromTable = $(this).find("td:eq(" + HeaderColumnEnums.Options + ") .dropdown-item");
            optionsFromTable.each(function () {
                options.push($(this).text());
            });

            var ranges = [];
            var rangesFromTable = $(this).find("td:eq(" + HeaderColumnEnums.Ranges + ") .dropdown-item");
            rangesFromTable.each(function () {
                ranges.push($(this).text());
            });

            settingsData.push({
                colIDVal: parseInt(colID),
                columnTypeVal: columnType,
                uniqueVal: unique,
                allowNullVal: allowNull,
                formatVal: format,
                optionsVal: options,
                rangesVal: ranges,
            });
        });

        var jsonData = {
            docID: sessionID,
            settings: settingsData,
        };

        $.ajax({
            url: '/api/validatefile',
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(jsonData),
            success: function (data) {
                let parsedData = JSON.parse(data);
                if (parsedData.status) {
                    setErrorCountersData(parsedData.counters);

                    getHeadersForErrorTable();
                    createTable();

                    $("#fileUploadBlock").css("display", "none");
                    $("#menuTabs").css("display", "block");

                    $('#packageDocsUploadModal').modal('hide');

                    appendAnalysisTable();
                }
            },
            error: function () {

            },
        });

    });

    if (sessionID && sessionID.length != 0) {
        modificationInterval();

        $.ajax({
            url: '/api/getvalidation?sessionID=' + sessionID,
            type: "GET",
            success: function (data) {
                let parsedData = JSON.parse(data);
                if (parsedData.status) {
                    setErrorCountersData(parsedData.counters);
                    settingsData = parsedData.settings;

                    getHeadersForErrorTable();
                    createTable();

                    $("#fileUploadBlock").css("display", "none");
                    $("#menuTabs").css("display", "block");

                    $('#packageDocsUploadModal').modal('hide');

                    appendAnalysisTable();
                }
            },
            error: function () {

            },
        });
    }


  $("#uploadButton").on("click", function (e) {
      e.preventDefault();

      var files = $("#files").prop('files');

      if (files.length != 0) {
          
          var formData = new FormData();

          formData.append("file", files[0]);

          var noHeader = $("#noHeaderCheckBox").prop('checked');
          var headerStarts = $("#headerStartsOn").val();
          var rowsCount = $("#rowsCount").val();

          $.ajax({
              url: "/api/uploadfile?noHeader=" + noHeader + "&headerStarts=" + headerStarts + "&rowsCount=" + rowsCount,
              data: formData,
              processData: false,
              contentType: false,
              type: "POST",
              success: function (data) {

                  let parsedData = JSON.parse(data);
                  if (parsedData.status) {
                      $("#fileUploadErrorMessage").css("display", "none");

                      for (var i = 0; i != parsedData.headers.length; i++) {
                          var row = `<tr col-id="${parsedData.headers[i].ColumnIndex}"><td>${parsedData.headers[i].ColumnName}</td>${headerTableRowData}</tr>`;
                          $("#headersTable").find('tbody').append(row);
                      }

                      sessionID = parsedData.sessionID;
                      $('#packageDocsUploadModal').modal('show');

                      window.history.replaceState(null, null, "?sessionID=" + sessionID);
                      modificationInterval();
                  } else {
                      $("#fileUploadErrorMessage").text(parsedData.errorMessage);
                      $("#fileUploadErrorMessage").css("display", "block");
                  }
              },
              error: function () { },
          });

      } else {

          var urlData = {
              URL: $("#urlInput").val(),
              noHeader: $("#noHeaderCheckBox").prop('checked'),
              headerStarts: parseInt($("#headerStartsOn").val()),
              rowsCount: isNaN(parseInt($("#rowsCount").val())) ? 0 : parseInt($("#rowsCount").val()),
          };

          $.ajax({
              url: "/api/uploadurl",
              type: "POST",
              contentType: "application/json",
              data: JSON.stringify(urlData),
              success: function (data) {

                  let parsedData = JSON.parse(data);
                  if (parsedData.status) {
                      $("#fileUploadErrorMessage").css("display", "none");

                      for (var i = 0; i != parsedData.headers.length; i++) {
                          var row = `<tr col-id="${parsedData.headers[i].ColumnIndex}"><td>${parsedData.headers[i].ColumnName}</td>${headerTableRowData}</tr>`;
                          $("#headersTable").find('tbody').append(row);
                      }

                      sessionID = parsedData.sessionID;
                      $('#packageDocsUploadModal').modal('show');

                      window.history.replaceState(null, null, "?sessionID=" + sessionID);
                  } else {
                      $("#fileUploadErrorMessage").text(parsedData.errorMessage);
                      $("#fileUploadErrorMessage").css("display", "block");
                  }
              },
              error: function () { },
          });
      }
  });



  //Validation part

    var headers = [];
    var columnCount = 0;

    function getHeadersForErrorTable() {

        headers = [];
        columnCount = 0;

        $.ajax({
            url: "api/gettableheaders?sessionID=" + sessionID,
            type: "GET",
            async: false,
            success: function (data) {

                let parsedData = JSON.parse(data);
                if (parsedData.status) {
                    columnCount = parsedData.headers.length;
                    for (var i = 0; i < parsedData.headers.length; i++) {
                        var header = {
                            "data": parsedData.headers[i] == null ? "Column" + i : parsedData.headers[i],
                            "title": parsedData.headers[i] == null ? "Column" + i : parsedData.headers[i],
                            "name": parsedData.headers[i] == null ? "Column" + i : parsedData.headers[i],
                        };
                        headers.push(header);
                    }

                    var header = {
                        "data": null,
                        "render": function (data, type, row, meta) {

                            return `<a class="btn btn-danger remove-row" style="padding: 10px 15px 10px 15px; cursor: pointer;" title="Remove row"><i class="fa fa-trash" style="font-size: 20px;color: white;"></i></a>`;

                        }
                    };
                    headers.push(header);
                }
            },
            error: function () {
            },
        });
    }

    $(document).on('click', ".remove-row", function (e) {
        e.preventDefault();
        var rowID = parseInt($(this).parent().attr('row-id'));

        $.ajax({
            url: "api/removerow?sessionID=" + sessionID + "&rowID=" + rowID,
            type: "GET",
            async: false,
            success: function (data) {
                let parsedData = JSON.parse(data);
                if (parsedData.status) {
                    $('#cellValidationTable').DataTable().ajax.reload(function () {
                        if ($('#cellValidationTable').find("tbody tr td").length == 1) {
                            $("#cellValidationTable_wrapper").hide();
                            $("#noRecordsInErrorTable").css("display", "block");
                            $("#noRecordsInErrorTable").addClass("d-flex justify-content-center");
                        }
                    }, false);

                    getCounters();
                }
            },
            error: function () {
            },
        });
    });

    function getCounters() {
        $.ajax({
            url: "api/getcounters?sessionID=" + sessionID,
            type: "GET",
            success: function (data) {
                let parsedData = JSON.parse(data);
                if (parsedData.status) {
                    setErrorCountersData(parsedData.counters);
                }
            },
            error: function () {
            },
        });
    }

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        $($.fn.dataTable.tables(true)).css('width', '100%');
        $($.fn.dataTable.tables(true)).DataTable().columns.adjust().draw();
    }); 

    function createTable() {

        $("#cellValidationTable").DataTable({
            processing: true,
            serverSide: true,
            searching: false,
            ordering: false,
            bLengthChange: true,
            scrollX: true,
            ajax: {
                url: "api/errorstable?sessionID=" + sessionID,
                type: "POST",
                dataSrc: "data",
            },
            columns: headers,
            aaSorting: [],
            language: {
                "zeroRecords": "No data retrieved"
            },
            rowCallback: function (row, data) {
                var columnWithError = data.ErrorColumnNums;

                for (var i = 1; i <= columnCount; i++) {
                    var errorsArray = columnWithError.split(";");
                    errorsArray = errorsArray.filter(item => item);

                    if (columnWithError.includes(";" + i + "|")) {
                        var errorCodes = ";";
                        var errorsArray = errorsArray.filter(item => item.startsWith(i + "|"));

                        errorsArray.forEach(getErrors);
                        function getErrors(item) {
                            errorCodes += item.split("|")[1] + ";";
                        }

                        if (errorsArray && errorsArray.length == 1) {
                            if (errorCodes.includes(";" + ErrorCodes.Null + ";")) {
                                $('td:eq(' + (i - 1) + ')', row).addClass("stats__box--yellow");
                            } else if (errorCodes.includes(";" + ErrorCodes.WrongType + ".")) {
                                $('td:eq(' + (i - 1) + ')', row).addClass("stats__box--orange");
                            } else if (errorCodes.includes(";" + ErrorCodes.Duplicate + ";")) {
                                $('td:eq(' + (i - 1) + ')', row).addClass("stats__box--sand");
                            } else if (errorCodes.includes(";" + ErrorCodes.WrongOptions + ";")) {
                                $('td:eq(' + (i - 1) + ')', row).addClass("stats__box--light-red");
                            } else if (errorCodes.includes(";" + ErrorCodes.WrongRanges + ";")) {
                                $('td:eq(' + (i - 1) + ')', row).addClass("stats__box--salad");
                            } else if (errorCodes.includes(";" + ErrorCodes.WrongFormat + ";")) {
                                $('td:eq(' + (i - 1) + ')', row).addClass("stats__box--dark-orange");
                            }
                        } else if (errorsArray && errorsArray.length > 1) {
                            $('td:eq(' + (i - 1) + ')', row).addClass("many-errors");
                        } else {
                            $('td:eq(' + (i - 1) + ')', row).addClass("many-errors");
                        }

                        $('td:eq(' + (i - 1) + ')', row).attr("error-code", errorCodes);
                        $('td:eq(' + (i - 1) + ')', row).attr('row-id', data.RowNum);
                        $('td:eq(' + (i - 1) + ')', row).attr('col-id', i);
                        $('td:eq(' + (i - 1) + ')', row).addClass('cellWithError');

                    }

                    $('td:last-child', row).attr('row-id', data.RowNum);
                }
            },
            fnInitComplete: function (data) {
                if (data._iRecordsTotal == 0) {
                    $(this).parent().hide();
                    $("#noRecordsInErrorTable").css("display", "block");
                    $("#noRecordsInErrorTable").addClass("d-flex justify-content-center");
                }
            },
        });

        $('#cellValidationTable').on('click', 'tbody td.cellWithError', function (e) {
            e.preventDefault();

            var errorCodes = $(this).attr("error-code");
            var rowNum = parseInt($(this).attr('row-id'));
            var columnNum = parseInt($(this).attr('col-id'));
            var oldValue = $(this).text();

            $("#changedValue").val(oldValue);
            $("#saveErrorChangeButton").attr("error-code", errorCodes);
            $("#saveErrorChangeButton").attr('row-id', rowNum);
            $("#saveErrorChangeButton").attr('col-id', columnNum);

            var settingsData = getNecessaryFromSettingsArray(columnNum);

            $("#cellEditType").text("Column Type: " + settingsData.columnTypeVal);
            $("#cellEditCanNull").text("Can be Null: " + boolToString(settingsData.allowNullVal));
            $("#cellEditIsUnique").text("Should be Unique: " + boolToString(settingsData.uniqueVal));
            $("#cellEditFormat").html("Format: " + ((settingsData.formatVal && settingsData.formatVal.length > 0) ? settingsData.formatVal : "<b>No Format</b>"));

            if (settingsData.optionsVal && settingsData.optionsVal.length > 0) {
                var message = "";
                for (var i = 0; i != settingsData.optionsVal.length - 1; i++) {
                    message += "<b>" + settingsData.optionsVal[i] + "</b> ; ";
                }
                message += "<b>" + settingsData.optionsVal[settingsData.optionsVal.length - 1] + "</b>";

                $("#cellEditOptions").html("Options: " + message);
            } else {
                $("#cellEditOptions").html("Options: " + "<b>No Options</b>");
            }

            if (settingsData.rangesVal && settingsData.rangesVal.length > 0) {
                var message = "";
                for (var i = 0; i != settingsData.rangesVal.length - 1; i++) {
                    message += "<b>" + settingsData.rangesVal[i] + "</b> ; ";
                }
                message += "<b>" + settingsData.rangesVal[settingsData.rangesVal.length - 1] + "</b>";

                $("#cellEditRanges").html("Ranges: " + message);
            } else {
                $("#cellEditRanges").html("Ranges: " + "<b>No Options</b>");
            }

            $("#errorValidationModal").modal('show');

        });
    }


    $('#saveErrorChangeButton').on('click', function (e) {
        e.preventDefault();

        var rowNum = parseInt($(this).attr('row-id'));
        var columnNum = parseInt($(this).attr('col-id'));
        var newVal = $("#changedValue").val();

        let newValData = {
            newSessionID: sessionID,
            newRowNum: parseInt(rowNum),
            newColumnNum: parseInt(columnNum),
            newValue: newVal,
        };

        $.ajax({
            url: "api/fixerror",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(newValData),
            success: function (data) {
                let parsedData = JSON.parse(data);
                if (parsedData.status) {
                    $('#cellValidationTable').DataTable().ajax.reload();
                    setErrorCountersData(parsedData.counters);
                    $("#errorValidationModal").modal('hide');
                }
                else {
                    if (parsedData.errorMessage) {
                        $("#errorValidationErrorMessage").text(parsedData.errorMessage);
                    }
                    else {
                        $("#errorValidationErrorMessage").text("Unable to process request. Please, try again after certain period of time.");
                    }
                    $("#errorValidationErrorMessage").css("display", "block");
                }
            },
            error: function () {
            },
        });
    });


    function getNecessaryFromSettingsArray(columnID) {
        if (settingsData) {
            for (var i = 0; i != settingsData.length; i++) {
                if (settingsData[i].colIDVal == columnID) {
                    return settingsData[i];
                }
            }
        }
    }

    function boolToString(boolVal) {
        if (boolVal == true) return "Yes";
        else if (boolVal == false) return "No";
    }

    //Analysis part

    $("#reCheckAnalyze").on("click", function (e) {
        e.preventDefault();
        appendAnalysisTable();
    });


    function appendAnalysisTable() {
        $.ajax({
            url: "/api/getanalysis?sessionID=" + sessionID,
            type: "GET",
            success: function (data) {

                let parsedData = JSON.parse(data);
                if (parsedData.status) {

                    $("#analysisTable > tbody").empty();

                    for (var i = 0; i != parsedData.analysis.length; i++) {
                        var row = `<tr><td>${parsedData.analysis[i].title}</td><td>${parsedData.analysis[i].type}</td><td>${parsedData.analysis[i].validCount}</td><td>${parsedData.analysis[i].inValidCount}</td><td>${parsedData.analysis[i].nullCount}</td><td>${parsedData.analysis[i].notNullCount}</td><td>${parsedData.analysis[i].uniqueCount}</td><td>${parsedData.analysis[i].distinctCount}</td><td>${parsedData.analysis[i].specialCharsCount}</td></tr>`;
                        $("#analysisTable").find('tbody').append(row);
                    }

                    buildChart(parsedData.analysis);
                }
            },
            error: function () { },
        });
    }

    function buildChart(analysisRes) {
        if (analysisRes && analysisRes.length > 0) {

            var chartLabels = [];
            var chartDatasets = [];

            var genColors = generateRandomColor();
            var validCount = {
                label: "Valid",
                backgroundColor: genColors.rgbaOpacityCol,
                borderColor: genColors.rgbaCol,
                borderWidth: 1,
                data: [],
            };

            genColors = generateRandomColor();
            var nonValidCount = {
                label: "Invalid",
                backgroundColor: genColors.rgbaOpacityCol,
                borderColor: genColors.rgbaCol,
                borderWidth: 1,
                data: [],
            };

            genColors = generateRandomColor();
            var nullCount = {
                label: "Null",
                backgroundColor: genColors.rgbaOpacityCol,
                borderColor: genColors.rgbaCol,
                borderWidth: 1,
                data: [],
            };

            genColors = generateRandomColor();
            var nonNullCount = {
                label: "Non-null",
                backgroundColor: genColors.rgbaOpacityCol,
                borderColor: genColors.rgbaCol,
                borderWidth: 1,
                data: [],
            };

            genColors = generateRandomColor();
            var uniqueCount = {
                label: "Unique",
                backgroundColor: genColors.rgbaOpacityCol,
                borderColor: genColors.rgbaCol,
                borderWidth: 1,
                data: [],
            };

            genColors = generateRandomColor();
            var distinctCount = {
                label: "Distinct",
                backgroundColor: genColors.rgbaOpacityCol,
                borderColor: genColors.rgbaCol,
                borderWidth: 1,
                data: [],
            };

            genColors = generateRandomColor();
            var specialCharsCount = {
                label: "Special characters",
                backgroundColor: genColors.rgbaOpacityCol,
                borderColor: genColors.rgbaCol,
                borderWidth: 1,
                data: [],
            };

            for (var i = 0; i != analysisRes.length; i++) {
                chartLabels.push(analysisRes[i].title);

                validCount.data.push(parseInt(analysisRes[i].validCount));
                nonValidCount.data.push(parseInt(analysisRes[i].inValidCount));
                nullCount.data.push(parseInt(analysisRes[i].nullCount));
                nonNullCount.data.push(parseInt(analysisRes[i].notNullCount));
                uniqueCount.data.push(parseInt(analysisRes[i].uniqueCount));
                distinctCount.data.push(parseInt(analysisRes[i].distinctCount));
                specialCharsCount.data.push(parseInt(analysisRes[i].specialCharsCount));
            }

            chartDatasets.push(validCount);
            chartDatasets.push(nonValidCount);
            chartDatasets.push(nullCount);
            chartDatasets.push(nonNullCount);
            chartDatasets.push(uniqueCount);
            chartDatasets.push(distinctCount);
            chartDatasets.push(specialCharsCount);

            new Chart($("#analysisCanvasFirst"), {
                type: 'bar',
                data: {
                    labels: chartLabels,
                    datasets: chartDatasets,
                },
                options: {
                    scales: {
                        yAxes: [{
                            ticks: {
                                beginAtZero: true,
                            }
                        }]
                    }
                }
            });


            new Chart($("#analysisCanvasSecond"), {
                type: 'horizontalBar',
                data: {
                    labels: chartLabels,
                    datasets: chartDatasets,
                },
                options: {
                    scales: {
                        yAxes: [{
                            ticks: {
                                beginAtZero: true,
                            }
                        }]
                    }
                }
            });

        }
    }

    function generateRandomColor() {
        const randomColor = Math.floor(Math.random() * 16777215).toString(16);

        let returnColors = {
            rgbaOpacityCol: 'rgba(' + parseInt(randomColor.slice(-6, -4), 16)
                + ', ' + parseInt(randomColor.slice(-4, -2), 16)
                + ', ' + parseInt(randomColor.slice(-2), 16)
                + ', 0.2)',

            rgbaCol: 'rgba(' + parseInt(randomColor.slice(-6, -4), 16)
                + ', ' + parseInt(randomColor.slice(-4, -2), 16)
                + ', ' + parseInt(randomColor.slice(-2), 16)
                + ', 1)',
        };

        return returnColors;
    }

    $("#errorValidationModalCloseBtn").on("click", function (e) {
        $("#errorValidationErrorMessage").css("display", "none");
    });

    function setErrorCountersData(counters) {
        if (counters && counters != 0) {
            $('#TotalCellsCount').text(counters.AllCells);
            $('#EmptyCellsCount').text(counters.NullCells);
            $('#DuplicateCellsCount').text(counters.DuplicateCells);
            $('#WrongTypeCellsCount').text(counters.WrongTypeCells);
            $('#WrongFormatCellsCount').text(counters.WrongFormatCells);
            $('#WrongOptionsCellsCount').text(counters.WrongOptionsCells);
            $('#WrongRangesCellsCount').text(counters.WrongRangesCells);
        }
    }

    //options adder

    $(document).on('click', ".add-option-btn", function (e) {
        var parentInputBlock = $(this).parent().parent().siblings(".form-control");
        var inputVal = parentInputBlock.val();
        var parentBlock = $(this).parent().parent().parent().siblings(".dropdown").find(".dropdown-menu");

        if (!checkIfElementExistInDropdownMenu(parentBlock, inputVal)) {

            var newOptionVal = '<div class="dropdown-item">' +
                '<a class="remove-option"><i class="fa fa-remove"></i></a>' +
                inputVal +
                '</div>';

            parentBlock.append(newOptionVal);
        }
    });

    $(document).on('click', ".dropdown-menu .remove-option", function (e) {
        var parentBlock = $(this).parent();
        parentBlock.remove();
    });

    $(document).on('click', ".options-menu", function (e) {
        e.stopPropagation();
    });

    //ranges adder

    $(document).on('click', ".add-range-btn", function (e) {
        var startVal = parseFloat($(this).parent().find(".start-range").val());
        if (Number.isNaN(startVal)) {
            startVal = 0;
            $(this).parent().find(".start-range").val("0");
        }

        var endVal = parseFloat($(this).parent().find(".end-range").val());
        if (Number.isNaN(endVal)) {
            endVal = 0;
            $(this).parent().find(".end-range").val("0");
        }

        if (startVal <= endVal) {
            $(this).parent().find(".start-range").removeClass("is-invalid");
            $(this).parent().find(".end-range").removeClass("is-invalid");

            var rangeVal = parseFloat(startVal) + " - " + parseFloat(endVal);
            if (rangeVal.length <= 50) {
                var parentBlock = $(this).parent().siblings(".dropdown").find(".dropdown-menu");

                if (!checkIfElementExistInDropdownMenu(parentBlock, rangeVal)) {
                    var newOptionVal = '<div class="dropdown-item">' +
                        '<a class="remove-option"><i class="fa fa-remove"></i></a>' +
                        rangeVal +
                        '</div>';

                    parentBlock.append(newOptionVal);
                }
                else {
                    $(this).parent().find(".start-range").addClass("is-invalid");
                    $(this).parent().find(".end-range").addClass("is-invalid");
                }
            }
        }
        else {
            $(this).parent().find(".start-range").addClass("is-invalid");
            $(this).parent().find(".end-range").addClass("is-invalid");
        }
    });

    $(document).on('click', ".ranges-menu", function (e) {
        e.stopPropagation();
    });

    $(document).on('click', '.range-numeric', function (e) {
        $(this).removeClass("is-invalid");
    });

    //diagram switcher

    $(document).on('change', "#diagramSwitch", function (e) {
        var checked = $(this).prop('checked'); 
        if (checked) {
            $("#analysisDiagramFirst").css("display", "block");
            $("#analysisDiagramSecond").css("display", "none");
        }
        else {
            $("#analysisDiagramFirst").css("display", "none");
            $("#analysisDiagramSecond").css("display", "block");
        }
    });

    //dropdrown-menu
    function checkIfElementExistInDropdownMenu(dropMenu, newElem) {
        var presented = false;
        var dropMenuElems = dropMenu.find(".dropdown-item");

        if (dropMenuElems) {

            var count = dropMenuElems.length;
            if (count <= 50) {

                dropMenuElems.each(function () {

                    var val = $(this).text();
                    if (val == newElem) {
                        presented = true;
                        return true;
                    }
                });
            }
            else {
                presented = true;
            }
        }
        return presented;
    }

    $("#noHeaderCheckBox").on('change', function (e) {
        var checked = $(this).prop('checked');
        if (checked) {
            $("#headerStartsOn").prop("readonly", true);
        }
        else {
            $("#headerStartsOn").prop("readonly", false);
        }
    });

    $("#downloadDataChangeBtn").on('click', function (e) {
        e.preventDefault();

        if (sessionID && sessionID.length != 0) {
            window.open('/api/downloadfile?sessionID=' + sessionID + '', '_blank');
        }
    });


    //Session modification
    function modificationInterval() {
        $(window).unload(function () {
            $.ajax({
                url: '/api/updatesession?sessionID=' + sessionID,
                type: "GET",
                success: function (data) {
                },
                error: function () {
                },
            });
        });

        setInterval(function () {
            $.ajax({
                url: '/api/updatesession?sessionID=' + sessionID,
                type: "GET",
                success: function (data) {
                },
                error: function () {
                },
            });
        }, 7200000);
    }
});