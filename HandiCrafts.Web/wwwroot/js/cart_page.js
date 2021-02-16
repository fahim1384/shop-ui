
$("#addressBox").html(localStorage.getItem("address"));
$("#fullname").html(localStorage.getItem("fullname"));
$(".totalPayment").html(shoppingCart.totalCart());

if (shoppingCart.totalCount() == 0) {
    $(".cart-box").addClass("d-none");
    $(".cart-box-empty").removeClass("d-none");
} else {
    $(".cart-box-empty").addClass("d-none");
    $(".cart-box").removeClass("d-none");
}

$("#btnSetOffer").on("click", function () {
    if (isSetOffer == false)
        GetOfferValueByCode_UI();
    else {
        $("#messageTag").html("شما قبلا از کد تخفیف استفاده کرده اید");
        $("#messagesModal").modal("show");
    }
});

// شیوه ی پرداخت
function GetPaymentTypeList() {
    $.ajax({
        type: 'POST',
        url: "/Shoppingcart/GetPaymentTypeList_UI",
        data: {},
        success: function (result) {
            if (result.success == true) {
                $("#paymentTypesBox").empty();

                $.each(result.data?.objList, function (e, v) {
                    var check = "";
                    if (e == 0) check = "checked";
                    $("#paymentTypesBox").append(`<div class="col-6 col-sm-4 mt-10">
                                                                                            <label>
                                                                                                <input type="radio" name="optionsPaymentTypeRadios" value="${v.id}" ${check}>
                                                                                                <b>${v.title}</b>
                                                                                                <br />
                                                                                                <small class="text-muted">${v.description}</small>
                                                                                            </label>
                                                                                        </div>`);
                });
            }
        }
    });
}

// اعمال کد تخفیف
function GetOfferValueByCode_UI() {
    $.ajax({
        type: 'POST',
        url: "/Shoppingcart/GetOfferValueByCode_UI",
        data: { offerCode: $("#inputOfferCode").val() },
        success: function (result) {
            debugger
            if (result.success == true) {
                allData.total = shoppingCart.totalCart();
                $("span.totalPayment").html(comma(allData.total - allData.totalbeforeOffer + allData.totalbeforeHazinehBastehbandi));
                //if (result.data?.objList.length == 0) return false;

                objOffers = result.data?.obj;
                var total = $("span.total-cart").text().replace(/,/g, '');

                // اگر درصد مشخص باشد
                if (objOffers?.value)
                    $("#offerPrecent").html(comma(objOffers?.value));

                if (objOffers?.maximumPrice != null) {
                    /*
                     *  توجه : تخفیف ها به دوشکل هست:
                     *  1- بدون درصد  مثلا از کل فاکتور 20000 کم شود
                     *  2- با درصد مثلا 2 درصد برای فاکتور کم شود
                     *  value در اینجا هماه درصد تخفیف هست
                     */

                    if (objOffers?.value) {
                        // با درصد
                        if (objOffers?.maximumPrice < parseInt(total) * parseInt(objOffers?.value)) {
                            $("#totalBeforeOffer").html(comma(parseInt(total) - parseInt(objOffers?.maximumPrice)));
                            $(".totalPayment").html(comma(parseInt(total) - parseInt(objOffers?.maximumPrice)));
                            allData.totalbeforeOffer = parseInt(total) - parseInt(objOffers?.maximumPrice);
                            allData.offerEmalshode = parseInt(objOffers?.maximumPrice);
                        } else {
                            $("#totalBeforeOffer").html(comma(parseInt(total) - parseInt(total) * parseInt(objOffers?.value)));
                            $(".totalPayment").html(comma(parseInt(total) - parseInt(total) * parseInt(objOffers?.value)));
                            allData.totalbeforeOffer = parseInt(total) - parseInt(total) * parseInt(objOffers?.value);
                            allData.offerEmalshode = parseInt(total) * parseInt(objOffers?.value);
                        }
                    } else {
                        // بدون درصد
                        if (objOffers?.maximumPrice < parseInt(total)) {
                            $("#totalBeforeOffer").html(comma(parseInt(total) - parseInt(objOffers?.maximumPrice)));
                            $(".totalPayment").html(comma(parseInt(total) - parseInt(objOffers?.maximumPrice)));
                            allData.totalbeforeOffer = parseInt(total) - parseInt(objOffers?.maximumPrice);
                            allData.offerEmalshode = parseInt(objOffers?.maximumPrice);
                        } else {
                            $("#totalBeforeOffer").html(0);
                            $(".totalPayment").html(0);
                            allData.totalbeforeOffer = 0;
                            allData.offerEmalshode = parseInt(objOffers?.maximumPrice);
                        }
                    }
                }
                //}

                CustomerOrderPreview();

            } else {
                $("#messageTag").html("کد تخفیف شما فاقد اعتبار می باشد");
                $("#messagesModal").modal("show");
            }
        }
    });
}

