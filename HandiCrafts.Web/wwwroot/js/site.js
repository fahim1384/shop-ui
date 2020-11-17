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
        }

        try {
            CustomerOrderPreview();
        } catch (e) {
            ;
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
            <td><a href="/Product/index?id=${cartArray[i].prodid}"><img src='${cartArray[i].prodimage}' class='img-fluid width-100px border-default border-radius-5 p-2px' /></a></td>
            <td class="text-left"><a href="/Product/index?id=${cartArray[i].prodid}"><h5 class="prod-name-cart">${cartArray[i].name}</h5><span class="mellicode">شناسنامه یکتا: ${cartArray[i].mellicode ? cartArray[i].mellicode:'0000'}</span></a></td>
            <td> <h5 class="prod-name-cart">${cartArray[i].price}</h5></td>
            <td><form class="form-inline mt-2" dir="rtl">
                        <div class="spinner border-radius-5 m-auto">
                            <span class="fa fa-plus plus-item" data-product-name="${ cartArray[i].name}" data-product-id="${cartArray[i].prodid}" data-product-price="${cartArray[i].price}"></span>
                            <input type="text" name="quantity" readonly data-product-id='${ cartArray[i].prodid}' data-product-name='${cartArray[i].name}' value='${cartArray[i].count}' class="form-control width-50px border-0 item-count">
                            <span class="fa fa-minus minus-item" data-product-name="${ cartArray[i].name}" data-product-id="${cartArray[i].prodid}" data-product-price="${cartArray[i].price}"></span>
                        </div>
                    </form>
              </td>
            <td><h5 class="prod-name-cart">${cartArray[i].total}</h5></td>
            <td class="packageIdCartTd"><input type='hidden' name='packageIdselected' value='${cartArray[i].packageId}' data-pkInputHide='${cartArray[i].prodid}' data-hazineh="0" /><select class="form-control packageComboInCart" data-pkCmbId='${cartArray[i].prodid}'><option value="">انتخاب ..</option></select></td>
            <td dir="ltr">
                    <span class="fa fa-check cyan fa-2x fa-bold basket-action-span"></span><span class="fa-2x">/</span>
                    <span class="fa fa-times fa-2x delete-item basket-action-span" data-product-name="${cartArray[i].name}" data-product-id="${cartArray[i].prodid}"></span>
             </td>
         </tr>`;
                }
                $('.show-cart').html(output);
                $('.total-cart').html(shoppingCart.totalCart());
                $('.total-count').html(shoppingCart.totalCount());
            }

            // Delete item button

            $('.show-cart').on("click", ".delete-item", function (event) {
                var prodid = $(this).data('product-id')
                shoppingCart.removeItemFromCartAll(prodid);
                displayCart();
            })


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