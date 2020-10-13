$("#btnSubmit").click(function () {
    var Name = $("#f-name").val();
    var Website = $("#website").val();
    var Email = $("#email").val();
    var MonthlyOrder = $("#monthly-volume").val();
    var OrderType = $("#order-type").val();
    var Message = $("#message").val();
    if (Name === "" || Website === "" || Email === "" || MonthlyOrder === "" || OrderType === "" || Message ==="") {
        alertify.error("Please fill all required fields");
    }
    else {
        $.ajax({
            url: "/home/contactus",
            method: "POST",
            data: { Name: Name, Website: Website, Email: Email, MonthlyOrder: MonthlyOrder, OrderType: OrderType, Message: Message }
        }).done(function (response) {
            if (response === "success") {
                alertify.success('Your request has been recieved successfully.');
                $("#f-name").val('');
                $("#website").val('');
                $("#email").val('');
                $("#monthly-volume").val('');
                $("#order-type").val('');
                $("#message").val('');
            }
            else {
                alertify.error(response);
            }
        }).fail(function (jqXHR, textStatus) {
            alertify.error("Something went wrong! Try again later.");
        });
    }
});