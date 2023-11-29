document.addEventListener("DOMContentLoaded", function () {
	addEditModeBar();
	addListeners();
});

var addEditModeBar = function() {
	var editNav = document.createElement("div");
	editNav.id = "editNav";
	editNav.innerHTML = '<span class="editmode-save">Save</span>';
	editNav.classList.add("editmode-navigation");
	editNav.classList.add("editmode-navigation-fixed");
	var editNavSpace = document.createElement("div");
	editNavSpace.innerHTML = 'SPACE';
	editNavSpace.classList.add("editmode-navigation");
	document.body.insertBefore(editNavSpace, document.body.firstChild);
	document.body.insertBefore(editNav, document.body.firstChild);
};

var addListeners = function() {
	document.getElementById("editNav").addEventListener("click", saveChanges);

	var editables = document.getElementsByClassName("core-editable");

	for (var i = 0; i < editables.length; i++) {
		editables[i].addEventListener("input", markChanged, false);
	}
};

var markChanged = function (event) {
	event.currentTarget.classList.add("core-changed");

	if (event.currentTarget.parentNode.localName === "p") {
		event.currentTarget.parentNode.classList.add("core-change-mark");
	} else {
		event.currentTarget.classList.add("core-change-mark");
	}
};

function saveChanges () {
	var changes = [];
	var changedElements = $('.core-editable.core-changed');
	changedElements.each((index, element) => {
		changes.push({
			"filePath": element.dataset.editFilepath,
			"fieldName": element.dataset.editFieldname,
			"oldValue": element.dataset.orginalValue,
			"value": encodeURIComponent(element.innerHTML)
		});
	});

	$.ajax({
		type: "POST",
		url: "/api/admin/edit/saveChanges",
		data: JSON.stringify({
			"pageUrl": window.location.href,
			"changes": changes
		}),
		success: saveChangesSuccess,
		contentType: 'application/json'
	});
}

function saveChangesSuccess() {
	window.location.reload();
}