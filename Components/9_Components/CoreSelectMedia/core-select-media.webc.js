class CoreSelectMedia extends CoreHTMLElement {
  constructor() {
    super();
    this._title = "no title";
    this._btnNameAdd = this.translate("Add");
    this._btnNameDelete = this.translate("Delete");
    this._labelPreview = this.translate("Preview");
    this._selecttypes = "";
  }

  translate(key) {
    if (CoreTranslationService) {
      return CoreTranslationService.translate(key);
    }
    return key;
  }

  render() {
    this.insertAdjacentHTML(
      "beforeend",
      `
		<div class='mb-3 p-3 d-flex flex-column bg-light border rounded'>
			<div class="d-flex mb-3">
				<button type="button" class="btn btn-dark btn-add d-flex align-items-center justify-content-center me-3">
					<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-plus-circle-fill me-2" viewBox="0 0 16 16">
						<path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM8.5 4.5a.5.5 0 0 0-1 0v3h-3a.5.5 0 0 0 0 1h3v3a.5.5 0 0 0 1 0v-3h3a.5.5 0 0 0 0-1h-3v-3z"/>
					</svg>
					${this._btnNameAdd}
				</button>
				<button type="button" class="btn btn-dark btn-delete d-flex align-items-center justify-content-center">
					<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash-fill me-2" viewBox="0 0 16 16">
						<path d="M2.5 1a1 1 0 0 0-1 1v1a1 1 0 0 0 1 1H3v9a2 2 0 0 0 2 2h6a2 2 0 0 0 2-2V4h.5a1 1 0 0 0 1-1V2a1 1 0 0 0-1-1H10a1 1 0 0 0-1-1H7a1 1 0 0 0-1 1H2.5zm3 4a.5.5 0 0 1 .5.5v7a.5.5 0 0 1-1 0v-7a.5.5 0 0 1 .5-.5zM8 5a.5.5 0 0 1 .5.5v7a.5.5 0 0 1-1 0v-7A.5.5 0 0 1 8 5zm3 .5v7a.5.5 0 0 1-1 0v-7a.5.5 0 0 1 1 0z"/>
					</svg>
					${this._btnNameDelete}
				</button>
			</div>
      <core-media-preview id="mediapreview" class=""/>
		</div>
		<core-modal title="Select Media">
			<label for="image">Media</label>
			<div class="row">
				<div class="col">
					<core-selecttreeview data-selectable-types="${this._selecttypes}" data-api-subtreeitems="/api/core/assetlib/subitems" data-api-getpath="">
						<core-selecttree-item type='folder' data-selectable-types="${this._selecttypes}" title='Root' id='22222222-2222-2222-2222-222222222222' data-has-subitems='true'></core-assettree-item>
					</core-selecttreeview>
				</div>
				<div class="col">
					<div>${this._labelPreview}</div>
					<core-media-preview id="previewmodal"/>
				</div>
			</div>
		</core-modal>
        `
    );
  }

  connectedCallback() {
    setTimeout(() => {
      this.connectedCallbackStart();
    });
  }

  connectedCallbackStart() {
    if (this.getAttribute("data-fieldid") != null) {
      this._fieldid = this.getAttribute("data-fieldid");
      this._field = document.querySelector('input[id="' + this._fieldid + '"]');
    }
    //data-selectable-types
    if (this.hasAttribute("data-selectable-types")) {
      this._selecttypes = this.getAttribute("data-selectable-types");
    }
    this.render();
    this._initNodes();
    this._initListners();
  }

  _initNodes() {
    this.modalDialog = this.querySelector("core-modal");
    this.addButton = this.querySelector(".btn-add");
    this.deleteButton = this.querySelector(".btn-delete");
    this._mediaPreview = this.querySelector("#mediapreview");
    this._mediaPreviewModal = this.querySelector("#previewmodal");
    if (this._field.value && this._field.value !== "" && this._field.value !== this.guidEmpty) {
      this._mediaPreview.setAttribute("mediaid", this._field.value);
	  }
    this._updateButtonDisplay();
  }

  _initListners() {
    this.addButton.addEventListener("click", this._showAddDialog.bind(this));
    this.deleteButton.addEventListener("click", this._deleteMedia.bind(this));
    this.addEventListener("itemselected", (e) => {
      e.stopPropagation();
      this._previewMediaModal(e.detail.id());
    });
  }

  _deleteMedia() {
    if (this._field) {
      this._field.value = this.guidEmpty;
      this._mediaPreview.setAttribute("mediaid", this._field.value);
      this._field.dispatchEvent(new Event("change"));
      this._updateButtonDisplay();
    }
  }

  _previewMediaModal(id) {
    if (id != null) {
      this._mediaPreviewModal.setAttribute("mediaid", id);
    }
  }

  _hidePreview() {
    this._mediaPreview.classList.add("hidden");
  }

  _showPreview() {
    this._mediaPreview.classList.remove("hidden");
  }

  _showAddDialog() {
    this._getMediaPath();
  }

  _showDialog() {
    this.modalDialog.show(
      this._modalOk.bind(this),
      this._modalCancel.bind(this)
    );
  }

  _getMediaPath() {
    if (this._field.value == "") {
      this._showDialog();
    } else {
      CoreStateStore.setValue(CoreStateStore.START_LOADER);
      this.ajax(
        {
          type: "GET",
          url: "/api/admin/media/getassetidpath",
          data: { assetId: this._field.value },
          contentType: "application/json",
        },
        this._getMediaPathSuccess.bind(this),
        () => {
          CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
        }
      );
    }
  }

  _updateButtonDisplay(){
    if(this._field.value == "" || this._field.value == this.guidEmpty){
      this.show(".btn-add");
		  this.hide(".btn-delete");
    } else {
      this.hide(".btn-add");
      this.show(".btn-delete");  
    }
  }

  _getMediaPathSuccess(data) {
    var tree = this.querySelector("core-selecttreeview");
    if (data.parentIds) {
      tree.setAttribute("data-itempathids", data.parentIds.join("/"));
    }
    CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
    this._showDialog();
  }

  _modalCancel() {
    console.log("EditorAddComponent: cancel event raised");
  }

  _modalOk() {
    var tree = this.querySelector("core-selecttreeview");
    this._field.value = tree.selected;
    this._mediaPreview.setAttribute("mediaid", tree.selected);
    this._field.dispatchEvent(new Event("change"));
    this._updateButtonDisplay();
    this._resetTree();
  }

  _resetTree() {
    var tree = this.querySelector("core-selecttreeview");
    tree.reset();
  }
}
window.customElements.define("core-select-media", CoreSelectMedia);
