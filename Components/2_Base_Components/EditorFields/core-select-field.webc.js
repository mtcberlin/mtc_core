class CoreSelectField extends CoreFieldBase {
  // _additionalFields = {
  //     'SimpleText':{'fldtype':'text','valuetype':'string','label':'Simple Language'},
  //     'dgsVideo':{'fldtype':'text','valuetype':'string','label':'Sign language video'}
  // };

  disconnectedCallback() {
    // this.inputfld.removeEventListener("input", this._changeEvent.bind(this));
  }

  _readInfos() {
    super._readInfos();
    this.hasAdditionalFields = false;
  }

  _initNodes() {
    super._initNodes();

    this.inputfld = this.querySelector(".js-text input");

    if (this.hasAdditionalFields) {
      for (const fld in this._additionalFields) {
        if (this._additionalFields.hasOwnProperty(fld)) {
          this._additionalFields[fld].fld = new CoreAdditionalField(
            fld,
            this._additionalFields[fld].label,
            this._additionalFields[fld].fldtype,
            this._additionalFields[fld].valuetype,
            this.fieldInfos,
            this._menuSlot,
            this._additionalFieldsSlot,
            this._setStateChanged.bind(this)
          );
          this._additionalFields[fld].fld.renderMenuEntry();
          this._additionalFields[fld].fld.renderField();
          this._additionalFields[fld].fld.init();
        }
      }
    }
  }

  _addListener() {
    this.inputfld.addEventListener("input", this._changeEvent.bind(this));
  }

  resetState() {
    super.resetState();
    if (this.getFieldValues() != undefined) {
      if (this.hasAdditionalFields == false) {
        this.fieldInfos.fieldValue = this.getFieldValues().fieldValue;
      } else {
        // reset bei additonal fields
        for (const fld in this._additionalFields) {
          if (this._additionalFields.hasOwnProperty(fld)) {
            if (this._additionalFields[fld].fld != undefined) {
              this._additionalFields[fld].fld.resetState();
            }
          }
        }
      }
    }
  }

  // _render() {
  //     super._render();
  //  }

  _renderField(field) {
    let readOnly = "";
    if (field.readonly) {
      readOnly = "readonly=readonly";
    }

    const displayName =
      field.displayName === null ? field.fieldName : field.displayName;
    const value =
      field.hasAdditionalFields && field.fieldValue
        ? field.fieldValue.value
        : field.fieldValue;

    return `
        <div class="js-text c-fld-main">
            <label class='form-label c-fld-label' for='${
              field.fieldName
            }'>${displayName}</label>
            <input class='form-control js-main-fld ' type='text' id='${
              field.fieldName
            }' name='${field.fieldName}.Value' value='${
      value == null ? "" : value
    }' ${readOnly} />
        </div>
        `;
  }

  getFieldValues() {
    if (this.fieldInfos.readonly) return;

    if (this.hasAdditionalFields) {
      var result = [
        {
          fieldValue: this.querySelector(`#${this.fieldInfos.fieldName}`).value,
          fieldType: this.fieldInfos.fieldType,
          fieldPath: `${this.fieldInfos.fieldPath}.Value`,
        },
      ];

      for (const fld in this._additionalFields) {
        if (this._additionalFields.hasOwnProperty(fld)) {
          result.push(this._additionalFields[fld].fld.getValue());
        }
      }

      return result;
    } else {
      let currentValue = this.querySelector("input").value;
      return {
        fieldValue: currentValue,
        fieldType: this.fieldInfos.fieldType,
        fieldPath: this.fieldInfos.fieldPath,
      };
    }
  }
}
window.customElements.define("core-select-field", CoreSelectField);
