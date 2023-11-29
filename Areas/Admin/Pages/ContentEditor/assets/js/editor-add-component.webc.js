/// <reference path="../../../../../../Components/0_Base/corehtmlelement.webc.js" />
class EditorAddComponent extends CoreHTMLElement {
  _isConnected = false;
  _tid = this.generateUUID();
  addButton = null;

  constructor() {
    super();
    this.buttonName = "Add";
  }

  connectedCallback() {
    setTimeout(() => {
      this.buttonName = this.getAttribute("buttonname");
      this.modalTitle = this.getAttribute("modaltitle");
      if (!this._isConnected) {
        this._render();
        this.addListener();
        this._isConnected = true;
      }
    });
  }

  disconnectedCallback() {
    if (this._isConnected) {
    }
  }

  addListener() {
    if (!this._isConnected) {
      console.log("<<<< addListener: " + this._tid);
      this.modalDialog = this.querySelector("core-modal");
      this.addButton = this.querySelector(".btn");
      this.addButton.addEventListener("click", this._loadComponents.bind(this));
      this.querySelector(".js-component-list").addEventListener(
        "click",
        this._elementClicked.bind(this)
      );
    }
  }

  _render() {
    /*const container = document.createElement("div");
		container.innerHTML = `
        <style>
        .btn{
            text-decoration: none;
            cursor: pointer;
            display: inline-block;
            border: 0;
            padding: 0.75rem 1.5rem;
            border-radius: 3px;
            background-color: #e2e2e2;
        }
        </style>
        <button class="btn">
            ${this.buttonName}
        </button>
        <core-modal title="Add Component">
            <ul class="js-component-list"></ul>
        </core-modal>
        `;

		const shadowRoot = this.attachShadow({ mode: 'open' });
		shadowRoot.appendChild(container);
		*/
    this.innerHTML = `
        <button type="button" class="btn btn-dark-gray d-flex align-items-center justify-content-center me-3">
					<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-plus-circle-fill me-2" viewBox="0 0 16 16">
						<path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM8.5 4.5a.5.5 0 0 0-1 0v3h-3a.5.5 0 0 0 0 1h3v3a.5.5 0 0 0 1 0v-3h3a.5.5 0 0 0 0-1h-3v-3z"/>
					</svg>
            ${this.buttonName}
        </button>
        <core-modal title="${this.modalTitle}">
            <ul class="js-component-list"></ul>
        </core-modal>
        `;
  }

  _modalCancel() {
    console.log("EditorAddComponent: cancel event raised");
  }

  _modalOk() {
    var selection = this.querySelector('[name="component"]:checked');
    if (selection) {
      var event = new CustomEvent("addComponent", {
        bubbles: true,
        detail: { componentName: selection.value },
      });
      this.dispatchEvent(event);
    }
  }

  _loadComponents() {
    const pageId = CoreStateStore.getValue(
      CoreStateStore.PAGE_EDITOR_CONTENT_ID
    );
    const lang = CoreStateStore.getValue(
      CoreStateStore.PAGE_EDITOR_CONTENT_LANG
    );
    const placeholderName = this.dataset["placeholderName"];
    this.ajax(
      {
        type: "GET",
        url: "/admin/contenteditor/getPlaceholderComponents",
        data: { pageId: pageId, lang: lang, placeholderName: placeholderName },
        contentType: "application/json",
      },
      this._onPlaceholderComponentsSuccess.bind(this),
      () => {}
    );
  }

  _elementClicked(event) {
    var li = event.target.closest("li");
    if (li) {
      var radio = li.querySelector("input");
      radio.checked = true;
      if (this.querySelector(".selected")) {
        this.querySelector(".selected").classList.remove("selected");
      }
      li.classList.add("selected");
    }
    event.stopPropagation();
  }

  _onPlaceholderComponentsSuccess(result) {
    this.querySelector(".js-component-list").innerHTML = result.data.value;
    this.modalDialog.show(
      this._modalOk.bind(this),
      this._modalCancel.bind(this)
    );
  }
}
window.customElements.define("editor-add-component", EditorAddComponent);
