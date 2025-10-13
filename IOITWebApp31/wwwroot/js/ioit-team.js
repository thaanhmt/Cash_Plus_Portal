//thông báo
$(function () {
    toastr.options = {
        "closeButton": true
    };
    if ($('#success').val()) {
        displayMessage($('#success').val(), 'success');
    }
    if ($('#info').val()) {
        displayMessage($('#info').val(), 'info');
    }
    if ($('#warning').val()) {
        displayMessage($('#warning').val(), 'warning');
    }
    if ($('#error').val()) {
        displayMessage($('#error').val(), 'error');
    }
});
var displayMessage = function (message, msgType) {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "positionClass": "toast-top-right",
        "onClick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "8000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
    toastr[msgType](message);
};

const validateEmail = (email) => {
    return email.match(
        /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
    );
};

//liên hệ
function AddContact(k) {
    if ($("#fullname").val() == undefined || $("#fullname").val() == "") {
        toastr.warning('Bạn chưa nhập Họ và tên!', 'Thông báo');
        return;
    }
    else if ($("#phone").val() == undefined || $("#phone").val() == "") {
        toastr.warning('Bạn chưa nhập Số điện thoại!', 'Thông báo');
        return;

    }
    //else {
    //    var regex = /^(([^<>()\[\]\.,;:\s@@\"]+(\.[^<>()\[\]\.,;:\s@@\"]+)*)|(\".+\"))@@(([^<>()[\]\.,;:\s@@\"]+\.)+[^<>()[\]\.,;:\s@@\"]{2,})$/i;
    //    if (!regex.test(String($("#email").val()).toLowerCase())) {
    //        toastr.warning('Bạn chưa nhập đúng Email!', 'Thông báo');
    //        return;
    //    }
    //}
    var dateBook = "";
    if (k == 1) {
        if ($("#title").val() == undefined || $("#title").val() == "") {
            toastr.warning('Bạn chưa nhập Tiêu đề liên hệ!', 'Thông báo');
            return;
        }
        if ($("#content").val() == undefined || $("#content").val() == "") {
            toastr.warning('Bạn chưa nhập Nội dung liên hệ!', 'Thông báo');
            return;
        }
    }
    else if (k == 3) {
        if ($("#service").val() == undefined || $("#service").val() == "") {
            toastr.warning('Bạn chưa chọn dịch vụ!', 'Thông báo');
            return;
        }
        if ($("#date").val() == undefined || $("#date").val() == "") {
            toastr.warning('Bạn chưa chọn ngày khám!', 'Thông báo');
            return;
        }
        if ($("#time").val() == undefined || $("#time").val() == "") {
            toastr.warning('Bạn chưa chọn giờ khám!', 'Thông báo');
            return;
        }
        dateBook = $("#date").val() + ' ' + $("#time").val();
    }


    var obj = {
        FullName: $("#fullname").val(),
        Email: $("#email").val(),
        Title: $("#title").val(),
        Phone: $("#phone").val(),
        Note: $("#content").val(),
        ServiceId: $("#service").val(),
        Note: dateBook,
        TypeContactId: k
    };

    $.ajax({
        url: '/web/contact/SendContact',
        contentType: 'application/json; charset=utf-8',
        type: 'POST',
        dataType: 'json',
        data: JSON.stringify(obj),
        success: function (result) {
            if (result["meta"]["error_code"] == 200) {
                if (k == 1)
                    toastr.success('Thêm Liên hệ thành công!', 'Thông báo');
                else if (k == 3)
                    toastr.success('Đặt lịch khám thành công!', 'Thông báo');
                $("#fullname").val("");
                $("#phone").val("");
                $("#email").val("");
                $("#service").val("");
                $("#date").val("");
                $("#time").val("");
                $("#title").val("");
                $("#content").val("");
            }
            //else if (result["meta"]["error_code"] == 212) {
            //    toastr.error('Bạn đã gửi thông tin với số điện thoại ' + $("#phone").val(), 'Thông báo');
            //}
            else {
                toastr.error('Gửi thông tin không thành công, bạn vui lòng thực hiện lại!', 'Thông báo');
            }
        },
        error: function (xhr, status) {
            toastr.error('Thêm Liên hệ không thành công!', 'Thông báo');
        }
    });

}

