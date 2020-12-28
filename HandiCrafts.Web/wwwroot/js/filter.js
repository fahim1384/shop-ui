//*********************************************

function log(message) {
    $("<div>").text(message).prependTo("#searchresult");
    $("#searchresult").scrollTop(0);
}

$("#searchKey").autocomplete({
    source: function (request, response) {
        $.ajax({
            url: "/Home/Search",
            //dataType: "jsonp",
            data: {
                searchtxt: request.term
            },
            success: function (data) {
                if (data.success == true) {
                    $("#searchresult").empty();
                    $.each(data.data.objList, function (e, v) {
                        $("#searchresult").append(`<div class="search-item"><a class="text-muted" href="/Product/Index?id=${v.productId}"><i class="fa fa-search"></i> ${v.productName} <div class="b-block pl-4 small color-firouzi">در دسته ${v.catProductName}</div></a></div>`);
                    });

                    if (data.data.objList.length == 0) {
                        $("#searchresult").append(`<div class="search-item text-center">موردی یافت نشد</div>`);
                    }
                } else {
                    $("#searchresult").empty();
                    $("#searchresult").append(`<div class="search-item text-center">${data.data.message}</div>`);
                }

                $("#searchresult").removeClass("d-none");
                $('#main-curtain').addClass('visible');
                $('.container-overflow-wrap').css({ 'position': 'inherit' });
                //response(data);
            }
        });
    },
    minLength: 3,
    select: function (event, ui) {
    },
    search: function (event, ui) {
        $("#searchresult").empty();
        $("#searchresult").append(`<div class="search-item text-center">درحال جستجو ...</div>`);
    },
    open: function (event, ui) {
        $("#searchresult").empty();
        $("#searchresult").append(`<div class="search-item text-center">پایان جستجو ...</div>`);
    },
    close: function () {
    }
});


$("#searchKey").focus(function () {
    $(".search-input-parentbox").css('background-color', '#fff !important');

});

$("#searchKey").blur(function () {
    $(".search-input-parentbox").css('background-color', '');
    //$("#searchresult").addClass("d-none");
});

$("#main-curtain").on("click", function () {
    $("#searchresult").empty().addClass("d-none");
    $("#main-curtain").removeClass("visible");
    //$('.container-overflow-wrap').css({ 'position': 'inherit' });
    $('.container-overflow-wrap').css({ 'position': 'absolute' });
    closeNav();
});

$("#cancelsearchbtn").on("click", function () {
    $("#searchKey").val('');
    $("#searchresult").empty().addClass("d-none");
    $("#main-curtain").removeClass("visible");
    $("#cancelsearchbtn").removeClass("d-flex").addClass("d-none");
    $('.container-overflow-wrap').css({ 'position': 'absolute' });
});

$("#searchKey").on("keypress", function (e) {
    if ($("#searchKey").val() != "") {
        if (e.keyCode == 13) {
            window.location = "/search/?q=l";
        }

        $("#cancelsearchbtn").addClass("d-flex").removeClass("d-none");
    }
});