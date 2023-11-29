
class CoreAssetTreeItem extends CoreTreeViewItem {

	constructor() {
		super();
		this._nodeName = "core-assettree-item";
		this._rootNodeName = "core-assettreeview";
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
		CoreStateStore.subscribe(CoreStateStore.ASSET_EDITOR_CONTENT_ID, this._highlightItem.bind(this), this.id );
		CoreStateStore.subscribe(CoreStateStore.EDITOR_EVENT_SAVED, this._updateData.bind(this), this.id );

		super._initActions();
	}

	_initExpandedState(){
		super._initExpandedState();	
		var pageId = CoreStateStore.getValue(CoreStateStore.ASSET_EDITOR_CONTENT_ID);
		if(pageId != null && pageId == this.id){
			this._highlightItem(pageId, false);
		}
	}

	_createSubItem(subItem) {
		const item = document.createElement(this._nodeName);
		item.setAttribute("title", subItem.name);
		item.setAttribute("draggable", this._isDraggable);
		item.setAttribute("id", subItem.id);
		item.setAttribute("data-parentid", subItem.parentId);
		item.setAttribute("type", subItem.type);
		item.dataset.hasSubitems = subItem.hasSubItems;
		if(subItem.insertOptions){
			item.dataset.insertOptions = JSON.stringify(subItem.insertOptions);
		}
		this._content.append(item);
	}

	_onAddTreeItem(type) {
		if(type == "image"){
			this.onAddImage();
		} else if(type == "folder"){
			this.onAddFolder();
		} else if(type == "video"){
			this.onAddVideo();
		}
	}

	_disconnectActions() {
		// this.shadowRoot.querySelector(".js-add-img").removeEventListener('click', this.onAddImageClick.bind(this));
		// this.shadowRoot.querySelector(".js-add-folder").removeEventListener('click', this.onAddFolderClick.bind(this));
		//this.querySelector(".js-add-img").removeEventListener('click', this.onAddImageClick.bind(this));
		//this.querySelector(".js-add-folder").removeEventListener('click', this.onAddFolderClick.bind(this));
	}

	select() {
		//console.log("asset tree item SELECT: "+ this.id);
		if(!this._header.hasAttribute("selected")){
			const state = CoreStateStore.setValue("page-editor-request-id",{ "assetId": this.id, "assetType": this._type }, false);
			if (!state.canceled) {
				//this._header.setAttribute("selected", this.id);
				//this.dispatchEvent(this.eventSelected);
			}	
		}
	}

	_updateDataSuccess( result ){
		if( result.success && result.data != null ){
			//console.log("_updateDataSuccess >>> " + result.data.name);
			this._nodeTitle.innerHTML = result.data.name;
		}
	}


	onAddImage() {
		CoreStateStore.setValue(CoreStateStore.START_LOADER);

		this.ajax({
			type: "GET",
			url: "/api/core/assetlib/addimage",
			data: { "parentId": this.id },
			contentType: 'application/json'
		}, this.onImageAddSuccess.bind(this), () => {
			CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
		});
	}

	onImageAddSuccess(data) {
		CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
		if (data.success) {
			this._refresh();
		}
	}

	onAddFolder() {
		CoreStateStore.setValue(CoreStateStore.START_LOADER);

		this.ajax({
			type: "GET",
			url: "/api/core/assetlib/addfolder",
			data: { "parentId": this.id },
			contentType: 'application/json'
		}, this.onFolderAddSuccess.bind(this), () => {
			CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
		});
	}

	onFolderAddSuccess(data) {
		if (data.success) {
			this._hasSubItems = data.subItems.length > 0;
			this._renderSubItems(data.subItems);
		}

		CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
	}

	onAddVideo() {
		CoreStateStore.setValue(CoreStateStore.START_LOADER);

		this.ajax({
			type: "GET",
			url: "/api/core/assetlib/addvideo",
			data: { "parentId": this.id },
			contentType: 'application/json'
		}, this.onVideoAddSuccess.bind(this), () => {
			CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
		});
	}

	onVideoAddSuccess(data) {
		CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
		if (data.success) {
			this._refresh();
		}
	}

	reorderChildren(sourceId, targetId, position) {
		let newSortList = [];
		let i = 1;
		this._subItems.forEach((item) => {
			if(item.id === targetId) {
				if(position === "before") {
					newSortList.push({
						itemId: sourceId,
						parentId: this.id,
						sort: i
					});
					i++;
				}
				
				newSortList.push({
					itemId: targetId,
					parentId: this.id,
					sort: i
				});
				i++;

				if(position === "after") {
					newSortList.push({
						itemId: sourceId,
						parentId: this.id,
						sort: i
					});
					i++;
				}
			} else if(item.id !== sourceId) {
				newSortList.push({
					itemId: item.id,
					parentId: this.id,
					sort: i
				});
				i++;
			}
		});
		
		this.ajax(
			{
				type: "POST",
				url: "/api/core/assetlib/reorder",
				data: JSON.stringify({items: newSortList}),
				contentType: "application/json",
			},
			() => {
				window.location.reload();
			}, () => { }
		);
	}

}

window.customElements.define('core-assettree-item', CoreAssetTreeItem);  