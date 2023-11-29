/// <reference path="../0_Base/corestatestore.webc.js" />
/// <reference path="../0_Base/corehtmlelement.webc.js" />

class BaseEditor extends CoreHTMLElement {

	saveBtn;
	messages;
	isItemModified = false;
	_loadItemUrl = "";
	_saveItemUrl = "";
	_labels={
		lbl_easyLang:"Easy language",
		lbl_dgs:"Sign language Video",
	};

	constructor() {
		super();
		CoreStateStore.clearValue(CoreStateStore.FIELD_EVENT_CHANGED);
		CoreStateStore.subscribe(CoreStateStore.FIELD_EVENT_CHANGED, this._fieldChanged.bind(this));
	}


	/* ########
	*	WC Events
	######## */

	connectedCallback() {
		// workaround for chrom and safari
		// https://stackoverflow.com/questions/62962138/how-to-get-the-contents-of-a-custom-element
		setTimeout(() => {
			this.initConfig();
			this.restoreItem();
			this.addListener();
		}, 100);
	}

	disconnectedCallback() {
		CoreStateStore.clearValue(CoreStateStore.FIELD_EVENT_CHANGED);
	}

	/* ########
	*	Init
	######## */

	initConfig() {
		console.log("BaseEditor initConfig");
		if (this.hasAttribute("data-url-load")) {
			this._loadItemUrl = this.getAttribute("data-url-load");
		}
		if (this.hasAttribute("data-url-save")) {
			this._saveItemUrl = this.getAttribute("data-url-save");
		}
	}

	addListener() {
		this.saveBtn = this.querySelector("#SavePage");
		this.saveBtn.addEventListener("click", this._onSaveEvent.bind(this));
		this.querySelector("#DeleteBtn").addEventListener("click", this.onItemDelete.bind(this));
	}


	/* ########
	*	Item Handling
	######## */

	_setItemModified() {
		console.log("_setItemModified");
		CoreStateStore.setValue(CoreStateStore.EDITOR_EVENT_MODIFIED);
	}

	restoreItem() {
		console.log("BaseEditor restoreItem")
		const pageId = CoreStateStore.getValue(CoreStateStore.PAGE_EDITOR_CONTENT_ID);
		const type = CoreStateStore.getValue(CoreStateStore.PAGE_EDITOR_CONTENT_TYPE);
		if (pageId && type) {
			CoreStateStore.setValue(CoreStateStore.START_LOADER);
			this.loadItem(pageId, type);
		}
	}



	/* ########
	*	Field Handling
	######## */

	_getFieldsHtml(fields) {
		let sorted = fields.sort((a, b) => {return a.sort - b.sort});
		let sections = CoreUtils.groupBy(sorted, "section");
		let resultHtml = "";
		for (let section in sections) {
			resultHtml += this._getSectionHtml(sections[section]);
		}

		return resultHtml;
	}

	_getSectionHtml(fields) {
		const sectionName = fields[0].section === null ? this.defaultSectionName : fields[0].section;
		fields.sort((a, b) => {
			return a.sort < b.sort ? -1 : 1;
		});
		let outerHtmlStrat = `<core-panel expanded='true' title='${sectionName}'><div slot='content'>`;
		let fieldsHtml = "";
		let outerHtmlEnd = "</div></core-panel>";
		fields.forEach(field => {
			if (field.selectType === "radio") {
				fieldsHtml += this._getRadioFieldHtml(field);
			} else if (field.fieldType === "string" || field.fieldType === "guid") {
				fieldsHtml += this._getStringFieldHtml(field);
			} else if (field.fieldType === "range") {
				fieldsHtml += this._getRangeFieldHtml(field);
			} else if (field.fieldType === "richtext") {
				fieldsHtml += this._getRichTextFieldHtml(field);
			} else if (field.fieldType === "linkfield") {
				fieldsHtml += this._getLinkFieldHtml(field);
			} else if (field.fieldType === "boolean") {
				fieldsHtml += this._getCheckboxFieldHtml(field);
			} else if (field.fieldType === "image") {
				fieldsHtml += this._getImageHtml(field);
			} else if (field.fieldType === "imagefield") {
				fieldsHtml += this._getImageSelectHtml(field);
			} else if (field.fieldType === "video") {
				fieldsHtml += this._getVideoFieldHtml(field);
			} else if (field.fieldType === "file") {
				fieldsHtml += this._getFieldHtml(field);
			} else {
				console.log(`No renderer found for type ${field.fieldType}`);
			}
		})
		return outerHtmlStrat + fieldsHtml + outerHtmlEnd;
	}

	_getStringFieldHtml(field) {
		return `<core-string-field class="js-editor-field" data-labels='${JSON.stringify(this._labels)}' data-data='${JSON.stringify({definition: field})}'></core-string-field>`;
	}

	_getRangeFieldHtml(field) {
		return `<core-range-field class="js-editor-field" data-labels='${JSON.stringify(this._labels)}' data-data='${JSON.stringify({definition: field})}'></core-range-field>`;
	}

	_getLinkFieldHtml(field) {
		return `<core-link-field class="js-editor-field" data-labels='${JSON.stringify(this._labels)}' data-data='${JSON.stringify({definition: field})}'></core-link-field>`;
	}

	_getRichTextFieldHtml(field) {
		return `<core-richtext-field class="js-editor-field" data-labels='${JSON.stringify(this._labels)}' data-data='${JSON.stringify({definition: field})}'></core-richtext-field>`;
	}

