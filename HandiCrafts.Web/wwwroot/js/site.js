// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(".select2").select2({ width: 'resolve' });

//localStorage.setItem("token", `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJIYW5kQ3JhZnRTZXJ2ZXJBY2Nlc3NUb2tlbiIsImp0aSI6ImI1NDVmMzA0LTIxZjUtNGZmNi1iOWM1LWM0YjMxNTZmMjE1MSIsImlhdCI6IjEwLzE2LzIwMjAgNTozNzowNiBQTSIsIklkIjoiMyIsIkZ1bGxOYW1lIjoi2KfYs9mF2KfYuduM2YQg2YjYp9it2K_bjCIsIlVzZXJOYW1lIjoiMTQ4MjcyNTc5IiwiRW1haWwiOiJ2YWhlZGkuZXNtYWVpbEBnbWFpbC5jb20iLCJyb2xlIjoiMiIsImV4cCI6MTYwMjk1NjIyNiwiaXNzIjoiSGFuZENyYWZ0QXV0aGVudGljYXRpb25TZXJ2ZXIiLCJhdWQiOiJIYW5kQ3JhZnRDbGllbnQifQ.7wkPFeZUS8syXPyWuhioU5G83oaeLOhHMvrkVuArnlg`)


var shoppingCart = (function () {
    // =============================
    // Private methods and propeties
    // =============================
    cart = [];

    // Constructor
    function Item(name, price, count, prodid, prodimage, packageId, mellicode/*,colorId,offerId*/) {
        this.name = name;
        this.price = price;
        this.count = count;
        this.prodid = prodid;
        this.prodimage = prodimage;
        this.mellicode = mellicode;

        //this.colorId = colorId;
        this.packageId = packageId;
        //this.offerId = offerId;
    }

    // Save cart
    function saveCart() {
        sessionStorage.setItem('shoppingCart', JSON.stringify(cart));

        if (shoppingCart.totalCount() == 0) {
            $(".cart-box").addClass("d-none");
            $(".cart-box-empty").removeClass("d-none");
        } else {
            $(".cart-box-empty").addClass("d-none");
            $(".cart-box").removeClass("d-none");

            try {
                CustomerOrderPreview();
            } catch (e) {
                ;
            }
        }
    }

    // Load cart
    function loadCart() {
        cart = JSON.parse(sessionStorage.getItem('shoppingCart'));
    }
    if (sessionStorage.getItem("shoppingCart") != null) {
        loadCart();
    }


    // =============================
    // Public methods and propeties
    // =============================
    var obj = {};

    // Add to cart
    obj.addItemToCart = function (name, price, count, prodid, prodimage, packageId, mellicode/*, colorId, offerId*/) {
        for (var item in cart) {
            if (cart[item].prodid === prodid) {
                cart[item].count++;
                saveCart();
                return;
            }
        }
        var item = new Item(name, price, count, prodid, prodimage, packageId, mellicode/*, colorId, offerId*/);
        cart.push(item);
        saveCart();
    }
    // Set count from item
    obj.setCountForItem = function (prodid, count) {
        for (var i in cart) {
            if (cart[i].prodid === prodid) {
                cart[i].count = count;
                break;
            }
        }
    };
    // Remove item from cart
    obj.removeItemFromCart = function (prodid) {
        for (var item in cart) {
            if (cart[item].prodid === prodid) {
                cart[item].count--;
                if (cart[item].count === 0) {
                    cart.splice(item, 1);
                }
                break;
            }
        }
        saveCart();
    }

    // Remove all items from cart
    obj.removeItemFromCartAll = function (prodid) {
        for (var item in cart) {
            if (cart[item].prodid === prodid) {
                cart.splice(item, 1);
                break;
            }
        }
        saveCart();
    }

    // Set count from item
    obj.changeSelectedPackage = function (prodid, packageId) {
        for (var i in cart) {
            if (cart[i].prodid === prodid) {
                cart[i].packageId = packageId;
                break;
            }
        }
    };

    // Clear cart
    obj.clearCart = function () {
        cart = [];
        saveCart();
    }

    // Count cart 
    obj.totalCount = function () {
        var totalCount = 0;
        for (var item in cart) {
            totalCount += cart[item].count;
        }
        return totalCount;
    }

    // Total cart
    obj.totalCart = function () {
        var totalCart = 0;
        for (var item in cart) {
            totalCart += cart[item].price * cart[item].count;
        }
        return Number(totalCart.toFixed(0));
    }

    // List cart
    obj.listCart = function () {
        var cartCopy = [];
        for (i in cart) {
            item = cart[i];
            itemCopy = {};
            for (p in item) {
                itemCopy[p] = item[p];

            }
            itemCopy.total = Number(item.price * item.count).toFixed(0);
            cartCopy.push(itemCopy)
        }
        return cartCopy;
    }

    // cart : Array
    // Item : Object/Class
    // addItemToCart : Function
    // removeItemFromCart : Function
    // removeItemFromCartAll : Function
    // clearCart : Function
    // countCart : Function
    // totalCart : Function
    // listCart : Function
    // saveCart : Function
    // loadCart : Function
    return obj;
})();


