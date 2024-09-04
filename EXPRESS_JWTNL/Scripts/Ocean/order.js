//==========전역변수================
var dataView = new Slick.Data.DataView();
var dataView1 = new Slick.Data.DataView();
var grid; var columns; var checkData = false;
var grid1; var columns1; var checkData = false;
var Mdata;
var SkuData = [];
var HblData = [];
var HouseData;
var HouseData_CHK = [];
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

    $("#StartDate").val(_fnPlusDate(-10));
    $("#EndDate").val(_fnPlusDate(0));
    fileCus();


    $(document).on('keyup', '#Search_Value', function (e) { 
        if (e.keyCode == 13) {
            $("#SearchBtn").click();
        }
    });


    // 컬럼
    columns = [
        { id: "NEXT_SEND", name: "API", field: "NEXT_SEND", sortable: true, resizable: false, width: 80 },
        /*{ id: "FAIL_YN", name: "보류", field: "FAIL_YN", sortable: true, resizable: false, width: 50, cssClass: "text-center_1" },*/
        { id: "WAYBILL_NO", name: "WayBillNo", field: "WAYBILL_NO", sortable: true, resizable: false, width: 130 },
        { id: "LOGISTICS_CODE", name: "Logistics Code", field: "LOGISTICS_CODE", sortable: true, resizable: false, width: 120 },
        { id: "COP_NO", name: "CopNo", field: "COP_NO", sortable: true, resizable: false, width: 200 },
        { id: "BIG_BAG", name: "BigBag", field: "BIG_BAG", sortable: true, resizable: false, width: 200 },
        { id: "PASS_PORT", name: "PassPort", field: "PASS_PORT", sortable: true, resizable: false, width: 200 },
        { id: "RECIVER_NAME", name: "Reciver Name", field: "RECIVER_NAME", sortable: true, resizable: false, width: 180 },
        { id: "RECIVER_ADDR", name: "Reciver Addr", field: "RECIVER_ADDR", sortable: true, resizable: false, width: 1000 },
        //{ id: "RETURNER_NAME", name: "Returner Name", field: "RETURNER_NAME", sortable: true, resizable: false, width: 180 },
        //{ id: "RETURNER_ADDR", name: "Returner Addr", field: "RETURNER_ADDR", sortable: true, resizable: false, width: 1000 },
        { id: "SENDER_NAME", name: "Sender Name", field: "SENDER_NAME", sortable: true, resizable: false, width: 300 },
        { id: "SENDER_ADDR", name: "Sender addr", field: "SENDER_ADDR", sortable: true, resizable: false, width: 1000 },
        { id: "TOTAL_PRICE", name: "Total price", field: "TOTAL_PRICE", sortable: true, resizable: false, width: 100 },
        { id: "CURRENCY", name: "Currency", field: "CURRENCY", sortable: true, resizable: false, width: 80 },
        { id: "ITEM_NAME", name: "Item Name", field: "ITEM_NAME", sortable: true, resizable: false, width: 250 },
        { id: "HS_CODE", name: "Hs code", field: "HS_CODE", sortable: true, resizable: false, width: 100 },
        { id: "ITEM_ID", name: "Item Id", field: "ITEM_ID", sortable: true, resizable: false, width: 150 },
        { id: "GOODS_PRICE",name: "GoodsPrice", field: "GOODS_PRICE", sortable: true, resizable: false, width: 100 },
        { id: "TAX_PRICE",name: "Tax price", field: "TAX_PRICE", sortable: true, resizable: false, width: 100 },
        { id: "POST_PRICE", name: "Post Price", field: "POST_PRICE", sortable: true, resizable: false, width: 100 },
        { id: "CLEAER_MODE", name: "Clearance Mode", field: "CLEAER_MODE", sortable: true, resizable: false, width: 120 },
        { id: "GROSS_WEIGHT", name: "GrossWeight", field: "GROSS_WEIGHT", sortable: true, resizable: false, width: 120 },
        { id: "SEQ", name: "SEQ", field: "SEQ", sortable: true, resizable: false, width: 120,hidden :true}
    ];


    var options = {
        enableCellNavigation: true,
        enableAddRow: false,
        autosizeColsMode: Slick.GridAutosizeColsMode.FitViewportToCols,
        enableColumnReorder:false
    }


    checkboxSelector = new Slick.CheckboxSelectColumn({
        cssClass: "slick-cell-checkboxsel"
    });


    grid = new Slick.Grid("#myGrid", dataView, columns, options);
    grid.setSelectionModel(new Slick.RowSelectionModel({ selectActiveRow: false }));
    grid.registerPlugin(checkboxSelector);

    //// 선택된 행 가져오기
    //var selectedRows = checkboxSelector.getSelectedRows();



    grid.onClick.subscribe(function (e, args) {
        var row = args.row; // 클릭된 행의 인덱스
        var col = args.cell; // 클릭된 열의 인덱스

        // 클릭된 열의 정보를 가져옴
        var column = grid.getColumns()[col];

        // 만약 특정 컬럼을 클릭한 경우에만 동작하도록 조건을 설정할 수 있습니다.
        if (column.field === "WAYBILL_NO") {
            var item = grid.getDataItem(row); // 클릭된 행의 데이터를 가져옴
            var value = item[column.field]; // 클릭된 컬럼의 값
            var value2 = _fnToNull(item.CARGO_ID);
            WaybillTrackingPop(value, value2);
        }

    });

    grid.onSort.subscribe(function (e, args) {
        var comparer = function (a, b) {
            var x = a[args.sortCol.field], y = b[args.sortCol.field];
            return (x === y ? 0 : (x > y ? 1 : -1));
        };
        dataView.sort(comparer, args.sortAsc);
        grid.invalidate(); // 그리드를 다시 그리도록 설정
        grid.render(); // 변경 사항을 적용하여 그리드를 다시 렌더링
    });

    //익셉션 조회 밑 바인딩
    $.ajax({
        type: "POST",
        url: "/Order/LoadExcept",
        async: true,
        dataType: "json",
        success: function (result) {
            if (_fnToNull(result)) {
                var dataset = JSON.parse(result);

                if (dataset.Result[0]["trxCode"] == "Y") {
                    fnSettingPopArea(dataset.LEFT_EXCEPT); //30 바인딩
                    fnSettingPopArea(dataset.RIGHT_EXCEPT); // 40 바인딩
                } else {
                    _fnAlertMsg("관리자에게 문의해주세요");
                }
            }
            else {
                _fnAlertMsg("관리자에게 문의해주세요");
            }
        },
        error: function (xhr, status, error) {
            console.log(error);
        },
        beforeSend: function () {
            $("#ProgressBar_Loading").show(); //프로그래스 바
        },
        complete: function () {
            $("#ProgressBar_Loading").hide(); //프로그래스 바
        }
    });

    function fnSettingPopArea(obj) {
        var list = obj
        var strHtml = "";

        for (var i = 0; i < list.length; i++) {
            strHtml += "<option value='" + list[i].SATAUS_CD + "'>" + list[i].NAME+"</option>";
        }

        if (list[0].SATAUS_CD.substring(0, 2) == "30") {
            $("#Hold_CD").empty();
            $("#Hold_CD").append(strHtml);
        }
        else {
            $("#Hold_Detail").empty();
            $("#Hold_Detail").append(strHtml);
        }
        
    }

});

$(document).on("change", "#Search_Type", function () {
        var sch_type = _fnToNull(this.value);

        if (sch_type == "MBL_NO") {
            $("#Multi_BL").attr("disabled", false);
        }
        else {
            $("#Multi_BL").attr("disabled", true);
            $("#Multi_BL").val("");
        }

});

// 삭제 버튼 클릭 이벤트 핸들러
$(document).on('click', '#btnDelete', function () {
    // 체크된 행의 데이터 가져오기
    var selectedData = getSelectedData();

    // 선택된 데이터가 없을 경우에 대한 예외처리
    if (selectedData.length === 0) {
        _fnAlertMsg("삭제할 행을 선택하세요.");
        return;
    }
    _fnAlertMsg_confirm("삭제하시겠습니까?");

    // 그리드에서 모든 선택을 해제
    /*grid.setSelectedRows([]);*/
    // 그리드 다시 그리기
    grid.invalidate();
    grid.render();
});

