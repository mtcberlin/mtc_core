class CoreRichTextField extends CoreStringField {

	_mainField = {
        'Value':{'type':'rtf','label':''}
    };

    _additionalFields = {
        'SimpleText':{'fldtype':'rtf', 'valuetype':'string', 'label':'Simple Language'},
        'dgsVideo':{'fldtype':'dgs', 'valuetype':'string', 'label':'Sign language video'}
    };

	_addListener() {
		this.editorMain.events.on('change', this._changeEvent.bind(this));
	}

	 _render() {
		super._render();
		this.editorMain = Jodit.make(`#${this.fieldInfos.fieldName}`);
		if(this.fieldInfos.hasAdditionalFields) {
			this.editorMain.value = this.fieldInfos.fieldValue ? this.fieldInfos.fieldValue.value : "";
		} else {
			this.editorMain.value = this.fieldInfos.fieldValue;
		}
	 }

    _renderField(field) {

		let readOnly = "";
        if (field.readonly) {
            readOnly = "readonly=readonly";
        }

		const displayName = field.displayName === null ? field.fieldName : field.displayName;

        return `
	        <div class="js-text">
    	        <label class='form-label c-fld-label' for='${field.fieldName}'>${displayName}</label>
				<textarea id="${field.fieldName}" name="${field.fieldName}.Value" class="form-control js-main-fld"></textarea>
			</div>
        `;
    }

	getFieldValues() {
		//dont need to change readoly fields
		if (this.fieldInfos.readonly) return;

		if (this.fieldInfos.hasAdditionalFields) {
			var result = [{
				fieldValue: this.editorMain.value !== "<p><br></p>" ? this.editorMain.value : "",
				fieldType: this.fieldInfos.fieldType,
				fieldPath: `${this.fieldInfos.fieldPath}.Value`
			}];

			for (const fld in this._additionalFields) {
                if (this._additionalFields.hasOwnProperty(fld)) {
                    result.push(this._additionalFields[fld].fld.getValue());
				}
			}

			return result;

		} else {
			return {
				fieldValue: this.editorMain.value !== "<p><br></p>" ? this.editorMain.value : "",
				fieldType: this.fieldInfos.fieldType,
				fieldPath: this.fieldInfos.fieldPath
			};
		}
	}

}
window.customElements.define('core-richtext-field', CoreRichTextField);