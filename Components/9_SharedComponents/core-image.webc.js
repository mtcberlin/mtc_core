class CoreImage extends CoreHTMLElement {

    constructor() {
        super();
    }

    connectedCallback() {
        setTimeout(() => {
            this._connectedCallbackStart();
        });
    }
    _connectedCallbackStart(){
		this._initNodes();
		this._addListener();
	}

	_initNodes(){
		if(this.hasAttribute('buttonname')){
			this.buttonName = this.getAttribute('buttonname');
		}
	}

	_addListener() {
		console.log("CoreImage addListener");
        this._modal = this.querySelector("dialog");
		this.openBtn = this.querySelector(".js-ci-btn-open");
		if(this.openBtn){
			this.openBtn.addEventListener("click", this._showDialog.bind(this));
		}
	}

	_render() {
	}

	_showDialog(){
		console.log("CoreImage _showDialog");
        this._modal.showModal();
	}
}
window.customElements.define('core-image', CoreImage);