//bình luận
function AddComment(id, idParent) {

    if ($("#fullname").val() == undefined || $("#fullname").val() == "") {
        toastr.warning('Bạn chưa nhập Họ và tên!', 'Thông báo');
        return;
    }
    else if ($("#email").val() == undefined || $("#email").val() == "") {
        toastr.warning('Bạn chưa nhập Email!', 'Thông báo');
        return;

    }
    else if ($("#comment-customer").val() == undefined || $("#comment-customer").val() == "") {
        toastr.warning('Bạn chưa nhập Ý kiến!', 'Thông báo');
        return;

    }
    //else {
    //    if (!validateEmail($("#email-" + idParent).val())) {
    //        toastr.warning('Bạn chưa nhập đúng Email!', 'Thông báo');
    //        return;
    //    }
    //}
    
    var obj = {
        CustomerName: $("#fullname").val(),
        TargetId: id,
        TargetType: 1,
        CommentParentId: idParent,
        Email: $("#email").val(),
        Contents: $("#comment-customer").val(),
    };

    $.ajax({
        url: '/web/comment',
        contentType: 'application/json; charset=utf-8',
        type: 'POST',
        dataType: 'json',
        data: JSON.stringify(obj),
        success: function (result) {
            if (result["meta"]["error_code"] == 200) {
                toastr.success('Gủi bình luận thành công. Bình luận đang được xét duyệt!', 'Thông báo');
                $("#list-comment").load("/Detail/ListComment?id=" + id);
                $("#fullname").val("");
                $("#email").val("");
                $("#comment-customer").val("");
            }
            else {
                toastr.error('Gửi bình luận không thành công, bạn vui lòng thực hiện lại!', 'Thông báo');
            }
        },
        error: function (xhr, status) {
            toastr.error('Thêm bình luận không thành công!', 'Thông báo');
        }
    });

}

function ReplyComment(id, status) {
    if (status) {
        $("#btshow-" + id).addClass("d-none");
        $("#bthide-" + id).removeClass("d-none");
        $("#view-" + id).removeClass("d-none");
    }
    else {
        $("#btshow-" + id).removeClass("d-none");
        $("#bthide-" + id).addClass("d-none");
        $("#view-" + id).addClass("d-none");
    }
}

function AddReplyComment(id, idParent) {

    if ($("#fullname-" + idParent).val() == undefined || $("#fullname-" + idParent).val() == "") {
        toastr.warning('Bạn chưa nhập Họ và tên!', 'Thông báo');
        return;
    }
    else if ($("#email-" + idParent).val() == undefined || $("#email-" + idParent).val() == "") {
        toastr.warning('Bạn chưa nhập Email!', 'Thông báo');
        return;

    }
    else if ($("#comment-customer-" + idParent).val() == undefined || $("#comment-customer-" + idParent).val() == "") {
        toastr.warning('Bạn chưa nhập Ý kiến!', 'Thông báo');
        return;

    }
    else {
        if (!validateEmail($("#email-" + idParent).val())) {
            toastr.warning('Bạn chưa nhập đúng Email!', 'Thông báo');
            return;
        }
    }

    var obj = {
        CustomerName: $("#fullname-" + idParent).val(),
        TargetId: id,
        TargetType: 1,
        CommentParentId: idParent,
        Email: $("#email-" + idParent).val(),
        Contents: $("#comment-customer-" + idParent).val(),
    };

    $.ajax({
        url: '/web/comment',
        contentType: 'application/json; charset=utf-8',
        type: 'POST',
        dataType: 'json',
        data: JSON.stringify(obj),
        success: function (result) {
            if (result["meta"]["error_code"] == 200) {
                toastr.success('Gủi bình luận thành công. Bình luận đang được xét duyệt!', 'Thông báo');
                $("#list-comment").load("/Detail/ListComment?id=" + id);
                $("#fullname-" + idParent).val("");
                $("#email-" + idParent).val("");
                $("#comment-customer-" + idParent).val("");
            }
            else {
                toastr.error('Gửi bình luận không thành công, bạn vui lòng thực hiện lại!', 'Thông báo');
            }
        },
        error: function (xhr, status) {
            toastr.error('Thêm bình luận không thành công!', 'Thông báo');
        }
    });

}

