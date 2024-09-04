//==========전역변수================
var dataView = new Slick.Data.DataView();
var dataView1 = new Slick.Data.DataView();
var grid; var columns; var checkData = false;
var grid1; var columns1; var checkData = false;
var Mdata;
var SkuData = [];
var HblData = [];
var HouseData;
var MasterData;
var checkboxSelector;



//==========전역변수================

$(function () {
    //로그인 세션 확인
    if (_fnToNull($("#Session_USR_ID").val()) == "") {
        window.location = window.location.origin;
    }

    

    $('#StartDate').datepicker();
    $('#StartDate').datepicker("option", "maxDate", $("#EndDate").val());
    $('#StartDate').datepicker("option", "onClose", function (selectedDate) {
        $("#EndDate").datepicker("option", "minDate", selectedDate);
    });

    $('#EndDate').datepicker();
    $('#EndDate').datepicker("option", "minDate", $("#StartDate").val());
    $('#EndDate').datepicker("option", "onClose", function (selectedDate) {
        $("#StartDate").datepicker("option", "maxDate", selectedDate);
    });

    $("#StartDate").val(_fnPlusDate(-7));
    $("#EndDate").val(_fnPlusDate(0));


    //fileCus();


    //$(document).on('keyup', '#Search_Value', function (e) { 
    //    if (e.keyCode == 13) {
    //        $("#SearchBtn").click();
    //    }
    //});


    // 컬럼
    columns = [
        { id: "MBL_NO", name: "MBL", field: "MBL_NO", sortable: true, resizable: false, width: 205, cssClass: "text-center" },
        { id: "ETD", name: "ETD", field: "ETD", sortable: true, resizable: false, width: 205, cssClass: "text-center" },
        { id: "ETA", name: "ETA", field: "ETA", sortable: true, resizable: false, width: 205, cssClass: "text-center" },
        { id: "PARCEL_CNT", name: "H 건수", field: "PARCEL_CNT", sortable: true, resizable: false, width: 205, cssClass: "text-center" },
        { id: "RATE", name: "반입률", field: "RATE", sortable: true, resizable: false, width: 205, cssClass: "text-center" },
        { id: "CARRY_IN", name: "반입", field: "CARRY_IN", sortable: true, resizable: false, width: 205, cssClass: "text-center" },
        { id: "WAIT", name: "보류", field: "WAIT", sortable: true, resizable: false, width: 205, cssClass: "text-center" },
        { id: "DECLAR", name: "신고", field: "DECLAR", sortable: true, resizable: false, width: 205, cssClass: "text-center" },
        { id: "CARRY_OUT", name: "반출", field: "CARRY_OUT", sortable: true, resizable: false, width: 205, cssClass: "text-center" }
    ];

    var options = {
        enableCellNavigation: true,
        enableAddRow: false,
        autosizeColsMode: Slick.GridAutosizeColsMode.FitViewportToCols,
        enableColumnReorder:false
    }


    grid = new Slick.Grid("#myGrid", dataView, columns, options);
    grid.setSelectionModel(new Slick.RowSelectionModel({ selectActiveRow: false }));

    //// 선택된 행 가져오기
    //var selectedRows = checkboxSelector.getSelectedRows();



    grid.onSort.subscribe(function (e, args) {
        var comparer = function (a, b) {
            var x = a[args.sortCol.field], y = b[args.sortCol.field];
            return (x === y ? 0 : (x > y ? 1 : -1));
        };
        dataView.sort(comparer, args.sortAsc);
        grid.invalidate(); // 그리드를 다시 그리도록 설정
        grid.render(); // 변경 사항을 적용하여 그리드를 다시 렌더링
    });

    //#region 익셉션 조회 밑 바인딩
    //$.ajax({
    //    type: "POST",
    //    url: "/Order/LoadExcept",
    //    async: true,
    //    dataType: "json",
    //    success: function (result) {
    //        if (_fnToNull(result)) {
    //            var dataset = JSON.parse(result);

    //            if (dataset.Result[0]["trxCode"] == "Y") {
    //                fnSettingPopArea(dataset.LEFT_EXCEPT); //30 바인딩
    //                fnSettingPopArea(dataset.RIGHT_EXCEPT); // 40 바인딩
    //            } else {
    //                _fnAlertMsg("관리자에게 문의해주세요");
    //            }
    //        }
    //        else {
    //            _fnAlertMsg("관리자에게 문의해주세요");
    //        }
    //    },
    //    error: function (xhr, status, error) {
    //        console.log(error);
    //    },
    //    beforeSend: function () {
    //        $("#ProgressBar_Loading").show(); //프로그래스 바
    //    },
    //    complete: function () {
    //        $("#ProgressBar_Loading").hide(); //프로그래스 바
    //    }
    //});

    //function fnSettingPopArea(obj) {
    //    var list = obj
    //    var strHtml = "";

    //    for (var i = 0; i < list.length; i++) {
    //        strHtml += "<option value='" + list[i].SATAUS_CD + "'>" + list[i].NAME+"</option>";
    //    }

    //    if (list[0].SATAUS_CD.substring(0, 2) == "30") {
    //        $("#Hold_CD").empty();
    //        $("#Hold_CD").append(strHtml);
    //    }
    //    else {
    //        $("#Hold_Detail").empty();
    //        $("#Hold_Detail").append(strHtml);
    //    }
        
    //}

    //#endregion


});

