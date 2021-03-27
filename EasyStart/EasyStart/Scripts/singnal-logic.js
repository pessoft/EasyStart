$(document).ready(function () {
    OrderHub = $.connection.orderHub;

    OrderHub.client.addNewOrder = function (newOrder) {
        newOrder = processsingOrder(newOrder);
        Orders.push(newOrder);
        showCountOrder(Orders.length);
        notifySoundNewOrder();
        removeEmptyOrders(Pages.Order);
        CardOrderRenderer.renderOrder(newOrder, Pages.Order);
    };

    OrderHub.client.changeOrderStatus = function (orderId, orderStatus) {
        changeOrderStatus(orderId, orderStatus)
    };

    $.connection.hub.start().done(function () {
        console.log('Connection estabilished')
        const currentBrunch = $("#current-brnach").val();
        addListenByBranch([currentBrunch]);
    });

    $.connection.hub.disconnected(function () {
        console.log('Connection disconnected')
        setTimeout(function () {
            $.connection.hub.start();
        }, 10000);
    });
});

var OrderHub;

function addListenByBranch(brnachIds) {
    OrderHub.server.connect(brnachIds);
}

