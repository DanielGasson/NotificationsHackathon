$(document).ready(function() {
	$('.table > tbody > tr').click(function () {
		var id = $(this).find('td').attr('id');
		$.get("/Notifications/Message", { id: id }, function(data) {
			$(document).find('#messageContainer').replaceWith(data);
		});
	});
});
