/// <reference path="../../../../../../Components/0_Base/corestatestore.webc.js" />
/// <reference path="../../../../../../Components/1_BaseField/base_editor.webc.js" />

class PageEditor extends BaseEditor {

	defaultTabName = "Allgemein";
	defaultSectionName = "Main";
	pageConfigurationId;
	//_loadItemUrl = "/admin/getPageProperties";
	//_saveItemUrl = "/admin/contenteditor/savepage";

	apiPlaceholderUrl = "/admin/getPlaceholderHtml";
	apiAddVersionUrl = "/admin/contenteditor/addVersion";


	constructor() {
		super();
		CoreStateStore.subscribe(CoreStateStore.PAGE_EDITOR_REQUEST_ID, this.onLoadRequest.bind(this));
		CoreStateStore.subscribe(CoreStateStore.EDITOR_EVENT_SAVED, this._loadPlaceholderHtml.bind(this));
	}

	disconnectedCallback() {
		//super.disconnectedCallback();
	}


	addListener() {
		super.addListener();

		this.querySelector(".js-export").addEventListener("click", this.onExportPage.bind(this));
		this.querySelectorAll(".js-change-lang").forEach(e => { e.addEventListener("click", this.onChangeLanguage.bind(this)) });
		this.querySelector(".js-add-lang").addEventListener("click", this._openAddLanguageDialog.bind(this));
	}

	_addStateEventListener() {
		this.querySelectorAll(".js-tab-nav-item").forEach(e => { e.addEventListener("click", this._onTabNavClicked.bind(this)) });
	}

	/* ########
	*	Helper
	######## */

	_getCurrentLanguage() {
		return CoreStateStore.getValue(CoreStateStore.PAGE_EDITOR_CONTENT_LANG);
	}

	_setCurrentLanguage(lang) {
		// this._setCurrentLanguage();
		CoreStateStore.setValue(CoreStateStore.PAGE_EDITOR_CONTENT_LANG, lang);
	}

	_setPreviewUrl(url) {
		this.querySelector(".js-preview").href = `${url}?preview=1`;
	}

	_getCurrentItemId() {
		return CoreStateStore.getValue(CoreStateStore.PAGE_EDITOR_CONTENT_ID);
	}

	_setCurrentItemId(id, type) {
		this.pageId = id;
		this.itemId = id;
		CoreStateStore.setValue(CoreStateStore.PAGE_EDITOR_CONTENT_ID, id);
		CoreStateStore.setValue(CoreStateStore.PAGE_EDITOR_CONTENT_TYPE, type);
	}


	/* ########
	*	Placeholder
	######## */

	_loadPlaceholderHtml() {
		const pageId = this._getCurrentItemId();
		const lang = this._getCurrentLanguage();

		if (pageId && lang) {
			this.ajax({
				type: "GET",
				url: this.apiPlaceholderUrl,
				data: { "pageId": pageId, "lang": lang },
				contentType: 'application/json'
			}, this.onPlaceholderHtmlSuccess.bind(this), () => { });
		} else {
			console.error("No PageId or Lang is present to load the placeholderHtml");
		}
	}

	onPlaceholderHtmlSuccess(result) {
		const contentPane = this.querySelector("#content-tab-pane");
		contentPane.parentNode.removeChild(contentPane);
		if (result.data.placeholderHtml && result.data.placeholderHtml.value) {
			this._renderContentTab(result.data.placeholderHtml.value);
		}
	}

	/* ########
	*	Tabs
	######## */

	_resetTabs() {
		this.querySelector(".js-tab-nav").innerHTML = "";
		this.querySelector(".js-tab-content").innerHTML = "";
	}

	_renderContentTab(placeholderHtml, isActive) {
		let content = document.createElement("div");
		content.setAttribute("id", `content-tab-pane`);
		content.setAttribute("role", "tabpanel");
		content.setAttribute("aria-labelledby", `content-tab`);
		content.setAttribute("tabindex", "0");
		content.classList.add("tab-pane");
		content.classList.add("fade");
		if (isActive) {
			content.classList.add("active");
			content.classList.add("show");
		}
		content.innerHTML = placeholderHtml;
		this.querySelector(".js-tab-content").prepend(content);
	}

