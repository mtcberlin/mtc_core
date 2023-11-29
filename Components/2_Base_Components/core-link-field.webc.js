/// <reference path="../1_BaseField/core-field-base.webc.js" />

class CoreLinkField extends CoreFieldBase {
  _isInternalLink = false;

  disconnectedCallback() {
    // if (this.inputfld) {
    //   this.inputfld.removeEventListener("input", this._changeEvent.bind(this));
    // }
  }

  _readInfos() {
    super._readInfos();
    this._fieldValuesChanged = false;
    this._initLinkType();
  }

  _initNodes() {
    this.modalDialog = this.querySelector("core-modal");
    this._addButton = this.querySelector("#btn_add");
    this._editButton = this.querySelector("#btn_edit");
    this._deleteButton = this.querySelector("#btn_delete");

    this._updateButtons();
  }

  _updateButtons() {
    if (this.querySelector(".js_fld_link").value == "") {
      this._addButton.classList.remove("hidden");
      this._editButton.classList.add("hidden");
      this._deleteButton.classList.add("hidden");
    } else {
      this._addButton.classList.add("hidden");
      this._editButton.classList.remove("hidden");
      this._deleteButton.classList.remove("hidden");
    }
  }

  _addListener() {
    this._addButton.addEventListener("click", this._addLink.bind(this));
    this._editButton.addEventListener("click", this._addLink.bind(this));
    this._deleteButton.addEventListener("click", this._deleteLink.bind(this));

    var radios = this.querySelectorAll(
      `input[name=${this.fieldInfos.fieldName}_type]`
    );

    radios.forEach((el) => {
      el.addEventListener("change", this._linkTypeChange.bind(this));
    });
  }

  _initLinkType() {
    this._isInternalLink = this.fieldInfos.fieldValue.type == "0";
  }

  _updateLinkType() {
    this._isInternalLink =
      this.querySelector(
        `input[name=${this.fieldInfos.fieldName}_type]:checked`
      ).value == "0";
    this._updateLinkNode();
  }

  _updateLinkNode() {
    if (this._isInternalLink) {
      this.querySelector(`.js-fld-link-internal`).classList.remove("hidden");
      this.querySelector(`.js-fld-link-external`).classList.add("hidden");
    } else {
      this.querySelector(`.js-fld-link-external`).classList.remove("hidden");
      this.querySelector(`.js-fld-link-internal`).classList.add("hidden");
    }
  }

  _linkTypeChange() {
    this._updateLinkType();
  }

  resetState() {
    super.resetState();
    if (this.getFieldValues() != undefined) {
      this.fieldInfos.fieldValue = this.getFieldValues().fieldValue;
    }
  }

  _changeEvent() {
    this._setStateChanged();
  }

  _render() {
    let field = this.fieldInfos;
    this.innerHTML = `<div class='c-fld'>
			${this._renderMainPart(field)}
			</div>`;
    this._renderPreview();

    if (this._isInternalLink) {
      this.querySelector(`input[id=typ_internal]`).checked = true;
    } else {
      this.querySelector(`input[id=typ_external]`).checked = true;
    }
  }

