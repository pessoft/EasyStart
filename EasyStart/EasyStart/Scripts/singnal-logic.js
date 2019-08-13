$(document).ready(function () {
    OrderHub = $.connection.newOrderHub;

    OrderHub.client.addNewOrder = function (newOrder) {
        newOrder = processsingOrder(newOrder);
        Orders.push(newOrder);
        showCountOrder(Orders.length);
        notifySoundNewOrder();
        removeEmptyOrders(Pages.Order);
        CardOrderRenderer.renderOrder(newOrder, Pages.Order);
    };

    $.connection.hub.start().done(function () {
        const currentBrunch = $("#current-brnach").val();
        addListenByBranch([currentBrunch]);
    });
});

var OrderHub;

function addListenByBranch(brnachIds) {
    OrderHub.server.connect(brnachIds);
}