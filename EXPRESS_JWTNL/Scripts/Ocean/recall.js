//#region ☆☆☆☆☆전역변수☆☆☆☆☆
var dataView = new Slick.Data.DataView();
var dataView1 = new Slick.Data.DataView();
var grid; var columns; var checkData = false;
var grid1; var columns1; var checkData = false;
var checkboxSelector;
var checkedItems = [];
var UpdateBtnOpen = "N";

//#endregion

$(function () {
    //로그인 세션 확인
    if (_fnToNull($("#Session_USR_ID").val()) == "") {
        window.location = window.location.origin;
    }

    //#region ☆날짜 셋팅☆
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
    //#endregion

    $("#sendPopText").text("전송 항목");

    $(document).on('keyup', '#Search_Value', function (e) { 
        if (e.keyCode == 13) {
            $("#SearchBtn").click();
        }
    });

    //#region ☆그리드 셋팅☆
    // 컬럼
    columns = [
        { id: "NEXT_SEND", name: "API", field: "NEXT_SEND", sortable: true, resizable: false, width: 50 },
        { id: "MBL_NO", name: "MASTER NO", field: "MBL_NO", sortable: true, resizable: false, width: 120},
        { id: "WAYBILL_NO", name: "WayBillNo", field: "WAYBILL_NO", sortable: true, resizable: false, width: 130, cssClass: "text-center" },
        { id: "LOGISTICS_CODE", name: "Logistics Code", field: "LOGISTICS_CODE", sortable: true, resizable: false, width: 120 },
        { id: "RECIVER_NAME", name: "Reciver Name", field: "RECIVER_NAME", sortable: true, resizable: false, width: 180 },
        { id: "RECIVER_ADDR", name: "Reciver Addr", field: "RECIVER_ADDR", sortable: true, resizable: false, width: 1000 },
        { id: "SENDER_NAME", name: "Sender Name", field: "SENDER_NAME", sortable: true, resizable: false, width: 300 },
        { id: "SENDER_ADDR", name: "Sender addr", field: "SENDER_ADDR", sortable: true, resizable: false, width: 1000 },
        { id: "TOTAL_PRICE", name: "Total price", field: "TOTAL_PRICE", sortable: true, resizable: false, width: 100 },
        { id: "CURRENCY", name: "Currency", field: "CURRENCY", sortable: true, resizable: false, width: 80, cssClass: "text-center_1" },
        { id: "COP_NO", name: "CopNo", field: "COP_NO", sortable: true, resizable: false, width: 200 },
        { id: "CLEAER_MODE", name: "Clearance Mode", field: "CLEAER_MODE", sortable: true, resizable: false, width: 120 },
        /*{ id: "MBL_NO", name: "MASTER NO", field: "MBL_NO", sortable: true, resizable: false, width: 120, hidden: true },*/
        { id: "PARTNER_CD", name: "PARTNER CODE", field: "PARTNER_CD", sortable: true, resizable: false, width: 120, hidden: true },
        { id: "CARGO_ID", name: "CARGOID", field: "CARGO_ID", sortable: true, resizable: false, width: 120, hidden: true },
        { id: "SEQ", name: "SEQ", field: "SEQ", sortable: true, resizable: false, width: 120, hidden: true }
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

    

    grid.onSort.subscribe(function (e, args) {
        var comparer = function (a, b) {
            var x = a[args.sortCol.field], y = b[args.sortCol.field];
            return (x === y ? 0 : (x > y ? 1 : -1));
        };
        dataView.sort(comparer, args.sortAsc);
        grid.invalidate(); // 그리드를 다시 그리도록 설정
        grid.render(); // 변경 사항을 적용하여 그리드를 다시 렌더링
    });





   //#endregion

});


//#region ☆☆☆조회 검색 이벤트☆☆☆
$(document).on("change", "#Search_Type", function () {
    var sch_type = _fnToNull(this.value);

    if (sch_type == "MBL_NO") {
        $("#Multi_BL").attr("disabled", false);
        $("#ymd_text").text("입항일");
    }
    else {
        $("#Multi_BL").attr("disabled", true);
        $("#Multi_BL").val("");
        $("#ymd_text").text("수신일");
    }
});

//#region 날짜 셋팅
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


//조회
$(document).on('click', '#SearchBtn', function () {
    
    columns = [
        { id: "NEXT_SEND", name: "API", field: "NEXT_SEND", sortable: true, resizable: false, width: 50 },
        /*{ id: "MBL_NO", name: "MasterWayBillNo", field: "MBL_NO", sortable: true, resizable: false, width: 130, cssClass: "text-center" },*/
        { id: "MBL_NO", name: "MASTER NO", field: "MBL_NO", sortable: true, resizable: false, width: 120 },
        { id: "WAYBILL_NO", name: "WayBillNo", field: "WAYBILL_NO", sortable: true, resizable: false, width: 130, cssClass: "text-center" },
        { id: "LOGISTICS_CODE", name: "Logistics Code", field: "LOGISTICS_CODE", sortable: true, resizable: false, width: 120 },
        { id: "RECIVER_NAME", name: "Reciver Name", field: "RECIVER_NAME", sortable: true, resizable: false, width: 180 },
        { id: "RECIVER_ADDR", name: "Reciver Addr", field: "RECIVER_ADDR", sortable: true, resizable: false, width: 1000 },
        { id: "SENDER_NAME", name: "Sender Name", field: "SENDER_NAME", sortable: true, resizable: false, width: 300 },
        { id: "SENDER_ADDR", name: "Sender addr", field: "SENDER_ADDR", sortable: true, resizable: false, width: 1000 },
        { id: "TOTAL_PRICE", name: "Total price", field: "TOTAL_PRICE", sortable: true, resizable: false, width: 100 },
        { id: "CURRENCY", name: "Currency", field: "CURRENCY", sortable: true, resizable: false, width: 80 ,cssClass: "text-center_1"},
        { id: "COP_NO", name: "CopNo", field: "COP_NO", sortable: true, resizable: false, width: 200 },
        { id: "CLEAER_MODE", name: "Clearance Mode", field: "CLEAER_MODE", sortable: true, resizable: false, width: 120 },
        //{ id: "MBL_NO", name: "MASTER_NO", field: "MBL_NO", sortable: true, resizable: false, width: 120, hidden: true },
        { id: "PARTNER_CD", name: "PARTNER CODE", field: "PARTNER_CD", sortable: true, resizable: false, width: 120, hidden: true },
        { id: "CARGO_ID", name: "CARGOID", field: "CARGO_ID", sortable: true, resizable: false, width: 120, hidden: true },        
        { id: "SEQ", name: "SEQ", field: "SEQ", sortable: true, resizable: false, width: 120, hidden: true }
    ];
    
    var objJsonData = new Object();

    //#region Validation



    if (_fnToNull($("#StartDate").val().replace(/-/gi, "")) == "") {
        _fnAlertMsg("시작일을 입력 해 주세요.");
        return false;
    }

    if (_fnToNull($("#EndDate").val().replace(/-/gi, "")) == "") {
        _fnAlertMsg("종료일을 입력 해 주세요.");
        return false;
    }

    //#endregion


    //#region MakeParams & Ajax Call
    objJsonData.SEARCH_TYPE = $("#Search_Type option:selected").val();
    objJsonData.SEARCH_VALUE = _fnToNull($("#Search_Value").val());
    objJsonData.EXCEPT_TYPE = $("#Except_Type option:selected").val(); // exception 값
    objJsonData.STRT_YMD = $("#StartDate").val().substring(0, 10).replace(/-/gi, "");
    objJsonData.END_YMD = $("#EndDate").val().substring(0, 10).replace(/-/gi, "");
    objJsonData.PARTNER_CD = $("#REQ_SVC_TYPE option:selected").val();
    objJsonData.MULTI = $("#Multi_BL").val();



    $.ajax({
        type: "POST",
        url: "/Order/fnSearchNewCallBack",
        async: true,
/*        dataType: "json",*/
        data: { "vJsonData": _fnMakeJson(objJsonData) },
        success: function (result) {
            var html = $.parseHTML(result);
            var target = $(html).find('#test');
            var dataValue = target.attr('value');
            $("#tot_cnt").text("");
            if (_fnToNull(dataValue) != "") {

                //#region 상단 버튼 셋팅
                if (objJsonData.EXCEPT_TYPE != "CARROUT") {
                    UpdateBtnOpen = "Y";
                    $("#btnUpdate").removeAttr("disabled");
                    $("#btnUpdate").css("cursor", "pointer");
                    $("#btnUpdate").removeClass("gray");
                    $("#btnUpdate").addClass("deep_green");
                }
                else {
                    //UpdateBtnOpen = "Y";
                    //$("#btnUpdate").removeAttr("disabled");
                    //$("#btnUpdate").css("cursor", "pointer");
                    //$("#btnUpdate").removeClass("gray");
                    //$("#btnUpdate").addClass("deep_green");
                    UpdateBtnOpen = "N";
                    $("#btnUpdate").attr("disabled", true);
                    $("#btnUpdate").css("cursor", "default");
                    $("#btnUpdate").removeClass("deep_green");
                    $("#btnUpdate").addClass("gray");
                }

                //#endregion

                var mstData;
                var jsonData = JSON.parse(dataValue).CallBack; // 받은 데이터에서 필요한 부분 추출
                if (objJsonData.SEARCH_VALUE != "") {
                    mstData = JSON.parse(dataValue).MBL_DATA;
                }

                if (jsonData.length > 0) {
                    var v_type;
                    var Ie_type;

                    //var jsonMstData = JSON.parse(result).MST; //HEADER 데이터
                    //var jsonDtlData = JSON.parse(result).DTL; // DTL 데이터

                    //// 검색조건 있으면 마스터 셋팅
                    //if (objJsonData.SEARCH_VALUE != "") {
                    //    if (mstData[0].IE_TYPE == "I") {
                    //        Ie_type = "수입";
                    //    } else {
                    //        Ie_type = "수출";
                    //    }
                    //    $("#lbl_EXIM").text(Ie_type);//수출입구분
                    //    $("#lbl_MBL_NO").text(mstData[0].MBL_NO);//MAWB
                    //    if (mstData[0].TRANS_TYPE == "2") {
                    //        v_type = "해운";
                    //    } else if (mstData[0].TRANS_TYPE == "4") {
                    //        v_type = "도로";
                    //    } else if (mstData[0].TRANS_TYPE == "5") {
                    //        v_type = "항공";
                    //    } else {
                    //        v_type = "우편";
                    //    }
                    //    $("#lbl_TSTYPE").text(v_type);//Transport Type
                    //    $("#lbl_TSNO").text(mstData[0].VOY);//Transport No
                    //    $("#lbl_ETD").text(formattingDateEng(mstData[0].ETD));//ETD
                    //    $("#lbl_POL").text(mstData[0].POL);//출발지
                    //    $("#lbl_ETA").text(formattingDateEng(mstData[0].ETA));//ETA
                    //    $("#lbl_POD").text(mstData[0].POD);//도착지
                    //}


                    // 데이터를 표시할 배열 초기화
                    var data = [];
                    var hblCnt = 0;
                    var chk_hbl_no = "";
                    //#region 데이터 바인딩
                    for (var i = 0; i < jsonData.length; i++) {
                        if (chk_hbl_no != jsonData[i].HBL_NO) {
                            hblCnt += 1;
                            chk_hbl_no = jsonData[i].HBL_NO;
                        }
                        data.push({
                            NEXT_SEND: jsonData[i].STATE,
                            MBL_NO: jsonData[i].MBL_NO,
                            WAYBILL_NO: jsonData[i].HBL_NO,
                            LOGISTICS_CODE: jsonData[i].LOGISTIC_CD,
                            RECIVER_NAME: jsonData[i].CNE_NM,
                            RECIVER_ADDR: jsonData[i].CNE_ADDR,
                            SENDER_NAME: jsonData[i].SHP_NM,
                            SENDER_ADDR: jsonData[i].SHP_ADDR,
                            TOTAL_PRICE: jsonData[i].TOT_PRICE,
                            CURRENCY: jsonData[i].CURR_NM,
                            COP_NO: jsonData[i].COP_NO,
                            CLEAER_MODE: jsonData[i].CLEAER_MODE,
                            CARGO_ID: jsonData[i].UNIPASS_CARGO_INFO_ID,
                            PARTNER_CD: jsonData[i].PARTNER_CD,
                            /*MBL_NO: jsonData[i].MBL_NO,*/
                            SEQ: jsonData[i].SEQ,
                            id: i
                        });
                    }
                    $("#tot_cnt").text(hblCnt.toString().replace(/\B(?<!\.\d*)(?=(\d{3})+(?!\d))/g, ","));

                    grid.onBeforeAppendCell.subscribe(function (e, args) {
                        return customCellValidation(grid, args);
                    });

                    //#endregion

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

                //#region 타입 바인딩 및 팝업 열기

                var typeJsonData = new Object();
                typeJsonData.SEARCH_TYPE = $("#Except_Type option:selected").val();

                $.ajax({
                    type: "POST",
                    url: "/Order/BindCallbackSend",
                    async: true,
                    dataType: "json",
                    data: { "vJsonData": _fnMakeJson(typeJsonData) },
                    success: function (result) {
                        if (_fnToNull(result)) {
                            var dataset = JSON.parse(result);

                            if (dataset.Result[0]["trxCode"] == "Y") {
                                fnSettingSinglePopArea(dataset.NEXT_LIST);
                                /*layerPopup("#SingleExcept");*/
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
                });
            //#endregion
            }
            else {
                grid.setSelectedRows([]);
                _fnAlertMsg("데이터가 없습니다.");
                dataView.beginUpdate();
                dataView.setItems([]);
                dataView.endUpdate();

                grid.setColumns(columns);
                grid.invalidate();
                grid.render();
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
    //#endregion




    //#region 화물진행정보 조회 셋팅
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

            if (value2 == "") {
                _fnAlertMsg("화물진행정보가 없습니다.");
                return false;
            }

            objJsonData.SEARCH_TYPE = "PopUp";
            objJsonData.SEARCH_VALUE = value;
            objJsonData.SEARCH_VALUE2 = value2;

            $.ajax({
                type: "POST",
                url: "/Order/fnUniSearchData",
                async: false,
                dataType: "json",
                data: { "vJsonData": _fnMakeJson(objJsonData) },
                success: function (result) {
                    if (JSON.parse(result).Result[0]["trxCode"] == "Y") {
                        if (JSON.parse(result).UMST.length > 0) {
                            layerPopup("#Tracking");
                            var jsonMstData = JSON.parse(result).MST; //HEADER 데이터
                            var jsonDtlData = JSON.parse(result).DTL; // DTL 데이터
                            var jsonUMstData = JSON.parse(result).UMST; // Unipass Mst 데이터
                            var jsonUDtlData = JSON.parse(result).UDTL; // Unipass DTL 데이터

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

                            //#region 기본 사유 팝업 셋팅
                            $("#btnCntr").removeAttr("disabled");
                            $("#Hold_CD").val("3004");
                            $("#Hold_CD").removeAttr("disabled");
                            $("#Hold_Detail").val("0000");
                            $("#Hold_Detail").removeAttr("disabled");
                            $("#Waybill").val(item.HBL_NO);
                            $("#Mngt_id").val(item.MNGT_ID);
                            $("#Cop_no").val(item.COP_NO);
                            $("#Clear_mode").val(item.CLEAER_MODE);
                            $("#btnCntr").removeClass("none_use");

                            //#endregion

                            var options1 = {
                                enableCellNavigation: true,
                                enableAddRow: false,
                                autosizeColsMode: Slick.GridAutosizeColsMode.FitViewportToCols,
                                enableColumnReorder: false
                            }
                            // 컬럼
                            var columns1 = [
                                
                                { id: "SEQ", name: "순번", field: "SEQ", sortable: false, resizable: false, width: 100 },
                                { id: "SEND_YN", name: "API", field: "SEND_YN", sortable: false, resizable: false, width: 130 },
                                { id: "PROC_NM", name: "처리 단계", field: "CARGTRCNRELABSOPTPCD", sortable: false, resizable: false, width: 352 },
                                { id: "PROC_TIME", name: "처리 일시", field: "PRCSDTTM", sortable: false, resizable: false, width: 352 },
                                { id: "PROC_LOC", name: "장치장 명", field: "SHEDNM", sortable: false, resizable: false, width: 352 },
                                //{ id: "SEND_YN", name: "API", field: "SEND_YN", sortable: true, resizable: false, width: 352 },
                                { id: "COP_NO", name: "cop_no", field: "COP_NO", sortable: false, resizable: false, width: 352, hidden: true},
                                { id: "HBL_NO", name: "houseb/l", field: "HBL_NO", sortable: false, resizable: false, width: 352, hidden: true},
                                { id: "CLEAER_MODE", name: "clear_mode", field: "CLEAER_MODE", sortable: true, resizable: false, width: 352, hidden: true},
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
                                    HBL_NO: jsonDtlData[0].HBL_NO,
                                    MBL_NO: jsonUDtlData[i].MBL_NO,
                                    CNTR_NO: jsonUDtlData[i].CNTR_NO,
                                    SEQ: jsonUDtlData[i].SEQ,
                                    CARGTRCNRELABSOPTPCD: jsonUDtlData[i].CARGTRCNRELABSOPTPCD,
                                    PRCSDTTM: formattingDateKOR(jsonUDtlData[i].PRCSDTTM),
                                    SHEDNM: jsonUDtlData[i].SHEDNM,
                                    SEND_YN: jsonUDtlData[i].SEND_YN,
                                    MNGT_ID: jsonUMstData[0].MNGT_ID,
                                    COP_NO: jsonDtlData[0].COP_NO,
                                    CLEAER_MODE: jsonDtlData[0].CLEAER_MODE,
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
                                    if (value == "통관목록보류" || value == "통관보류" || value == "수입(사용소비) 심사진행") {

                                        var objJsonData = new Object();
                                        objJsonData.MNGT_ID = item.MNGT_ID;

                                        //#region 기본 사유 팝업 셋팅
                                        $("#btnCntr").removeAttr("disabled");
                                        $("#Hold_CD").val("3004");
                                        $("#Hold_CD").removeAttr("disabled");
                                        $("#Hold_Detail").val("0000");
                                        $("#Hold_Detail").removeAttr("disabled");
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
                                                    }
                                                    if (_fnToNull(result["HOLD"][0].RTN_40) != "") {
                                                        $("#Hold_Detail").val(result["HOLD"][0].RTN_40);
                                                        $("#Hold_Detail").attr("disabled", 'true');
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
    });

    //#endregion

})

//#endregion





//#region ☆☆☆팝업 내 이벤트☆☆☆

//단일항목 일괄 업데이트 (팝업내 저장 버튼)
$(document).on('click', '#s_btnCntr', function () {

    var objJsonData = new Object();

    //30xx
    if (!$("#S_Hold_CD").hasClass("complete")) {
        var sendData = checkedItems;
        var featTxt = "";
        var clear_mode_text = "";
        if ($("#S_Hold_CD option:selected").val().substr(0, 2) != "40") {
            featTxt = "{\"clearanceMode\" : \"\"}";
            clear_mode_text = "";
        }
        if ($("#S_Hold_CD option:selected").text().includes("KRJY")) {
            featTxt = "{\"clearanceMode\" : \"KRJY\"}";
            clear_mode_text = "KRJY";
        }
        if ($("#S_Hold_CD option:selected").text().includes("KRML")) {
            featTxt = "{\"clearanceMode\" : \"KRML\"}";
            clear_mode_text = "KRML";
        }


        for (var i = 0; i < sendData.length; i++) {
            sendData[i].returnStatus = $("#S_Hold_CD option:selected").val();
            if (featTxt != "") {
                sendData[i].feature = featTxt;
                sendData[i].CLEAR_MODE = clear_mode_text;
            }
        }

        objJsonData.LIST = (sendData);
        $.ajax({
            type: "POST",
            url: "/Order/fnSendMulti",
            async: false,
            dataType: "json",
            data: { "vJsonData": _fnMakeJson(objJsonData) },
            success: function (result) {

                var send_state = $("#S_Hold_CD option:selected").val();
                var combine_state = _fnToNull(_fnSelectNext(send_state));
                if (combine_state == "") {
                    if (result.rec_cd == "Y") {
                        layerClose("#SingleExcept");
                        _fnAlertMsg("상태값 전송이 완료되었습니다.");
                    } 
                    else {
                        _fnAlertMsg("관리자에게 문의해주세요.");
                    }
                }
                else {
                     var sendData = checkedItems;
                     for (var i = 0; i < sendData.length; i++) {
                         sendData[i].returnStatus = combine_state;
                         if (featTxt != "") {
                             sendData[i].feature = featTxt;
                             sendData[i].CLEAR_MODE = clear_mode_text;
                         }
                     }
                     objJsonData.LIST = (sendData);
                     $.ajax({
                         type: "POST",
                         url: "/Order/fnSendMulti",
                         async: false,
                         dataType: "json",
                         data: { "vJsonData": _fnMakeJson(objJsonData) },
                         success: function (result) {
                             if (result.rec_cd == "Y") {
                                 layerClose("#SingleExcept");
                                 _fnAlertMsg("상태값 전송이 완료되었습니다.");
                             }
                             else {
                                 _fnAlertMsg("관리자에게 문의해주세요.");
                             }
                         },
                         error: function (xhr, status, error) {
                             console.log("error multi 3002 " + error);
                         },
                         beforeSend: function () {
                             $("#ProgressBar_Loading").show(); //프로그래스 바
                         },
                         complete: function () {
                             $("#ProgressBar_Loading").hide(); //프로그래스 바
                         }
                     });

                }
                


            },
            error: function (xhr, status, error) {
                console.log("error multi 30 " + error);
            },
            beforeSend: function () {
                $("#ProgressBar_Loading").show(); //프로그래스 바
            },
            complete: function () {
                $("#ProgressBar_Loading").hide(); //프로그래스 바
            }
        });
    }
    
    //연계 데이터 추가 전송 기능
   

    objJsonData.EXCEP2 = (checkedItems);

});

//#endregion

//#region ☆☆☆☆☆상단 버튼 이벤트☆☆☆☆☆

//상태 업데이트 (상태 업데이트 팝업열림)
$(document).on("click", "#btnUpdate", function () {
    //#region Check list num 
    // 리스트 만들기
    checkedItems = [];
    var listobj = new Object();
    var rows = grid.getSelectedRows();
    nTime = formatDateTimeToNumberGolobal();
    //#endregion

    if (UpdateBtnOpen == "Y") {
        if (rows.length != 0) {

            //#region 3.3 전송 할 데이터 객체 바인딩
            rows.forEach(function (row) {
                listobj = new Object();
                listobj.guid = dataView.getItem(row).CARGO_ID;
                listobj.MBL_NO = dataView.getItem(row).MBL_NO;
                listobj.wayBillNo = dataView.getItem(row).WAYBILL_NO;
                listobj.copNo = dataView.getItem(row).COP_NO;
                listobj.returnTime = nTime;
                listobj.RTN_TIME = nTime.split("+")[0];
                listobj.PARTNER_CD = dataView.getItem(row).PARTNER_CD;
                listobj.feature = "{\"clearanceMode\" : \"" + dataView.getItem(row).CLEAER_MODE+"\"}";
                checkedItems.push(listobj); // dataView는 해당 데이터의 DataView입니다.
                // dataView가 아닌 경우에는 grid.getDataItem(row)을 사용할 수 있습니다.
            });
            //#endregion

            //#region 오류 타입별 코드 바인딩

            //var typeJsonData = new Object();
            //typeJsonData.SEARCH_TYPE = $("#Except_Type option:selected").val();

            layerPopup("#SingleExcept");

            //#region 타입 바인딩 및 팝업 열기(미사용)

            /*
            $.ajax({
                type: "POST",
                url: "/Order/BindExcept",
                async: true,
                dataType: "json",
                data: { "vJsonData": _fnMakeJson(typeJsonData) },
                success: function (result) {
                    if (_fnToNull(result)) {
                        var dataset = JSON.parse(result);

                        if (dataset.Result[0]["trxCode"] == "Y") {
                            fnSettingSinglePopArea(dataset.EXCEP_LIST);
                            layerPopup("#SingleExcept");
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
            }); 
            */

            //#endregion
            
            //#endregion
        }
        else {
            _fnAlertMsg("업데이트 할 항목을 선택해주세요.");
        }
    }
    else {
        _fnAlertMsg("해당 기능은 조회 및 선택시 사용 가능합니다.");
    }
    
});

//상단 엑셀 다운로드
$(document).on("click", "#btnExcel", function () {
    var selectedRows = getSelectedData();


    //Validation
    if (selectedRows.length == 0) {
        _fnAlertMsg("조회 후 이용해주세요.");
        return false;
    }
    else {
        downloadExcelFile(selectedRows);
    }


});

//#endregion

//#region =================function=================

//엑셀 다운로드
function downloadExcelFile(data) {

    var headers = [["LAST_CODE", "MBL_NO", "WayBillNo", "LogisticsCode", "RecName", "RecAddr", "SendName", "SendAddr", "TotalPrice", "Currency", "CopNo", "ClearanceMode"]];


    var worksheet = XLSX.utils.json_to_sheet(data);
    XLSX.utils.sheet_add_json(worksheet, headers, { skipHeader: true, origin: "A1" });

    worksheet["!rows"] = [{ hpt: 30, outlineLevel: 1 }]; // 각 행의 높이 지정
    worksheet['!cols'] = [
        { wpx: 80 }, // LAST_CODE
        { wpx: 120 },  // WayBillNo
        { wpx: 100 },  // LogisticsCode
        { wpx: 120 },  // RecName
        { wpx: 120 },  // RecAddr
        { wpx: 120 },  // SendName
        { wpx: 120 },  // SendAddr
        { wpx: 100 },  // TotalPrice
        { wpx: 100 },  // Currency
        { wpx: 100 },   // CopNo
        { wpx: 100 }, // ClearanceMode
        { wpx: 100 }  // CARGO_ID
    ];

    var excel_name = "CallBack" + getCurrentDateTime_YMD() + ".xlsx";
    var workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "SelectedData");
    XLSX.writeFile(workbook, excel_name);
}


//연계전송 상태값 셋팅
function _fnSelectNext(SendText) {
    var rtnStateText = "";
    switch (SendText) {
        case "2020":
            rtnStateText = "2030";
            break;
        case "2040":
            rtnStateText = "3006";
            break;
        case "2050":
            rtnStateText = "2060";
            break;
        case "3001":
            rtnStateText = "3002";
            break;
        default:
            rtnStateText = "";
            break;
    }
    return rtnStateText;
}

//싱글선택 화면 셋팅
function fnSettingSinglePopArea(obj) {
        var list = obj
        var strHtml = "";
    
        for (var i = 0; i < list.length; i++) {
            strHtml += "<option value='" + list[i].SATAUS_CD + "'>" + list[i].NAME + "</option>";
        }

        $("#S_Hold_CD").empty();
        $("#S_Hold_CD").append(strHtml);

        //#region  초기 셋팅
        if (list[0].TYPE == "CC") {
            $("#S_Hold_CD").val("3004");
        }

        if (list[0].TYPE == "CC" || list[0].TYPE == "INFO") {
            $(".excep_warining").css("visibility", "hidden");
        }
        else {
            $(".excep_warining").css("visibility", "visible");
        }
        //#endregion
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

function customCellValidation(grid, args) {
    
        if (grid.getColumns()[args.cell].id == 'PROC_NM' && (args.value == "통관목록보류" || args.value == "통관보류" )) {
            return "red_1";
        }
    
        if (grid.getColumns()[args.cell].id == 'WAYBILL_NO' && args.value != "") {
    
            if (args.dataContext["FAIL_YN"] == "Y") {
                return "red_1";
            }
            if (args.dataContext["NEXT_SEND"] != null) {
                if (args.dataContext["NEXT_SEND"].toString().substring(0, 2) == "30" || args.dataContext["NEXT_SEND"].toString().substring(0, 2) == "40") {
                    return "blue";
                }
            }
            
        }
        return null;
    }

//#endregion