  _renderMainPart(field) {
    let readOnly = "";
    if (field.readonly) {
      readOnly = "readonly=readonly";
    }
    const displayName =
      field.displayName === null ? field.fieldName : field.displayName;
    const value = field.fieldValue == "" ? "" : field.fieldValue;

    const mainValue = value.text == null ? "" : value.text;
    const simpleValue = value.textSimple == null ? "" : value.textSimple;
    const linkValue = value != "" && value.link != null ? value.link : "";
    const iconValue = value.icon == null ? "" : value.icon;
    const targetValue = value.target == null ? "" : value.target;

    return `
		<div class="js-text">
			<label class='form-label c-fld-label' for='${
        field.fieldName
      }'>${displayName}</label>
		</div>
		<div class="js-preview">
		</div>
		<div>
			<button id="btn_add" type="button" class="hidden btn btn-dark">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-plus-circle-fill me-2" viewBox="0 0 16 16">
          <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM8.5 4.5a.5.5 0 0 0-1 0v3h-3a.5.5 0 0 0 0 1h3v3a.5.5 0 0 0 1 0v-3h3a.5.5 0 0 0 0-1h-3v-3z"/>
        </svg>
        ${this.translate("Add Link")}
      </button>
			<button id="btn_edit" type="button" class="hidden btn btn-dark">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-pencil-square me-2" viewBox="0 0 16 16">
          <path d="M15.502 1.94a.5.5 0 0 1 0 .706L14.459 3.69l-2-2L13.502.646a.5.5 0 0 1 .707 0l1.293 1.293zm-1.75 2.456-2-2L4.939 9.21a.5.5 0 0 0-.121.196l-.805 2.414a.25.25 0 0 0 .316.316l2.414-.805a.5.5 0 0 0 .196-.12l6.813-6.814z"/>
          <path fill-rule="evenodd" d="M1 13.5A1.5 1.5 0 0 0 2.5 15h11a1.5 1.5 0 0 0 1.5-1.5v-6a.5.5 0 0 0-1 0v6a.5.5 0 0 1-.5.5h-11a.5.5 0 0 1-.5-.5v-11a.5.5 0 0 1 .5-.5H9a.5.5 0 0 0 0-1H2.5A1.5 1.5 0 0 0 1 2.5v11z"/>
        </svg>
        ${this.translate("Edit Link")}
      </button>
			<button id="btn_delete" type="button" class="hidden btn btn-dark">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash-fill me-2" viewBox="0 0 16 16">
          <path d="M2.5 1a1 1 0 0 0-1 1v1a1 1 0 0 0 1 1H3v9a2 2 0 0 0 2 2h6a2 2 0 0 0 2-2V4h.5a1 1 0 0 0 1-1V2a1 1 0 0 0-1-1H10a1 1 0 0 0-1-1H7a1 1 0 0 0-1 1H2.5zm3 4a.5.5 0 0 1 .5.5v7a.5.5 0 0 1-1 0v-7a.5.5 0 0 1 .5-.5zM8 5a.5.5 0 0 1 .5.5v7a.5.5 0 0 1-1 0v-7A.5.5 0 0 1 8 5zm3 .5v7a.5.5 0 0 1-1 0v-7a.5.5 0 0 1 1 0z"/>
        </svg>
        ${this.translate("Delete Link")}
      </button>
		</div>
		<core-modal title="Link">
			<div class="js-text">
				<label class='form-label c-fld-label' for='${
          field.fieldName
        }_typ'>${this.translate("Typ")}</label>
				<fieldset>
					<input class="js-type" type="radio" id="typ_internal" name="${
            field.fieldName
          }_type" value="0">
					<label for="typ_internal"> ${this.translate("internal Link")}</label> 
					<input class="js-type" type="radio" id="typ_external" name="${
            field.fieldName
          }_type" value="1">
					<label for="typ_external"> ${this.translate("external Link")}</label> 
				</fieldset>
			</div>	
			<div class="js-text">
				<label class='form-label c-fld-label' for='${field.fieldName}'>${this.translate(
      "Text"
    )}</label>
				<input class='form-control js-main-fld js_fld_text' type='text' id='${
          field.fieldName
        }_text' name='${
      field.fieldName
    }.Text' value='${mainValue}' ${readOnly} />
			</div>
			<div class="js-text">
				<label class='form-label c-fld-label' for='${field.fieldName}'>${this.translate(
      "Text"
    )} (${this.translate("Simple Language")})</label>
				<input class='form-control js-main-fld js_fld_textsimple' type='text' id='${
          field.fieldName
        }_textsimple' name='${
      field.fieldName
    }.TextSimple' value='${simpleValue}' ${readOnly} />
			</div>
			<div class="js-text js-fld-link-external">
				<label class='form-label c-fld-label' for='${field.fieldName}'>${this.translate(
      "Link Url"
    )}</label>
				<input class='form-control js-main-fld js_fld_link' type='text' id='${
          field.fieldName
        }_link' name='${
      field.fieldName
    }.Link' value='${linkValue}' ${readOnly} />
			</div>
			<div class="js-text js-fld-link-internal">
				<label class='form-label c-fld-label' for='${field.fieldName}'>${this.translate(
      "Link Internal"
    )}</label>
			</div>
			<div class="js-text">
				<label class='form-label c-fld-label' for='${field.fieldName}'>${this.translate(
      "Icon"
    )}</label>
				<input class='form-control js-main-fld js_fld_icon' type='text' id='${
          field.fieldName
        }_icon' name='${
      field.fieldName
    }.Icon' value='${iconValue}' ${readOnly} />
			</div>
			<div class="js-text">
				<label class='form-label c-fld-label' for='${field.fieldName}'>${this.translate(
      "Target"
    )}</label>
				<input class='form-control js-main-fld js_fld_target hidden' type='text' id='${
          field.fieldName
        }_target' name='${
      field.fieldName
    }.Target' value='${targetValue}' ${readOnly} />
				<divclass='form-control js_fld_select_target"></div>
				<select name="target_select" id="target_select">
					<option value="_blank">${this.translate("Link Target New Page")}</option>
					<option value="_self" selected>${this.translate(
            "Link Target Same Page"
          )}</option>
				</select>
			</div>
		</core-modal>
		`;
  }

