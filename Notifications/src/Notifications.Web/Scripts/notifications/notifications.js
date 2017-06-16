$(document).ready(function() {
	$('.table > tbody > tr').click(function () {
		var id = $(this).find('td').attr('id');
		//window.alert('Row was clicked with id ' + id);

		$.ajax({
			type: "GET",
			url: "/Notifications/Message",
			data: id,
			success: function(result) {
				//window.alert('Row was clicked with id ' + id);
			}
		});
	});
});
