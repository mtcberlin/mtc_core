class CoreQrCode extends CoreHTMLElement {

	_width = 100;
	_height = 100;
	_qrtext = "";

	constructor() {
		super();
	}

	connectedCallback() {
		setTimeout(() => {
			this._connectedCallbackStart();
		}, 100);
	}

	static get observedAttributes() {
		return ['data-text'];
	}


	attributeChangedCallback(name, oldValue, newValue) {
		if (name === 'data-text') {
			this._qrtext = newValue;
			this.update();
		}
	}

	_connectedCallbackStart() {
		this._initConfig();
		this._render();
		this._qrNode = this.shadowRoot.querySelector(".qrcode");
		this._createCode();
	}

	_initConfig() {
		if (this.hasAttribute('width')) {
			this._width = parseInt(this.getAttribute('width'));
		}
		if (this.hasAttribute('height')) {
			this._height = parseInt(this.getAttribute('height'));
		}
	}

	_render() {
		const container = document.createElement("div");
		container.innerHTML = `<div class='qrcode'>
        </div>`;

		const shadowRoot = this.attachShadow({ mode: 'open' });
		shadowRoot.appendChild(container);
	}

	_createCode() {
		if (this._qrNode) {
			this._qrcode = new QRCode(this._qrNode, {
				width: this._width,
				height: this._height
			});
			this.update();
		}
	}

	update() {
		if (this._qrNode) {
			if (this._qrtext != "") {
				this._qrcode.makeCode(this._qrtext);
				this.show();
			} else {
				this.hide();
			}
		}
	}

}

window.customElements.define('core-qrcode', CoreQrCode);