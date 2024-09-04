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

    fileCus();


    // 컬럼
    columns = [
        { id: "HBL_NO", name: "WayBillNo", field: "HBL_NO", sortable: true, resizable: true, width: 120 },
        { id: "LOGISTIC_CD", name: "Logistics Code", field: "LOGISTIC_CD", sortable: true, resizable: true, width: 120 },
        { id: "COP_NO", name: "CopNo", field: "COP_NO", sortable: true, resizable: true, width: 130 },
        { id: "BIG_BAG", name: "BigBag", field: "BIG_BAG", sortable: true, resizable: true, width: 160 },
        { id: "PASS_PORT", name: "PassPort", field: "PASS_PORT", sortable: true, resizable: true, width: 120 },
        { id: "CNE_NM", name: "Receiver Name", field: "CNE_NM", sortable: true, resizable: true, width: 120 },
        { id: "CNE_ZIP", name: "Receiver Zip", field: "CNE_ZIP", sortable: true, resizable: true, width: 120 },
        { id: "CNE_MAIL", name: "Receiver Mail", field: "CNE_MAIL", sortable: true, resizable: true, width: 160 },
        { id: "CNE_ADDR", name: "Receiver Addr", field: "CNE_ADDR", sortable: true, resizable: true, width: 1000 },
        { id: "CNE_MO", name: "Receiver Phone", field: "CNE_MO", sortable: true, resizable: true, width: 150 },
        { id: "SHP_NM", name: "Sender Name", field: "SHP_NM", sortable: true, resizable: true, width: 200 },
        { id: "SHP_ADDR", name: "Sender Addr", field: "SHP_ADDR", sortable: true, resizable: true, width: 200 },
        { id: "SHP_MO", name: "Sender Phone", field: "SHP_MO", sortable: true, resizable: true, width: 100 },
        { id: "TOT_PRICE", name: "Total price", field: "TOT_PRICE", sortable: true, resizable: true, width: 100 },
        { id: "CURR_NM", name: "Currency", field: "CURR_NM", sortable: true, resizable: true, width: 100 },
        { id: "SKU_PRICE", name: "GoodsPrice", field: "SKU_PRICE", sortable: true, resizable: true, width: 80 },
        { id: "TAX_PRICE", name: "Tax price", field: "TAX_PRICE", sortable: true, resizable: true, width: 120 },
        { id: "POST_PRICE", name: "Post Price", field: "POST_PRICE", sortable: true, resizable: true, width: 100 },
        { id: "CLEAER_MODE", name: "Clearance Mode", field: "CLEAER_MODE", sortable: true, resizable: true, width: 120 },
        { id: "GRS_WGT", name: "GrossWeight", field: "GRS_WGT", sortable: true, resizable: true, width: 150 },
        { id: "ITEM_NM", name: "Item Name", field: "ITEM_NM", sortable: true, resizable: true, width: 200 },
        { id: "HS_CD", name: "Hs code", field: "HS_CD", sortable: true, resizable: true, width: 120 },
        { id: "ITEM_ID", name: "Item Id", field: "ITEM_ID", sortable: true, resizable: true, width: 160 },
        { id: "QTY", name: "QTY", field: "QTY", sortable: true, resizable: true, width: 80 },
        { id: "P_URL", name: "URL", field: "P_URL", sortable: true, resizable: true, width: 400 },
        { id: "MBL_NO", name: "MASTER_NO", field: "MBL_NO", sortable: true, resizable: true, width: 130 },
        { id: "PARTNER_CD", name: "PARTNER_CODE", field: "PARTNER_CD", sortable: true, resizable: true, width: 120 }
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



    grid.onClick.subscribe(function (e, args) {
        var row = args.row; // 클릭된 행의 인덱스
        var col = args.cell; // 클릭된 열의 인덱스

        // 클릭된 열의 정보를 가져옴
        var column = grid.getColumns()[col];

        // 만약 특정 컬럼을 클릭한 경우에만 동작하도록 조건을 설정할 수 있습니다.
        //if (column.field === "WAYBILL_NO") {
        //    var item = grid.getDataItem(row); // 클릭된 행의 데이터를 가져옴
        //    var value = item[column.field]; // 클릭된 컬럼의 값
        //    var value2 = _fnToNull(item.CARGO_ID);
        //    WaybillTrackingPop(value, value2);
        //}

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


});


//엑셀업로드
$(document).on('click', '#btnUpload', function () {
    columns = [
        { id: "HBL_NO", name: "WayBillNo", field: "HBL_NO", sortable: true, resizable: true, width: 120 },
        { id: "LOGISTIC_CD", name: "Logistics Code", field: "LOGISTIC_CD", sortable: true, resizable: true, width: 120 },
        { id: "COP_NO", name: "CopNo", field: "COP_NO", sortable: true, resizable: true, width: 130 },
        { id: "BIG_BAG", name: "BigBag", field: "BIG_BAG", sortable: true, resizable: true, width: 160 },
        { id: "PASS_PORT", name: "PassPort", field: "PASS_PORT", sortable: true, resizable: true, width: 120 },
        { id: "CNE_NM", name: "Receiver Name", field: "CNE_NM", sortable: true, resizable: true, width: 120 },
        { id: "CNE_ZIP", name: "Receiver Zip", field: "CNE_ZIP", sortable: true, resizable: true, width: 120 },
        { id: "CNE_MAIL", name: "Receiver Mail", field: "CNE_MAIL", sortable: true, resizable: true, width: 160 },
        { id: "CNE_ADDR", name: "Receiver Addr", field: "CNE_ADDR", sortable: true, resizable: true, width: 1000 },
        { id: "CNE_MO", name: "Receiver Phone", field: "CNE_MO", sortable: true, resizable: true, width: 150 },
        { id: "SHP_NM", name: "Sender Name", field: "SHP_NM", sortable: true, resizable: true, width: 200 },
        { id: "SHP_ADDR", name: "Sender Addr", field: "SHP_ADDR", sortable: true, resizable: true, width: 200 },
        { id: "SHP_MO", name: "Sender Phone", field: "SHP_MO", sortable: true, resizable: true, width: 100 },
        { id: "TOT_PRICE", name: "Total price", field: "TOT_PRICE", sortable: true, resizable: true, width: 100 },
        { id: "CURR_NM", name: "Currency", field: "CURR_NM", sortable: true, resizable: true, width: 100 },
        { id: "SKU_PRICE", name: "GoodsPrice", field: "SKU_PRICE", sortable: true, resizable: true, width: 80 },
        { id: "TAX_PRICE", name: "Tax price", field: "TAX_PRICE", sortable: true, resizable: true, width: 120 },
        { id: "POST_PRICE", name: "Post Price", field: "POST_PRICE", sortable: true, resizable: true, width: 100 },
        { id: "CLEAER_MODE", name: "Clearance Mode", field: "CLEAER_MODE", sortable: true, resizable: true, width: 120 },
        { id: "GRS_WGT", name: "GrossWeight", field: "GRS_WGT", sortable: true, resizable: true, width: 150 },
        { id: "ITEM_NM", name: "Item Name", field: "ITEM_NM", sortable: true, resizable: true, width: 200 },
        { id: "HS_CD", name: "Hs code", field: "HS_CD", sortable: true, resizable: true, width: 120 },
        { id: "ITEM_ID", name: "Item Id", field: "ITEM_ID", sortable: true, resizable: true, width: 160 },
        { id: "QTY", name: "QTY", field: "QTY", sortable: true, resizable: true, width: 80 },
        { id: "P_URL", name: "URL", field: "P_URL", sortable: true, resizable: true, width: 400 },
        { id: "MBL_NO", name: "MASTER_NO", field: "MBL_NO", sortable: true, resizable: true, width: 130 },
        { id: "PARTNER_CD", name: "PARTNER_CODE", field: "PARTNER_CD", sortable: true, resizable: true, width: 120 }
    ];

    var fileInput = document.getElementById("fileInput");
    var file = fileInput.files[0];
    if (file) {
        var reader = new FileReader();
        reader.onload = function (e) {
            var data = e.target.result;
            var workbook = XLSX.read(data, { type: 'binary' });
            var Mngt_No = getCurrentDateTime();
            var jsonData = [];

            workbook.SheetNames.forEach(function (item, index, array) {
                var sheetData = XLSX.utils.sheet_to_json(workbook.Sheets[item]);
                sheetData.forEach(function (row) {
                    row.MNGT_NO = Mngt_No;
                    jsonData.push(row);
                });
            });


            var stringJson = JSON.stringify(jsonData, null, 2);


            //자료가 있을 때 
            if (jsonData.length > 0) {
                $.ajax({
                    type: "POST",
                    url: "/Order/fnGetList",
                    async: true,
                    /*        dataType: "json",*/
                    data: { "vJsonData": stringJson },
                    success: function (result) {
                        var html = $.parseHTML(result);
                        var target = $(html).find('#consol_list');
                        var dataValue = target.attr('value');

                        $("#tot_cnt").text("");

                        if (_fnToNull(dataValue) != "") {
                            var jsonData = JSON.parse(dataValue).Table1; // 받은 데이터에서 필요한 부분 추출
                            if (jsonData.length > 0) {
                                // 데이터를 표시할 배열 초기화
                                var data = [];
                                var hblCnt = 0;
                                var chk_hbl_no = "";

                                for (var i = 0; i < jsonData.length; i++) {
                                    if (chk_hbl_no != jsonData[i].HBL_NO) {
                                        hblCnt += 1;
                                        chk_hbl_no = jsonData[i].HBL_NO;
                                    }
                                    data.push({
                                        NEXT_SEND: jsonData[i].NEXT_SEND,
                                        FAIL_YN: jsonData[i].FAIL_YN,
                                        HBL_NO: jsonData[i].HBL_NO,
                                        LOGISTIC_CD: jsonData[i].LOGISTIC_CD,
                                        COP_NO: jsonData[i].COP_NO,
                                        BIG_BAG: jsonData[i].BIG_BAG,
                                        PASS_PORT: jsonData[i].PASS_PORT,
                                        CNE_NM: jsonData[i].CNE_NM,
                                        CNE_ADDR: jsonData[i].CNE_ADDR,
                                        CNE_MO: jsonData[i].CNE_MO,
                                        SHP_NM: jsonData[i].SHP_NM,
                                        SHP_ADDR: jsonData[i].SHP_ADDR,
                                        SHP_MO: jsonData[i].SHP_MO,
                                        TOT_PRICE: jsonData[i].TOT_PRICE,
                                        CURR_NM: jsonData[i].CURR_NM,
                                        SKU_PRICE: jsonData[i].SKU_PRICE,
                                        TAX_PRICE: jsonData[i].TAX_PRICE,
                                        POST_PRICE: jsonData[i].POST_PRICE,
                                        CLEAER_MODE: jsonData[i].CLEAER_MODE,
                                        GRS_WGT: jsonData[i].GRS_WGT,
                                        ITEM_NM: jsonData[i].ITEM_NM,
                                        HS_CD: jsonData[i].HS_CD,
                                        ITEM_ID: jsonData[i].ITEM_ID,
                                        QTY: jsonData[i].QTY,
                                        P_URL: jsonData[i].P_URL,
                                        UNIPASS_CARGO_INFO_ID: jsonData[i].UNIPASS_CARGO_INFO_ID,
                                        MBL_NO: jsonData[i].MBL_NO,
                                        PARTNER_CD: jsonData[i].PARTNER_CD,
                                        CNE_ZIP: jsonData[i].CNE_ZIP,
                                        CNE_MAIL: jsonData[i].CNE_MAIL,
                                        SEQ: jsonData[i].SEQ,
                                        id: i
                                    });
                                }

                                $("#tot_cnt").text(hblCnt.toString().replace(/\B(?<!\.\d*)(?=(\d{3})+(?!\d))/g, ","));

                            }
                        }

                        // 그리드에서 모든 선택을 해제
                        grid.setSelectedRows([]);
                        dataView.beginUpdate();
                        dataView.setItems(data);
                        dataView.endUpdate();

                        grid.setColumns(columns);
                        grid.invalidate();
                        grid.render();
                        layerClose("#_Upload");
                    },
                    error: function (xhr, status, error) {
                        alert(xhr.responseText);
                        alert(error);
                    },
                    beforeSend: function () {
                        $("#ProgressBar_Loading").show(); //프로그래스 바
                    },
                    complete: function () {
                        $("#ProgressBar_Loading").hide(); //프로그래스 바
                    }

                })
            }
            

        };

        reader.readAsArrayBuffer(file);
    }
});



$(document).on('click', '#ExcelUpload', function () {
    /*$("#file_Nm").text("파일을 선택해주세요.")*/
    layerPopup("#_Upload");
});



$(document).on("click", "#btnExcel", function () {
    var selectedRows = dataView.getItems();

    //Validation
    if (selectedRows.length == 0) {
        _fnAlertMsg("조회 후 이용해주세요.");
        return false;
    }
    else {

        /*delete selectedRows.SEQ;*/

        downloadExcelFile2(selectedRows);
    }


});

    //=================function=================

function downloadExcelFile2(data) {

    var headers = [["LAST_CODE", "ExceptYN", "WayBillNo", "LogisticsCode", "CopNo", "BigBag", "PassPort", "RecName", "RecAddr", "RecPhone", "SendName", "SendAddr", "SendPhone", "TotalPrice", "Currency", "GoodsPrice", "TaxPrice", "PostPrice", "ClearanceMode", "Weight", "ItemName", "HSCODE", "ItemId", "QTY", "URL", "MNGTID", "MblNo", "PartnerCode", "RecZip", "RecMail"]];

    //for (var i = 0; i < data.length; i++) {
    //    delete data[i].id;
    //}
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

    var excel_name = getCurrentDateTime_YMD() + ".xlsx";
    var workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "SelectedData");
    XLSX.writeFile(workbook, excel_name);
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

function fileCus() {
    $(document).on("change", ".file_cus input[type=file]", function () {
        const fileName = $(this).val().split("\\").pop();
        $(this).siblings(".file_name").text(fileName || "파일을 선택해주세요.");
    });
}

function getCurrentDateTime() {
    // 현재 날짜와 시간 가져오기
    var now = new Date();

    // 년, 월, 일, 시, 분, 초, 밀리초를 각각 변수에 저장
    var year = now.getFullYear();
    var month = String(now.getMonth() + 1).padStart(2, '0'); // 월은 0부터 시작하므로 +1
    var day = String(now.getDate()).padStart(2, '0');
    var hours = String(now.getHours()).padStart(2, '0');
    var minutes = String(now.getMinutes()).padStart(2, '0');
    var seconds = String(now.getSeconds()).padStart(2, '0');
    var milliseconds = String(now.getMilliseconds()).padStart(3, '0'); // 밀리초는 3자리 수

    // 형식에 맞게 문자열로 결합
    var currentDateTime = `${year}${month}${day}${hours}${minutes}${seconds}${milliseconds}`;

    return currentDateTime;
}