// *****************************************
// Triggers / Events
// ***************************************** 

var cartEvents = (function () {

    return {
        init() {

            // Add item
            $('.add-to-cart').off("click");
            $('.add-to-cart').click(function (event) {

                event.preventDefault();
                var prodid = $(this).data('product-id');
                var prodimage = $(this).data('product-image');
                var name = $(this).data('product-name');
                var price = Number($(this).data('product-price'));
                var mellicode = $(this).data('product-mellicode');

                //var colorId = $(this).data('product-colorId');
                var packageId = $(this).data('product-packageId');
                //var offerId = $(this).data('product-offerId');

                shoppingCart.addItemToCart(name, price, 1, prodid, prodimage, packageId, mellicode/*, colorId, offerId*/);
                displayCart();

                $("#shoppingCartModal").modal();
            });

            // Clear items
            $('.clear-cart').off("click");
            $('.clear-cart').click(function () {
                shoppingCart.clearCart();
                displayCart();
            });


            //function displayCart() {
            //    var cartArray = shoppingCart.listCart();
            //    var output = "";
            //    for (var i in cartArray) {
            //        output += "<tr>"
            //            + "<td>" + cartArray[i].name + "</td>"
            //            + "<td>(" + cartArray[i].price + ")</td>"
            //            + "<td><div class='input-group'><button class='minus-item input-group-addon btn btn-primary' data-name=" + cartArray[i].name + ">-</button>"
            //            + "<input type='number' class='item-count form-control' data-name='" + cartArray[i].name + "' value='" + cartArray[i].count + "'>"
            //            + "<button class='plus-item btn btn-primary input-group-addon' data-name=" + cartArray[i].name + ">+</button></div></td>"
            //            + "<td><button class='delete-item btn btn-danger' data-name=" + cartArray[i].name + ">X</button></td>"
            //            + " = "
            //            + "<td>" + cartArray[i].total + "</td>"
            //            + "</tr>";
            //    }
            //    $('.show-cart').html(output);
            //    $('.total-cart').html(shoppingCart.totalCart());
            //    $('.total-count').html(shoppingCart.totalCount());
            //}

            function displayCart() {
                var cartArray = shoppingCart.listCart();
                var output = "";
                for (var i in cartArray) {
                    output += `<tr>
            <td class="text-center"><a href="/Product/index?id=${cartArray[i].prodid}"><img src='${cartArray[i].prodimage}' class='img-fluid width-100px border-default border-radius-5 p-2px' /></a></td>
            <td data-title="نام محصول:" class="text-left"><a href="/Product/index?id=${cartArray[i].prodid}"><h5 class="prod-name-cart">${cartArray[i].name}</h5><span class="mellicode">شناسنامه یکتا: ${cartArray[i].mellicode ? cartArray[i].mellicode : '0000'}</span></a></td>
            <td data-title="قیمت:"> <h5 class="prod-name-cart">${comma(cartArray[i].price)}</h5></td>
            <td data-title="تعداد:"><form class="form-inline mt-2" dir="rtl">
                        <div class="spinner border-radius-5 m-auto">
                            <span class="fa fa-plus plus-item" data-product-name="${ cartArray[i].name}" data-product-id="${cartArray[i].prodid}" data-product-price="${cartArray[i].price}"></span>
                            <input type="text" name="quantity" readonly data-product-id='${ cartArray[i].prodid}' data-product-name='${cartArray[i].name}' value='${cartArray[i].count}' class="form-control width-50px border-0 item-count">
                            <span class="fa fa-minus minus-item" data-product-name="${ cartArray[i].name}" data-product-id="${cartArray[i].prodid}" data-product-price="${cartArray[i].price}"></span>
                        </div>
                    </form>
              </td>
            <td data-title="جمع کل:"><h5 class="prod-name-cart">${comma(cartArray[i].total)}</h5></td>
            <td data-title="نوع بسته بندی:" class="packageIdCartTd"><input type='hidden' name='packageIdselected' value='${cartArray[i].packageId}' data-pkInputHide='${cartArray[i].prodid}' data-hazineh="0" /><select class="form-control packageComboInCart" data-pkCmbId='${cartArray[i].prodid}'><option value="">انتخاب ..</option></select></td>
            <td data-title="حذف/ذخیره در لیست خریدبعدی:" dir="ltr">
                    <span class="fa fa-check cyan fa-2x fa-bold basket-action-span"></span><span class="fa-2x">/</span>
                    <span class="fa fa-times fa-2x delete-item basket-action-span" data-product-name="${cartArray[i].name}" data-product-id="${cartArray[i].prodid}"></span>
             </td>
         </tr>`;
                }
                $('.show-cart').html(output);
                $('.total-cart').html(comma(shoppingCart.totalCart()));
                $('.total-count').html(comma(shoppingCart.totalCount()));

                popupCart();
            }


            function popupCart() {
                var cartArray = shoppingCart.listCart();
                var output = "";
                if (cartArray.length == 0) {
                    /*output += `<div class="c-header__profile-dropdown-account-container">
                                                                <div class="c-header__profile-dropdown-user">
                                                                    <span class="col-12 small font-weight-600">سبد کالای شما خالی می باشد</span>
                                                                </div>
                                                            </div>`;*/
                    $(".empty-basket-popup").removeClass("d-none");
                    $(".basket-popup-main li:not(.empty-basket-popup)").remove();
                }
                else {
                    output = `<li> <div class=""><div class="c-header__profile-dropdown js-dropdown-menu basket-popup-header"><div class="c-header__profile-dropdown-account-container go-basket">
            <div class="c-header__profile-dropdown-user">
                <span class="col-4 small font-weight-600">${shoppingCart.totalCount()} کالا</span>
                <span class="col-8 text-right"><a class="cyan" href="/Shoppingcart/cart">مشاهده سبد خرید <i class="fa fa-chevron-left smaller"></i></a></span>
            </div>
        </div></div></div></li>
        <li> <div class=""><div class="c-header__profile-dropdown js-dropdown-menu basket-popup-header">
                <div class="c-header__profile-dropdown-actions"><ul class="basket-products-popup-ul" style="list-style-type: none;padding: 0;">`;
                    for (var i in cartArray) {
                        output += `<li><div class="c-header__profile-dropdown-action-container">
                <a href="/Product/index?id=${cartArray[i].prodid}" class="c-header__profile-dropdown-action c-header__profile-dropdown-action--orders ">
                    <div class="col-3 p-0"><img class="max-width-70px border-radius-3" src="${cartArray[i].prodimage}" /></div>
                    <div class="col-9">
                        <span class="font-size-1rem color-menu-sabad font-weight-600">${cartArray[i].name}</span>
                        <span class="cyan d-block">
                            <img src="/image/icons/avaiable.png" class="width-24px small" />
                            در سرای موجود می باشد
                        </span>
                    </div>
                </a>
                <div class="row m-0 pt-5px pb-5px">
                    <div class="col-8 text-center"><span class="color-menu-sabad small font-weight-bold">${cartArray[i].count} عدد</span></div>
                    <div class="col-4 text-right">
                        <span class="delete-item basket-action-span" data-product-id="${cartArray[i].prodid}">
                            <img src="/image/icons/delete.png" class="width-24px" />
                        </span>
                    </div>
                </div>
            </div></li>`;
                    }

                    output += `</ul></li>
<li><div class="c-header__profile-dropdown js-dropdown-menu basket-popup-header"><div class="p-2">
                <div class="row m-0 pt-5px pb-5px bolderspan">
                    <div class="col-6 text-left">
                        <span class="color-menu-sabad">مبلغ قابل پرداخت</span>
                        <span class="d-block color-menu-sabad"><span>${comma(shoppingCart.totalCart())}</span> تومان</span>
                    </div>
                    <div class="col-6 text-right">
                        <a class="btn btn-info mt-5px" href="/shoppingcart/cart">ثبت سفارش</a>
                    </div>
                </div>
            </div></div>
        </li>`;


                    var indxRnd = Math.floor(Math.random() * 2000) + 1;
                    if ($("div[data-rndbasketid=" + indxRnd + "]").length > 0) indxRnd++;

                    $('.basket-popup-main').find("li.empty-basket-popup").addClass("d-none");
                    $('.basket-popup-main').find("li:not(.empty-basket-popup)").remove();
                    $('.basket-popup-main').append(output);
                }
            }

            // Delete item button

            // حذف در پاپاپ سبد
            $('.basket-popup-main').on("click", ".delete-item", function (event) {
                var prodid = $(this).data('product-id')
                shoppingCart.removeItemFromCartAll(prodid);
                displayCart();
            })

            // حذف در صفحه سبد خرید
            $('.show-cart').on("click", ".delete-item", function (event) {
                var prodid = $(this).data('product-id')
                shoppingCart.removeItemFromCartAll(prodid);
                displayCart();
            });


            // -1
            $('.show-cart').on("click", ".minus-item", function (event) {
                var prodid = $(this).data('product-id');
                //var price = $(this).data('product-price');
                //var name = $(this).data('product-name');

                shoppingCart.removeItemFromCart(prodid);
                displayCart();
            })
            // +1
            $('.show-cart').on("click", ".plus-item", function (event) {
                var prodid = $(this).data('product-id');
                var price = $(this).data('product-price');
                var name = $(this).data('product-name');

                shoppingCart.addItemToCart(name, price, 1, prodid);
                displayCart();
            })

            // Item count input
            $('.show-cart').on("change", ".item-count", function (event) {
                var prodid = $(this).data('product-id');
                var count = Number($(this).val());
                shoppingCart.setCountForItem(prodid, count);
                displayCart();
            });

            //====================

            displayCart();

        }
    }


})();

