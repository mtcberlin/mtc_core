class CoreRangeField extends CoreFieldBase {
  _additionalFields = {};

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
  }

  _addListener() {
    this.inputfld.addEventListener("input", this._changeEvent.bind(this));
  }

  resetState() {
    super.resetState();
    if (this.getFieldValues() != undefined) {
      if (this.hasAdditionalFields == false) {
        this.fieldInfos.fieldValue = this.getFieldValues().fieldValue;
      }
    }
  }

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
    console.dir(this.fieldInfos);
    const rangeConfig =
      this.fieldInfos.fieldConfig != null
        ? JSON.parse(this.fieldInfos.fieldConfig)
        : null;
    // <input class='form-control js-main-fld' type='text' id='${field.fieldName}' name='${field.fieldName}.Value' value='${value == null ? "" : value}' ${readOnly} />
    if (rangeConfig != null) {
      return `
            <div class="js-text c-fld-main">
                <label class='form-label c-fld-label' for='${
                  field.fieldName
                }'>${displayName}</label>
                <input type="range" class="form-range form-control js-main-fld" min="${
                  rangeConfig.min
                }" max="${rangeConfig.max}" step="${rangeConfig.step}" id="${
        field.fieldName
      }" name='${field.fieldName}.Value' value='${
        value == null ? "" : value
      }' ${readOnly} >
            </div>
            `;
    } else {
      return `
            <div class="js-text c-fld-main">
                <label class='form-label c-fld-label' for='${
                  field.fieldName
                }'>${displayName}</label>
                <input type="text" class="form-control js-main-fld" id="${
                  field.fieldName
                }" name='${field.fieldName}.Value' value='${
        value == null ? "" : value
      }' ${readOnly} >
            </div>
            `;
    }
  }

  getFieldValues() {
    if (this.fieldInfos.readonly) return;

    // if (this.hasAdditionalFields) {
    //     var result = [{
    //         fieldValue: this.querySelector(`#${this.fieldInfos.fieldName}`).value,
    //         fieldType: this.fieldInfos.fieldType,
    //         fieldType: "string",
    //         fieldPath: `${this.fieldInfos.fieldPath}.Value`
    //     }];

    //     for (const fld in this._additionalFields) {
    //         if (this._additionalFields.hasOwnProperty(fld)) {
    //             result.push(this._additionalFields[fld].fld.getValue());
    //         }
    //     }

    //     return result;
    // } else {
    let currentValue = this.querySelector("input").value;
    return {
      fieldValue: currentValue,
      fieldType: "string",
      fieldPath: this.fieldInfos.fieldPath,
    };
    //        }
  }
}
window.customElements.define("core-range-field", CoreRangeField);