//load ấn phẩm
function LoadPublication(k) {

    var textS = $("#search-key").val();
    if (textS == undefined) textS = "";
    textS = removeVietnamese(textS);
    var year = $("#edition-year").val();
    if (year == undefined) year = -1;
    var topic = $("#search-scd").val();
    if (topic == undefined) topic = "";
    var author = $("#author").val();
    if (author == undefined) author = -1;
    var departments = $("#departments").val();
    if (departments == undefined) departments = -1;
    $("#list-publication").load("/Home/ListPublication?textS=" + textS +"&year="
        + year + "&topic=" + topic +"&author="
        + author + "&department=" + departments + "&p=" + k);
    $(".publication-sigle .wapper-publications").fadeIn();
    $(".publication-sigle .head-publications-single").fadeOut();
    $(".publication-sigle .section-other-publication").fadeOut();
}

function AddPublication(id) {

    if ($("#fullname").val() == undefined || $("#fullname").val() == "") {
        toastr.warning('Bạn chưa nhập Họ và tên!', 'Thông báo');
        return;
    }
    else if ($("#email").val() == undefined || $("#email").val() == "") {
        toastr.warning('Bạn chưa nhập Email!', 'Thông báo');
        return;

    }
    //else if ($("#comment-customer").val() == undefined || $("#comment-customer").val() == "") {
    //    toastr.warning('Bạn chưa nhập Ghi chú!', 'Thông báo');
    //    return;

    //}
    else if ($("#numberphone").val() == undefined || $("#numberphone").val() == "") {
        toastr.warning('Bạn chưa nhập số điện thoại!', 'Thông báo');
        return;
    }
    //else {
    //    if (!validateEmail($("#email"))) {
    //        toastr.warning('Bạn chưa nhập đúng Email!', 'Thông báo');
    //        return;
    //    }
    //}

    var obj = {
        FullName: $("#fullname").val(),
        Email: $("#email").val(),
        Title: "Đặt mua ấn phẩm " + $("#name-publication").val(),
        Phone: $("#numberphone").val(),
        Note: $("#comment-customer").val(),
        Contents: $("#coquan-customer").val(),
        Address: $("#diachi-customer").val(),
        TypeContact: 12,
        NewsId: id
    };

    $.ajax({
        url: '/web/contact/SendContact',
        contentType: 'application/json; charset=utf-8',
        type: 'POST',
        dataType: 'json',
        data: JSON.stringify(obj),
        success: function (result) {
            if (result["meta"]["error_code"] == 200) {
                toastr.success('Đặt ấn phẩm thành công!', 'Thông báo');
                $("#fullname").val("");
                $("#email").val("");
                $("#comment-customer").val("");
                $("#numberphone").val("");
                $("#coquan-customer").val("");
                $("#diachi-customer").val("");
                $(".model-open-contact").hide();
                $(".overlay_menu.active").removeClass("active");
            }
            else if (result["meta"]["error_code"] == 212) {
                toastr.error('Bạn đã đặt ấn phẩm ' + $("#name-publication").val() + '!', 'Thông báo');
            }
            else {
                toastr.error('Đặt ấn phẩm không thành công, bạn vui lòng thực hiện lại!', 'Thông báo');
            }
        },
        error: function (xhr, status) {
            toastr.error('Đặt ấn phẩm không thành công!', 'Thông báo');
        }
    });

}

//load trang câu hỏi

//load trang data tổng quan
//function LoadDataTqPage(id, k) {
//    console.log("vào đây" + id);
//    var obj = {
//        id: id,
//        p: k,
//    };
//    $("#ListDataTq").load("/Home/ListDataTq", obj);
//}
//load trang data tất cả
//function LoadDataAllPage(k) {
//    var obj = {
//        p: k,
//    };
//    $("#ListDataAll").load("/Home/ListDataAll", obj);
//}
//function LoadDataAllPage(id, k, ex, uid, aid, textS) {
//    if (textS == undefined) textS = "";

//    var obj = {
//        textS: textS,
//        id: id,
//        ex: ex,
//        uid: uid,
//        aid: aid,
//        p: k,
//    };
//    $("#ListDataAll").load("/Home/ListDataAll" + obj);
//}
function removeVietnamese(str) {
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
    str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
    str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
    str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
    str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
    str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
    str = str.replace(/Đ/g, "D");
    // Some system encode vietnamese combining accent as individual utf-8 characters
    // Một vài bộ encode coi các dấu mũ, dấu chữ như một kí tự riêng biệt nên thêm hai dòng này
    str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // ̀ ́ ̃ ̉ ̣  huyền, sắc, ngã, hỏi, nặng
    str = str.replace(/\u02C6|\u0306|\u031B/g, ""); // ˆ ̆ ̛  Â, Ê, Ă, Ơ, Ư
    // Remove extra spaces
    // Bỏ các khoảng trắng liền nhau
    str = str.replace(/ + /g, " ");
    str = str.trim();
    // Remove punctuations
    // Bỏ dấu câu, kí tự đặc biệt
    str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'|\"|\&|\#|\[|\]|~|\$|_|`|-|{|}|\||\\/g, " ");
    str = str.replace(' ', "-");
    return str;
}

