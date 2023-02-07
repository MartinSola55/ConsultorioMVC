// --------------add active class-on another-page move----------
jQuery(document).ready(function ($) {
	// Get current path and find target link
	var path = window.location.pathname.split("/");

	// Account for home page with empty path
	if (path[1] == '') {
		path[1] = 'Turnos';
	}

	var target = $('#navbarContainer ul li a[href="/' + path[1] + '"]');
	// Add active class to target link
	target.parent().addClass('active');
});