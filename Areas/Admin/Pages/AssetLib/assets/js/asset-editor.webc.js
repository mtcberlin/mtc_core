/// <reference path="../../../../../../Components/1_BaseField/base_editor.webc.js" />

class AssetEditor extends BaseEditor {

	apiPlaceholderUrl = "/admin/getPlaceholderHtml";
	apiAddVersionUrl = "/admin/contenteditor/addVersion";

	constructor() {
		super();
		CoreStateStore.subscribe(CoreStateStore.PAGE_EDITOR_REQUEST_ID, this.onLoadRequest.bind(this));
	}


	disconnectedCallback() {
	}

	initConfig(){
		super.initConfig();
		this.content = this.querySelector("#itemcontent");
	}

	/* ########
	*	Helper
	######## */

	_getCurrentLanguage(){
		return CoreStateStore.getValue(CoreStateStore.PAGE_EDITOR_CONTENT_LANG);
	}

	_setCurrentLanguage(lang){
		CoreStateStore.setValue(CoreStateStore.PAGE_EDITOR_CONTENT_LANG, lang);
	}

	_getCurrentItemId(){
		return CoreStateStore.getValue(CoreStateStore.ASSET_EDITOR_CONTENT_ID);
	}

	_setCurrentItemId(itemId){
		this.pageId = itemId;
		this.itemId = itemId;
		CoreStateStore.setValue(CoreStateStore.ASSET_EDITOR_CONTENT_ID, itemId);
	}

	/* ########
	*	Save Item
	######## */

	_getSaveObject() {
        var result = {
            pageId: this._getCurrentItemId(),
            lang: "de",
            version: 1,
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
        if(result.fields.length > 0){
            return result;
        } else {
            return null;
        }
    }

	/* ########
	*	Load Item
	######## */

	_getLoadObject(itemId) {
		return { "id": itemId };
	}
	
	onLoadRequest(asset, state) {
		if(!this.isItemModified){
			this.asset = asset;
			CoreStateStore.setValue(CoreStateStore.START_LOADER);
			this.loadItem(this.asset.assetId);
			this.resetEditor();	
		} else {
			if (confirm("Sie haben noch ungespeicherten Änderungen, möchsten Sie diese verwerfen?") == true) {
				CoreStateStore.setValue(CoreStateStore.START_LOADER);
				this.asset = asset;
				this.loadItem(this.asset.assetId);
				this.resetEditor();			
			} else {
				state.canceled = true;
			}
		}
	}

	_onLoadItemSuccess(itemId, type, result) {
		if (result && result.data && itemId === result.data.id) {
			this.itemId = itemId;
			if(result.data && result.data.definition){
				this.content.innerHTML = this._getFieldsHtml(result.data.definition);
			}

			CoreStateStore.setValue(CoreStateStore.ASSET_EDITOR_CONTENT_ID, itemId);
			CoreStateStore.setValue(CoreStateStore.EDITOR_EVENT_OPENED);
			this.show();
		} else {
			//alert("error");
		}
		CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
	}

	/* ########
	*	Delete Item
	######## */

	onItemDelete(e) {
		const assetId = CoreStateStore.getValue(CoreStateStore.ASSET_EDITOR_CONTENT_ID);
		if (assetId) {
			var result = confirm("Wollen Sie wirklich löschen?");
			if (result) {
				CoreStateStore.clearValue(CoreStateStore.ASSET_EDITOR_CONTENT_ID);
				window.location = e.currentTarget.dataset.deleteUrl + "&assetId=" + assetId;
			}
		}
	}


	onLoadItemError(pageId, result) {
		if (pageId === result.data.id) {
			this.querySelector(".js-tab-nav").innerHTML = "";
			this.querySelector(".js-tab-content").innerHTML = "";

			CoreStateStore.setValue(CoreStateStore.EDITOR_EVENT_OPENED);
			CoreStateStore.setValue(CoreStateStore.EDITOR_EVENT_WARNING, `Zu der gewählten Seite, existiert keine <b>${result.data.language}</b> Sprachversion. Bitte legen Sie eine an oder wechseln die Sprache.`);

			this._setCurrentItemId(itemId);
			this._setCurrentLanguage(result.data.language);

			this._updateLanguageButtons(result.data.language, result.data.languages);
			this.show();
		} else {
			alert("error");
		}
		CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
	}

	restoreItem() {
		const assetId = this._getCurrentItemId();
		if (assetId) {
			CoreStateStore.setValue(CoreStateStore.START_LOADER);
			this.loadItem(assetId);
		}
	}


	loadAssetData(pageId) {

	}

	onCreateVersion() {

	}

	_onAssetFrameSuccess(view) {
		this.querySelector(".js-version").classList.remove("h-hidden");
		this.querySelector("#AssetFields").innerHTML = view;
		CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
		this.querySelector("#image").addEventListener("change", this.uploadImage.bind(this));
		// this.querySelector(".js-delete-image").addEventListener("click", this._onImageRemove.bind(this));
	}

	onAssetDelete(e) {

	}
}
window.customElements.define('asset-editor', AssetEditor);