// جزییات محصولات
function GetProductByIdList_UI() {
    var productIdList = [];
    var productsInCart = shoppingCart.listCart();
    $.each(productsInCart, function (e, v) {
        productIdList.push(this.prodid);
    });

    if (productIdList.length == 0) {
        $(".cart-box").addClass("d-none");
        $(".cart-box-empty").removeClass("d-none");

        return false;
    }

    $.ajax({
        type: 'POST',
        url: "/Shoppingcart/GetProductByIdList_UI",
        data: { model: productIdList },
        success: function (result) {
            debugger
            if (result.success == true) {
                if (result.data?.objList.length == 0) return false;

                console.log(result.data);

                productsdetaillist = result.data.objList;

                CustomerOrderPreview();
            }
            else {
                $("#messageTag").html(result.message);
                $("#messagesModal").modal("show");
            }
        }
    });
}

// دریافت آدرس پیشفرض و نام فرد
function GetCustomerDefultAddress_UI() {
    $.ajax({
        type: 'POST',
        url: "/Shoppingcart/GetCustomerDefultAddress_UI",
        success: function (result) {
            if (result.success == true) {
                if (result.data == null || result.data.obj == null) {
                    window.location = "/ShoppingCart/Address";
                }

                var dataInfo = result.data.obj;
                $("#addressBox").html(`${dataInfo?.provinceName ?? ''} ${dataInfo?.cityName ?? ''} ${dataInfo?.address ?? ''}، ${dataInfo?.issureMobile ?? ''}`);
                $("#fullname").html(`${dataInfo?.issureName ?? ''} ${dataInfo?.issureFamily ?? ''}`);
                localStorage.setItem("addressId", dataInfo.id);
            } else {
                window.location = "/ShoppingCart/Address";
            }
        }
    });
}

// انواع ارسال پست یا رایگان یا...
function GetPostTypeList_UI() {
    $.ajax({
        type: 'POST',
        url: "/Shoppingcart/GetPostTypeList_UI",
        success: function (result) {
            $("#posttypesBox").html('');

            if (result.success == true) {
                var options = "";
                var posttypes = result.data.objList;
                if (result.data.objList.length > 0) {
                    $.each(posttypes, function (e, v) {
                        var checked = "";
                        if (e == 0) checked = "checked='checked'";
                        $("#posttypesBox").append(`<div class="col-6 col-sm-4 mt-10">
                                                        <label>
                                                            <input type="radio" name="optionsPostTypes" value='${this.id}' price='${this.price}' ${checked}>
                                                            <b>${this.title}</b>
                                                        </label>
                                                    </div>`);
                    });

                    bindPostTypes();
                    $("input[name=optionsPostTypes]").trigger("change");
                }
            }
        }
    });
}

var bindPostTypes = () => {
    $("input[name=optionsPostTypes]").on("change", function () {
        CustomerOrderPreview();
    });
}