//#region 미사용



//ETD 날짜 yyyymmdd 로 입력 시 yyyy-mm-dd 로 변경
$(document).on("focusout", "#StartDate", function () {
    var vValue = $("#StartDate").val();

    if (vValue.length > 0) {
        var vValue_Num = vValue.replace(/[^0-9]/g, "");
        if (vValue != "") {
            vValue = vValue_Num.substring("0", "4") + "-" + vValue_Num.substring("4", "6") + "-" + vValue_Num.substring("6", "8");
            $(this).val(vValue);
        }

        //값 벨리데이션 체크
        if (!_fnisDate_layer($(this).val())) {
            $(this).val(_fnPlusDate(0));
        }

        //날짜 벨리데이션 체크
        var vETD = $("#StartDate").val().replace(/[^0-9]/g, "");
        var vETA = $("#EndDate").val().replace(/[^0-9]/g, "");

        if (vETA < vETD) {
            _fnAlertMsg("시작일자는 종료일자보다 이후 일수 없습니다.");
            $("#StartDate").val(vETA.substring("0", "4") + "-" + vETA.substring("4", "6") + "-" + vETA.substring("6", "8"));
        }
    }

});

//ETD 날짜 yyyymmdd 로 입력 시 yyyy-mm-dd 로 변경
$(document).on("focusout", "#EndDate", function () {
    var vValue = $("#EndDate").val();

    if (vValue.length > 0) {
        var vValue_Num = vValue.replace(/[^0-9]/g, "");
        if (vValue != "") {
            vValue = vValue_Num.substring("0", "4") + "-" + vValue_Num.substring("4", "6") + "-" + vValue_Num.substring("6", "8");
            $(this).val(vValue);
        }
        //값 벨리데이션 체크
        if (!_fnisDate_layer($(this).val())) {
            $(this).val(_fnPlusDate(10));
        }

        //날짜 벨리데이션 체크
        var vETD = $("#StartDate").val().replace(/[^0-9]/g, "");
        var vETA = $("#EndDate").val().replace(/[^0-9]/g, "");

        if (vETA < vETD) {
            _fnAlertMsg("종료일자는 시작일자보다 이전 일수 없습니다.");
            $("#EndDate").val(vETD.substring("0", "4") + "-" + vETD.substring("4", "6") + "-" + vETD.substring("6", "8"));
        }
    }
});

//#endregion



