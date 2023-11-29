/// <reference path="../../../../../../Components/0_Base/corehtmlelement.webc.js" />

class EditorPlaceholder extends CoreHTMLElement {

	_isConnected = false;
	_tid = this.generateUUID();

	constructor() {
		super();
	}

	connectedCallback() {
		// workaround for chrom and safari
		// https://stackoverflow.com/questions/62962138/how-to-get-the-contents-of-a-custom-element
		setTimeout(() => {
			if(!this._isConnected){
				console.log(">>>> timeout");
				this.addListener();
				this._loadComponentContent();
				this._isConnected = true;
			}
		});
	}

	disconnectedCallback() {
		if(this._isConnected){}
	}


	addListener() {
		if(!this._isConnected){
			console.log(">>>> addListener");
			this.addEventListener("addComponent", this._addComponent.bind(this));
			this.querySelectorAll(".js-remove-component").forEach(e => { e.addEventListener("click", this._removeComponent.bind(this)) });
			this.querySelectorAll(".js-move-component").forEach(e => { e.addEventListener("click", this._moveComponent.bind(this)) });
			this.querySelectorAll(".js-save-component").forEach(e => { e.addEventListener("click", this._saveComponent.bind(this)) });	
		}
	}

	_addComponent(e) {
		console.log("_addComponent");
		e.preventDefault();
		const pageId = CoreStateStore.getValue(CoreStateStore.PAGE_EDITOR_CONTENT_ID);
		const lang = CoreStateStore.getValue(CoreStateStore.PAGE_EDITOR_CONTENT_LANG);
		const placeholderName = this.dataset["placeholderName"];
		const componentName = e.detail.componentName;
		this.ajax({
			type: "POST",
			url: "/admin/contenteditor/addComponent",
			data: JSON.stringify({ "pageId": pageId, "lang": lang, "placeholderName": placeholderName, "componentName": componentName }),
			contentType: 'application/json'
		}, () => {window.location.reload()}, () => { });
	}

	_removeComponent(e) {
		const pageId = CoreStateStore.getValue(CoreStateStore.PAGE_EDITOR_CONTENT_ID);
		const lang = CoreStateStore.getValue(CoreStateStore.PAGE_EDITOR_CONTENT_LANG);
		const placeholderName = this.dataset["placeholderName"];
		const componentId = e.currentTarget.dataset["componentid"];
		this.ajax({
			type: "POST",
			url: "/admin/contenteditor/removeComponent",
			data: JSON.stringify({ "pageId": pageId, "lang": lang, "placeholderName": placeholderName, "componentId": componentId }),
			contentType: 'application/json'
		}, () => {window.location.reload()}, () => { });
	}

	_moveComponent(e) {
		const pageId = CoreStateStore.getValue(CoreStateStore.PAGE_EDITOR_CONTENT_ID);
		const lang = CoreStateStore.getValue(CoreStateStore.PAGE_EDITOR_CONTENT_LANG);
		const placeholderName = this.dataset["placeholderName"];
		const componentId = e.currentTarget.dataset["componentid"];
		const componentPos = e.currentTarget.dataset["componentPosition"];
		this.ajax({
			type: "POST",
			url: "/admin/contenteditor/setComponentPosition",
			data: JSON.stringify({ "pageId": pageId, "lang": lang, "placeholderName": placeholderName, "componentId": componentId, "newPosition": componentPos }),
			contentType: 'application/json'
		}, () => {window.location.reload()}, () => { });
	}

	_loadComponentContent() {
		const coms = this.querySelectorAll(".js-editor-fields");
		coms.forEach((com) => {
			const datasource = com.dataset["datasourceId"];
			const componentName = com.dataset["componentName"];
			this.ajax({
				type: "GET",
				url: `/api/editor/${componentName.toLowerCase()}/get`,
				data: {datasource: datasource},
				responseType: "text"
			}, this._addComponentResult.bind(this, com), () => { });
		});
	}

	_addComponentResult(component, data) {
		component.innerHTML = data;
	}

	_saveComponent(e) {
		const datasource = e.currentTarget.dataset["datasourceId"];
		var contentRoot = this.querySelector(`.js-editor-fields[data-datasource-id="${datasource}"]`);

		var result = this.serializeForm(contentRoot.querySelector("form"));
		this.ajax({
			type: "POST",
			url: `/api/editor/${contentRoot.dataset["componentName"].toLowerCase()}/save`,
			data: JSON.stringify(result),
			contentType: "application/json"
		}, () =>{}, () => { });
	}
	
}
window.customElements.define('editor-placeholder', EditorPlaceholder);