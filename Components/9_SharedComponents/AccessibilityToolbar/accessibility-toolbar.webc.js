/// <reference path="../../0_Base/corehtmlelement.webc.js" />
class AccessibilityToolbar extends CoreHTMLElement {

	_componentContainer;

	connectedCallback() {
		setTimeout(() => {
			this._connectedCallbackStart();
		});

	}

	_connectedCallbackStart() {
		this._componentContainer = this.closest(".js-component");

		this._attachEventHandlers();

		const callback = (mutationList, observer) => {
			var hasAttributeChanges = mutationList.filter(m => {
				return m.type === "attributes";
			}).length > 0;

			if (hasAttributeChanges) {
				this._update();
			}
		};

		const observer = new MutationObserver(callback);
		observer.observe(document.body, { attributes: true });
	}

	_attachEventHandlers() {
		const dgsButton = this.querySelector(".js-dgs-video-btn");
		if (dgsButton) {
			dgsButton.addEventListener('click', e => {
				e.preventDefault();

				this._updateDgsVideo(true);
			});
		}
	}

	_update() {
		this._updateDgsVideo();
	}

	_updateDgsVideo(toggle = false) {
		const dgsVideos = this._componentContainer.querySelectorAll(".js-dgs-video");
		dgsVideos.forEach(dgsVideo => {
			if(dgsVideo && toggle) {
				dgsVideo.classList.toggle("d-none");
			} else if(dgsVideo) {
				dgsVideo.classList.toggle("d-none", !document.body.classList.contains("access-dgs-video"));
			}				
		});
	}

}

window.customElements.define('accessibility-toolbar', AccessibilityToolbar);