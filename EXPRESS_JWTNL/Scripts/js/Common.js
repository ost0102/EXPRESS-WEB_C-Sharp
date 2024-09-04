$(document).on('click', '.pop', function () {
    $('body').addClass('layer_on');
})

$(document).on('click', '.hamburger_box > img', function () {
	$(this).parent('.hamburger_box').addClass('on');
	$(this).css('left', 'unset'); /* 'left' 속성을 '300px'로 설정 */
	$(this).css('right', '-13px');
	$('.container').css('margin-left', '200px');
	$('.slide_menu').fadeIn(300);
	$('.hide_title').fadeIn(300);
	$('.menu-list').show();
});

$(document).on('click', '.hamburger_box.on > img', function () {
	$(this).parent('.hamburger_box.on').removeClass('on');
	/*$(this).parent('.hamburger_box').removeClass('on');*/
	$(this).css('left', '50%');
	$(this).css('right', 'unset');
	$('.container').css('margin-left', '40px');
	$('.slide_menu').fadeOut(300);
	$('.hide_title').fadeOut(300);
	$('.menu-list').hide();
});


$(document).on('click', '.menu-list__title', function () {
	$(this).siblings('.menu-list__sub').slideDown();
	$(this).addClass('on');
	$(this).children('.asc').hide();
	$(this).children('.desc').show();
});

$(document).on('click', '.menu-list__title.on', function () {
	$(this).siblings('.menu-list__sub').slideUp();
	$(this).removeClass('on');
	$(this).children('.asc').show();
	$(this).children('.desc').hide();
});


/* 레이어팝업 */
var layerPopup = function (obj) {
	var $laybtn = $(obj),
		$glayer_zone = $(".layer_zone");
	if ($glayer_zone.length === 0) { return; }
	//$glayer_zone.hide()
	$("body").addClass("layer_on");
	$laybtn.fadeIn(200);

	$glayer_zone.on("click", ".close:not(.off)", function (e) {
		var $this = $(this),
			$t_layer = $this.parents(".layer_zone");
		$("body").removeClass("layer_on");
		$t_layer.fadeOut(300);
	});
};

/* 레이어팝업 */
var layerPopup = function (obj) {
	var $laybtn = $(obj),
		$glayer_zone = $(".layer_zone");
	if ($glayer_zone.length === 0) { return; }
	//$glayer_zone.hide()
	$("body").addClass("layer_on");
	$laybtn.fadeIn(200);

	$glayer_zone.on("click", ".close:not(.off)", function (e) {
		var $this = $(this),
			$t_layer = $this.parents(".layer_zone");
		$("body").removeClass("layer_on");
		$t_layer.fadeOut(300);
	});
};

/* 레이어팝업 닫기 */
var layerClose = function (obj) {
	var $laybtn = $(obj);
	$("body").removeClass("layer_on");
	$laybtn.hide();
};



//Null 값 0으로
function _fnToZero(data) {
	// undifined나 null을 null string으로 변환하는 함수. 
	if (String(data) == 'undefined' || String(data) == 'null' || String(data) == '' || String(data) == 'NaN') {
		return '0'
	} else {
		return data
	}
}

function _fnSetCookie(name, value, hours) {
	if (hours) {
		var date = new Date();
		date.setTime(date.getTime() + (hours * 60 * 60 * 1000));
		var expires = "; expires=" + date.toGMTString();
	} else {
		var expires = "";
	}
	document.cookie = name + "=" + value + expires + "; path=/";
}

function _fnDelCookie(cookie_name) {
	_fnSetCookie(cookie_name, "", "-1");
}

function _fnGetCookie(cookie_name) {
	var x, y;
	var val = document.cookie.split(';');

	for (var i = 0; i < val.length; i++) {
		x = val[i].substr(0, val[i].indexOf('='));
		y = val[i].substr(val[i].indexOf('=') + 1);
		x = x.replace(/^\s+|\s+$/g, ''); // 앞과 뒤의 공백 제거하기
		if (x == cookie_name) {
			return unescape(y); // unescape로 디코딩 후 값 리턴
		}
	}
}
//날짜 플러스
function _fnPlusDate(date) {
	var nowDate = new Date();
	var weekDate = nowDate.getTime() + (date * 24 * 60 * 60 * 1000);
	nowDate.setTime(weekDate);

	var weekYear = nowDate.getFullYear();
	var weekMonth = nowDate.getMonth() + 1;
	var weedDay = nowDate.getDate();

	if (weekMonth < 10) { weekMonth = "0" + weekMonth; }
	if (weedDay < 10) { weedDay = "0" + weedDay; }
	var result = weekYear + "-" + weekMonth + "-" + weedDay;
	return result;
}