$(document).on('click', '#SearchBtn', function () {
    columns = [
        { id: "MBL_NO", name: "MBL", field: "MBL_NO", sortable: true, resizable: false, width: 205, cssClass: "text-center"  },
        { id: "ETD", name: "ETD", field: "ETD", sortable: true, resizable: false, width: 205, cssClass: "text-center" },
        { id: "ETA", name: "ETA", field: "ETA", sortable: true, resizable: false, width: 205, cssClass: "text-center"  },
        { id: "PARCEL_CNT", name: "H 건수", field: "PARCEL_CNT", sortable: true, resizable: false, width: 205, cssClass: "text-center"  },
        { id: "RATE", name: "반입률", field: "RATE", sortable: true, resizable: false, width: 205, cssClass: "text-center"  },
        { id: "CARRY_IN", name: "반입", field: "CARRY_IN", sortable: true, resizable: false, width: 205, cssClass: "text-center" },
        { id: "WAIT", name: "보류", field: "WAIT", sortable: true, resizable: false, width: 205, cssClass: "text-center"  },
        { id: "DECLAR", name: "신고", field: "DECLAR", sortable: true, resizable: false, width: 205, cssClass: "text-center" },
        { id: "CARRY_OUT", name: "반출", field: "CARRY_OUT", sortable: true, resizable: false, width: 205, cssClass: "text-center" }
    ];

    var objJsonData = new Object();

    if (_fnToNull($("#StartDate").val().replace(/-/gi, "")) == "") {
        _fnAlertMsg("ETD를 입력 해 주세요.");
        return false;
    }

    if (_fnToNull($("#EndDate").val().replace(/-/gi, "")) == "") {
        _fnAlertMsg("ETA를 입력 해 주세요.");
        return false;
    }

    objJsonData.STRT_YMD = $("#StartDate").val().substring(0, 10).replace(/-/gi, "");
    objJsonData.END_YMD = $("#EndDate").val().substring(0, 10).replace(/-/gi, "");
    objJsonData.MBL_NO = $("#Mbl_Value").val();

    $.ajax({
        type: "POST",
        url: "/Order/fnSearchRateData",
        async: true,
/*        dataType: "json",*/
        data: { "vJsonData": _fnMakeJson(objJsonData) },
        success: function (result) {
            var html = $.parseHTML(result);
            var target = $(html).find('#rate_list');
            var dataValue = target.attr('value');

            if (_fnToNull(dataValue) != "") {
                var jsonData = JSON.parse(dataValue).Table1; // 받은 데이터에서 필요한 부분 추출
                if (jsonData.length > 0) {
                    // 데이터를 표시할 배열 초기화
                    var data = [];

                    for (var i = 0; i < jsonData.length; i++) {
                        data.push({
                            MBL_NO: jsonData[i].MBL_NO,
                            ETD: formattingDateEng(jsonData[i].ETD),
                            ETA: formattingDateEng(jsonData[i].ETA),
                            PARCEL_CNT: jsonData[i].PARCEL_CNT,
                            RATE: _fnToZero(jsonData[i].RATE),
                            CARRY_IN: _fnToZero(jsonData[i].CARRY_IN),
                            WAIT: _fnToZero(jsonData[i].WAIT),
                            DECLAR: _fnToZero(jsonData[i].DECLAR),
                            CARRY_OUT: _fnToZero(jsonData[i].CARRY_OUT),
                            id: i
                        });
                    }

                    // 그리드에서 모든 선택을 해제
                    grid.setSelectedRows([]);
                    dataView.beginUpdate();
                    dataView.setItems(data);
                    dataView.endUpdate();

                    
                    grid.setColumns(columns);
                    grid.invalidate();
                    grid.render();
                    
                }
                else {
                    _fnAlertMsg("데이터가 없습니다.");
                    dataView.beginUpdate();
                    dataView.setItems([]);
                    dataView.endUpdate();

                    grid.setColumns(columns);
                    grid.invalidate();
                    grid.render();

                }
            }
                
          //}

        }, error: function (xhr, status, error) {
            status = true;
            console.log(error);
        },
        beforeSend: function () {
            $("#ProgressBar_Loading").show(); //프로그래스 바
        },
        complete: function () {
            $("#ProgressBar_Loading").hide (); //프로그래스 바
        }
    });

})

$(document).on("click", "#btnExcel", function () {
    var selectedRows = dataView.getItems();


    //Validation
    if (selectedRows.length == 0) {
        _fnAlertMsg("조회 후 이용해주세요.");
        return false;
    }
    else {
        //필요없는 값 삭제
        delete selectedRows.SEQ;

        downloadExcelFile3(selectedRows);
    }


});

