class CoreImageField extends CoreFileField {
  _addListener() {
    this.querySelector(`#input-${this.id}`).addEventListener(
      "change",
      this._uploadFile.bind(this)
    );
    this.querySelector(".js-delete-image").addEventListener(
      "click",
      this._onImageRemove.bind(this)
    );
  }

  _uploadFile() {
    var input = document.getElementById(`input-${this.id}`);
    var files = input.files;
    var formData = new FormData();
    formData.append("id", this.itemId);
    formData.append("file", files[0]);

    this.ajax(
      {
        type: "POST",
        url: this._apiUpload,
        data: formData,
      },
      this._onUploadSuccess.bind(this),
      this._onUploadFailed.bind(this)
    );
  }

  _onUploadSuccess(result) {
    if (result.success) {
      this.querySelector(".js-img-input").classList.add("d-none");
      this.querySelector(".js-img-view img").src = result.imgPath;
      this.querySelector(".js-file-type").innerText = result.mimeType;
      this.querySelector(".js-file-size").innerText = result.fileSize;
      this.querySelector(".js-img-view").classList.remove("d-none");
      this.querySelector(".js-delete-image").classList.remove("d-none");
      document.getElementById(`input-${this.id}-fileName`).value =
        result.fileName;
      this.querySelector(".js-fileinput").value = "";
      this._setStateChanged();
    }
  }

  _onUploadFailed() {}

  _render() {
    let field = this.fieldInfos;
    let readOnly = "";
    if (field.readonly) {
      readOnly = "readonly=readonly";
    }

    this.innerHTML = this._renderBlock(field);

    if (field.fieldValue == "") {
      this.querySelector(".js-img-view").classList.add("d-none");
      this.querySelector(".js-img-input").classList.remove("d-none");
      this.querySelector(".js-delete-image").classList.add("d-none");
    } else {
      this.querySelector(".js-img-view").classList.remove("d-none");
      this.querySelector(".js-img-input").classList.add("d-none");
      this.querySelector(".js-delete-image").classList.remove("d-none");
    }
  }

  _renderBlock(field) {
    return `<div class="d-flex flex-column flex-md-row c-fld border border-dark rounded mb-4">

					<div class="fld-group js-img-input d-flex align-items-center justify-content-center ${
            field.fieldValue === "" ? "" : "d-none"
          }">
						<input type="button" class="btn btn-dark mt-3 ms-3" value="${this.translate(
              "Select Image"
            )}" onclick="document.getElementById('input-${this.id}').click();" 
            />
						<input type="file" accept=".png,.jpg,.jpeg" class="control d-none js-fileinput" id="input-${
              this.id
            }" name="${this.id}" />
						<input type="hidden" class="control" id="input-${this.id}-fileName" value='${
      field.fieldValue
    }' />
					</div>

					<div class="d-flex justify-content-between col bg-light p-4 rounded js-img-view">
						<div class="d-flex flex-column flex-md-row">

							<div class="me-md-4" style="height: 200px">
								<img style="height: 100%" src='/uploads/${this.itemId}/${field.fieldValue}' />
							</div>

							<div class="mt-2 mt-md-0">

								<div class="d-flex flex-column mb-1">
									<div class="bg-dark-gray text-white rounded-top fw-bold px-2 py-1 fs-sm">
										${this.translate("Name")} 
									</div>
									<div class="bg-white rounded-bottom border border-dark-gray px-2 py-1">
										${field.fieldValue === "" ? "" : field.fieldValue}
									</div>
								</div>

								<div class="d-flex flex-column mb-1">
									<div class="bg-dark-gray text-white rounded-top fw-bold px-2 py-1 fs-sm">
										${this.translate("Typ")}
									</div>
									<div class="bg-white rounded-bottom border border-dark-gray px-2 py-1">
										<span class="js-file-type"></span>
									</div>
								</div>

								<div class="d-flex flex-column mb-1">
									<div class="bg-dark-gray text-white rounded-top fw-bold px-2 py-1 fs-sm">
										${this.translate("Size")}
									</div>
									<div class="bg-white rounded-bottom border border-dark-gray px-2 py-1">
									<span class="js-file-size"> MB</span>
									</div>
								</div>
								
							</div>
						
						</div>
						<button class="d-flex btn btn-light rounded-0 ms-md-1 mt-2 mt-md-0 js-delete-image" data-filename="">
							<img src="/admin/img/icons/icon_trash.png" height="24" weight="24" alt="trash icon">
						</button>
					</div>
						

				</div>`;
  }

  getFieldValues() {
    if (this.fieldInfos.readonly) return;

    // check if the value changed
    var fldname = `#input-${this.id}-fileName`;
    let currentValue = this.querySelector(fldname).value;

    return {
      fieldValue: currentValue,
      fieldType: this.fieldInfos.fieldType,
      fieldPath: this.fieldInfos.fieldPath,
    };
  }

  _onImageRemove() {
    this.querySelector(".js-img-view").classList.add("d-none");
    this.querySelector(".js-img-input").classList.remove("d-none");
    document.getElementById(`input-${this.id}-fileName`).value = "";
    this.querySelector(".js-delete-image").classList.add("d-none");
    this._setStateChanged();
  }
}
window.customElements.define("core-image-field", CoreImageField);