	_getCheckboxFieldHtml(field) {
		return `<core-checkbox-field class="js-editor-field" data-labels='${JSON.stringify(this._labels)}' data-data='${JSON.stringify({definition: field})}'></core-checkbox-field>`;
	}

	_getRadioFieldHtml(field) {
		return `<core-radio-field class="js-editor-field" data-labels='${JSON.stringify(this._labels)}' data-data='${JSON.stringify({definition: field})}'></core-radio-field>`;
	}

	_getImageHtml(field) {
		return `<core-image-field class="js-editor-field" data-labels='${JSON.stringify(this._labels)}' data-data='${JSON.stringify({definition: field})}' data-itemid='${this.itemId}'></core-image-field>`;
	}

	_getImageSelectHtml(field) {
		return `<core-imagefield-edit class="js-editor-field" data-data='${JSON.stringify({definition: field, values: field.fieldValue})}' data-itemid='${this.itemId}'></core-imagefield-edit>`;
	}

	_getVideoFieldHtml(field) {
		return `<core-video-field class="js-editor-field" data-data='${JSON.stringify({definition: field})}' data-itemid='${this.itemId}'></core-video-field>`;
	}

	_getFieldHtml(field) {
		return `<core-file-field class="js-editor-field" data-data='${JSON.stringify({definition: field})}' data-itemid='${this.itemId}'></core-file-field>`;
	}

	_fieldChanged(obj) {
		if (obj && obj.value.length > 0) {
			if (this.isItemModified == false) {
				console.log("_fieldChanged set modified");
				this.saveBtn.removeAttribute("disabled");
				this._setItemModified();
				this.isItemModified = true;
			}
		} else {
			console.log("_fieldChanged remove modified");
			this.saveBtn.setAttribute("disabled","");
			this.isItemModified = false;
			CoreStateStore.setValue(CoreStateStore.EDITOR_EVENT_CLEARED, this.itemId);
		}
	}


	/* ########
	*	Delete Item
	######## */

	onItemDelete(e) {
		const pageId = CoreStateStore.getValue(CoreStateStore.PAGE_EDITOR_CONTENT_ID);
		if (pageId) {
			CoreStateStore.clearValue(CoreStateStore.PAGE_EDITOR_CONTENT_ID);
			var result = confirm("Wollen Sie diese und alle unterseiten löschen?");
			if (result) {
				window.location = e.currentTarget.dataset.deleteUrl + "&pageId=" + pageId;
			}
		}
	}

	/* ########
	*	Reset Item
	######## */

	resetEditor() {
		CoreStateStore.clearValue(CoreStateStore.FIELD_EVENT_CHANGED);
		CoreStateStore.clearValue(CoreStateStore.EDITOR_EVENT_SAVEFAILED);
		CoreStateStore.setValue(CoreStateStore.EDITOR_EVENT_CLOSED);
		this.isItemModified = false;
	}

	/* ########
	*	Load Item
	######## */

	loadItem(id, type) {
		console.log("loadItem -> _loadItemUrl: " + this._loadItemUrl);
		this.ajax({
			type: "GET",
			url: this._loadItemUrl,
			data: this._getLoadObject(id, type),
			contentType: 'application/json'
		}, this._onLoadItemSuccess.bind(this, id, type), this._onLoadItemError.bind(this, id, type));
	}

	_getLoadObject(id, type) {
		return { "id": id, "type": type };
	}

	onLoadRequest(item, state) {
		if (!this.isItemModified) {
			CoreStateStore.setValue(CoreStateStore.START_LOADER);
			this.loadItem(item.id, item.type);
			this.resetEditor();
		} else {
			if (confirm("Sie haben noch ungespeicherten Änderungen, möchsten Sie diese verwerfen?") == true) {
				CoreStateStore.setValue(CoreStateStore.START_LOADER);
				this.loadItem(item.id, item.type);
				this.resetEditor();
			} else {
				state.canceled = true;
			}
		}
	}

	_onLoadItemSuccess(itemId, type, result) {
		CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
	}

	_onLoadItemError(itemId, type, result) {
		CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
	}

	/* ########
	*	Save Item
	######## */

	_onSaveEvent(e) {
		this._saveItem(this._onSaveItemSuccess.bind(this));
	}

	_saveItem(cb_success) {
		let saveObj = this._getSaveObject();
		if (saveObj != null) {
			this.ajax({
				type: "POST",
				url: this._saveItemUrl,
				data: JSON.stringify(saveObj),
				contentType: 'application/json'
			}, cb_success, () => { this.saveBtn.error() });
		}
	}

	_getSaveObject() {
		return null;
	}

	_onSaveItemSuccess(result) {
		if (result.success == true) {
			CoreStateStore.clearValue(CoreStateStore.FIELD_EVENT_CHANGED);
			if (this.isItemModified == true) {
				CoreStateStore.setValue(CoreStateStore.EDITOR_EVENT_SAVED, this.itemId);
				this.isItemModified = false;
			}
			this.saveBtn.setAttribute("disabled", "");
		} else {
			CoreStateStore.setValue(CoreStateStore.EDITOR_EVENT_SAVEFAILED, this.itemId);
		}
	}

	toggleVersionHint(noVersion) {
		this.querySelector(".js-no-content").classList.toggle("h-hidden", !noVersion);
		this.querySelector(".js-version").classList.toggle("h-hidden", noVersion);
	}

}
window.customElements.define('base-editor', BaseEditor);