	_renderTab(label, fields, isActive) {

		let name = label.replaceAll(" ", "-")
		let li = document.createElement("li");
		li.setAttribute("role", "presentation");
		li.classList.add("nav-item");
		li.classList.add("c-tab-item");
		li.innerHTML = `<button class="nav-link js-tab-nav-item ${isActive ? "active" : ""}" id="${name}-tab" data-bs-toggle="tab" data-bs-target="#${name}-tab-pane" type="button" role="tab" aria-controls="allgemein-tab-pane" aria-selected="true">${label}</button>`;
		this.querySelector(".js-tab-nav").prepend(li);

		let content = document.createElement("div");
		content.setAttribute("id", `${name}-tab-pane`);
		content.setAttribute("role", "tabpanel");
		content.setAttribute("aria-labelledby", `${name}-tab`);
		content.setAttribute("tabindex", "0");
		content.classList.add("tab-pane");
		content.classList.add("fade");
		if (isActive) {
			content.classList.add("active");
			content.classList.add("show");
		}
		content.innerHTML = this._getFieldsHtml(fields);
		this.querySelector(".js-tab-content").prepend(content);
	}

	_onTabNavClicked(e) {
		CoreStateStore.setValue(CoreStateStore.PAGE_EDITOR_SELECTED_TAB, e.currentTarget.id);
	}

	/* ########
	*	Save Item
	######## */

	_getSaveObject() {
		var result = {
			pageId: this._getCurrentItemId(),
			lang: this._getCurrentLanguage(),
			fields: []
		};
		const fields = this.querySelectorAll('.js-editor-field');
		fields.forEach((field) => {
			let info = field.getFieldValues();
			if (info !== undefined && !(info instanceof Array)) {
				result.fields.push(info);
			} else if (info instanceof Array) {
				info.forEach(ele => {
					result.fields.push(ele);
				});
			}
		});
		if (result.fields.length > 0) {
			return result;
		} else {
			return null;
		}
	}

	/* ########
	*	Load Item
	######## */

	_getLoadObject(id, type) {
		const lang = this._getCurrentLanguage();
		return { "id": id, "type": type, "lang": lang };
	}

	_onLoadItemSuccess(id, type, result) {
		if (id === result.data.id) {

			this._resetTabs();
			this._renderTabs(result, CoreStateStore.getValue(CoreStateStore.PAGE_EDITOR_SELECTED_TAB));

			this._renderPlaceholder(result, CoreStateStore.getValue(CoreStateStore.PAGE_EDITOR_SELECTED_TAB));

			this._setCurrentItemId(id, type);
			this._setCurrentLanguage(result.data.language);
			this._setPreviewUrl(result.data.url);

			this._addStateEventListener();
			CoreStateStore.setValue(CoreStateStore.EDITOR_EVENT_OPENED);
			this._updateLanguageButtons(result.data.language, result.data.languages);
			this.toggleVersionHint(false);
			this.show();
		} else {
			alert("error");
		}
		CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
	}

	_renderPlaceholder(result, activeTab) {
		if (result.data.placeholderHtml && result.data.placeholderHtml.value) {
			let li = document.createElement("li");
			li.setAttribute("role", "presentation");
			li.classList.add("nav-item");
			li.classList.add("c-tab-item");
			li.innerHTML = `<button class="nav-link js-tab-nav-item ${activeTab === "content-tab" ? "active" : ""}" id="content-tab" data-bs-toggle="tab" data-bs-target="#content-tab-pane" type="button" role="tab" aria-controls="allgemein-tab-pane" aria-selected="true">Content</button>`;
			this.querySelector(".js-tab-nav").prepend(li);
			this._renderContentTab(result.data.placeholderHtml.value, activeTab === "content-tab");
		}
	}

	_renderTabs(result, activeTab) {
		let tabs = CoreUtils.groupBy(result.data.definition, "tab");
		for (let tab in tabs) {
			let tabName = tab === "null" ? this.defaultTabName : tab;
			let tabId = tabName.replaceAll(" ", "-");
			this._renderTab(tabName, tabs[tab], activeTab === tabId + "-tab");
		}
	}

