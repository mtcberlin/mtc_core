class CoreFileField extends CoreFieldBase {
  _apiUpload = "/api/core/assetlib/upload/file";

  constructor() {
    super();
    CoreStateStore.subscribe(
      CoreStateStore.EDITOR_EVENT_SAVED,
      this._showAndRegisterDeleteBtn.bind(this),
      this.id
    );
    this.itemId = this.dataset.itemid;
  }

  disconnectedCallback() {
    super.disconnectedCallback();
  }

  _addListener() {
    this.fld = this.querySelector(`#input-${this.id}`);
    this.fld.addEventListener("change", this._uploadFile.bind(this));
    this._showAndRegisterDeleteBtn();
  }

  _showAndRegisterDeleteBtn() {
    const deleteBtns = this.querySelectorAll(".js-delete-file");
    if (deleteBtns.length > 0) {
      deleteBtns.forEach((btn) => {
        this._registerOnBtnDelete(btn);
        btn.classList.remove("hidden");
      });
    }
  }

  _registerOnBtnDelete(btn) {
    if (!btn.classList.contains("has-listener")) {
      btn.addEventListener("click", this._onImageRemove.bind(this));
      btn.classList.add("has-listener");
    }
  }

  _uploadFile() {
    var input = document.getElementById(`input-${this.id}`);
    var files = input.files;
    const file = files[0];
    var formData = new FormData();
    formData.append("id", this.itemId);
    formData.append("file", file);

    this.ajax(
      {
        type: "POST",
        headers: {
          "content-length:": file.size,
        },
        url: this._apiUpload,
        data: formData,
      },
      this._onUploadSuccess.bind(this),
      this._onUploadFailed.bind(this),
      this._onUploadProgressStart.bind(this, file),
      this._onUploadProgress.bind(this),
      this._onUploadProgressFinish.bind(this)
    );
  }

  _onUploadProgressStart(file, event) {}

  _onUploadSuccess(result) {
    if (result.success) {
      this.querySelector(".js-file-input").classList.add("h-hidden");

      const fileView = this.querySelector(".js-file-view");
      fileView.querySelector(".js-file-name").innerText = result.fileName;
      fileView.classList.remove("h-hidden");
      document.getElementById(`input-${this.id}-fileName`).value =
        result.fileName;
      this.querySelector(".js-fileinput").value = "";
      this._setStateChanged();
    }
  }

  _onUploadFailed() {}

  _onUploadProgress(e) {}

  _onUploadProgressFinish() {}

  _changeEvent() {
    this._setStateChanged();
  }

  _render() {
    let field = this.fieldInfos;
    let readOnly = "";
    if (field.readonly) {
      readOnly = "readonly=readonly";
    }

    this.innerHTML = `
			<div class="c-fld mx-5">
				<label class="fw-bold">${
          field.displayName === null ? field.fieldName : field.displayName
        }</label>
				${this._renderUploadContainer(field)}
				${this._renderUploadBtn(field)}
			</div>`;
  }

  _renderUploadBtn(field) {
    return `<div class="fld-group pt-3 pb-3 js-file-input ${
      field.fieldValue === "" ? "" : "h-hidden"
    }">
					<input type="button" value="${this.translate(
            "Select File"
          )}" onclick="document.getElementById('input-${this.id}').click();" />
					<input type="file" accept=".vtt" class="control js-fileinput hidden" id="input-${
            this.id
          }" name="${this.id}" />
					<input type="hidden" class="control" id="input-${this.id}-fileName" value='${
      field.fieldValue
    }' />
				</div>`;
  }

  _renderUploadContainer(field) {
    return `
		<div class="fld-group js-file-view ${
      field.fieldValue === "" ? "h-hidden" : ""
    }">
			<div class="d-flex flex-column flex-md-row js-vod-node mb-3">
				<div class="col bg-light p-4 me-md-1">
					<div class="d-flex flex-column flex-md-row">
						<svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" fill="currentColor" class="bi bi-file-earmark-text" viewBox="0 0 16 16">
							<path d="M5.5 7a.5.5 0 0 0 0 1h5a.5.5 0 0 0 0-1h-5zM5 9.5a.5.5 0 0 1 .5-.5h5a.5.5 0 0 1 0 1h-5a.5.5 0 0 1-.5-.5zm0 2a.5.5 0 0 1 .5-.5h2a.5.5 0 0 1 0 1h-2a.5.5 0 0 1-.5-.5z"/>
							<path d="M9.5 0H4a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2V4.5L9.5 0zm0 1v2A1.5 1.5 0 0 0 11 4.5h2V14a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1h5.5z"/>
						</svg>
						<div class="mt-2 mt-md-1 ms-md-4">
							<div>Name: <span class="js-file-name">${field.fieldValue}</span></div>
						</div>
					</div>
				</div>
					
				<button class="btn btn-light rounded-0 px-4 ms-md-1 mt-2 mt-md-0 hidden js-delete-file">
					<img src="/admin/img/icons/icon_trash.png" height="24" weight="24" alt="trash icon">
				</button>
			</div>
		</div>`;
  }

  getFieldValues() {
    if (this.fieldInfos.readonly) return;

    return {
      fieldValue: document.getElementById(`input-${this.id}-fileName`).value,
      fieldType: this.fieldInfos.fieldType,
      fieldPath: this.fieldInfos.fieldPath,
    };
  }

  _onImageRemove() {
    this.querySelector(".js-file-view").classList.add("h-hidden");
    this.querySelector(".js-file-input").classList.remove("h-hidden");
    document.getElementById(`input-${this.id}-fileName`).value = "";
    this._setStateChanged();
  }
}
window.customElements.define("core-file-field", CoreFileField);