cartEvents.init();

//==========================================
function getParameterByName(name, url = window.location.href) {
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

function getQueryStringValue(key) {
    return decodeURIComponent(window.location.search.replace(new RegExp("^(?:.*[&\\?]" + encodeURIComponent(key).replace(/[\.\+\*]/g, "\\$&") + "(?:\\=([^&]*))?)?.*$", "i"), "$1"));
}

function comma(Number) {
    Number += '';
    Number = Number.replace(',', '');
    x = Number.split('.');
    y = x[0];
    z = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(y))
        y = y.replace(rgx, '$1' + ',' + '$2');
    return y + z;
}

function notify(
    options = {
        title: null,
        message: null
    },
    settings = {
        type : "info",
        element : "body",
        from: "top",
        align: "right",
        position:null,
        allow_dismiss:true,
        newest_on_top: false,
        showProgressbar: false,
        offset: 20,
        spacing: 10,
        z_index: 1031,
        delay: 5000,
        timer: 1000,
        url_target: '_blank',
        mouse_over: null,
        onShow: null,
        onShown: null,
        onClose: null,
        onClosed: null,
        icon_type: 'class'
    }) {
    $.notify({
        // options
        icon: 'glyphicon glyphicon-warning-sign',
        title: options.title,
        message: options.message//,
        //url: url,
        //target: '_blank'
    }, {
        // settings
        element: settings.element,
        position: settings.position,
        type: settings.type,
        allow_dismiss: settings.allow_dismiss,
        newest_on_top: settings.newest_on_top,
        showProgressbar: settings.showProgressbar,
        placement: {
            from: settings.from,
            align: settings.align
        },
        offset: settings.offset,
        spacing: settings.spacing,
        z_index: settings.z_index,
        delay: settings.delay,
        timer: settings.timer,
        url_target: settings.url_target,
        mouse_over: settings.mouse_over,
        animate: {
            enter: 'animated fadeInDown',
            exit: 'animated fadeOutUp'
        },
        onShow: settings.onShow,
        onShown: settings.onShown,
        onClose: settings.onClose,
        onClosed: settings.onClosed,
        icon_type: settings.icon_type,
        template: '<div data-notify="container" class="col-xs-11 col-sm-3 alert alert-{0} mt-5" role="alert">' +
            '<button type="button" aria-hidden="true" class="close" data-notify="dismiss">×</button>' +
            '<span data-notify="icon"></span> ' +
            '<span data-notify="title">{1}</span><br/> ' +
            '<span data-notify="message">{2}</span>' +
            '<div class="progress height-0-2rem" data-notify="progressbar">' +
            '<div class="progress-bar progress-bar-{0}" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;"></div>' +
            '</div>' +
            '<a href="{3}" target="{4}" data-notify="url"></a>' +
            '</div>'
    });
}