class CoreCheckboxField extends CoreFieldBase {
  _render() {
    let field = this.fieldInfos;
    let readOnly = "";
    if (field.readonly) {
      readOnly = "readonly=readonly";
    }

    let checked = "";
    if (field.fieldValue) {
      checked = "checked=checked";
    }
    this.innerHTML = `<div class='c-fld mb-3'>
		<label class='form-label' for='${field.fieldName}'>${
      field.displayName === null ? field.fieldName : field.displayName
    }</label>
		<div class='form-control'><input type='checkbox' class='control' id='${
      field.fieldName
    }' name='${field.fieldName}' ${checked} ${readOnly} /></div>
		</div>`;
  }

  _initNodes() {
    this.inputfld = this.querySelector("input");
  }

  _addListener() {
    this.inputfld.addEventListener("input", this._changeEvent.bind(this));
  }

  // _changeEvent() {
  // 	this._setStateChanged();
  // }

  resetState() {
    super.resetState();
    this.fieldInfos.fieldValue = this.querySelector("input").checked;
  }

  getFieldValues() {
    //dont need to change readoly fields
    if (this.fieldInfos.readonly) return;

    // check if the value changed
    let currentValue = this.querySelector("input").checked;
    //if (currentValue === this.fieldInfos.fieldValue) return;

    return {
      fieldValue: currentValue.toString(),
      fieldType: this.fieldInfos.fieldType,
      fieldPath: this.fieldInfos.fieldPath,
    };
  }
}
window.customElements.define("core-checkbox-field", CoreCheckboxField);
