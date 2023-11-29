/// <reference path="../1_BaseField/core-field-base.webc.js" />

class CoreVideoFieldEdit extends CoreFieldBase {


	_additionalFields = {
        // 'Caption':{'fldtype':'text','valueType':'string','label':'Caption'},
        // 'CaptionSimple':{'fldtype':'text','valueType':'string','label':'Caption Simple Language'},
        // 'ImageDescription':{'fldtype':'multiline','valueType':'string','label':'Image Description'},
        // 'ImageDescriptionSimple':{'fldtype':'multiline','valueType':'string','label':'Image Description Simple Language'},
        // 'dgsVideo':{'fldtype':'dgs','valueType':'string','label':'Sign language video'}
    };

	buttonNameAdd = this.translate("Add Video"); //"Add Image";
	buttonNameDelete = this.translate("Delete Video"); //"Add Image";
	textAdd = this.translate("Add Video Text");//"Please click on the Button to select a Image";
	titleAddImage = this.translate("Add Video");//"Add Image";
	labelImage = this.translate("Video");//"Image";
	labelPreview = this.translate("Video Preview");//"Preview";

	_emptyGuid = "00000000-0000-0000-0000-000000000000";

	_readInfos() {
		super._readInfos();

		if (this.hasAttribute('buttonname')) {
			this.buttonNameAdd = this.getAttribute('buttonname');
		}
		if (this.hasAttribute('textadd')) {
			this.textAdd = this.getAttribute('textadd');
		}
		if (this.hasAttribute('titleaddimage')) {
			this.titleAddImage = this.getAttribute('titleaddimage');
		}
		if (this.hasAttribute('labelimage')) {
			this.labelimage = this.getAttribute('labelimage');
		}
		if (this.hasAttribute('labelpreview')) {
			this.labelPreview = this.getAttribute('labelpreview');
		}

		this.isExtended = true;
		this.fieldInfos.hasAdditionalFields = true;
		this.hasAdditionalFields = true;

		if (!this.fieldInfos.fieldValue) {
			this.fieldValues = {
				assetId: this._emptyGuid,
				caption: null,
				captionSimple: null,
				dgsVideo: null,
				imageDescription: null,
				imageDescriptionSimple: null
			}
		}
	}

	_initNodes() {
        super._initNodes();

		this._fld = this.querySelector('#video');
		this.modalDialog = this.querySelector("core-modal");
		this.addButton = this.querySelector(".btn-add");
		this.deleteButton = this.querySelector(".btn-delete");

		if (this.hasAdditionalFields) {
            for (const fld in this._additionalFields) {
                if (this._additionalFields.hasOwnProperty(fld)) {
                    this._additionalFields[fld].fld = new CoreAdditionalField( fld, this._additionalFields[fld].label, this._additionalFields[fld].fldtype, this._additionalFields[fld].valueType, this.fieldInfos, this._menuSlot,this._additionalFieldsSlot, this._setStateChanged.bind(this));
                    this._additionalFields[fld].fld.renderMenuEntry();
                    this._additionalFields[fld].fld.renderField();
                    this._additionalFields[fld].fld.init();
                }
            }
        }
	}

	_addListener() {
		this._fld.addEventListener('change', this._changeEvent.bind(this));
	}

	_changeEvent() {
		this._setStateChanged();
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
						if(this._additionalFields[fld].fld != undefined){
							this._additionalFields[fld].fld.resetState();
						}
					}
				}
            }
        }
    }



	_renderField(field) {
		const displayName = field.displayName === null ? field.fieldName : field.displayName;
		const value = field.hasAdditionalFields ? this.fieldValues.assetId : field.fieldValue;

		return `
		<style>
		.row {
			display: flex;
		}  
		.column {
			flex: 50%;
		}
		</style>
		<div class='mb-3'>
		<label class='form-label c-fld-label' for='${field.fieldName}'>${displayName}</label>
		<div class="js-description">${this.textAdd}</div>
		<core-select-media data-selectable-types="video" data-fieldid="video" data-api-subtreeitems="/api/core/assetlib/subitems" data-rootid="22222222-2222-2222-2222-222222222222">
			<input type="text" class="control hidden" id="video" name="${field.fieldName}.AssetId" value="${value == null ? "" : value}">
		</core-select-media>

        `;

	}

	getFieldValues() {
        if (this.fieldInfos.readonly) return;

        if (this.hasAdditionalFields) {
            var result = [{
				fieldValue: this.querySelector(`#video`).value,
				fieldType: "guid",
				fieldPath: `${this.fieldInfos.fieldPath}.AssetId`
			}];

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
                fieldPath: this.fieldInfos.fieldPath
            };
        }
    }

}
window.customElements.define('core-videofield-edit', CoreVideoFieldEdit);
