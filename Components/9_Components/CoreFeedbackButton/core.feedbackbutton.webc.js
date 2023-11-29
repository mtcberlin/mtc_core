class CoreFeedbackButton extends HTMLElement {

	btn = undefined;

	constructor() {
		super(); // always call super() first in the ctor.
	}

	connectedCallback() {
		// workaround for chrom and safari
		// https://stackoverflow.com/questions/62962138/how-to-get-the-contents-of-a-custom-element
		setTimeout(() => {
			this.connectedCallbackStart();
		});
	}

	_render() {
		
      const html = `
		  <style>
		  ::slotted(.loading) {
				background-color: gray !important;
			}
			::slotted(.success) {
				background-color: green !important;
				transition: background-color 1000ms linear;
			}
			::slotted(.error) {
				background-color: red !important;
				transition: background-color 1000ms linear;
			}
		  </style>
		  <button><slot></slot></button>`;
  
		const shadowRoot = this.attachShadow({ mode: 'open' });
		shadowRoot.innerHTML = html;
	  }

	connectedCallbackStart() {
		console.log("CoreFeedbackButton connectedCallback");
		this._render();
		this.btn = this.querySelector("button");
	}

	startLoading() {
		this.btn.classList.remove("success");
		this.btn.classList.remove("error");
		this.btn.classList.add("loading");
		this.btn.setAttribute("disabled", "disabled");
	}

	success() {
		this.btn.classList.remove("loading");
		//this.btn.classList.add("success");
		this.btn.removeAttribute("disabled");
	}

	error() {
		this.btn.classList.remove("loading");
		this.btn.classList.add("error");
		this.btn.removeAttribute("disabled");
	}

}
window.customElements.define('core-feedback-button', CoreFeedbackButton);