// نوع بسته بندی محصول
let LoadPackages = (prd, pkselected) => {
    $.ajax({
        type: 'POST',
        url: "/Product/GetProductPackingTypeList_UI",
        data: {
            productId: prd
        },
        success: function (result) {
            allData.total = shoppingCart.totalCart();
            $("span.totalPayment").html(comma(allData.total - allData.totalbeforeOffer + allData.totalbeforeHazinehBastehbandi));
            if (result.success == true && result.data != null) {
                var options = "";
                $("select.packageComboInCart[data-pkCmbId='" + prd + "']").empty();//.append('<option value="">انتخاب ..</option>');
                if (result.data.objList.length == 0) $("select.packageComboInCart[data-pkCmbId='" + prd + "']").attr("disabled", true);

                $.each(result.data.objList, function (e2, v2) {
                    console.log("prd:", prd);
                    console.log("this.packinggTypeId:", this.id/*packinggTypeId*/);
                    var selected = "";
                    var pkhazineh = 0;
                    if (result.data.objList.length == 1) {
                        selected = "selected='selected'";

                        shoppingCart.changeSelectedPackage(prd, this.id/*packinggTypeId*/);

                        $("input[data-pkinputhide='" + prd + "']").val(this.id/*packinggTypeId*/);
                        $("input[data-pkinputhide='" + prd + "']").attr("data-hazineh", this?.price ?? 0);

                        $("#packingprice").html(comma(this?.price ?? 0));
                        allData.totalbeforeHazinehBastehbandi = parseInt(allData.totalbeforeOffer) + parseInt(this?.price ?? 0);

                        $("span.totalPayment").html(comma(allData.total - allData.totalbeforeOffer + allData.totalbeforeHazinehBastehbandi));
                    }

                    if (pkselected != undefined && pkselected != null) {
                        selected = "selected='selected'";

                        shoppingCart.changeSelectedPackage(prd, this.id/*packinggTypeId*/);

                        $("input[data-pkinputhide='" + prd + "']").val(this.id/*packinggTypeId*/);
                        $("input[data-pkinputhide='" + prd + "']").attr("data-hazineh", this?.price ?? 0);

                        $("#packingprice").html(comma(this?.price ?? 0));
                        allData.totalbeforeHazinehBastehbandi = parseInt(allData.totalbeforeOffer) + parseInt(this?.price ?? 0);

                        $("span.totalPayment").html(comma(allData.total - allData.totalbeforeOffer + allData.totalbeforeHazinehBastehbandi));
                    }

                    var opTitle = this.packingTypeName != undefined ? this.packingTypeName : "عادی";
                    pkhazineh = this?.price ?? 0;

                    options += `<option value='${this.id/*packinggTypeId*/}' ${selected} hazineh='${pkhazineh}'>${opTitle}</option>`;
                });

                $("select.packageComboInCart[data-pkCmbId=" + prd + "]").append(options);
            }
        }
    });
};

function productsPackageListLoad() {
    var productsInCart = shoppingCart.listCart();

    $.each(productsInCart, function (e, vx) {
        LoadPackages(vx.prodid, $("input[data-pkinputhide='" + vx.prodid + "']").val());
    });
}

$("select.packageComboInCart").off("click");
$("select.packageComboInCart").on("change", function () {

    if ($(this).val() != "") {
        allData.total = shoppingCart.totalCart();
        $("span.totalPayment").html(comma(allData.total - allData.totalbeforeOffer + allData.totalbeforeHazinehBastehbandi));
        shoppingCart.changeSelectedPackage($(this).attr("data-pkCmbId"), $(this).val());

        $("input[data-pkinputhide='" + $(this).attr("data-pkCmbId") + "']").val($(this).val());
        $("input[data-pkinputhide='" + $(this).attr("data-pkCmbId") + "']").attr("data-hazineh", $(this).find('option:selected').attr("hazineh"));

        $("#packingprice").html(comma($(this).find('option:selected').attr("hazineh")));
        allData.totalbeforeHazinehBastehbandi = parseInt(allData.totalbeforeOffer) + parseInt($(this).find('option:selected').attr("hazineh"));

        $("span.totalPayment").html(comma(allData.total - allData.totalbeforeOffer + allData.totalbeforeHazinehBastehbandi));

        CustomerOrderPreview();

    }
});

function findofferid(productid) {
    var _offerid = null;
    debugger
    for (var i = 0; i < productsdetaillist.length; i++) {
        if (productsdetaillist[i] == productid) {
            _offerid = productsdetaillist[i]?.offerId ?? null;
        }
    }

    return _offerid;
}

$("#btnInsertOrder").on("click", function () {
    InsertCustomerOrder_UI();
});

