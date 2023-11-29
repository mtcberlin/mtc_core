class CoreHTMLElement extends HTMLElement {

	observeList = [];
	guidEmpty = "00000000-0000-0000-0000-000000000000";


	ajax(options, successCallback, errorCallback, uploadStartCallback, uploadProgressCallback, uploadFinishCallback) {
		var xhr = new XMLHttpRequest();
		xhr.responseType = options.responseType ? options.responseType : 'json';
		xhr.onreadystatechange = () => {
			if (xhr.readyState == XMLHttpRequest.DONE) {   // XMLHttpRequest.DONE == 4
				if (xhr.status == 200) {
					successCallback(xhr.response);
				}
				else if (xhr.status == 400) {
					errorCallback(xhr.response);
				}
				else {
					errorCallback();
				}
			}
		};

		if (!options.type || options.type.toLowerCase() === "get" || options.type.toLowerCase() === "delete") {
			options.url = this._updateUrlParameter(options.data, options.url);
		}

		xhr.open(options.type ? options.type : "GET", options.url, true);

		if (options.contentType) {
			xhr.setRequestHeader("Content-Type", options.contentType);
		}

		if (uploadStartCallback) {
			xhr.upload.onloadstart = (e) => { uploadStartCallback(e) };
		}

		if (uploadProgressCallback) {
			xhr.upload.onprogress = (e) => { uploadProgressCallback(e) };
		}

		if (uploadFinishCallback) {

			xhr.upload.onload = (e) => { uploadFinishCallback(e) };
		}

		if (options.type && options.type.toLowerCase() === "post") {
			xhr.send(options.data);
		} else {
			xhr.send();
		}

		return xhr;
	}

	generateUUID() {
		let
			d = new Date().getTime(),
			d2 = (performance && performance.now && (performance.now() * 1000)) || 0;
		return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
			let r = Math.random() * 16;
			if (d > 0) {
				r = (d + r) % 16 | 0;
				d = Math.floor(d / 16);
			} else {
				r = (d2 + r) % 16 | 0;
				d2 = Math.floor(d2 / 16);
			}
			return (c == 'x' ? r : (r & 0x7 | 0x8)).toString(16);
		});
	};

	_updateUrlParameter(param, url) {
		url = url || window.location.href;
		const parsedUri = new URL(url, window.location.href);
		for (const key in param) {
			if (param[key] instanceof Array) {
				param[key].forEach((element) => {
					parsedUri.searchParams.append(key, element);
				});
			} else if (param[key] != undefined) {
				parsedUri.searchParams.set(key, param[key]);
			}
		}

		return parsedUri.toString();
	}

	getUrlParameter(param) {
		const parsedUri = new URL(window.location.href);
		return parsedUri.searchParams.get(param);
	}

	initObserver(target) {

		if (target == null) return;

		var ol = this.observeList;
		// eine Instanz des Observers erzeugen
		var observer = new MutationObserver(function (mutations) {
			//console.log("mutations");
			mutations.forEach(function (mutation) {
				ol.forEach(
					function (obj) {
						//console.log("check mutation");
						if (obj.attribute != "") {
							if (mutation.attributeName == obj.attribute) {
								if (mutation.target != null) {
									if (mutation.oldValue != mutation.target.getAttribute(mutation.attributeName)) {
										obj.callback(mutation);
									}
								}
							}
						}
					}
				);
			});
		});

		// Konfiguration des Observers: alles melden - Änderungen an Daten, Kindelementen und Attributen
		var config = { attributes: true, attributeOldValue: true, childList: true, subtree: true, characterData: true };

		// eigentliche Observierung starten und Zielnode und Konfiguration übergeben
		observer.observe(target, config);
	}


	serializeForm(form) {
		var result = {};
		var formData = new FormData(form);
		for (const key of formData.keys()) {
			if (key.indexOf(".") > -1) {
				const split = key.split('.');
				let next = result;
				for (let i = 0; i < split.length; i++) {
					if (i == split.length - 1) {
						next[split[i]] = formData.get(key);
					} else if (next[split[i]] == undefined) {
						next[split[i]] = {};
						next = next[split[i]];
					} else {
						next = next[split[i]];
					}
				}
			} else {
				result[key] = formData.get(key);
			}
		}

		return result;
	};

	hide(selector) {
		if (selector !== undefined) {
			this.querySelectorAll(selector).forEach(ele => ele.classList.toggle("d-none", true));
		} else {
			this.classList.toggle("d-none", true);
		}
	}

	show(selector) {
		if (selector !== undefined) {
			this.querySelectorAll(selector).forEach(ele => ele.classList.toggle("d-none", false));
		} else {
			this.classList.toggle("d-none", false);
		}
	}
}