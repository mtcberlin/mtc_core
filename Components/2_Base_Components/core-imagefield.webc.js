class CoreImageFiedOld extends CoreFieldBase {

	constructor() {
		super();
		this.buttonName = "Add Image";
	}

	_initNodes(){
		if(this.hasAttribute('buttonname')){
			this.buttonName = this.getAttribute('buttonname');
		}
		if(this.hasAttribute('value')){
			this.value = this.getAttribute('value');
		} else {
			this.value = "";
		}
	}

	addListener() {
		console.log("addListener EditorImageFied");
		this.modalDialog = this.querySelector("core-modal");
		console.log(this.modalDialog);
		this.addButton = this.querySelector(".btn");
		this.addButton.addEventListener("click", this._showAddDialog.bind(this));
		//this.querySelector(".js-component-list").addEventListener("click", this._elementClicked.bind(this));

		this.addEventListener('itemselected', e => {
            console.log("EVENT: " + e.detail.id());
                e.stopPropagation(); 
            this._previewImage(e.detail.id());
          });

	}

	_render() {

		this.innerHTML = `
		<style>
		.row {
			display: flex;
		}  
		.column {
			flex: 50%;
		}
		</style>
		<button type="button" class="btn">
            ${this.buttonName}
        </button>
        <core-modal title="Add Image">
			<label for="image">Image</label>
			<div class="row">
				<div class="col">
					<core-selecttreeview data-selectable-types="image" data-api-subtreeitems="/api/core/assetlib/subitems" data-api-getpath="">
    			    	<core-selecttree-item type='folder' title='Root' id='22222222-2222-2222-2222-222222222222' data-has-subitems='true'></core-assettree-item>
					</core-selecttreeview>
				</div>
				<div class="col">
					<label for="image">Preview</label>
					<core-image-preview id="previewmodal"/>
				</div>
			</div>

		</core-modal>
		<input type="text" class="control js-editor-field" id="image" name="image" value="${this.value}">
		<core-image-preview id="previewimg" imgid="${this.value}"/>
        `;

	}

	_previewImage(id){
		var image = this.querySelector('#previewmodal');
		if(id != null){
			//image.show(id);
			image.setAttribute("imgid", id);
		}
	}

	_modalCancel() {
		console.log("EditorAddComponent: cancel event raised");
		this._resetTree();
	}

	_modalOk() {
		var imagefld = this.querySelector('#image');
		var tree = this.querySelector('core-selecttreeview');
		console.log("tree.selected: " + tree.selected);
		imagefld.value = tree.selected;

		var image = this.querySelector('#previewimg');
		if(image != null){
			image.setAttribute("imgid", tree.selected);
		}

		/*
		var selection = this.querySelector('[name="component"]:checked');
		if (selection) {
			var event = new CustomEvent(
				'addComponent', {
				bubbles: true,
				detail: { componentName: selection.value }
			});
			this.dispatchEvent(event);
		}*/
		this._resetTree();
	}

	_showAddDialog() {
		//this._showDialog();
		this._getMediaPath();
	}

	_showDialog(){
		this.modalDialog.show(this._modalOk.bind(this), this._modalCancel.bind(this));
	}

	_getMediaPath(){

		var imagefld = this.querySelector('#image');
		
		if(imagefld.value == ""){
			this._showDialog();
		} else {
			CoreStateStore.setValue(CoreStateStore.START_LOADER);
			this.ajax({
				type: "GET",
				url: "/api/admin/media/getassetidpath",
				data: { "assetId": imagefld.value },
				contentType: 'application/json'
			}, this._getMediaPathSuccess.bind(this), () => {
				CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
			});	
		}
	}

	_getMediaPathSuccess(data){
		console.log(data);
		var tree = this.querySelector('core-selecttreeview');
		if(data.parentIds){
			tree.setAttribute("data-itempathids", data.parentIds.join("/"));
		}
		CoreStateStore.setValue(CoreStateStore.STOP_LOADER);	
		this._showDialog();
	}

	_resetTree(){
		var tree = this.querySelector('core-selecttreeview');
		tree.reset();
	}

	_elementClicked(event){
		/*
		var li = event.target.closest("li");
		if(li){
			var radio = li.querySelector("input");
			radio.checked = true;
			if(this.querySelector(".selected")){
				this.querySelector(".selected").classList.remove("selected");
			}
			li.classList.add("selected");	
		}
		*/
		event.stopPropagation();
	}


}
window.customElements.define('core-image-field-old', CoreImageFiedOld);