  _renderSelectTree() {
    if (
      this.querySelector(".js-fld-link-internal core-selecttreeview") == null
    ) {
      this.querySelector(".js-fld-link-internal").insertAdjacentHTML(
        "beforeend",
        `<core-selecttreeview data-api-subtreeitems="/api/core/contenteditor/gettreesubitems" data-api-getpath="">` +
          `<core-selecttree-item type='folder' title='Root' type="root" id='11111111-1111-1111-1111-111111111111' data-has-subitems='true'></core-assettree-item>` +
          `</core-selecttreeview>`
      );
    }
  }

  _renderPreview() {
    const _previewNode = this.querySelector(".js-preview");

    _previewNode.innerHTML = `	
		${this._renderPreviewText()}
		${this._renderPreviewTextSimple()}
		${this._renderPreviewLink()}
		`;
  }

  _renderPreviewText() {
    const value_text = this.querySelector(".js_fld_text").value;
    if (value_text == "") {
      return "";
    }
    return `<div>Text: ${value_text}</div>`;
  }

  _renderPreviewTextSimple() {
    const value = this.querySelector(".js_fld_textsimple").value;
    if (value == "") {
      return "";
    }
    return `<div>Text (einfache Sprache): ${value}</div>`;
  }

  _renderPreviewLink() {
    const value = this.querySelector(".js_fld_link").value;
    if (value == "") {
      return "";
    }
    return `<div>Link: ${value}</div>`;
  }

  _addLink() {
    this._renderSelectTree();
    this._updateLinkNode();

    this.modalDialog.show(
      this._modalOk.bind(this),
      this._modalCancel.bind(this)
    );
  }

  _deleteLink() {
    //this.modalDialog.show(this._modalOk.bind(this), this._modalCancel.bind(this));
    this.querySelector(".js_fld_text").value = "";
    this.querySelector(".js_fld_textsimple").value = "";
    this.querySelector(".js_fld_link").value = "";
    this.querySelector(".js_fld_icon").value = "";
    this.querySelector(".js_fld_target").value = "";
    this._changeEvent();
    this._fieldValuesChanged = true;
    this._renderPreview();
    this._updateButtons();
  }

  _modalCancel() {
    console.log("LinkField: _modalCancel");
  }

  _modalOk() {
    this._updateLinkType();
    if (this._isInternalLink) {
      var tree = this.querySelector("core-selecttreeview");
      if (tree && tree.selected != "") {
        this.querySelector(".js_fld_link").value = tree.selected;
      }
    }

    this._changeEvent();
    this._fieldValuesChanged = true;
    this._renderPreview();
    this._updateButtons();
  }

  getFieldValues() {
    //dont need to change readoly fields
    if (this.fieldInfos.readonly) return;

    // check if the value changed
    //let currentValue = this.querySelector("input").value;
    //if (currentValue === this.fieldInfos.fieldValue) return;
    //if (this._fieldValuesChanged == false) return;

    return [
      {
        fieldValue: this.querySelector(`#${this.fieldInfos.fieldName}_text`)
          .value,
        fieldType: this.fieldInfos.fieldType,
        fieldPath: `${this.fieldInfos.fieldPath}.Text`,
      },
      {
        fieldValue: this.querySelector(
          `#${this.fieldInfos.fieldName}_textsimple`
        ).value,
        fieldType: this.fieldInfos.fieldType,
        fieldPath: `${this.fieldInfos.fieldPath}.TextSimple`,
      },
      {
        fieldValue: this.querySelector(`#${this.fieldInfos.fieldName}_icon`)
          .value,
        fieldType: this.fieldInfos.fieldType,
        fieldPath: `${this.fieldInfos.fieldPath}.Icon`,
      },
      {
        fieldValue: this.querySelector(`#${this.fieldInfos.fieldName}_link`)
          .value,
        fieldType: this.fieldInfos.fieldType,
        fieldPath: `${this.fieldInfos.fieldPath}.Link`,
      },
      {
        fieldValue: this.querySelector(
          `input[name=${this.fieldInfos.fieldName}_type]:checked`
        ).value,
        fieldType: this.fieldInfos.fieldType,
        fieldPath: `${this.fieldInfos.fieldPath}.Type`,
      },
      {
        fieldValue: this.querySelector(`#${this.fieldInfos.fieldName}_target`)
          .value,
        fieldType: this.fieldInfos.fieldType,
        fieldPath: `${this.fieldInfos.fieldPath}.Target`,
      },
    ];
  }
}
window.customElements.define("core-link-field", CoreLinkField);