// ثبت سفارش
function InsertCustomerOrder_UI() {
    if (localStorage.getItem("addressId") == null || localStorage.getItem("addressId") == undefined) {
        $("#messageTag").html("شما آدرسی جهت ارسال انتخاب نکرده اید لطفا آنرا انتخاب نمایید");
        $("#messagesModal").modal("show");
        return false;
    }

    var posttype = $("input[name=optionsPostTypes]:checked").val();

    var model = {
        PaymentTypeId: $("input[name=optionsPaymentTypeRadios]:checked").val(),
        OfferId: objOffers?.offerId ?? null,
        CustomerAddressId: localStorage.getItem("addressId"),
        CustomerDescription: null,
        ProductList: [],
        PostTypeId: posttype != undefined ? posttype : 1
    };

    $.each(shoppingCart.listCart(), function (e, v) {
        var pkg = null;
        if ($("input[data-pkinputhide='" + v.prodid + "']").val() == "" || $("input[data-pkinputhide='" + v.prodid + "']").val() == "undefined") { } else {
            pkg = $("input[data-pkinputhide='" + v.prodid + "']").val();
        }

        var productObj = {
            ProductId: v.prodid,
            Count: v.count,
            ColorId: null,
            PackingTypeId: pkg,//v.packageId,
            OfferId: findofferid(v.prodid)
        };

        model.ProductList.push(productObj);
    });

    $.ajax({
        type: 'POST',
        url: "/Shoppingcart/InsertCustomerOrder_UI",
        data: { model: model },
        success: function (result) {
            
            if (result.success == true) {
                console.log(result.data);
                orderResltObj = result.data.obj;
                //BankUrl//CustomerOrderId/OrderNo/PostPrice/RedirectToBank/

                if (orderResltObj.redirectToBank == false) {
                    window.location = "/order/OnlinePaymentResultSuccess";
                } else {
                    window.location = orderResltObj.bankUrl;
                }
            }
            else {
                $("#messageTag").html(result.message);
                $("#messagesModal").modal("show");
            }
        }
    });
}

// اطلاعات سبد خرید و بروزرسانی قیمت ها
function CustomerOrderPreview() {
    console.log("CustomerOrderPreview is called ...");
    debugger;
    //allData.total = shoppingCart.totalCart();
    //$("span.totalPayment").html(comma(allData.total - allData.totalbeforeOffer + allData.totalbeforeHazinehBastehbandi));
    if (localStorage.getItem("addressId") != null) {
        var posttype = $("input[name=optionsPostTypes]:checked").val();

        if (posttype == undefined) return false;

        var model = {
            PaymentTypeId: $("input[name=optionsPaymentTypeRadios]:checked").val(),
            OfferId: objOffers?.offerId ?? null,
            CustomerAddressId: localStorage.getItem("addressId"),
            CustomerDescription: null,
            ProductList: [],
            PostTypeId: posttype != undefined ? posttype : null
        };

        $.each(shoppingCart.listCart(), function (e, v) {
            var pkg = null;
            if ($("input[data-pkinputhide='" + v.prodid + "']").val() == "" || $("input[data-pkinputhide='" + v.prodid + "']").val() == "undefined") { } else {
                pkg = $("input[data-pkinputhide='" + v.prodid + "']").val();
            }

            var productObj = {
                ProductId: v.prodid,
                Count: v.count,
                ColorId: null,
                PackingTypeId: pkg,//v.packageId,
                OfferId: findofferid(v.prodid)
            };

            model.ProductList.push(productObj);
        });

        $.ajax({
            type: 'POST',
            url: "/Shoppingcart/CustomerOrderPreview",
            data: { model: model },
            success: function (result) {
                debugger
                if (result.success == true) {
                    console.log(result.data);
                    var obj = result.data.obj;

                    //$("#offerPrecent").html(result.data.?.value);
                    $("#totalBeforeOffer").html(comma(obj.orderPrice));
                    //$("#packingprice").html(this?.price ?? 0);
                    $("span.totalPayment").html(comma(obj.finalPrice));
                    $("span.hazine-ersal").html(comma(obj.postServicePrice));
                    $("#offerPrecent").val(obj.offerValue);
                }
                else {
                    $("#messageTag").html(result.message);
                    $("#messagesModal").modal("show");
                }
            }
        });
    }
}

//============ Load ============

var orderResltObj = {};
var isSetOffer = false;
var objOffers = {};
var productsdetaillist = [];
var allData = {
    total: shoppingCart.totalCart(),
    totalbeforeOffer: 0,
    offerEmalshode: 0,
    hazinehBastehbandi: 0,
    totalbeforeHazinehBastehbandi: 0
}

if (shoppingCart.totalCount() != 0) {
    GetCustomerDefultAddress_UI();
    GetProductByIdList_UI();
    GetPostTypeList_UI();
    GetPaymentTypeList();

    productsPackageListLoad();
}

                        //}