	_onLoadItemError(itemId, type, result) {
		if (result && result.data && result.data.id === itemId) {
			this._setCurrentItemId(itemId, type);
			this._updateLanguageButtons(result.data.language, result.data.languages);
			this.toggleVersionHint(true);
			this.show();
			CoreStateStore.setValue(CoreStateStore.EDITOR_EVENT_OPENED);
			CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
		}
	}

	/* ########
	*	Version
	######## */


	onCreateVersion() {
		CoreStateStore.setValue(CoreStateStore.START_LOADER);
		const pageId = this._getCurrentItemId();
		const lang = this._getCurrentLanguage();
		this.ajax({
			type: "GET",
			url: this.apiAddVersionUrl,
			data: { "pageId": pageId, "lang": lang },
			contentType: 'application/json'
		}, this.onLoadItemSuccess.bind(this, pageId), () => { });
	}

	resetEditor() {
		CoreStateStore.clearValue(CoreStateStore.FIELD_EVENT_CHANGED);
		this.isItemModified = false;
	}

	/* ########
	*	Export
	######## */

	onExportPage(e) {
		e.preventDefault();
		const pageId = this._getCurrentItemId();
		if (pageId) {
			this.ajax({
				type: "GET",
				url: `/api/admin/contentpackage/serialize`,
				data: {
					pageId: pageId
				},
				contentType: 'application/json'
			}, this.onExportSuccess.bind(this), () => { });
		}
	}

	onExportSuccess(data) {
		debugger;
	}

	/* ########
	*	Language Handling
	######## */

	_updateLanguageButtons(currentLanguage, pageLanguages) {
		console.log("_updateLanguageButtons: " + pageLanguages);

		if (currentLanguage) {
			this.querySelectorAll(`.js-change-lang`).forEach(ele => ele.classList.remove("active"));
			this.querySelector(`.js-change-lang[data-lang="${currentLanguage}"]`).classList.add("active");
			this.querySelector(`.js-current-lang`).textContent = currentLanguage;
		}

		if (pageLanguages) {
			this.querySelectorAll(`.js-change-lang`).forEach(ele => {
				if (pageLanguages.indexOf(ele.dataset["lang"]) > -1) {
					ele.classList.remove("h-hidden");
				} else {
					ele.classList.add("h-hidden");
				}
			});
			this.querySelectorAll(`.js-languages-to-add`).forEach(ele => {
				if (pageLanguages.indexOf(ele.value) == -1) {
					ele.parentNode.classList.remove("h-hidden");
				} else {
					ele.parentNode.classList.add("h-hidden");
				}
			});
		}
	}

	onChangeLanguage(e) {
		var lang = e.currentTarget.dataset.lang;
		this._setCurrentLanguage(lang);
		this.restoreItem();
	}

	_openAddLanguageDialog() {
		const modalDialog = this.parentNode.querySelector("core-modal");
		modalDialog.show(this.addLanguageModalConfirm.bind(this), () => { });
	}

	addLanguageModalConfirm() {
		var selection = this.querySelector('[name="languageVerion"]:checked');
		if (selection) {
			CoreStateStore.setValue(CoreStateStore.START_LOADER);
			const pageId = this._getCurrentItemId();
			this.ajax({
				type: "GET",
				url: "/api/core/contenteditor/addVersion",
				data: { "pageId": pageId, "lang": selection.value },
				contentType: 'application/json'
			}, this._onLoadItemSuccess.bind(this, pageId, CoreStateStore.getValue(CoreStateStore.PAGE_EDITOR_CONTENT_TYPE)), (result) => {
				alert(result.message);
				CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
			});
		}
	}

	/* ########
	*	altes Zeug?
	######## */


	/*
_fetchFromObject(obj, prop) {

	if (typeof obj === 'undefined') {
		return false;
	}

	if (obj === null) {
		return undefined;
	}

	var _index = prop.indexOf('.')
	if (_index > -1) {
		return this._fetchFromObject(obj[prop.substring(0, _index)], prop.substr(_index + 1));
	}

	return obj[prop];
}
*/
	/*
	_setPageDataToField(prop, data) {
		for (const propertyKey in prop) {
			const field = this.querySelector(`#${propertyKey}`);
			if (field) {
				field.value = data[propertyKey];
			} else if (typeof (data[propertyKey]) === "object") {
				this._setPageDataToField(propertyKey, data);
			}
		}
	}
	*/

}
window.customElements.define('page-editor', PageEditor);