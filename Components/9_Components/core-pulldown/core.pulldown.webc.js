
class CorePullDown extends CoreHTMLElement {
    constructor() {
        super();
        console.log("CorePullDown ");
		this._createShadow();
    }
    connectedCallback() {
		this._title = this.getAttribute('title');
		this._type = this.getAttribute('type') || "page";

		setTimeout(() => {
			this.connectedCallbackStart();
		});
	}

	connectedCallbackStart() {
		if (!this.hasAttribute("id")) {
			this.id = "id_" + this.generateUUID();
		}
        this._addListener();
        this._initNodes();
    }

    _initNodes(){
		this._header = this.shadowRoot.querySelector(".header");
		this._content = this.shadowRoot.querySelector(".content");
    }

    _addListener() {
		this.shadowRoot.querySelector(".header").addEventListener('click', this.onHeaderClick.bind(this));
		this.shadowRoot.querySelector(".content").addEventListener('click', this.onContentClick.bind(this));
	}

	onHeaderClick(e) {
		e.stopPropagation();
		console.log("onHeaderClick: " + this.id);
		this._open();
	}

	onContentClick(e) {
		e.stopPropagation();
		console.log("onContentClick: " + this.id);
		this._close();
	}

    _open(){
        this._content.classList.remove("hidden");
    }

    _close(){
        this._content.classList.add("hidden");
    }

    _createShadow() {
		// Create shadow DOM for the component.
		let shadowRoot = this.attachShadow({ mode: 'open' });
		shadowRoot.innerHTML = `
            <style>
                :host .header{
                    display: flex; 
                    justify-content: space-between;
                    border: solid #eee 1px;
                    border-radius: 0 0 7px 7px;
                }
                .content{
                	position:fixed;
                    border: solid #aaa 1px;
                    border-radius: 0 0 7px 7px;
                    padding: 0.5em 0.5em;
                    background-color:#eee;
                }
                .hidden{
                    display:none;
                }
            </style>
            <div id="" aria-labelledby="" role="none" class="">
                <div class="header">
                +
                </div>
                <div class="content hidden">
                    <slot></slot>
                </div>
            </div>
        `;
	}
}

window.customElements.define('core-pulldown', CorePullDown);  