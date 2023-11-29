class EditorEditComponent extends CoreHTMLElement {

	_isConnected = false;

	connectedCallback() {
		setTimeout(() => {
			if (!this._isConnected) {
				this.modalTitle = this.getAttribute('data-modal-title');
				this._render();
				this.addListener();
				this._isConnected = true;
			}
		});
	}

	addListener() {
		this.modalDialog = this.querySelector("core-modal");
		this.querySelector(".js-open-modal").addEventListener("click", this._loadComponentContent.bind(this));
	}

	_render() {
		const coreModal = document.createElement("core-modal");
		coreModal.setAttribute("title", !this.modalTitle == "" ? this.modalTitle : "Edit Component");
		coreModal.classList.add("js-component-content");
		coreModal.setAttribute("modal-width", "80%");
		this.append(coreModal);
	}

	_modalCancel() {

	}

	_modalOk() {
		console.log("EditorEditComponent: Ok event raised");
	}

	_loadComponentContent() {
		const datasource = this.dataset["datasourceId"];
		const componentName = this.dataset["componentName"];
		this.ajax({
			type: "GET",
			url: `/api/editor/${componentName.toLowerCase()}/get`,
			data: { datasource: datasource, lang: "en" },
			responseType: "text"
		}, this._onComponentResult.bind(this), () => { });
	}

	_modalOk() {
		var form = this.modalDialog.querySelector("form");

		var result = this.serializeForm(form);
		this.ajax({
			type: "POST",
			url: `/api/editor/${this.dataset["componentName"].toLowerCase()}/save`,
			data: JSON.stringify(result),
			contentType: "application/json"
		}, () => { }, () => { });
	}

	_onComponentResult(result) {
		this.querySelector(".js-component-content").innerHTML = result;
		this.modalDialog.show(this._modalOk.bind(this), this._modalCancel.bind(this));
	}

}
window.customElements.define('editor-edit-component', EditorEditComponent);