$(document).on('click', '#Delete_Excel', function () {
    // 체크된 행의 데이터 가져오기
    layerClose("#alert_confirm");
    var selectedData = getSelectedData();
    var arr = new Array();
    // 선택된 행을 역순으로 순회하면서 삭제하기 (역순으로 순회하여야 인덱스 변화에 따른 오류를 방지할 수 있습니다)
    for (var i = selectedData.length - 1; i >= 0; i--) {
        var item = selectedData[i]; // 선택된 행의 데이터 가져오기
        var itemId = item.id; // 해당 데이터의 고유 ID 가져오기 (예: 아이템 ID)
        var waybillNo = item.WAYBILL_NO; // WAYBILL_NO 가져오기
        var seqNo = item.SEQ; // WAYBILL_NO 가져오기
        var mblNo = item.MBL_NO; // WAYBILL_NO 가져오기
        if (_fnToNull(seqNo) == "") {
            // 선택된 행을 역순으로 순회하면서 삭제하기 (역순으로 순회하여야 인덱스 변화에 따른 오류를 방지할 수 있습니다)
            for (var i = selectedData.length - 1; i >= 0; i--) {
                var item = selectedData[i]; // 선택된 행의 데이터 가져오기
                var itemId = item.id; // 해당 데이터의 고유 ID 가져오기 (예: 아이템 ID)
                dataView.deleteItem(itemId); // 해당 ID에 해당하는 데이터 삭제
            }
            layerClose("#alert_confirm");
            // 그리드에서 모든 선택을 해제
            grid.setSelectedRows([]);

            // 그리드 다시 그리기
            grid.invalidate();
            grid.render();

            return false;
        }
        // WAYBILL_NO를 키-값 쌍으로 가지는 객체 생성
        var Obj = { WAYBILL_NO: waybillNo, SEQ: seqNo, MBL_NO: mblNo};



        arr.push(Obj);

    }


   
    $.ajax({
        type: "POST",
        url: "/Order/fnDeleteExcel",
        async: true,
        dataType: "json",
        data: { "vJsonData": _fnMakeJson(arr) },
        success: function (result) {
            if (result["rec_cd"] == "Y") {
                $("#Search_Type option:eq(0)").prop("selected", true);
                $("#Search_Value").val($("#lbl_MBL_NO").text());
                $("#SearchBtn").click();
                _fnAlertMsg("삭제되었습니다.");
                /*layerClose("#alert_confirm");*/


            } else {
                layerClose("#alert_confirm");
                _fnAlertMsg("삭제 실패 하였습니다. 관리자에게 문의해주세요");
            }
        },
        error: function (xhr, status, error) {
        console.log(error);
        },
        beforeSend: function () {
            $("#ProgressBar_Loading").show(); //프로그래스 바
        },
        complete: function () {
            $("#ProgressBar_Loading").hide(); //프로그래스 바
        }


    })


});


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

