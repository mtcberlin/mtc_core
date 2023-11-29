/// <reference path="../../2_Base_Components/CoreTreeView/core.treeviewitem.webc.js" />

class CoreContentEditorTreeItem extends CoreTreeViewItem {

	constructor() {
		super();
		
		this._nodeName = "core-contenteditortree-item";
		this._rootNodeName = "core-contenteditortreeview";
		this._isDraggable = true;
	}

	_highlightItem( value, bubbleEvent = true){
		if(value == this.id){
			this._header.setAttribute("selected", this.id);
			if(bubbleEvent){
				this.dispatchEvent(this.eventSelected);
			}	
		}
	}

	_initActions(){
		CoreStateStore.subscribe(CoreStateStore.PAGE_EDITOR_CONTENT_ID, this._highlightItem.bind(this), this.id );
		CoreStateStore.subscribe(CoreStateStore.EDITOR_EVENT_SAVED, this._updateData.bind(this), this.id );
		
		super._initActions();
	}

	_disconnectActions() {
		//this.shadowRoot.querySelector(".js-add-page").removeEventListener('click', this.onAddPageClick.bind(this));
	}

	_initExpandedState(){
		super._initExpandedState();	
		var pageId = CoreStateStore.getValue(CoreStateStore.PAGE_EDITOR_CONTENT_ID);
		if(pageId != null && pageId == this.id){
			this._highlightItem(pageId, false);
		}
	}

	_initActionPage(){
		//this._actions.querySelector(".js-add-page").classList.remove("hidden");
	}

	_updateDataSuccess( result ){
		if( result.success && result.data != null ){
			console.log("_updateDataSuccess >>> " + result.data.name);
			this._nodeTitle.innerHTML = result.data.name;
		}
	}

	_onAddTreeItem(type) {
		//e.stopPropagation();
		CoreStateStore.setValue(CoreStateStore.START_LOADER);
		var lang = CoreStateStore.getValue(CoreStateStore.PAGE_EDITOR_CONTENT_LANG);
		this.ajax({
			type: "GET",
			url: "/api/core/contenteditor/addpage",
			data: { "parentId": this.id, "lang": lang, "type":type },
			contentType: 'application/json'
		}, this.onPageAddSuccess.bind(this), () => {
			CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
		});
	}

	onPageAddSuccess(data) {
		CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
		if (data.success) {
			this._refresh();
		}
	}

	select() {
		if(!this._header.hasAttribute("selected")){
			const state = CoreStateStore.setValue(CoreStateStore.PAGE_EDITOR_REQUEST_ID, { id: this.id, type: this._type }, false);
			if (!state.canceled) {
				//this._header.setAttribute("selected", this.id);
				//this.dispatchEvent(this.eventSelected);
			}	
		}
	}


}

window.customElements.define('core-contenteditortree-item', CoreContentEditorTreeItem);  