$(function () {
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

    var dataView = new Slick.Data.DataView();

    // 컬럼
    var columns = [
        { id: "HBL_NO", name: "House Number", field: "HBL_NO", sortable: true, resizable: false ,width:120},
        { id: "SHP_NM_ENG", name: "Shipper Name", field: "SHP_NM_ENG", sortable: true, resizable: false, width: 120},
        { id: "SHP_ADDR", name: "Shipper Address", field: "SHP_ADDR", sortable: true, resizable: false, width: 120},
        { id: "CNE_NM_ENG", name: "Consignee Name", field: "CNE_NM_ENG", sortable: true, resizable: false, width: 120},
        { id: "CNE_ADDR", name: "Consignee Address", field: "CNE_ADDR", sortable: true, resizable: false, width: 120},
        { id: "NFY_NM_ENG", name: "Notify Name", field: "NFY_NM_ENG", sortable: true, resizable: false, width: 120},
        { id: "MAIN_ITEM_NM_LOC", name: "中文品名", field: "MAIN_ITEM_NM_LOC", sortable: true, resizable: false, width: 120},
        { id: "MAIN_ITEM_NM", name: "Description", field: "MAIN_ITEM_NM", sortable: true, resizable: false, width: 120},
        { id: "PKG", name: "C/T", field: "PKG", sortable: true, resizable: false, width: 120},
        { id: "PKG_UNIT_CD", name: "Pieces Unit", field: "PKG_UNIT_CD", sortable: true, resizable: false, width: 120},
        { id: "GRS_WGT", name: "W/T", field: "GRS_WGT", sortable: true, resizable: false, width: 120},
        { id: "VOL_WGT", name: "Volume", field: "VOL_WGT", sortable: true, resizable: false, width: 120},
        { id: "CUSTOMS_CI_VALUE", name: "Value(US$)", field: "CUSTOMS_CI_VALUE", sortable: true, resizable: false, width: 120},
        { id: "WH_CD", name: "WareHouse", field: "WH_CD", sortable: true, resizable: false, width: 120},
        { id: "CNTR_NO", name: "Container Number", field: "CNTR_NO", sortable: true, resizable: false, width: 120},
        { id: "CNTR_TYPE", name: "Container Type", field: "CNTR_TYPE", sortable: true, resizable: false, width: 120},
        { id: "SEAL_NO1", name: "Seal1", field: "SEAL_NO1", sortable: true, resizable: false, width: 120},
        { id: "SEAL_NO2", name: "Seal2", field: "SEAL_NO2", sortable: true, resizable: false, width: 120},
        { id: "SEAL_NO3", name: "Seal3", field: "SEAL_NO3", sortable: true, resizable: false, width: 120},
        { id: "CNTR_PKG", name: "C/T", field: "CNTR_PKG", resizable: false, width: 120},
        { id: "CNTR_PKG_UNIT_CD", name: "Pieces Unit", field: "CNTR_PKG_UNIT_CD", sortable: true, resizable: false, width: 120},
        { id: "SHP_CTRY_CD", name: "발송국가코드", field: "SHP_CTRY_CD", sortable: true, resizable: false, width: 120},
        { id: "CNE_USE_TYPE", name: "용도구분", field: "CNE_USE_TYPE", sortable: true, resizable: false, width: 120},
        { id: "SHP_TYPE", name: "화물운송주선업자부호", field: "SHP_TYPE", sortable: true, resizable: false, width: 120},
        { id: "CNE_TEL_NO", name: "수하인전화번호", field: "CNE_TEL_NO", sortable: true, resizable: false, width: 120},
        { id: "RMK", name: "비고", field: "RMK", resizable: false, width: 120},
        { id: "WT_PC", name: "중국결재/착불", field: "WT_PC", sortable: true, resizable: false, width: 120},
        { id: "SHP_ZIP_NO", name: " Zip Code", field: "SHP_ZIP_NO", sortable: true, resizable: false, width: 120},
        { id: "WURL", name: "홈페이지 URL주소", field: "WURL", sortable: true, resizable: false, width: 120},
        { id: "CUSTOM_IMP_TYPE", name: "물품타입", field: "CUSTOM_IMP_TYPE", sortable: true, resizable: false, width: 120},
        { id: "CUSTOM_EXP_HS_CD", name: "HS CODE", field: "CUSTOM_EXP_HS_CD", sortable: true, resizable: false, width: 120 },
        //{ id: "PKG_UNIT_CD", name: "사업자(주민번호)", field: "PKG_UNIT_CD" },
        { id: "INNER_PKG", name: "총수량", field: "INNER_PKG", sortable: true, resizable: false, width: 120},
        { id: "CNE_REG_NO", name: "개인통관고유부호", field: "CNE_REG_NO", sortable: true, resizable: false, width: 120},
        { id: "CNTR_AGT_CD", name: "대리점", field: "CNTR_AGT_CD", sortable: true, resizable: false, width: 120},
        { id: "WURL_ID", name: "작성자", field: "WURL_ID", sortable: true, resizable: false, width: 120},
        { id: "SHP_TEL_NO", name: "송하인전화번호", field: "SHP_TEL_NO", sortable: true, resizable: false, width: 120 },
        { id: "CNE_NM_LOC", name: "수하인 한글 상호", field: "CNE_NM_LOC", sortable: true, resizable: false, width: 120},
        { id: "CNE_ADDR_LOC", name: "수하인 한글 주소", field: "CNE_ADDR_LOC", sortable: true, resizable: false, width: 120},
        { id: "PROXY_TYPE", name: "전자상거래유형코드", field: "PROXY_TYPE", sortable: true, resizable: false, width: 120 },
        { id: "FRGN_PROXY_CD", name: "해외판매자부호", field: "FRGN_PROXY_CD", sortable: true, resizable: false, width: 120 },
        { id: "FRGN_PROXY_NM", name: "해외판매자명", field: "FRGN_PROXY_NM", sortable: true, resizable: false, width: 120 },
        { id: "BUY_PROXY_CD", name: "구매대행업자 부호", field: "BUY_PROXY_CD", sortable: true, resizable: false, width: 120},
        { id: "BUY_PROXY_NM", name: "구매대행업자명", field: "BUY_PROXY_NM", sortable: true, resizable: false, width: 120},
        { id: "SELL_PROXY_CD", name: "판매중개자부호", field: "SELL_PROXY_CD", sortable: true, resizable: false, width: 120},
        { id: "SELL_PROXY_NM", name: "판매중개자명", field: "SELL_PROXY_NM", sortable: true, resizable: false, width: 120},
        { id: "PTN_ORD_NO", name: "주문번호", field: "PTN_ORD_NO", sortable: true, resizable: false, width: 100}
    ];

    var options = {
        enableCellNavigation: true,
        enableAddRow: false,
        enableAutoResize: true,
        enableColumnReorder: true,
        forceFitColumns: true
    }


    var grid = new Slick.Grid("#myGrid", dataView, columns, options);
    grid.autosizeColumns(true);

    var pager = new Slick.Controls.Pager(dataView, grid, "#pager");
    var columnpicker = new Slick.Controls.ColumnPicker(columns, grid, options);

    grid.onSort.subscribe(function (e, args) {
        var comparer = function (a, b) {
            var x = a[args.sortCol.field], y = b[args.sortCol.field];
            return (x === y ? 0 : (x > y ? 1 : -1));
        };
        dataView.sort(comparer, args.sortAsc);
    });

    // 로딩 바 엘리먼트에 데이터 로딩 진행 상태를 반영
    dataView.onRowCountChanged.subscribe(function (e, args) {
        grid.updateRowCount();
        grid.render();
    });

    dataView.onRowsChanged.subscribe(function (e, args) {
        grid.invalidateRows(args.rows);
        grid.render();
    });

    dataView.onPagingInfoChanged.subscribe((e, pagingInfo) => {
        grid.updatePagingStatusFromView(pagingInfo);
        // show the pagingInfo but remove the dataView from the object, just for the Cypress E2E test
        delete pagingInfo.dataView;
        console.log('on After Paging Info Changed - New Paging:: ', pagingInfo);
    });

    dataView.onBeforePagingInfoChanged.subscribe((e, previousPagingInfo) => {
        // show the previous pagingInfo but remove the dataView from the object, just for the Cypress E2E test
        delete previousPagingInfo.dataView;
        console.log('on Before Paging Info Changed - Previous Paging:: ', previousPagingInfo);
    });

    grid.onBeforeAppendCell.subscribe(function (e, args) {
        return customCellValidation(grid, args);
    });

    var errorIndexes = []; // 에러가 발생한 셀의 인덱스를 저장할 배열
    

    //$("#loadExcel").on("click", async function () {
    //    var fileInput = document.getElementById("fileInput");
    //    var file = fileInput.files[0];

    //    $(".layer_zone").show();
    //    await Progress_Gaze("0");

    //    if (file) {
    //        var reader = new FileReader();
    //        reader.onload = function (e) {
    //            var data = new Uint8Array(e.target.result);
    //            var workbook = XLSX.read(data, { type: "array" });
    //            var sheetName = workbook.SheetNames[0];
    //            var sheet = workbook.Sheets[sheetName];
    //            var jsonData = XLSX.utils.sheet_to_json(sheet, { header: 1 });


    //            // Assuming the data structure in the Excel file is:
    //            // | ID | Name | Value |
    //            var gridData = jsonData.slice(7).map(function (row, index) {
    //                var zip = row[27].toString();
    //                if (!checkSearchedWord(zip)) {
    //                    errorIndexes.push(index); // 에러가 발생한 셀의 인덱스 저장
    //                }
    //                GetAddr(zip, index);


    //                //은빈대리님 작업 할 예정 20240222 
    //                //errorIndexes.forEach(function (rowIndex) {
    //                //    // 각 셀에 대해 빨간색 테두리 스타일 설정
    //                //    for (var i = 0; i < grid.getColumns().length; i++) {
    //                //        var cellId = grid.getCellNode(rowIndex, i);
    //                //        $(cellId).addClass("error-cell"); // 에러 스타일 클래스 추가
    //                //    }
    //                //});
    //                // 데이터 뿌리기
    //                //return {
    //                //    id: index + 1, HBL_NO: row[0], SHP_NM_ENG: row[1], SHP_ADDR: row[2], CNE_NM_ENG: row[3], CNE_ADDR: row[4], NFY_NM_ENG: row[5]
    //                //    , MAIN_ITEM_NM_LOC: row[6], MAIN_ITEM_NM: row[7], PKG: row[8], PKG_UNIT_CD: row[9], GRS_WGT: row[10], VOL_WGT: row[11], CUSTOMS_CI_VALUE: row[12]
    //                //    , WH_CD: row[13], CNTR_NO: row[14], CNTR_TYPE: row[15], SEAL_NO1: row[16], SEAL_NO2: row[17], SEAL_NO3: row[18], CNTR_PKG: row[19]
    //                //    , CNTR_PKG_UNIT_CD: row[20], SHP_CTRY_CD: row[21], CNE_USE_TYPE: row[22], SHP_TYPE: row[23], CNE_TEL_NO: row[24], RMK: row[25], WT_PC: row[26]
    //                //    , SHP_ZIP_NO: row[27], WURL: row[28], CUSTOM_IMP_TYPE: row[29], CUSTOM_EXP_HS_CD: row[30], INNER_PKG: row[32], CNE_REG_NO: row[33], CNTR_AGT_CD: row[34]
    //                //    , WURL_ID: row[35], SHP_TEL_NO: row[36], CNE_NM_LOC: row[37], CNE_ADDR_LOC: row[38], PROXY_TYPE: row[39], FRGN_PROXY_CD: row[40], FRGN_PROXY_NM: row[41]
    //                //    , BUY_PROXY_CD: row[42], BUY_PROXY_NM: row[43], SELL_PROXY_CD: row[44], SELL_PROXY_NM: row[45], PTN_ORD_NO: row[46]
    //                //};
    //            });

    //            await Progress_Gaze("25");
    //            $("#Addr_Confirm").attr({ src: "/Images/pro_complete.png" });
    //            $("#Addr_Div").attr({ class: "pro_stat complete" });


    //            var gridData1 = jsonData.slice(7).map(function (row, index) {
    //                var person_type = row[29].toString();
    //                var person_no = row[33].toString();
    //                var person_name = row[3].toString();
    //                var person_tel = row[24].toString().replace(/-/g, "").trim();
    //                if (person_type == "A") {
    //                    PerSonKey(person_no, person_name, person_tel, index);
    //                }

    //                return { id: index + 1, ZIP_CODE: row[27], PERSON_NO: row[33], PERSON_NAME: row[3], PERSON_TEL: row[24] };
    //            });


    //            await Progress_Gaze("50");
    //            $("#Person_Confirm").attr({ src: "/Images/pro_complete.png" });
    //            $("#Person_Div").attr({ class: "pro_stat complete" });

    //            // Set the data to the DataView
    //            dataView.setItems(gridData);
    //            dataView.setPagingOptions({
    //                pageSize: 30  // 페이지당 표시할 항목 수
    //            });
    //            // Update the grid
    //            grid.setColumns(columns);
    //            grid.invalidate();
    //            grid.render();

    
    //        };

    //        reader.readAsArrayBuffer(file);
    //    }
    //});
    $("#loadExcel").on("click", function () {
        var fileInput = document.getElementById("fileInput");
        var file = fileInput.files[0];

        //$("#ProgressPop").show();
        Progress_Gaze("0").then(() => {
            if (file) {
                var reader = new FileReader();
                reader.onload = async function (e) {
                    var data = new Uint8Array(e.target.result);
                    var workbook = XLSX.read(data, { type: "array" });
                    var sheetName = workbook.SheetNames[0];
                    var sheet = workbook.Sheets[sheetName];
                    var jsonData = XLSX.utils.sheet_to_json(sheet, { header: 1 });

                     
                    //우편번호 검증 중
                        var gridData = jsonData.slice(7).map(function (row, index) {
                            try {
                        var zip = row[27].toString();
                        if (!checkSearchedWord(zip)) {
                            errorIndexes.push(index); // 에러가 발생한 셀의 인덱스 저장
                            
                        }
                            GetAddr(zip, index);
                           
                    } catch (error) {
                        $("#Addr_Confirm").attr({ src: "/Images/pro_yet.png" });
                                $("#Addr_Div").attr({ class: "pro_stat error" });
                                throw new Exception("예외를 일부러 발생시킵니다.");
                    }
                        //getAddr(zip, index);
                        //return { id: index + 1, ZIP_CODE: row[27], PERSON_NO: row[33], PERSON_NAME: row[3], PERSON_TEL: row[24] };
                    });

                    await Progress_Gaze("25");

                    //개인통관부호 검증 중
                    var gridData1 = jsonData.slice(7).map(function (row, index) {
                        try {
                            var person_type = row[22].toString();
                            var person_no = row[33].toString();
                            var person_name = row[3].toString();
                            var person_tel = row[24].toString().replace(/-/g, "").trim();
                            if (person_type == "1") {
                                
                                    PerSonKey(person_no, person_name, person_tel, index);
                                
                            }       
                        } catch (error) {
                            $("#Person_Confirm").attr({ src: "/Images/pro_yet.png" });
                            $("#Person_Div").attr({ class: "pro_stat error" });
                            throw new Error("예외를 일부러 발생시킵니다.");
                        }
                        //return { id: index + 1, ZIP_CODE: row[27], PERSON_NO: row[33], PERSON_NAME: row[3], PERSON_TEL: row[24] };
                    });

                    await Progress_Gaze("50");
                    //통관 분류 항목 검증 중





                    await Progress_Gaze("75");
                    //주소 로직
                    var gridData3 = jsonData.slice(7).map(function (row, index) {
                        var addr = row[38].toString();
                        var a = "";

                        a = korEng4(addr);

                        //return { id: index + 1, ZIP_CODE: row[27], PERSON_NO: row[33], PERSON_NAME: row[3], PERSON_TEL: row[24], PERSON_ADDR: a };
                    });

                    await Progress_Gaze("100");

                    // Set the data to the DataView
                    dataView.setItems(gridData);
                    dataView.setPagingOptions({
                        pageSize: 30  // 페이지당 표시할 항목 수
                    });
                    // Update the grid
                    grid.setColumns(columns);
                    grid.invalidate();
                        grid.render();
                 
                };

                reader.readAsArrayBuffer(file);
            }
        });
    });




    ///////////////////////////////////////////////////////////////////////////////

    $(document).on('click', '.close', function () {
        $(".layer_zone").hide();
        $('body').removeClass('layer_on');
    });
    /*함수 구역*/
    ///////////////////////////////////////////////////////////////////////////////


    function korEng4(strKoreanIn) {
        try {
            var ChoSungTable = ["G", "KK", "N", "D", "TT", "R", "M", "B", "PP", "S", "SS", "", "J", "JJ", "CH", "K", "T", "P", "H"];
            var jungsungtable = ["A", "AE", "YA", "YAE", "EO", "E", "YEO", "YE", "O", "WA", "WAE", "OE", "YO", "U", "WO", "WE", "WI", "YU", "EU", "UI", "I"];
            var jongsungtable = ["", "K", "KK", "K", "N", "N", "N", "T", "L", "L", "L", "L", "L", "L", "L", "L", "M", "P", "P", "S", "SS", "NG", "J", "CH", "K", "T", "P", "H"];

            //var SV_Kor = strKoreanIn.replace(/\r\n/g, " ").replace(/\r/g, " ").replace(/\n/g, " ").replace(/\s+/g, " ");
            var SV_Kor = strKoreanIn.replace(/\r\n/g, " ").replace(/\r/g, " ").replace(/\n/g, " ").replace(/[^\d a-zA-Zㄱ-힣가-힣]/g, "");


            var sb = '';

            for (var i = 0; i < SV_Kor.length; i++) {
                var Code = SV_Kor.charCodeAt(i);
                if ((Code > 43 && Code < 127) || (Code > 32 && Code < 44) || Code === 32) {
                    sb += SV_Kor[i];
                } else {
                    Code -= 0xAC00;
                    if (Code < 0)
                        return false;

                    var NumChoSung = Math.floor(Code / (21 * 28));
                    Code %= (21 * 28);
                    var NumJungSung = Math.floor(Code / 28);
                    var NumJongSung = Code % 28;

                    sb += ChoSungTable[NumChoSung] + jungsungtable[NumJungSung] + jongsungtable[NumJongSung];

                    if (i !== SV_Kor.length - 1)
                        strEnglishOut = sb;
                }
            }
            console.log(sb);
            return sb;
        } catch {
            return false;
        }
    }

    function Progress_Gaze(percent) {
        return new Promise(resolve => {
            if (percent == "25") {
                $("#Addr_Confirm").attr({ src: "/Images/pro_complete.png" });
                $("#Addr_Div").attr({ class: "pro_stat complete" });
            } else if (percent == "50"){
                $("#Person_Confirm").attr({ src: "/Images/pro_complete.png" });
                $("#Person_Div").attr({ class: "pro_stat complete" });
            } else if (percent == "75") {
                $("#Test_Confirm").attr({ src: "/Images/pro_complete.png" });
                $("#Test_Div").attr({ class: "pro_stat complete" });
            } else if (percent == "100") {
                $("#Eng_Confirm").attr({ src: "/Images/pro_complete.png" });
                $("#Eng_Div").attr({ class: "pro_stat complete" });
            }


            $('.dash-progress__bar').animate({
                width: percent + "%"
            }, 900, resolve);
            $("#Progress_percent").text(percent);
        });
    }

    //미완성본
    //function PerSonKey(person_no, person_name, person_tel, index) {

    //    $.ajax({
    //        url: "/Home/TestData1"
    //        , type: "POST"
    //        , async: false
    //        , data: { person_no: person_no, person_name: person_name, person_tel: person_tel }
    //        //, dataType: "jsonp"
    //        //, crossDomain: true
    //        , success: function (result) {
    //            if ($(result).find('tCnt').text() == "0") {
    //                errorIndexes.push(index);
    //            }

    //            console.log(person_no);
    //            console.log(result);

    //        }
    //        , error: function (xhr, status, error) {
    //        }
    //    });

    //};


    //function Progress_Gaze(percent) {

    //    $('.dash-progress__bar').css('width', '0');
    //    $("#Progress_percent").text(percent);
    //    // 해당 퍼센트로 너비를 부드럽게 변경합니다.
    //    $('.dash-progress__bar').animate({
    //        width: percent +"%"
    //    }, 900);
    //}

    function customCellValidation(grid, args) {
        if (
            grid.getColumns()[args.cell].id !== 'HBL_NO' &&
            grid.getColumns()[args.cell].id !== 'SHP_NM_ENG' &&
            grid.getColumns()[args.cell].id !== 'SHP_ADDR' &&
            grid.getColumns()[args.cell].id !== 'CNE_NM_ENG' &&
            grid.getColumns()[args.cell].id !== 'CNE_ADDR' &&
            grid.getColumns()[args.cell].id !== 'MAIN_ITEM_NM' &&
            grid.getColumns()[args.cell].id !== 'PKG' &&
            grid.getColumns()[args.cell].id !== 'PKG_UNIT_CD' &&
            grid.getColumns()[args.cell].id !== 'GRS_WGT' &&
            grid.getColumns()[args.cell].id !== 'VOL_WGT' &&
            grid.getColumns()[args.cell].id !== 'CUSTOMS_CI_VALUE' &&
            grid.getColumns()[args.cell].id !== 'WH_CD' &&
            grid.getColumns()[args.cell].id !== 'CNTR_NO' &&
            grid.getColumns()[args.cell].id !== 'CNTR_TYPE' &&
            grid.getColumns()[args.cell].id !== 'SEAL_NO1' &&
            grid.getColumns()[args.cell].id !== 'CNTR_PKG' &&
            grid.getColumns()[args.cell].id !== 'SHP_CTRY_CD' &&
            grid.getColumns()[args.cell].id !== 'CNE_USE_TYPE' &&
            grid.getColumns()[args.cell].id !== 'CNE_TEL_NO' &&
            grid.getColumns()[args.cell].id !== 'CUSTOM_IMP_TYPE' &&
            grid.getColumns()[args.cell].id !== 'INNER_PKG' &&
            grid.getColumns()[args.cell].id !== 'CNE_NM_LOC'
        ) {
            return null;
        }

        if (args.value == null || args.value === "") {
            return "red";
        }

        return null;
    }

    //function GetAddr(zip, index) {
    //    return new Promise((resolve, reject) => {
    //        $.ajax({
    //            url: "https://business.juso.go.kr/addrlink/addrLinkApi.do?keyword=" + zip + "&confmKey=devU01TX0FVVEgyMDI0MDIxMzE1NDAyMjExNDUxMjc=&resultType=json",
    //            type: "POST",
    //            success: function (jsonStr) {
    //                console.log(zip);
    //                if (jsonStr != null) {
    //                    // 주소기반산업지원서비스 API 이용 토탈 카운트 0인 경우가 우편번호가 없는 경우입니다.
    //                    if (jsonStr.results.common.totalCount == "0") {
    //                        errorIndexes.push(index);
    //                        reject(new Error("예외를 일부러 발생시킵니다."));
    //                    } else {
    //                        resolve(jsonStr); // API 호출이 성공하면 결과 반환
    //                    }
    //                }
    //            },
    //            error: function (xhr, status, error) {
    //                console.log(zip);
    //                console.log(xhr);
    //                console.log(status);
    //                console.log(error);
    //                reject(new Error("API 호출 중 오류 발생"));
    //            }
    //        });
    //    });
    //}


    //// 미완성본
    //function GetAddr(zip, index) {

    //    $.ajax({
    //        url: "https://business.juso.go.kr/addrlink/addrLinkApi.do?keyword=" + zip + "&confmKey=devU01TX0FVVEgyMDI0MDIxMzE1NDAyMjExNDUxMjc=&resultType=json"
    //        , type: "POST"
    //        , async: false
    //        , success: function (jsonStr) {

    //            console.log(zip);
    //            if (jsonStr != null) {
    //                //주소기반산업지원서비스 api 이용 토탈 카운트 0인경우가 우편번호가 없는 경우입니다.
    //                if (jsonStr.results.common.totalCount == "0") {
    //                        errorIndexes.push(index)
    //                        throw new Error("예외를 일부러 발생시킵니다.");
    //                }
    //            }
    //        }
    //        , error: function (xhr, status, error) {
    //            console.log(zip);
    //            console.log(xhr);
    //            console.log(status);
    //            console.log(error);
    //            throw new Error("API 호출 중 오류 발생");
    //            $("#Addr_Confirm").attr({ src: "/Images/pro_yet.png" });
    //            $("#Addr_Div").attr({ class: "pro_stat error" });


    //        }
    //    });
    //}



    //완성
    function PerSonKey(person_no, person_name, person_tel, index) {
        // AJAX 요청을 보냄
        var xhr = new XMLHttpRequest();
        xhr.open("POST", "/Home/TestData1", false); // false로 설정하여 동기적으로 처리
        xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4) {
                if (xhr.status === 200) {
                    var result = xhr.responseText;
                    if ($(result).find('tCnt').text() == "0") {
                        errorIndexes.push(index);
                        throw new Error("예외를 일부러 발생시킵니다.");
                    }
                    console.log(person_no);
                    console.log(result);
                } else {
                    console.error('에러 발생:', xhr.status);
                    // 에러 처리 로직 추가
                    throw new Error("예외를 일부러 발생시킵니다.");
                }
            }
        };

        xhr.send("person_no=" + person_no + "&person_name=" + person_name + "&person_tel=" + person_tel);

        // 에러 발생시 처리 로직
        try {
            throw new Error("예외를 일부러 발생시킵니다.");
        } catch (error) {
            $("#Person_Confirm").attr({ src: "/Images/pro_yet.png" });
            $("#Person_Div").attr({ class: "pro_stat error" });
            throw error; // 에러를 다시 throw하여 상위 코드로 전파
        }
    }


    //완성본
    function GetAddr(zip, index) {
        // AJAX 요청을 보냄
        var xhr = new XMLHttpRequest();
        xhr.open("POST", "https://business.juso.go.kr/addrlink/addrLinkApi.do?keyword=" + zip + "&confmKey=devU01TX0FVVEgyMDI0MDIxMzE1NDAyMjExNDUxMjc=&resultType=json", false); // false로 설정하여 동기적으로 처리
        xhr.send();

        // 요청이 성공했을 때의 처리
        if (xhr.status == 200) {
            var jsonStr = JSON.parse(xhr.responseText);
            console.log(zip);
            if (jsonStr != null) {
                //주소기반산업지원서비스 api 이용 토탈 카운트 0인경우가 우편번호가 없는 경우입니다.
                if (jsonStr.results.common.totalCount == "0") {
                    errorIndexes.push(index);
                    throw new Error("예외를 일부러 발생시킵니다.");
                }
            }
        } else {
            console.error('API 호출 중 오류 발생:', xhr.status);
            // 에러 처리 로직 추가
            throw new Error("API 호출 중 오류 발생");

        }
    }


    //function getAddr(zip,index) {

    //    $.ajax({
    //        url: "/Home/TestData"
    //        , type: "POST"
    //        , async: false
    //        , data: { "zip": zip }
    //        //, dataType: "jsonp"
    //        //, crossDomain: true
    //        , success: function (jsonStr) {
    //            if (jsonStr != null) {
    //                    //주소기반산업지원서비스 api 이용 토탈 카운트 0인경우가 우편번호가 없는 경우입니다.
    //                if (jsonStr.results.common.totalCount == "0") {
    //                    errorIndexes.push(index)
    //                    }
    //                }
    //            }
    //        , error: function (xhr, status, error) {
    //            console.log(zip);
    //            console.log(xhr);
    //            console.log(status);
    //            console.log(error);

    //        }
    //    });
    //}

    //특수문자, 특정문자열(sql예약어의 앞뒤공백포함) 제거
    function checkSearchedWord(obj) {
        if (obj.length > 0) {
            //특수문자 제거
            var expText = /[%=><]/;
            if (expText.test(obj) == true) {
                //alert("특수문자를 입력 할수 없습니다.");
                obj = obj.split(expText).join("");
                return false;
            }

            if (obj.length != 5) {
                //alert("우편번호 형식에 맞게 적어주세요. ex) 숫자 5글자");
                return false;
            }

            var regex = /^\d+$/;
            if (!regex.test(obj)) {
                console.log(obj);
                //alert("숫자만 입력하십시오");
                return false;
            };

            //특정문자열(sql예약어의 앞뒤공백포함) 제거
            var sqlArray = new Array(
                //sql 예약어
                "OR", "SELECT", "INSERT", "DELETE", "UPDATE", "CREATE", "DROP", "EXEC",
                "UNION", "FETCH", "DECLARE", "TRUNCATE"
            );

            var regex;
            for (var i = 0; i < sqlArray.length; i++) {
                regex = new RegExp(sqlArray[i], "gi");

                if (regex.test(obj)) {
                    alert("\"" + sqlArray[i] + "\"와(과) 같은 특정문자로 검색할 수 없습니다.");
                    obj = obj.replace(regex, "");
                    return false;
                }
            }
        }
        return true;
    }
});