function AddGopY() {
    if ($(".form-contact #fullname").val() == undefined || $(".form-contact #fullname").val() == "") {
        toastr.warning('Bạn chưa nhập Họ và tên!', 'Thông báo');
        return;
    }
    else if ($(".form-contact #email").val() == undefined || $(".form-contact #email").val() == "") {
        toastr.warning('Bạn chưa nhập Email!', 'Thông báo');
        return;

    }
    else if ($(".form-contact #numberphone").val() == undefined || $(".form-contact #numberphone").val() == "") {
        toastr.warning('Bạn chưa nhập số điện thoại!', 'Thông báo');
        return;
    }
    else if ($(".form-contact #title").val() == undefined || $(".form-contact #title").val() == "") {
        toastr.warning('Bạn chưa nhập tiêu đề!', 'Thông báo');
        return;
    }
    else if ($(".form-contact #note").val() == undefined || $(".form-contact #note").val() == "") {
        toastr.warning('Bạn chưa góp ý!', 'Thông báo');
        return;
    }
    //var file = $(".gopy-form #file")[0].files[0];
    //console.log(file);
    /*var data = new FormData();*/
    //data.append('file', file);
    event.preventDefault();
    var rcres = grecaptcha.getResponse();
    if (rcres.length) {
        var obj = {
            FullName: $(".form-contact #fullname").val(),
            Email: $(".form-contact #email").val(),
            Title: $(".form-contact #title").val(),
            Phone: $(".form-contact #numberphone").val(),
            Note: $(".form-contact #note").val(),
            TypeContact: 13,
        };
        //console.log(obj);
        $.ajax({
            url: '/web/contact/SendContactGopY',
            contentType: 'application/json; charset=utf-8',
            type: 'POST',
            dataType: 'json',
            data: JSON.stringify(obj),
            beforeSend: function () {
                $(".ajax-loading").addClass("active");
            },
            success: function (result) {
                if (result["meta"]["error_code"] == 200) {
                    toastr.success('Liên hệ đã được gửi tới ban quản trị!', 'Thông báo');
                    $(".form-contact #fullname").val("");
                    $(".form-contact #email").val("");
                    $(".form-contact #title").val("");
                    $(".form-contact #numberphone").val("");
                    $(".form-contact #note").val("");
                }
                else {
                    toastr.error('Đã có lỗi xẩy ra!', 'Thông báo');
                }
            },
            error: function (xhr, status) {
                toastr.error('Đã có lỗi xẩy ra!', 'Thông báo');
            }
        }).complete(function () {
            $('.ajax-loading').removeClass('active');
        });
        grecaptcha.reset();
    }
    else {
        toastr.warning('Bạn chưa xác thực!', 'Thông báo');
        return;
    }
}

function DownloadFileRar(id, idc, token) {
    const date = new Date();
    let time = date.getTime();
    $('.overlay_menu').addClass('active');
    $('.loadding-animate').show();
    $.ajax({
        url: '/web/S3File/downloadFiles/' + id + '/' + idc,
        method: 'GET',
        xhrFields: {
            responseType: 'blob'
        },
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", "Bearer " + token);
        },
        success: function (data) {
            $('.overlay_menu').removeClass('active');
            $('.loadding-animate').hide();
            var a = document.createElement('a');
            var url = window.URL.createObjectURL(data);
            a.href = url;
            a.download = 'dataset_' + time +'.rar';
            document.body.append(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(url);
        }
    });

}

function DownloadFile(id, idc, item) {
    $('.overlay_menu').addClass('active');
    $('.loadding-animate').show();
    $.ajax({
        url: '/web/S3File/downloadOneFile/' + id + '/' + idc + '/' + item.AttactmentId,
        method: 'GET',
        xhrFields: {
            responseType: 'blob'
        },
        success: function (data) {
            $('.overlay_menu').removeClass('active');
            $('.loadding-animate').hide();
            var a = document.createElement('a');
            var url = window.URL.createObjectURL(data);
            a.href = url;
            a.download = item.Name;
            document.body.append(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(url);
        }
    });

}

//function ViewFile(id) {

//    $("#file-" + id).removeClass("d-none");

//}