//엑셀업로드
$(document).on('click', '#btnUpload', function () {
    columns = [
        { id: "BIG_BAG", name: "Bag_Number", field: "BIG_BAG", sortable: true, resizable: false, width: 130 },
        { id: "WAYBILL_NO", name: "WayBillNo", field: "WAYBILL_NO", sortable: true, resizable: false, width: 130 },
        { id: "LOGISTICS_CODE", name: "Logistics Code", field: "LOGISTICS_CODE", sortable: true, resizable: false, width: 120 },
        { id: "CNE_NM", name: "Reciver Name", field: "CNE_NM", sortable: true, resizable: false, width: 180 },
        { id: "PASS_PORT", name: "PassPort", field: "PASS_PORT", sortable: true, resizable: false, width: 130 },
        { id: "CNE_ADDR", name: "Reciver Addr", field: "CNE_ADDR", sortable: true, resizable: false, width: 1000 },
        { id: "RECIVER_CITY", name: "Reciver City", field: "RECIVER_CITY", sortable: true, resizable: false, width: 130 },
        { id: "RECIVER_STATE", name: "Reciver State", field: "RECIVER_STATE", sortable: true, resizable: false, width: 130 },
        { id: "CNE_ZIP", name: "Reciver Zip", field: "CNE_ZIP", sortable: true, resizable: false, width: 130 },
        { id: "RECIVER_COUNTRY", name: "Reciver Country", field: "RECIVER_COUNTRY", sortable: true, resizable: false, width: 130 },
        { id: "CNE_MO", name: "Reciver Phone", field: "CNE_MO", sortable: true, resizable: false, width: 130 },
        { id: "CNE_MAIL", name: "Reciver Email", field: "CNE_MAIL", sortable: true, resizable: false, width: 130 },
        //{ id: "RETURNER_NAME", name: "Returner Name", field: "RETURNER_NAME", sortable: true, resizable: false, width: 180 },
        //{ id: "RETURNER_ADDR", name: "Returner Addr", field: "RETURNER_ADDR", sortable: true, resizable: false, width: 1000 },
        { id: "SHP_NM", name: "Sender Name", field: "SHP_NM", sortable: true, resizable: false, width: 300 },
        { id: "SENDER_OFFICE", name: "Sender Office", field: "SENDER_OFFICE", sortable: true, resizable: false, width: 300 },
        { id: "SHP_ADDR", name: "Sender addr", field: "SHP_ADDR", sortable: true, resizable: false, width: 1000 },
        { id: "SENDER_CITY", name: "Sender Office", field: "SENDER_OFFICE", sortable: true, resizable: false, width: 300 },
        { id: "SENDER_STATE", name: "Sender State", field: "SENDER_STATE", sortable: true, resizable: false, width: 300 },
        { id: "SHP_ZIP", name: "Sender Zip", field: "SHP_ZIP", sortable: true, resizable: false, width: 300 },
        { id: "SENDER_COUNTRY", name: "Sender Country", field: "SENDER_COUNTRY", sortable: true, resizable: false, width: 300 },
        { id: "SHP_MO", name: "Sender Phone", field: "SHP_MO", sortable: true, resizable: false, width: 300 },
        { id: "SHP_MAIL", name: "Sender Mail", field: "SHP_MAIL", sortable: true, resizable: false, width: 300 },
        { id: "TOTAL_PRICE", name: "Value", field: "TOTAL_PRICE", sortable: true, resizable: false, width: 100 },
        { id: "CURR", name: "Currency", field: "CURR", sortable: true, resizable: false, width: 80 },
        { id: "SKU_WEIGHT", name: "Weight", field: "SKU_WEIGHT", sortable: true, resizable: false, width: 100 },
        { id: "ITEM_NAME", name: "Item Name", field: "ITEM_NAME", sortable: true, resizable: false, width: 250 },
        { id: "HS_CODE", name: "Hs code", field: "HS_CODE", sortable: true, resizable: false, width: 100 },
        { id: "ITEM_ID", name: "Item Id", field: "ITEM_ID", sortable: true, resizable: false, width: 150 },
        { id: "QTY", name: "Qty", field: "QTY", sortable: true, resizable: false, width: 150 },
        { id: "SKU_PRICE", name: "Item Value", field: "SKU_PRICE", sortable: true, resizable: false, width: 100 },
        { id: "PARTNER_CD", name: "Partner Code", field: "PARTNER_CD", sortable: true, resizable: false, width: 150 },
        { id: "COP_NO", name: "CopNo", field: "COP_NO", sortable: true, resizable: false, width: 200 },
        { id: "CLEAER_MODE", name: "Clearance Mode", field: "CLEAER_MODE", sortable: true, resizable: false, width: 120 },
        /*{ id: "TAX_PRICE", name: "Tax price", field: "TAX_PRICE", sortable: true, resizable: false, width: 100 },*/
        { id: "P_URL", name: "Url", field: "P_URL", sortable: true, resizable: false, width: 100 },
        { id: "GROSS_WEIGHT", name: "GrossWeight", field: "GROSS_WEIGHT", sortable: true, resizable: false, width: 120 },
        { id: "PORT_CODE", name: "Port Code", field: "PORT_CODE", sortable: true, resizable: false, width: 120 },
        { id: "SEQ", name: "SEQ", field: "SEQ", sortable: true, resizable: false, width: 120, hidden: true }
    ];

    var fileInput = document.getElementById("fileInput");
    var file = fileInput.files[0];
    if (file) {
        var reader = new FileReader();
        reader.onload = function (e) {
            var data = new Uint8Array(e.target.result);
            var workbook = XLSX.read(data, { type: "array" });
            var sheetName = workbook.SheetNames[0];
            var sheet = workbook.Sheets[sheetName];
            var jsonData = XLSX.utils.sheet_to_json(sheet, { header: 1, defval: '' });


            var ETD;
            var ETA;
            var TRANS;
            var EI_TYPE;


            if (jsonData[1][1] == "수입") {
                EI_TYPE = "I";
            } else {
                EI_TYPE = "E";//수출입구분
            }
            $("#lbl_EXIM").text(jsonData[1][1]);//수출입구분
            $("#lbl_MBL_NO").text(jsonData[2][1]);//MAWB
            $("#lbl_TSTYPE").text(jsonData[1][3]);//Transport No
            $("#lbl_TSNO").text(jsonData[2][3]);//Transport No
            if (jsonData[1][5] == "") {//엑셀 데이터 ETD값 없을때
                $("#lbl_ETD").text(GetDateTime());//ETD
                ETD = formatDateTimeToNumber(GetDateTime());//ETD
            } else {
                $("#lbl_ETD").text(formattingDateEng(_fnFormateTime(jsonData[1][5],14)));//ETD
                ETD = _fnFormateTime(jsonData[1][5],14);//ETD
            }
            if (jsonData[1][7] == "") {//엑셀 데이터 ETA값 없을때
                $("#lbl_ETA").text(GetDateTime());//ETA
                ETA = formatDateTimeToNumber(GetDateTime());//ETA
            } else {
                $("#lbl_ETA").text(formattingDateEng(_fnFormateTime(jsonData[1][7]),14));//ETA
                ETA = _fnFormateTime(jsonData[1][7],14);//ETA
            }
            $("#lbl_POL").text(jsonData[2][5]);//출발지
            $("#lbl_POD").text(jsonData[2][7]);//도착지

                if (jsonData[1][3] == "해상") {
                    TRANS = "2";
                } else if (jsonData[1][3] == "도로") {
                    TRANS = "4";
                } else if (jsonData[1][3] == "항공") {
                    TRANS = "5";
                } else {
                    TRANS = "6";
                       }


            HouseData = jsonData.slice(7).map(function (row, index) {
                HouseData_CHK.push(row[1]);
                return {
                    id: index + 1,
                    BIG_BAG: cutStringToCharLength(_fnToNull(row[0]), 20),
                    WAYBILL_NO: cutStringToCharLength(_fnToNull(row[1]), 20),
                    LOGISTICS_CODE: cutStringToCharLength(_fnToNull(row[2]), 100),
                    CNE_NM: cutStringToCharLength(_fnToNull(row[3]), 100),
                    PASS_PORT: cutStringToCharLength(_fnToNull(row[4]), 200),
                    CNE_ADDR: cutStringToCharLength(_fnToNull(row[5]), 512),
                    RECIVER_CITY: cutStringToCharLength(_fnToNull(row[6]), 200),
                    RECIVER_STATE: cutStringToCharLength(_fnToNull(row[7]), 100),
                    CNE_ZIP: cutStringToCharLength(_fnToNull(row[8]), 100),
                    RECIVER_COUNTRY: cutStringToCharLength(_fnToNull(row[9]), 200),
                    CNE_MO: cutStringToCharLength(_fnToNull(row[10]), 100),
                    CNE_MAIL: cutStringToCharLength(_fnToNull(row[11]), 100),
                    SHP_NM: cutStringToCharLength(_fnToNull(row[12]), 100),
                    SENDER_OFFICE: cutStringToCharLength(_fnToNull(row[13]), 100),
                    SHP_ADDR: cutStringToCharLength(_fnToNull(row[14]), 512),
                    SENDER_CITY: cutStringToCharLength(_fnToNull(row[15]), 100),
                    SENDER_STATE: cutStringToCharLength(_fnToNull(row[16]), 100),
                    SHP_ZIP: cutStringToCharLength(_fnToNull(row[17]), 100),
                    SENDER_COUNTRY: cutStringToCharLength(_fnToNull(row[18]), 100),
                    SHP_MO: cutStringToCharLength(_fnToNull(row[19]), 30),
                    SHP_MAIL: cutStringToCharLength(_fnToNull(row[20]), 100),
                    TOTAL_PRICE: cutStringToCharLength(_fnToNull(row[21]), 100),
                    CURR: cutStringToCharLength(_fnToNull(row[22]), 100),
                    SKU_WEIGHT: cutStringToCharLength(_fnToNull(row[23]), 100),
                    ITEM_NAME: cutStringToCharLength(_fnToNull(row[24]), 300),
                    HS_CODE: cutStringToCharLength(_fnToNull(row[25]), 100),
                    ITEM_ID: cutStringToCharLength(_fnToNull(row[26]), 30),
                    QTY: cutStringToCharLength(_fnToNull(row[27]), 100),
                    SKU_PRICE: cutStringToCharLength(_fnToNull(row[28]), 100),
                    PARTNER_CD: cutStringToCharLength(_fnToNull(row[29]), 100),
                    COP_NO: cutStringToCharLength(_fnToNull(row[30]), 100),
                    CLEAER_MODE: cutStringToCharLength(_fnToNull(row[31]), 100),
                    P_URL: cutStringToCharLength(_fnToNull(row[32]), 100),
                    GROSS_WEIGHT: cutStringToCharLength(_fnToNull(row[33]), 100),
                    PORT_CODE: cutStringToCharLength(_fnToNull(row[34]), 100),
                    SEQ: cutStringToCharLength(_fnToNull(row[35]), 100)
                };
            });

            var HBL_CNT = new Set(HouseData_CHK);

            var HBL_CNT_FINAL = [...HBL_CNT];

            $("#tot_cnt").text(HBL_CNT_FINAL.length.toString().replace(/\B(?<!\.\d*)(?=(\d{3})+(?!\d))/g, ","));

            dataView.setItems(HouseData);

            columns.splice(0, 0, checkboxSelector.getColumnDefinition())
            grid.setColumns(columns);
            grid.invalidate();
            grid.render();
            layerClose("#_Upload");


        };

        reader.readAsArrayBuffer(file);
    }
});

$(document).on('click', '#DelteOrderNum', function () {
    $(this).siblings('input').val('');
})

$(document).on('click', '#ExcelUpload', function () {
    /*$("#file_Nm").text("파일을 선택해주세요.")*/
    layerPopup("#_Upload");
});



