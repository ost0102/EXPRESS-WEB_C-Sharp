$(function () {
    //로그인 아이디 저장 체크가 되어있을 시 데이터 넣는 로직
    var userInputId = _fnGetCookie("Prime_CK_USR_ID_REMEMBER_JWTNL");
    if (_fnToNull(userInputId) != "") {
        $("#USR_ID").val(userInputId);
        $("#company_login_keep").attr("checked", true);
    }

    $("#USR_ID").focus();

    $("#USR_ID").keyup(function (e) {
        if (e.keyCode == 13) {
            $("#USR_PWD").focus();
        }
    });

    $("#USR_PWD").keyup(function (e) {
        if (e.keyCode == 13) {
            $(".login-btn").click();
        }
    });
});

$(document).on('click', '.alignBox', function () {
    $(this).children($('.checkbox')).prop('checked', false);
    $(this).children($('.checkbox')).prop('checked', true);
});

$(document).on('click', '#AlertClose', function () {
    $(this).parents('.layer_zone').hide();
});



$(document).on('click', '.login-btn', function () {
    if (_fnToNull($("#USR_ID").val()) == "") {
        $(".warn_id").show();
        $(".warn_pw").hide();
        return false;
    }

    if (_fnToNull($("#USR_PWD").val()) == "") {
        $(".warn_id").hide();
        $(".warn_pw").show();
        return false;
    }
    var objJsonData = new Object();
    objJsonData.USR_ID = $("#USR_ID").val();
    objJsonData.PSWD = $("#USR_PWD").val();

    $.ajax({
        type: "POST",
        url: "/Login/fnLogin",
        async: false,
        dataType: "json",
        data: { "vJsonData": _fnMakeJson(objJsonData) },
        success: function (result) {

            if (JSON.parse(result).Result[0]["trxCode"] == "Y") {

                if (_fnToNull(JSON.parse(result).Table[0].USR_ID) != "") {
                    if ($('input[name=login_keep]')[0].checked) {
                        _fnSetCookie("Prime_CK_USR_ID_REMEMBER_JWTNL", JSON.parse(result).Table[0].USR_ID, "168");
                    } else {
                        _fnDelCookie("Prime_CK_USR_ID_REMEMBER_JWTNL");
                    }

                    $.ajax({
                        type: "POST",
                        url: "/Login/SaveLogin",
                        async: true,
                        data: { "vJsonData": _fnMakeJson(JSON.parse(result)) },
                        success: function (result, status, xhr) {
                            if (_fnToNull(result) == "Y") {
                                window.location = window.location.origin + "/Order/Order";
                            } else {
                                window.location = window.location.origin;
                            }
                        }
                    });
                } else {
                    _fnAlertMsg("로그인 정보가 없습니다. 다시 시도해주세요.");
                }
            }
            if (JSON.parse(result).Result[0]["trxCode"] == "N") {
                _fnAlertMsg("로그인 정보가 없습니다. 다시 시도해주세요.");
            }
        }, error: function (xhr, status, error) {
            status = true;
            console.log(error);
        }
    });
});

