// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(".select2").select2({ width: 'resolve' });

localStorage.setItem("token", `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJIYW5kQ3JhZnRTZXJ2ZXJBY2Nlc3NUb2tlbiIsImp0aSI6ImI1NDVmMzA0LTIxZjUtNGZmNi1iOWM1LWM0YjMxNTZmMjE1MSIsImlhdCI6IjEwLzE2LzIwMjAgNTozNzowNiBQTSIsIklkIjoiMyIsIkZ1bGxOYW1lIjoi2KfYs9mF2KfYuduM2YQg2YjYp9it2K_bjCIsIlVzZXJOYW1lIjoiMTQ4MjcyNTc5IiwiRW1haWwiOiJ2YWhlZGkuZXNtYWVpbEBnbWFpbC5jb20iLCJyb2xlIjoiMiIsImV4cCI6MTYwMjk1NjIyNiwiaXNzIjoiSGFuZENyYWZ0QXV0aGVudGljYXRpb25TZXJ2ZXIiLCJhdWQiOiJIYW5kQ3JhZnRDbGllbnQifQ.7wkPFeZUS8syXPyWuhioU5G83oaeLOhHMvrkVuArnlg`)


var shoppingCart = (function () {
    // =============================
    // Private methods and propeties
    // =============================
    cart = [];

    // Constructor
    function Item(name, price, count) {
        this.name = name;
        this.price = price;
        this.count = count;
    }

    // Save cart
    function saveCart() {
        sessionStorage.setItem('shoppingCart', JSON.stringify(cart));
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
    obj.addItemToCart = function (name, price, count) {
        for (var item in cart) {
            if (cart[item].name === name) {
                cart[item].count++;
                saveCart();
                return;
            }
        }
        var item = new Item(name, price, count);
        cart.push(item);
        saveCart();
    }
    // Set count from item
    obj.setCountForItem = function (name, count) {
        for (var i in cart) {
            if (cart[i].name === name) {
                cart[i].count = count;
                break;
            }
        }
    };
    // Remove item from cart
    obj.removeItemFromCart = function (name) {
        for (var item in cart) {
            if (cart[item].name === name) {
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
    obj.removeItemFromCartAll = function (name) {
        for (var item in cart) {
            if (cart[item].name === name) {
                cart.splice(item, 1);
                break;
            }
        }
        saveCart();
    }

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
// Add item
$('.add-to-cart').click(function (event) {
    event.preventDefault();
    var name = $(this).data('name');
    var price = Number($(this).data('price'));
    shoppingCart.addItemToCart(name, price, 1);
    displayCart();
});

// Clear items
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
            <td> <img src='/image/express.jpg' class='img-fluid' /></td>
            <td> ${cartArray[i].name} </td>
            <td> ${cartArray[i].price}</td>
            <td><form class="form-inline mt-2" dir="rtl">
                        <div class="spinner border-radius-5">
                            <span class="fa fa-plus plus-item" data-name="${ cartArray[i].name}"></span>
                            <input type="text" name="quantity" readonly data-name='${ cartArray[i].name}' value='${cartArray[i].count}' class="form-control width-50px border-0 item-count">
                            <span class="fa fa-minus minus-item" data-name="${ cartArray[i].name}"></span>
                        </div>
                    </form>
              </td>
            <td>${cartArray[i].total}</td>
            <td>
                    <span class="fa fa-check text-success fa-bold"></span><span>/</span>
                    <span class="fa fa-times delete-item" data-name="${cartArray[i].name}"></span>
             </td>
         </tr>`;
    }
    $('.show-cart').html(output);
    $('.total-cart').html(shoppingCart.totalCart());
    $('.total-count').html(shoppingCart.totalCount());
}

// Delete item button

$('.show-cart').on("click", ".delete-item", function (event) {
    var name = $(this).data('name')
    shoppingCart.removeItemFromCartAll(name);
    displayCart();
})


// -1
$('.show-cart').on("click", ".minus-item", function (event) {
    var name = $(this).data('name')
    shoppingCart.removeItemFromCart(name);
    displayCart();
})
// +1
$('.show-cart').on("click", ".plus-item", function (event) {
    var name = $(this).data('name')
    shoppingCart.addItemToCart(name);
    displayCart();
})

// Item count input
$('.show-cart').on("change", ".item-count", function (event) {
    var name = $(this).data('name');
    var count = Number($(this).val());
    shoppingCart.setCountForItem(name, count);
    displayCart();
});

displayCart();