// 검색
$(document).on('click', '#SearchBtn', function () {
    columns = [
        { id: "NEXT_SEND", name: "API", field: "NEXT_SEND", sortable: true, resizable: false, width: 50 },
        { id: "WAYBILL_NO", name: "WayBillNo", field: "WAYBILL_NO", sortable: true, resizable: false, width: 130, cssClass: "text-center" },
        { id: "LOGISTICS_CODE", name: "Logistics Code", field: "LOGISTICS_CODE", sortable: true, resizable: false, width: 120 },
        { id: "COP_NO", name: "CopNo", field: "COP_NO", sortable: true, resizable: false, width: 200 },
        { id: "BIG_BAG", name: "BigBag", field: "BIG_BAG", sortable: true, resizable: false, width: 200 },
        { id: "PASS_PORT", name: "PassPort", field: "PASS_PORT", sortable: true, resizable: false, width: 200 },
        { id: "RECIVER_NAME", name: "Reciver Name", field: "RECIVER_NAME", sortable: true, resizable: false, width: 180 },
        { id: "RECIVER_ADDR", name: "Reciver Addr", field: "RECIVER_ADDR", sortable: true, resizable: false, width: 1000 },
        { id: "RECIVER_PHONE", name: "Reciver Phone", field: "RECIVER_PHONE", sortable: true, resizable: false, width: 180 },
        { id: "SENDER_NAME", name: "Sender Name", field: "SENDER_NAME", sortable: true, resizable: false, width: 300 },
        { id: "SENDER_ADDR", name: "Sender addr", field: "SENDER_ADDR", sortable: true, resizable: false, width: 1000 },
        { id: "SENDER_PHONE", name: "Sender Phone", field: "SENDER_PHONE", sortable: true, resizable: false, width: 180 },
        { id: "TOTAL_PRICE", name: "Total price", field: "TOTAL_PRICE", sortable: true, resizable: false, width: 100 },
        { id: "CURRENCY", name: "Currency", field: "CURRENCY", sortable: true, resizable: false, width: 80 ,cssClass: "text-center_1"},
        { id: "ITEM_NAME", name: "Item Name", field: "ITEM_NAME", sortable: true, resizable: false, width: 250, cssClass: "text-center_1" },
        { id: "HS_CODE", name: "Hs code", field: "HS_CODE", sortable: true, resizable: false, width: 100 },
        { id: "URL", name: "URL", field: "URL", sortable: true, resizable: false, width: 100 },
        { id: "QTY", name: "QTY", field: "QTY", sortable: true, resizable: false, width: 100 },
        { id: "ITEM_ID", name: "Item Id", field: "ITEM_ID", sortable: true, resizable: false, width: 150 },
        { id: "GOODS_PRICE", name: "GoodsPrice", field: "GOODS_PRICE", sortable: true, resizable: false, width: 100 },
        { id: "TAX_PRICE", name: "Tax price", field: "TAX_PRICE", sortable: true, resizable: false, width: 100 },
        { id: "POST_PRICE", name: "Post Price", field: "POST_PRICE", sortable: true, resizable: false, width: 100 },
        { id: "CLEAER_MODE", name: "Clearance Mode", field: "CLEAER_MODE", sortable: true, resizable: false, width: 120 },
        { id: "GROSS_WEIGHT", name: "GrossWeight", field: "GROSS_WEIGHT", sortable: true, resizable: false, width: 120 },
        { id: "MBL_NO", name: "MASTER_NO", field: "MBL_NO", sortable: true, resizable: false, width: 120, hidden: true },
        { id: "PARTNER_CD", name: "PARTNER_CODE", field: "PARTNER_CD", sortable: true, resizable: false, width: 120, hidden: true },
        { id: "CARGO_ID", name: "CARGOID", field: "CARGO_ID", sortable: true, resizable: false, width: 120, hidden: true },
        { id: "CNE_ZIP", name: "CNEZIP", field: "CNE_ZIP", sortable: true, resizable: false, width: 120, hidden: true },
        { id: "CNE_MAIL", name: "CNEMAIL", field: "CNE_MAIL", sortable: true, resizable: false, width: 120, hidden: true },
        { id: "SEQ", name: "SEQ", field: "SEQ", sortable: true, resizable: false, width: 120, hidden: true }
    ];

    var objJsonData = new Object();

    //#region Validataion
    if (_fnToNull($("#Search_Value").val()) == "") {
        _fnAlertMsg("검색어를 입력해주세요.");
        return false;
    }

    if (_fnToNull($("#StartDate").val().replace(/-/gi, "")) == "") {
        _fnAlertMsg("ETD를 입력 해 주세요.");
        return false;
    }

    if (_fnToNull($("#EndDate").val().replace(/-/gi, "")) == "") {
        _fnAlertMsg("ETA를 입력 해 주세요.");
        return false;
    }
    //#endregion

    objJsonData.SEARCH_TYPE = $("#Search_Type option:selected").val();
    objJsonData.SEARCH_VALUE = $("#Search_Value").val();
    objJsonData.STRT_YMD = $("#StartDate").val().substring(0, 10).replace(/-/gi, "");
    objJsonData.END_YMD = $("#EndDate").val().substring(0, 10).replace(/-/gi, "");

    if (_fnToNull($("#Multi_BL").val().replace(/\n/gi, "")) != "") {
        objJsonData.MULTI_NO = $("#Multi_BL").val().replace(/\n/gi, ",");
    }
    else {
        objJsonData.MULTI_NO = "";
    }
        

    //console.log(objJsonData);

    $.ajax({
        type: "POST",
        //url: "/Order/fnSearchData",
        url: "/Order/fnNewOrderSearch",
        async: true,
/*        dataType: "json",*/
        data: { "vJsonData": _fnMakeJson(objJsonData) },
        success: function (result) {
            var html = $.parseHTML(result);
            var target = $(html).find('#test');
            var dataValue = target.attr('value');
            $("#tot_cnt").text('');
            //console.log(dataValue); // 여기서 원하는 작업을 수행하세요.
            //if (JSON.parse(result).Result[0]["trxCode"] == "Y") {
              //  if (JSON.parse(result).MST.length > 0) {

            if (_fnToNull(dataValue) != "") {
                var MjsonData = JSON.parse(dataValue).MBL_DATA; // 받은 데이터에서 필요한 부분 추출
                var HjsonData = JSON.parse(dataValue).HBL_DATA; // 받은 데이터에서 필요한 부분 추출

                //마스터 셋팅
                if (MjsonData.length > 0) {
                    var v_type;
                    var Ie_type;

                    //var jsonMstData = JSON.parse(result).MST; //HEADER 데이터
                    //var jsonDtlData = JSON.parse(result).DTL; // DTL 데이터



                    if (MjsonData[0].IE_TYPE == "I") {
                        Ie_type = "수입";
                    } else {
                        Ie_type = "수출";
                    }
                    $("#lbl_EXIM").text(Ie_type);//수출입구분
                    $("#lbl_MBL_NO").text(MjsonData[0].MBL_NO);//MAWB
                    if (MjsonData[0].TRANS_TYPE == "2") {
                        v_type = "해운";
                    } else if (MjsonData[0].TRANS_TYPE == "4") {
                        v_type = "도로";
                    } else if (MjsonData[0].TRANS_TYPE == "5") {
                        v_type = "항공";
                    } else {
                        v_type = "우편";
                    }
                    $("#lbl_TSTYPE").text(v_type);//Transport Type
                    $("#lbl_TSNO").text(MjsonData[0].VOY);//Transport No
                    $("#lbl_ETD").text(formattingDateEng(MjsonData[0].ETD));//ETD
                    $("#lbl_POL").text(MjsonData[0].POL);//출발지
                    $("#lbl_ETA").text(formattingDateEng(MjsonData[0].ETA));//ETA
                    $("#lbl_POD").text(MjsonData[0].POD);//도착지

                    // 데이터를 표시할 배열 초기화
                    var data = [];

                    var hblcnt = 0;
                    var ck_hbl_no = "";

                    for (var i = 0; i < HjsonData.length; i++) {
                        if (ck_hbl_no != HjsonData[i].HBL_NO) {
                            hblcnt += 1;
                            ck_hbl_no = HjsonData[i].HBL_NO;
                            //alert(hblcnt + " " + ck_hbl_no);
                        }

                        data.push({
                            NEXT_SEND: HjsonData[i].STATE,
                            FAIL_YN: HjsonData[i].FAIL_YN,
                            WAYBILL_NO: HjsonData[i].HBL_NO,
                            LOGISTICS_CODE: HjsonData[i].LOGISTIC_CD,
                            COP_NO: HjsonData[i].COP_NO,
                            BIG_BAG: HjsonData[i].BIG_BAG,
                            PASS_PORT: HjsonData[i].PASS_PORT,
                            RECIVER_NAME: HjsonData[i].CNE_NM,
                            RECIVER_ADDR: HjsonData[i].CNE_ADDR,
                            RECIVER_PHONE: HjsonData[i].CNE_MO,
                            //RETURNER_NAME: HjsonData[i].RET_NM,
                            //RETURNER_ADDR: HjsonData[i].RET_ADDR,
                            SENDER_NAME: HjsonData[i].SHP_NM,
                            SENDER_ADDR: HjsonData[i].SHP_ADDR,
                            SENDER_PHONE: HjsonData[i].SHP_MO,
                            TOTAL_PRICE: HjsonData[i].TOT_PRICE,
                            CURRENCY: HjsonData[i].CURR_NM,
                            GOODS_PRICE: HjsonData[i].SKU_PRICE,
                            TAX_PRICE: HjsonData[i].TAX_PRICE,
                            POST_PRICE: HjsonData[i].POST_PRICE,
                            CLEAER_MODE: HjsonData[i].CLEAER_MODE,
                            GROSS_WEIGHT: HjsonData[i].GRS_WGT,
                            ITEM_NAME: HjsonData[i].ITEM_NM,
                            HS_CODE: HjsonData[i].HS_CD,
                            ITEM_ID: HjsonData[i].ITEM_ID,
                            QTY: HjsonData[i].QTY,
                            URL: HjsonData[i].P_URL,
                            CARGO_ID: HjsonData[i].UNIPASS_CARGO_INFO_ID,
                            MBL_NO: HjsonData[i].MBL_NO,
                            PARTNER_CD: HjsonData[i].PARTNER_CD,
                            CNE_ZIP: HjsonData[i].CNE_ZIP,
                            CNE_MAIL: HjsonData[i].CNE_MAIL,
                            SEQ: HjsonData[i].SEQ,
                            id: i
                        });
                    }

                    $("#tot_cnt").text(hblcnt.toString().replace(/\B(?<!\.\d*)(?=(\d{3})+(?!\d))/g, ","));
                    grid.onBeforeAppendCell.subscribe(function (e, args) {
                        return customCellValidation(grid, args);
                    });

                    // 그리드에서 모든 선택을 해제
                    grid.setSelectedRows([]);
                    dataView.beginUpdate();
                    dataView.setItems(data);
                    dataView.endUpdate();

                    columns.splice(0, 0, checkboxSelector.getColumnDefinition())
                    grid.setColumns(columns);
                    grid.invalidate();
                    grid.render();
                    
                }
                else {
                    // 그리드에서 모든 선택을 해제
                    grid.setSelectedRows([]);
                    _fnAlertMsg("데이터가 없습니다.");
                    dataView.beginUpdate();
                    dataView.setItems([]);
                    dataView.endUpdate();

                    grid.setColumns(columns);
                    grid.invalidate();
                    grid.render();

                    $("#lbl_EXIM").text("");
                    $("#lbl_TSTYPE").text("");
                    $("#lbl_ETD").text("");
                    $("#lbl_ETA").text("");
                    $("#lbl_MBL_NO").text("");
                    $("#lbl_TSNO").text("");
                    $("#lbl_POL").text("");
                    $("#lbl_POD").text("");

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


   

    grid.onClick.subscribe(function (e, args) {
        var objJsonData = new Object();
        var row = args.row; // 클릭된 행의 인덱스
        var col = args.cell; // 클릭된 열의 인덱스

        // 클릭된 열의 정보를 가져옴
        var column = grid.getColumns()[col];

        // 만약 특정 컬럼을 클릭한 경우에만 동작하도록 조건을 설정할 수 있습니다.
        if (column.field === "WAYBILL_NO") {
            var item = grid.getDataItem(row); // 클릭된 행의 데이터를 가져옴
            var value = item[column.field]; // 클릭된 컬럼의 값
            var value2 = _fnToNull(item.CARGO_ID);

        }

        
    });
})

// 유니패스 팝업 열렸을 때
function WaybillTrackingPop(value, value2) {
    if (value2 == "") {
        _fnAlertMsg("화물진행정보가 없습니다.");
        return false;
    }

    var objJsonData = new Object();

    objJsonData.SEARCH_TYPE = "PopUp";
    objJsonData.SEARCH_VALUE = value;
    objJsonData.SEARCH_VALUE2 = value2;

    $.ajax({
        type: "POST",
        //url: "/Order/fnUniSearchData",
        url: "/Order/fnNewUnipassSearch",
        async: false,
        dataType: "json",
        data: { "vJsonData": _fnMakeJson(objJsonData) },
        success: function (result) {
            if (JSON.parse(result).Result[0]["trxCode"] == "Y") {
                if (JSON.parse(result).MST.length > 0) {
                    layerPopup("#Tracking");
                    var jsonMstData = JSON.parse(result).MST; //HEADER 데이터
                    var jsonDtlData = JSON.parse(result).DTL; // 중간 셀  
                    //var jsonUMstData = JSON.parse(result).UMST; // Unipass Mst 데이터
                    var jsonUDtlData = JSON.parse(result).UDTL; // Unipass DTL 데이터
                    /*
                    //중상단 데이터 바인딩
                    $("#H_NO").text(jsonDtlData[0].HBL_NO);//HBL 번호
                    $("#M_NO").text(jsonMstData[0].MBL_NO);//HBL 번호
                    $("#T_NO").text(jsonMstData[0].VOY);//TRANSPORT NO
                    $("#UNI_STATUS").text(jsonUMstData[0].CSCLPRGSSTTS);//화물진행상태
                    $("#UNI_TIME").text(formattingDateEng(jsonUMstData[0].PRCSDTTM));//화물진행일시
                    $("#L_CD").text(jsonDtlData[0].LOGISTIC_CD);//LOGISTICS CODE
                    $("#M_NO").text(jsonUMstData[0].MBL_NO);//MBL 번호
                    $("#SEND_NM").text(jsonDtlData[0].SHP_NM);//송하인명
                    $("#RECIVER_NM").text(jsonDtlData[0].CNE_NM);//수하인 명
                    $("#TRK_ETD").text(formattingDateEng(jsonMstData[0].ETD));//ETD
                    $("#START_LOC").text(jsonMstData[0].POL);//출발지
                    $("#TRK_ETA").text(formattingDateEng(jsonMstData[0].ETA));//ETA
                    $("#END_LOC").text(jsonMstData[0].POD);//도착지
                    */


                    $("#H_NO").text(jsonMstData[0].HBLNO);//HBL 번호
                    $("#UNI_TIME").text(formattingDateEng(jsonMstData[0].PRCSDTTM));//화물진행일시
                    $("#L_CD").text(jsonMstData[0].LOGISTIC_CD);//LOGISTICS CODE //화물진행번호
                    $("#UNI_STATUS").text(jsonMstData[0].CSCLPRGSSTTS);//화물진행상태

                    $("#PTN_CD").val(jsonDtlData[0].PARTNER_CD);
                    $("#M_NO").text(jsonDtlData[0].ORG_MBL);//MBL 번호
                    $("#T_NO").text(jsonDtlData[0].VOY);//TRANSPORT NO
                    $("#SEND_NM").text(jsonDtlData[0].SHP_NM);//송하인명
                    $("#RECIVER_NM").text(jsonDtlData[0].CNE_NM);//수하인 명
                    $("#TRK_ETD").text(formattingDateEng(jsonDtlData[0].ETD));//ETD
                    $("#START_LOC").text(jsonDtlData[0].POL);//출발지
                    $("#TRK_ETA").text(formattingDateEng(jsonDtlData[0].ETA));//ETA
                    $("#END_LOC").text(jsonDtlData[0].POD);//도착지
                    

                    var options1 = {
                        enableCellNavigation: true,
                        enableAddRow: false,
                        autosizeColsMode: Slick.GridAutosizeColsMode.FitViewportToCols,
                        enableColumnReorder: false
                    }
                    // 컬럼
                    var columns1 = [
                        /*{ id: "CNTR_NO", name: "Container No", field: "CNTR_NO", sortable: true, resizable: false, width: 220 },*/
                        { id: "GUID", name: "GUID", field: "GUID", sortable: false, resizable: false, width: 100, hidden: true  },
                        { id: "SEQ", name: "순번", field: "SEQ", sortable: false, resizable: false, width: 100 },
                        { id: "SEND_YN", name: "API", field: "SEND_YN", sortable: false, resizable: false, width: 130 },
                        { id: "PROC_NM", name: "처리 단계", field: "CARGTRCNRELABSOPTPCD", sortable: false, resizable: false, width: 352 },
                        { id: "PROC_TIME", name: "처리 일시", field: "PRCSDTTM", sortable: false, resizable: false, width: 352 },
                        { id: "PROC_LOC", name: "장치장 명", field: "SHEDNM", sortable: false, resizable: false, width: 352 },
                        //{ id: "SEND_YN", name: "API", field: "SEND_YN", sortable: true, resizable: false, width: 352 },
                        { id: "COP_NO", name: "cop_no", field: "COP_NO", sortable: false, resizable: false, width: 352, hidden: true },
                        { id: "HBL_NO", name: "houseb/l", field: "HBL_NO", sortable: false, resizable: false, width: 352, hidden: true },
                        { id: "L_STATUS", name: "마지막단계", field: "L_STATUS", sortable: false, resizable: false, width: 352, hidden: true },
                        //{ id: "CLEAER_MODE", name: "clear_mode", field: "CLEAER_MODE", sortable: true, resizable: false, width: 352, hidden: true },
                        { id: "MNGT_ID", name: "관리번호", field: "MNGT_ID", sortable: true, resizable: false, width: 352, hidden: true }
                    ];
                    grid1 = new Slick.Grid("#myGrid_PoPUp", dataView1, columns1, options1);
                    grid1.onBeforeAppendCell.subscribe(function (e, args) {
                        return customCellValidation(grid1, args);
                    });
                    // 데이터를 표시할 배열 초기화
                    var data = [];

                    for (var i = 0; i < jsonUDtlData.length; i++) {
                        data.push({
                            GUID: jsonUDtlData[i].MNGT_ID,
                            HBL_NO: jsonMstData[0].HBLNO,
                            MBL_NO: jsonUDtlData[i].MBL_NO,
                            //CNTR_NO: jsonUDtlData[i].CNTR_NO,
                            SEQ: jsonUDtlData[i].SEQ,
                            CARGTRCNRELABSOPTPCD: jsonUDtlData[i].CARGTRCNRELABSOPTPCD,
                            PRCSDTTM: formattingDateKOR(jsonUDtlData[i].PRCSDTTM),
                            SHEDNM: jsonUDtlData[i].SHEDNM,
                            SEND_YN: jsonUDtlData[i].SEND_YN,
                            MNGT_ID: jsonUDtlData[i].MNGT_ID,
                            COP_NO: jsonMstData[0].COP_NO,
                            L_STATUS: jsonUDtlData[i].L_STATUS,
                            //CLEAER_MODE: jsonDtlData[0].CLEAER_MODE,
                            id: i
                        });
                    }

                    dataView1.beginUpdate();
                    dataView1.setItems(data);
                    dataView1.endUpdate();

                    grid1.setColumns(columns1);
                    grid1.invalidate();
                    grid1.render();

                    grid1.onClick.subscribe(function (e, args1) {
                        var objJsonData = new Object();
                        var row = args1.row; // 클릭된 행의 인덱스
                        var col = args1.cell; // 클릭된 열의 인덱스

                        // 클릭된 열의 정보를 가져옴
                        var column = grid1.getColumns()[col];

                        // 만약 특정 컬럼을 클릭한 경우에만 동작하도록 조건을 설정할 수 있습니다.
                        if (column.field === "CARGTRCNRELABSOPTPCD") {
                            var item = grid1.getDataItem(row); // 클릭된 행의 데이터를 가져옴
                            var value = item[column.field]; // 클릭된 컬럼의 값
                            if ((value == "통관목록보류" || value == "수입(사용소비) 심사진행" || value == "통관보류") && (item["L_STATUS"] == "2040" || item["L_STATUS"] == "3006" )) {

                                var objJsonData = new Object();
                                objJsonData.MNGT_ID = item.MNGT_ID;

                                //#region 기본 사유 팝업 셋팅
                                $("#btnCntr").removeAttr("disabled");
                                $("#Hold_CD").val("3004");
                                $("#Hold_CD").removeAttr("disabled");
                                $("#Hold_CD").removeClass("complete");
                                $("#Hold_Detail").val("0000");
                                $("#Hold_Detail").removeAttr("disabled");
                                $("#Hold_CD").removeClass("Hold_Detail");
                                $("#Waybill").val(item.HBL_NO);
                                $("#Mngt_id").val(item.MNGT_ID);
                                $("#Cop_no").val(item.COP_NO);
                                $("#Clear_mode").val(item.CLEAER_MODE);
                                $("#btnCntr").removeClass("none_use");
                                //#endregion

                                $.ajax({
                                    type: "POST",
                                    url: "/Order/fnSearchHold",
                                    async: false,
                                    dataType: "json",
                                    data: { "vJsonData": _fnMakeJson(objJsonData) },
                                    success: function (result) {
                                        if (result["Result"][0].trxCode == "Y") {
                                            //_fnAlertMsg("보류수정 완료된 건입니다.");

                                            //#region 셋팅값 설정
                                            if (_fnToNull(result["HOLD"][0].RTN_30) != "") {
                                                $("#Hold_CD").val(result["HOLD"][0].RTN_30);
                                                $("#Hold_CD").attr("disabled", 'true');
                                                $("#Hold_CD").addClass("complete");
                                            }
                                            if (_fnToNull(result["HOLD"][0].RTN_40) != "") {
                                                $("#Hold_Detail").val(result["HOLD"][0].RTN_40);
                                                $("#Hold_Detail").attr("disabled", 'true');
                                                $("#Hold_Detail").addClass("complete");
                                            }

                                            if (_fnToNull(result["HOLD"][0].RTN_30) != "" && _fnToNull(result["HOLD"][0].RTN_40) != "") {
                                                $("#btnCntr").attr("disabled", 'true');
                                                $("#btnCntr").addClass("none_use");
                                            }
                                            //#endregion

                                        } else {

                                        }
                                        layerPopup("#CountryCode");
                                    },
                                    error: function (xhr, status, error) {
                                        console.log(error);
                                    },
                                    beforeSend: function () {
                                        $("#ProgressBar_Loading").show(); //프로그래스 바
                                    },
                                    complete: function () {
                                        $("#ProgressBar_Loading").hide(); //프로그래스 바
                                    }
                                });

                            }

                        }


                    });
                } else {
                    _fnAlertMsg("화물진행정보가 없습니다.");
                }
            }
        },
        error: function (xhr, status, error) {
            status = true;
            console.log(error);
        }

    })

}

//#region =========== Button Evnet  ===========

//예외 전송

$(document).on('click', '#btnCntr', function () {

    var expt1JsonData = new Object();
    var expt2JsonData = new Object();
    var APiobj = new Object();
    var nTime = formatDateTimeToNumberGolobal()
    //#region 전송 데이터 만들기 

    //30xx 예외
    if (!$("#Hold_CD").hasClass("complete")) {
        expt1JsonData.guid = $("#Mngt_id").val();
        expt1JsonData.MBL_NO = $("#M_NO").text();
        expt1JsonData.wayBillNo = $("#Waybill").val();
        expt1JsonData.copNo = $("#Cop_no").val();
        expt1JsonData.returnTime = nTime;
        expt1JsonData.RTN_TIME = nTime.split("+")[0];
        expt1JsonData.returnStatus = $("#Hold_CD option:selected").val(); // 코드
        expt1JsonData.PARTNER_CD = $("#PTN_CD").val();
        expt1JsonData.feature = "{\"clearanceMode\" : \"\"}";

        $.ajax({
            type: "POST",
            url: "/Order/fnExceptSoloSendSave",
            async: false,
            dataType: "json",
            data: { "vJsonData": _fnMakeJson(expt1JsonData) },
            success: function (result) {
                if (result["rec_cd"] == "Y") {
                    layerClose("#CountryCode");
                    layerClose("#Tracking");
                    _fnAlertMsg("Exception 전송이 완료되었습니다.");
                } else {
                    layerClose("#CountryCode");
                    _fnAlertMsg("수정 실패 하였습니다. 관리자에게 문의해주세요");
                }
            },
            error: function (xhr, status, error) {
                console.log("3.3 Error" + error);
            },
            beforeSend: function () {
                $("#ProgressBar_Loading").show(); //프로그래스 바
            },
            complete: function () {
                $("#ProgressBar_Loading").hide(); //프로그래스 바
            }
            
        });
    }


    //40xx 예외
    if (!$("#Hold_Detail").hasClass("complete") && $("#Hold_Detail option:selected").val() != "0000") { //미선택및 아직 선택값이 없는경우
        expt2JsonData.guid = $("#Mngt_id").val();
        expt2JsonData.MBL_NO = $("#M_NO").text();
        expt2JsonData.wayBillNo = $("#Waybill").val();
        expt2JsonData.copNo = $("#Cop_no").val();
        expt2JsonData.returnTime = nTime;
        expt2JsonData.RTN_TIME = nTime.split("+")[0];
        expt2JsonData.returnStatus = $("#Hold_Detail option:selected").val();
        expt2JsonData.PARTNER_CD = $("#PTN_CD").val();
        expt2JsonData.feature = "{\"clearanceMode\" : \"\"}";

        $.ajax({
            type: "POST",
            url: "/Order/fnExceptSoloSendSave",
            async: false,
            dataType: "json",
            data: { "vJsonData": _fnMakeJson(expt2JsonData) },
            success: function (result) {
                if (result["rec_cd"] == "Y") {
                    layerClose("#CountryCode");
                    layerClose("#Tracking");
                    _fnAlertMsg("Exception 전송이 완료되었습니다.");
                } else {
                    layerClose("#CountryCode");
                    _fnAlertMsg("수정 실패 하였습니다. 관리자에게 문의해주세요");
                }
            },
            error: function (xhr, status, error) {
                console.log("3.3 Error" + error);
            }
        });
    }

    //#endregion


    

    //objJsonData.CLEAER_MODE = $("#Clear_mode").val();

    //상태값
    //objJsonData.LEFT_SEARCH_CD = $("#Hold_CD option:selected").val();
    //objJsonData.LEFT_SEARCH_DATA = $("#Hold_CD option:selected").text();
    //objJsonData.RIGHT_SEARCH_CD = $("#Hold_Detail option:selected").val();
    //objJsonData.RIGHT_SEARCH_DATA = $("#Hold_Detail option:selected").text();

    //전송 


    //저장
    //$.ajax({
    //    type: "POST",
    //    url: "/Order/fnInsertHold",
    //    async: false,
    //    dataType: "json",
    //    data: { "vJsonData": _fnMakeJson(objJsonData) },
    //    success: function (result) {
    //        if (result["rec_cd"] == "Y") {
    //            layerClose("#CountryCode");
    //            layerClose("#Tracking");
    //            _fnAlertMsg("수정 되었습니다.");

    //        } else {
    //            layerClose("#CountryCode");
    //            _fnAlertMsg("수정 실패 하였습니다. 관리자에게 문의해주세요");
    //        }
    //    },
    //    error: function (xhr, status, error) {
    //        console.log(error);
    //    }
    //});

});


//엑셀자료 저장
    $(document).on('click', "#btnSave", function () {
        var checkedItems = [];
        var rows = grid.getSelectedRows();
        rows.forEach(function (row) {
            checkedItems.push(dataView.getItem(row)); // dataView는 해당 데이터의 DataView입니다.
            // dataView가 아닌 경우에는 grid.getDataItem(row)을 사용할 수 있습니다.
        });

        // 필터링된 체크된 항목을 다음 단계로 전달하거나 사용합니다.
        console.log(checkedItems); // 예시: 체크된 항목을 콘솔에 출력

        var selectedRows = dataView.getItems();
        if (selectedRows.length == 0) {
            _fnAlertMsg("엑셀을 업로드 해주세요.");
            return false;
        }
        if (_fnToNull($("#lbl_MBL_NO").text()) == "") {
            _fnAlertMsg("Master Number 는 필수값입니다. 다시 업로드 해주세요.");
            return false;
        }
        // 체크된 행의 데이터 가져오기
        var selectedData = getSelectedData();

        // 선택된 데이터가 없을 경우에 대한 예외처리
        if (selectedData.length === 0) {
            _fnAlertMsg("저장할 행을 선택하세요.");
            return;
        }
        HblData = [];
        SkuData = [];
        //****************
        var temp_bl = "";
        var seq = 1;
        var type_EI;
        var trans_type;
        var etd = formatDateTime($("#lbl_ETD").text());
        var eta = formatDateTime($("#lbl_ETA").text());
        var Parcel_Cnt = $("#tot_cnt").text();
        if ($("#lbl_EXIM").text() == "수입") {
            type_EI = "I"
        } else {
            type_EI = "E"
        } 

        if ($("#lbl_TSTYPE").text() == "해운") {
            trans_type = "2";
        } else if ($("#lbl_TSTYPE").text() == "도로") {
            trans_type = "4";
        } else if ($("#lbl_TSTYPE").text() == "항공") {
            trans_type = "5";
        }
        else {
            trans_type = "6";
        }



        var mngt_no = getFormattedDateAndRandomNumbers();

      var Mdata = {
            MBL_NO: _fnToNull($("#lbl_MBL_NO").text().replace(/-/g, '')),
            LINE_CD: _fnToNull(""),
            VSL: _fnToNull(""),
            VOY: _fnToNull($("#lbl_TSNO").text()),
            POL: _fnToNull($("#lbl_POL").text()),
            POD: _fnToNull($("#lbl_POD").text()),
            ETD: _fnToNull(etd),
            ETA: _fnToNull(eta),
            IE_TYPE: _fnToNull(type_EI),
            TRANS_TYPE: _fnToNull(trans_type), // 이놈이 문제
            MNGT_NO: mngt_no,
            INS_YMD: GetDateTime_INS_YMD(),
            INS_HM: GetDateTime_INS_HM(),
            INS_USR: "WEBPROC",
            PARCEL_CNT: Parcel_Cnt.replace(",",""),
            SORT: "M"

        };
        MasterData = [Mdata];




        for (var a = 0; a < checkedItems.length; a++) {
            var rowData = checkedItems[a];
            var tempHbl = {
                HBL_NO: cutStringToCharLength(_fnToNull(rowData.WAYBILL_NO), 20),
                MBL_NO: _fnToNull($("#lbl_MBL_NO").text().replace(/-/g, '')),
                LOGISTICS_CODE: cutStringToCharLength(_fnToNull(rowData.LOGISTICS_CODE), 20),
                COP_NO: cutStringToCharLength(_fnToNull(rowData.COP_NO), 20),
                SHP_NM: cutStringToCharLength(_fnToNull(rowData.SHP_NM), 100),
                SHP_ADDR: cutStringToCharLength(_fnToNull(rowData.SHP_ADDR), 512),
                CNE_NM: cutStringToCharLength(_fnToNull(rowData.CNE_NM), 100),
                CNE_ADDR: cutStringToCharLength(_fnToNull(rowData.CNE_ADDR), 512),
                RET_NM: cutStringToCharLength(_fnToNull(rowData.RETURNER_NAME), 100),
                RET_ADDR: cutStringToCharLength(_fnToNull(rowData.RETURNER_ADDR), 512),
                TOT_PRICE: cutStringToCharLength(_fnToNull(rowData.TOTAL_PRICE), 15),
                STATUS: "I",
                UNI_YN: "Y",
                CURR_NM: cutStringToCharLength(_fnToNull(rowData.CURR), 3),
                PARCEL_YN: "Y",
                MANIF_YN: "Y",
                CLEAER_MODE: cutStringToCharLength(_fnToNull(rowData.CLEAER_MODE), 4),
                INS_YMD: GetDateTime_INS_YMD(),
                INS_HM: GetDateTime_INS_HM(),
                UNIPASS_CARGO_INFO_ID: "",
                UPD_USR: "",
                UPD_YMD: "",
                UPD_HM: "",
                SEQ: 0,
                MNGT_NO: _fnToNull(mngt_no),
                INS_USR: "WEBPROC",
                PASS_PORT: cutStringToCharLength(_fnToNull(rowData.PASS_PORT), 20),
                CNE_ZIP: cutStringToCharLength(_fnToNull(rowData.CNE_ZIP), 20),
                CNE_MO: cutStringToCharLength(_fnToNull(rowData.CNE_MO), 30),
                CNE_MAIL: cutStringToCharLength(_fnToNull(rowData.CNE_MAIL), 100),
                SHP_ZIP: cutStringToCharLength(_fnToNull(rowData.SHP_ZIP), 20),
                SHP_MO: cutStringToCharLength(_fnToNull(rowData.SHP_MO), 30),
                SHP_MAIL: cutStringToCharLength(_fnToNull(rowData.SHP_MAIL), 100),
                BIG_BAG: cutStringToCharLength(_fnToNull(rowData.BIG_BAG), 50),
                PARTNER_CD: cutStringToCharLength(_fnToNull(rowData.PARTNER_CD), 20),
                SORT: "H"
            };
            HblData.push(tempHbl);

            var tmepSku = {
                HBL_NO: cutStringToCharLength(_fnToNull(rowData.WAYBILL_NO), 20),
                SEQ: 0,
                ITEM_NM: cutStringToCharLength(_fnToNull(rowData.ITEM_NAME), 300),
                HS_CD: cutStringToCharLength(_fnToNull(rowData.HS_CODE), 10),
                ITEM_ID: cutStringToCharLength(_fnToNull(rowData.ITEM_ID), 30),
                MBL_NO: _fnToNull($("#lbl_MBL_NO").text().replace(/-/g, '')),
                SKU_PRICE: cutStringToCharLength(_fnToNull(rowData.SKU_PRICE), 15),
                TAX_PRICE: 0,
                POST_PRICE: 0,
                GRS_WGT: cutStringToCharLength(_fnToNull(rowData.SKU_WEIGHT), 15),
                MNGT_NO: _fnToNull(mngt_no),
                CURR: cutStringToCharLength(_fnToNull(rowData.CURR), 5),
                P_URL: cutStringToCharLength(_fnToNull(rowData.P_URL), 100),
                STATUS: "online",
                QTY: cutStringToCharLength(_fnToNull(rowData.QTY), 100),
                SORT: "S"
            }
            SkuData.push(tmepSku);
        }


        var objMBL = new Object();
        var objHBL = new Object();
        var objSKU = new Object();

        //마스터 정보 담기
        objMBL = JSON.parse(JSON.stringify(MasterData));

        //하우스 정보 담기
        objHBL = JSON.parse(JSON.stringify(HblData));

        ////아이템 정보 담기
        objSKU = JSON.parse(JSON.stringify(SkuData));

        try {
            $.ajax({
                type: "POST",
                url: "/Order/fnSaveData",
                async: true,
                dataType: "text",
                data: { "vJsonData": JSON.stringify(objMBL) },
                success: function (result) {
                    // MBL 저장
                    if (result == "Y") {
                        $.ajax({
                            type: "POST",
                            url: "/Order/fnSaveData",
                            async: true,
                            dataType: "text",
                            data: { "vJsonData": JSON.stringify(objHBL) },
                            success: function (result) {
                                // HBL 저장
                                if (result == "Y") {
                                    $.ajax({
                                        type: "POST",
                                        url: "/Order/fnSaveData",
                                        async: true,
                                        dataType: "text",
                                        data: { "vJsonData": JSON.stringify(objSKU) },
                                        success: function (result) {
                                            //SKU 저장
                                            // 그리드에서 모든 선택을 해제
                                            if (result == "Y") {
                                                grid.setSelectedRows([]);
                                                $("#Search_Type option:eq(0)").prop("selected", true);
                                                $("#Search_Value").val(MasterData[0].MBL_NO);
                                                $("#SearchBtn").click();
                                                _fnAlertMsg("저장되었습니다.")
                                            }
                                            else {
                                                grid.setSelectedRows([]);
                                                _fnAlertMsg("데이터 오류 입니다. 확인해주세요.")
                                            }

                                        }, error: function (xhr, status, error) {
                                            console.log(error);
                                        },
                                        beforeSend: function () {
                                            $("#ProgressBar_Loading").show(); //프로그래스 바
                                        },
                                        complete: function () {
                                            $("#ProgressBar_Loading").hide(); //프로그래스 바
                                        }
                                    })
                                } else {
                                    grid.setSelectedRows([]);
                                    _fnAlertMsg("HBL 오류 데이터 확인.")
                                }

                            }, error: function (xhr, status, error) {
                                console.log(error);
                            },
                            beforeSend: function () {
                                $("#ProgressBar_Loading").show(); //프로그래스 바
                            }
                        })
                    } else {
                        grid.setSelectedRows([]);
                        _fnAlertMsg("MBL 오류 데이터 확인.")
                    }
                }, error: function (xhr, status, error) {
                    console.log(error);
                },
                beforeSend: function () {
                    $("#ProgressBar_Loading").show(); //프로그래스 바
                }

            })

        } catch (error) {
            console.error('에러 발생:', xhr.status);
        }

    })


//엑셀  다운로드
$(document).on("click", "#btnExcel", function () {
    var selectedRows = getSelectedData();

    //var modifiedRows = selectedRows.map(row => {
    //    const { NEXT_SEND, SEQ, id, CARGO_ID, ...rest } = row;
    //    return rest;
    //});
    //Validation
    if (selectedRows.length == 0) {
        _fnAlertMsg("조회 후 이용해주세요.");
        return false;
    }
    else {

        downloadExcelFile2(selectedRows);
    }


});

//#endregion

    //#region ================= Comm function=================

function downloadExcelFile2(data) {
    //var headers = [
    //    ["H B/L", "송하인", "수하인", "품명", "수량", "중량", "CBM", "송장금액", "거래구분", "용도구분", "통관구분", "전자상거래 유형", "개인통관부호", "개인통관부호 검증", "우편번호 검증", "TERMS", "통관료", "창고료", "세금", "멀티송장 여부"]
    //];

    var headers = [["LAST_CODE", "ExceptYN", "WayBillNo", "LogisticsCode", "CopNo", "BigBag", "PassPort", "RecName", "RecAddr", "RecPhone", "SendName", "SendAddr", "SendPhone", "TotalPrice", "Currency", "GoodsPrice", "TaxPrice", "PostPrice", "ClearanceMode", "Weight", "ItemName", "HSCODE", "ItemId", "QTY", "URL", "MNGTID", "MblNo", "PartnerCode", "RecZip", "RecMail"]];
    //var headers = [];

    var worksheet = XLSX.utils.json_to_sheet(data);
    XLSX.utils.sheet_add_json(worksheet, headers, { skipHeader: true, origin: "A1" });

    worksheet["!rows"] = [{ hpt: 30, outlineLevel: 1 }]; // 각 행의 높이 지정
    worksheet['!cols'] = [
        { wpx: 80 }, // LAST_CODE
        { wpx: 80 },  // ExceptYN
        { wpx: 120 },  // WayBillNo
        { wpx: 120 },  // LogisticsCode
        { wpx: 120 },  // CopNo
        { wpx: 120 },  // BigBag
        { wpx: 120 },  // PassPort
        { wpx: 80 },  // RecName
        { wpx: 300 },  // RecAddr
        { wpx: 100 },  // RecPhone
        { wpx: 100 },   // SendName
        { wpx: 300 }, // SendAddr
        { wpx: 100 },  // SendPhone
        { wpx: 60 },  // Total price
        { wpx: 60 },  // Currency
        { wpx: 80 },  // GoodsPrice
        { wpx: 60 },  // Tax price
        { wpx: 60 },  // Post Price
        { wpx: 60 },  // Clearance Mode
        { wpx: 60 },  // GrossWeight
        { wpx: 200 },   // Item Name
        { wpx: 60 }, // Hs code
        { wpx: 120 },  // Item Id
        { wpx: 60 },  // QTY
        { wpx: 400 },  // URL
        { wpx: 120 },  // MASTER_NO
        { wpx: 150 }, // PARTNER_CODE
        { wpx: 150 },  // MASTER_NO
        { wpx: 80 },  // MASTER_NO
        { wpx: 200 } // PARTNER_CODE
    ];



    var excel_name = $("#lbl_MBL_NO").text() + ".xlsx";
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


    function cutStringToCharLength(str, maxLength) {
        let byteLength = 0;
        let result = '';

        result = String(str).substring(0, maxLength);
        return result;
    }



    function cutStringToByteLength(str, maxLength) {
        let byteLength = 0;
        let result = '';
        for (let i = 0; i < str.length; i++) {
            // 현재 문자의 바이트 길이 계산
            const currentChar = str.charCodeAt(i);
            if (currentChar <= 0x7F) {
                byteLength += 1; // ASCII 범위 내의 문자
            } else if (currentChar <= 0x7FF) {
                byteLength += 2; // 2바이트 문자
            } else if (currentChar <= 0xFFFF) {
                byteLength += 3; // 3바이트 문자
            } else {
                byteLength += 4; // 4바이트 문자 (예: 이모지)
            }

            // 지정된 maxLength를 초과하는 경우, 반복을 종료
            if (byteLength > maxLength) {
                break;
            }
            result += str[i];
        }
        return result;
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

    //function _fnAlertMsg_confirm(msg) {
    //$(".alert_cont .inner .inner_cont p").text("");
    //$(".alert_cont .inner .inner_cont p").text(msg);
    //layerPopup('#alert_confirm');
    //}



    function fileCus() {
        $(document).on("change", ".file_cus input[type=file]", function () {
            const fileName = $(this).val().split("\\").pop();
            $(this).siblings(".file_name").text(fileName || "파일을 선택해주세요.");
        });
    }

function customCellValidation(grid, args) {

    //값이 있고 마지막 상태값이 보류일때 
    if (grid.getColumns()[args.cell].id == 'PROC_NM' && (args.value == "통관목록보류" || args.value == "수입(사용소비) 심사진행" || args.value == "통관보류") && (args.dataContext["L_STATUS"] == "2040" || args.dataContext["L_STATUS"] == "3006")) {
        return "blue";
    }

    if (grid.getColumns()[args.cell].id == 'WAYBILL_NO' && args.value != "") {

        //현재 전송 마지막이 보류건일 때
        if (args.dataContext["NEXT_SEND"] == "2040" || args.dataContext["NEXT_SEND"] == "3006" ) {
            return "red_1";
        }
    }
    return null;
}

//#endregion