function _fnMakeJson(data) {
	if (data != undefined) {
		var str = JSON.stringify(data);
		if (str.indexOf("[") == -1) {
			str = "[" + str + "]";
		}
		return str;
	}
}

function getFormattedDateAndRandomNumbers(date) {
	const now = new Date();

	// 날짜와 시간 포맷팅
	const year = now.getFullYear();
	const month = String(now.getMonth() + 1).padStart(2, '0'); // getMonth()는 0부터 시작
	const day = String(now.getDate()).padStart(2, '0');
	const hour = String(now.getHours()).padStart(2, '0');
	const minute = String(now.getMinutes()).padStart(2, '0');
	const second = String(now.getSeconds()).padStart(2, '0');

	// 날짜와 시간을 문자열로 결합
	const dateTime = `${year}${month}${day}${hour}${minute}${second}`;

	// 랜덤 숫자 3개 생성
	const randomNumbers = Math.floor(Math.random() * 1000); // 0에서 999까지의 랜덤 숫자

	// 결과 문자열 생성
	const result = dateTime + randomNumbers;

	return result;
}

function GetDateTime() {
	// 현재 날짜와 시간 가져오기
	var currentDate = new Date();

	// 날짜 및 시간의 각 부분 가져오기
	var year = currentDate.getFullYear();
	var month = ('0' + (currentDate.getMonth() + 1)).slice(-2); // 월은 0부터 시작하므로 1을 더하고 두 자리로 변환
	var day = ('0' + currentDate.getDate()).slice(-2); // 일을 두 자리로 변환
	var hours = ('0' + currentDate.getHours()).slice(-2); // 시를 두 자리로 변환
	var minutes = ('0' + currentDate.getMinutes()).slice(-2); // 분을 두 자리로 변환
	var seconds = ('0' + currentDate.getSeconds()).slice(-2); // 초를 두 자리로 변환

	// 형식화된 날짜와 시간 출력 (년월일시분초)
	var formattedDateTime = year + '-' + month + '-' + day + ' ' + hours + ':' + minutes + ':' + seconds;

	return formattedDateTime;
}
function GetDateTime_INS_YMD() {
	// 현재 날짜 가져오기
	var currentDate = new Date();

	// 날짜의 각 부분 가져오기
	var year = currentDate.getFullYear();
	var month = ('0' + (currentDate.getMonth() + 1)).slice(-2); // 월은 0부터 시작하므로 1을 더하고 두 자리로 변환
	var day = ('0' + currentDate.getDate()).slice(-2); // 일을 두 자리로 변환

	// 형식화된 날짜 출력 (년월일)
	var formattedDate = year + month + day;

	return formattedDate;
}

