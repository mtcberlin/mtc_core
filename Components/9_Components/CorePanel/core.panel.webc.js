class CorePanel extends CoreHTMLElement {
  _isConnected = false;
  _tid = this.generateUUID();

  constructor() {
    super();
  }

  connectedCallback() {
    this.setAttribute("slot", "item");

    setTimeout(() => {
      if (this._isConnected === false) {
        this.connectedCallbackStart();
        this._initListeners();
        this._isConnected = true;
      }
    });
  }

  disconnectedCallback() {
    if (this._isConnected) {
    }
  }

  connectedCallbackStart() {
    if (!this.hasAttribute("id")) {
      this.id = this.generateUUID();
    }

    this._createShadow();

    const state = CoreStateStore.getValue(
      CoreStateStore.PAGE_EDITOR_IS_PANEL_EXPANDED_PREFIX +
        this.getAttribute("title")
    );

    const template = this.querySelector("template");

    this._region = this.querySelector(".panel-region");
    this._btn = this.querySelector(".panel-trigger");

    this._btn.id = "btn_" + this.id;
    this._region.id = "region_" + this.id;
    this._region.setAttribute("aria-labelledby", this._btn.id);
    this._btn.setAttribute("aria-controls", this._region.id);

    let open = false;
    if (state === null || state === undefined || state === "") {
      open = this.getAttribute("expanded") === "true";
    } else {
      open = state === "true" ? true : false;
    }
    this.toggle(open);
  }

  _initListeners() {
    if (!this._isConnected) {
      this.querySelector("button").addEventListener(
        "click",
        this.onButtonClick.bind(this)
      );

      this.observeList.push({
        attribute: "changed",
        callback: this._changedContent.bind(this),
      });
      this.initObserver(this);

      this.eventExpanded = new CustomEvent("expanded", {
        bubbles: true,
        detail: { state: () => this.openState, id: () => this.id },
      });
    }
  }

  disconnectedCallback() {
    if (this._isConnected) {
      /*
			const tabsSlot = this.shadowRoot.querySelector('#tabsSlot');
			tabsSlot.removeEventListener('click', this._boundOnTitleClick);
			tabsSlot.removeEventListener('keydown', this._boundOnKeyDown);
			*/
    }
  }

  _changedContent(mutationObj) {
    var newValue = mutationObj.target.getAttribute(mutationObj.attributeName);
    if (newValue == "true") {
      var node = this.querySelector("h6");
      if(node){
        node.classList.add("modfified");
      }
    }
  }

  close() {
    this.toggle(false);
  }

  open() {
    this.toggle(true);
  }

  _createShadow() {
    let title = this.getAttribute("title");
    this.insertAdjacentHTML(
      "afterbegin",
      `
			<div class="bg-white">
		
					<button aria-expanded="true" class="panel-trigger d-flex align-items-center btn btn-white d-flex border-top border-1 w-100 h-3 rounded-0 px-0 outline-blue-focus" aria-controls="" id="">
            <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="black" class="bi bi-chevron-down ms-3 me-2 rotate-270" viewBox="0 0 16 16">
              <path fill-rule="evenodd" d="M1.646 4.646a.5.5 0 0 1 .708 0L8 10.293l5.646-5.647a.5.5 0 0 1 .708.708l-6 6a.5.5 0 0 1-.708 0l-6-6a.5.5 0 0 1 0-.708z"/>
            </svg>
						<h6 class="panel-title fw-bold mb-0">${title}<span class="panel-icon"></span></h6>
					</button>
	
				<div id="" aria-labelledby="" role="region" class="panel-region mx-5 pb-4">
					<div class="js-content"></div>
				</div>
			<div>
        `
    );
    this.querySelector(".js-content").insertAdjacentElement(
      "afterbegin",
      this.querySelector("[slot='content']")
    );
  }

  onButtonClick() {
    console.log("onButtonClick");
    this.toggle(!this.openState);
    this.dispatchEvent(this.eventExpanded);
  }

  toggle(open) {
    // don't do anything if the open state doesn't change
    if (open === this.openState) {
      return;
    }

    // update the internal state
    this.openState = open;

    // handle DOM updates
    this._btn.setAttribute("aria-expanded", `${open}`);
    this.setAttribute("expanded", `${open}`);
    const btn = this.querySelector(".bi-chevron-down");

    if (open) {
      this._region.removeAttribute("hidden");
      btn.classList.remove("rotate-270");
    } else {
      this._region.setAttribute("hidden", "");
      btn.classList.add("rotate-270");
    }
    CoreStateStore.setValue(
      CoreStateStore.PAGE_EDITOR_IS_PANEL_EXPANDED_PREFIX +
        this.getAttribute("title"),
      open
    );
  }
}

window.customElements.define("core-panel", CorePanel);