function formattingDateEng(time) {
    // Date 객체로 파싱
    var year = time.substring(0, 4);
    var month = time.substring(4, 6);
    var day = time.substring(6, 8);
    var hours = time.substring(8, 10);
    var minutes = time.substring(10, 12);
    var seconds = time.substring(12, 14);

    // 원하는 형식으로 포맷팅
    var formattedDate = year + "-" +
        month + "-" +
        day + " " +
        hours + ":" +
        minutes + ":" +
        seconds;


    return formattedDate;
}

    //=================function=================

function downloadExcelFile3(data) {


    var headers = [["MBL", "ETD", "ETA", "H 건수", "반입률", "반입", "보류", "신고", "반출"]];
    for (var i = 0; i < data.length; i++) {
        delete data[i].id;
    }
    //var headers = [];
    var worksheet = XLSX.utils.json_to_sheet(data);
    XLSX.utils.sheet_add_json(worksheet, headers, { skipHeader: true, origin: "A1" });

    worksheet["!rows"] = [{ hpt: 30, outlineLevel: 1 }]; // 각 행의 높이 지정
    worksheet['!cols'] = [
        { wpx: 100 }, // MBL
        { wpx: 120 },  // ETD
        { wpx: 120 },  // ETA
        { wpx: 60 },  // H 건수
        { wpx: 80 },  // 반입률
        { wpx: 60 },  // 반입
        { wpx: 60 },  // 보류
        { wpx: 60 },  // 신고
        { wpx: 60 },  // 반출
        { wpx: 60 }   // SEQ
    ];

    var excel_name = "Rate_" +getCurrentDateTime_YMD() + ".xlsx";
    var workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "SelectedData");
    XLSX.writeFile(workbook, excel_name);
}


    //날짜 유효성 체크 (윤달 포함)
    function _fnisDate_layer(vDate) {

        var vValue = vDate;
        var vValue_Num = vValue.replace(/[^0-9]/g, "");

        if (_fnToNull(vValue_Num) == "") {
            _fnAlertMsg("날짜를 입력 해 주세요.");
            return false;
        }

        //8자리가 아닌 경우 false
        if (vValue_Num.length != 8) {
            _fnAlertMsg("날짜를 20200101 or 2020-01-01 형식으로 입력 해 주세요.");
            return false;
        }

        var rxDatePattern = /^(\d{4})(\d{1,2})(\d{1,2})$/; //Declare Regex
        var dtArray = vValue_Num.match(rxDatePattern); // is format OK?

        if (dtArray == null) {
            return false;
        }

        dtYear = dtArray[1];
        dtMonth = dtArray[2];
        dtDay = dtArray[3];

        //yyyymmdd 체크
        if (dtMonth < 1 || dtMonth > 12) {
            _fnAlertMsg("존재하지 않은 월을 입력하셨습니다. 다시 한번 확인 해주세요");
            return false;
        }
        else if (dtDay < 1 || dtDay > 31) {
            _fnAlertMsg("존재하지 않은 일을 입력하셨습니다. 다시 한번 확인 해주세요");
            return false;
        }
        else if ((dtMonth == 4 || dtMonth == 6 || dtMonth == 9 || dtMonth == 11) && dtDay == 31) {
            _fnAlertMsg("존재하지 않은 일을 입력하셨습니다. 다시 한번 확인 해주세요");
            return false;
        }
        else if (dtMonth == 2) {
            var isleap = (dtYear % 4 == 0 && (dtYear % 100 != 0 || dtYear % 400 == 0));
            if (dtDay > 29 || (dtDay == 29 && !isleap)) {
                _fnAlertMsg("존재하지 않은 일을 입력하셨습니다. 다시 한번 확인 해주세요");
                return false;
            }
        }

        return true;
    }






    function _fnAlertMsg(msg) {
        $(".alert_cont .inner .inner_cont p").text("");
        $(".alert_cont .inner .inner_cont p").text(msg);
        layerPopup('#alert01');
    }

    function _fnAlertMsg_confirm(msg) {
        $(".alert_cont .inner .inner_cont p").text("");
        $(".alert_cont .inner .inner_cont p").text(msg);
        layerPopup('#alert_confirm');
     }







