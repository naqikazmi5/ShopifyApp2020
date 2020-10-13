$("#submitdetails").click(function () {
    var storeinfo = $("#storename").val();
    if (storeinfo === null || storeinfo === undefined || storeinfo === "") {
        alertify.error("Store name should not be empty!");
    }
    else {
        alertify.success('Request has been started, data will be synced within few minutes.');
        var productsync, customersync, ordersync = false;
        if ($("#ordersync").prop('checked') === true) {
            ordersync = true;
        }
        if ($("#productsync").prop('checked') === true) {
            productsync = true;
        }
        if ($("#customersync").prop('checked') === true) {
            customersync = true;
        }
        var storename = $("#storename").val().split(".")[0];
        $.ajax({
            url: "/home/ShopifyIntegration",
            method: "POST",
            data: { storename: storename, duration: $("#duration").val(), ordersync: ordersync, productsync: productsync, customersync: customersync }
        }).done(function (response) {
            if (response === "Location not found") {
                alertify.error(response);
            }
            else {
                alertify.success('Store data has been synced successfuly!');
            }
        }).fail(function (jqXHR, textStatus) {
            alertify.error("Something went wrong! Try again later.");
        });
    }
    
});
//$("#authreq").click(function () {
//    var storeinfo = $("#storename").val();
//    if (storeinfo === null || storeinfo === undefined || storeinfo === "") {
//        alertify.error("Store name should not be empty!");
//    }
//    else {
//        var storename = "";
//        var shop = $("#storename").val();
//        var check = shop.includes('.');
//        if (check === true) {
//            alert();
//            storename = shop;
//        }
//        else {
//            storename = "" + shop +".myshopify.com";
//        }
//        var url = "/home/install?shop=" + storename + "";
//        window.location.href = url; 
//        //$.ajax({
//        //    url: "/home/install",
//        //    method: "GET",
//        //    data: { shop: storename }
//        //}).done(function (response) {
//        //    alertify.success('Store data has been synced successfuly!');
//        //}).fail(function (jqXHR, textStatus) {
//        //    alertify.error("Something went wrong! Try again later.");
//        //});
//    }
//});
