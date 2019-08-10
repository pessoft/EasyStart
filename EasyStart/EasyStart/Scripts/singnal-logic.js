$(document).ready(function () {
    OrderHub = $.connection.newOrderHub;

    OrderHub.client.addNewOrder = function (newOrder) {
        newOrder = processsingOrder(newOrder);
        Orders.push(newOrder);
        showCountOrder(Orders.length);
        notifySoundNewOrder();
        renderOrder(newOrder);
        CardOrderRenderer.renderOrder(newOrder);
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