//현재 시간값 가져오기(초까지)
function getCurrentDateTime_YMD() {
	// 현재 날짜와 시간 가져오기
	var now = new Date();

	// 년, 월, 일, 시, 분, 초, 밀리초를 각각 변수에 저장
	var year = now.getFullYear();
	var month = String(now.getMonth() + 1).padStart(2, '0'); // 월은 0부터 시작하므로 +1
	var day = String(now.getDate()).padStart(2, '0');
	var hours = String(now.getHours()).padStart(2, '0');
	var minutes = String(now.getMinutes()).padStart(2, '0');
	var seconds = String(now.getSeconds()).padStart(2, '0');

	// 형식에 맞게 문자열로 결합
	var currentDateTime = `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;

	return currentDateTime;
}

function GetDateTime_INS_HM() {
	// 현재 시간 가져오기
	var currentDate = new Date();

	// 시간의 각 부분 가져오기
	var hours = ('0' + currentDate.getHours()).slice(-2); // 시를 두 자리로 변환
	var minutes = ('0' + currentDate.getMinutes()).slice(-2); // 분을 두 자리로 변환
	var seconds = ('0' + currentDate.getSeconds()).slice(-2); // 초를 두 자리로 변환


	// 형식화된 시간 출력 (시분)
	var formattedTime = hours + minutes + seconds;

	return formattedTime;
}


function formattingDateKOR(time) {
	// Date 객체로 파싱
	var date = new Date(
		parseInt(time.substring(0, 4)),  // 년도
		parseInt(time.substring(4, 6)) - 1, // 월 (0부터 시작하므로 1 빼줌)
		parseInt(time.substring(6, 8)),  // 일
		parseInt(time.substring(8, 10)), // 시간
		parseInt(time.substring(10, 12)), // 분
		parseInt(time.substring(12, 14)) // 초
	);

	// 원하는 형식으로 포맷팅
	var formattedDate = date.getFullYear() + "년 " +
		(date.getMonth() + 1) + "월 " +
		date.getDate() + "일 " +
		date.getHours() + "시 " +
		date.getMinutes() + "분 " +
		date.getSeconds() + "초";

	return formattedDate;
}

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
function formatDateTimeToNumber(dateTimeString) {
	// 날짜와 시간을 Date 객체로 변환
	var dateTime = new Date(dateTimeString);

	// 년, 월, 일, 시, 분, 초를 가져와서 2자리 숫자로 만듭니다.
	var year = dateTime.getFullYear().toString().padStart(4, '0');
	var month = (dateTime.getMonth() + 1).toString().padStart(2, '0');
	var day = dateTime.getDate().toString().padStart(2, '0');
	var hours = dateTime.getHours().toString().padStart(2, '0');
	var minutes = dateTime.getMinutes().toString().padStart(2, '0');
	var seconds = dateTime.getSeconds().toString().padStart(2, '0');

	// 년월일시분초를 조합하여 반환
	return year + month + day + hours + minutes + seconds;
}


function formatDateTimeToNumberGolobal() {
	// 날짜와 시간을 Date 객체로 변환
	var dateTime = new Date();

	// 년, 월, 일, 시, 분, 초를 가져와서 2자리 숫자로 만듭니다.
	var year = dateTime.getFullYear().toString().padStart(4, '0');
	var month = (dateTime.getMonth() + 1).toString().padStart(2, '0');
	var day = dateTime.getDate().toString().padStart(2, '0');
	var hours = dateTime.getHours().toString().padStart(2, '0');
	var minutes = dateTime.getMinutes().toString().padStart(2, '0');
	var seconds = dateTime.getSeconds().toString().padStart(2, '0');

	var rtnTime = year + month + day + hours + minutes + seconds + "+0900";

	// 년월일시분초를 조합하여 반환
	return rtnTime;
}

//역 변환 데이터
function formatDateTime(dateTimeString) {
	// 날짜와 시간을 Date 객체로 변환
	var dateTime = new Date(dateTimeString);

	// 년, 월, 일, 시, 분, 초를 가져와서 2자리 숫자로 만듭니다.
	var year = dateTime.getFullYear().toString().padStart(4, '0');
	var month = (dateTime.getMonth() + 1).toString().padStart(2, '0');
	var day = dateTime.getDate().toString().padStart(2, '0');
	var hours = dateTime.getHours().toString().padStart(2, '0');
	var minutes = dateTime.getMinutes().toString().padStart(2, '0');
	var seconds = dateTime.getSeconds().toString().padStart(2, '0');

	// 년월일시분초를 조합하여 반환
	return year + month + day + hours + minutes + seconds;
}



function _fnToNull(data) {
	// undifined나 null을 null string으로 변환하는 함수. 
	if (String(data) == 'undefined' || String(data) == 'null') {
		return ''
	} else {
		return data
	}
}

//#region 슬릭그리드 관련 Function
// 체크된 데이터 가져오기
function getSelectedData() {
	var selectedRows = grid.getSelectedRows(); // 선택된 행의 인덱스 가져오기
	var selectedData = [];

	// 선택된 행의 데이터 가져오기
	for (var i = 0; i < selectedRows.length; i++) {
		var item = dataView.getItem(selectedRows[i]); // 선택된 행의 데이터 객체 가져오기
		selectedData.push(item); // 선택된 데이터 배열에 추가
	}

	return selectedData;
}

//#endregion



function _fnFormateTime(sDate, len) {
	var strTime = sDate;

	if (strTime.toString().length < len) {
		strTime =  strTime.padEnd(len, '0');
    }

	return strTime
}