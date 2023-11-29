class CoreRadioField extends CoreFieldBase {
  _changeEvent() {
    super._changeEvent();
  }

  _addListener() {
    //this.observeList.push({attribute:"aria-checked", callback:this._groupChanged.bind(this)});
    //this.observeList.push({attribute:"radiochanged", callback:this._valueChanged.bind(this)});
    //this.initObserver(this);
    this.addEventListener("radioitemclicked", (e) => {
      console.log("EVENT: " + e.detail.id);
      e.stopPropagation();
      this._childItemChanged(e.detail.id);
    });
  }

  _render() {
    let field = this.fieldInfos;
    let readOnly = "";
    if (field.readonly) {
      readOnly = "readonly=readonly";
    }

    let options = "";
    field.selectOptions.forEach((option) => {
      options += this._renderOption(option, field);
    });

    // ToDo: why form-control
    this.innerHTML = `<div class="c-fld c-fld-radio mb-4">
			<label for="formGroupExampleInput" class="form-label c-fld-label fw-bold">${field.groupLabel}</label>
			<div class="">
				${options}
			</div>
		</div>`;
  }

  _renderOption(option, field) {
    let checked = field.fieldValue === option.key ? "checked=checked" : "";
    let label = option.name != "" ? option.name : "";
    if (option.thumbnail == null) option.thumbnail = "";
    let icon =
      option.thumbnail != ""
        ? `<img src="/img/ContentEditor/${option.thumbnail}"/>`
        : "";

    // ToDo: class form-check-inline optional
    return `<core-radio-entry><div class="form-check">
				<input class="form-check-input" type="radio" name="${field.fieldName}" id="${option.key}" value="${option.key}" ${checked}>
				<label class="form-check-label">
					${icon} ${label}
				</label>
			</div></core-radio-entry>`;
  }

  _childItemChanged(id) {
    console.log("Core Radio Field - _childItemChanged: " + id);
    //if(this.querySelector("input:checked") && this.querySelector("input:checked").id != id);
    //{
    this._setStateChanged();
    //}
    var flds = this.querySelectorAll("core-radio-entry");
    flds.forEach((element) => {
      element.update(id);
    });
  }

  /*
	_groupChanged(){
		console.log("CoreRadioField _groupChanged");
		var flds = this.querySelectorAll("editor-radio-entry");
		flds.forEach(element => {
			if(!element.checked){
				element.refresh();
			}
		});  
	}

	_valueChanged( mutationObj ){
		console.dir(mutationObj);
		var newValue = mutationObj.target.getAttribute(mutationObj.attributeName);
		console.log("new value: " + newValue);
		if(newValue == "true"){
			this._setStateChanged();
			this.querySelector(".fld-group").classList.add("fldchanged");	
		}
	}
	*/

  // _setStateChanged(){
  // 	console.log("RADIO FIELD -> _setStateChanged")
  // 	if(!this.isValueChanged){
  // 		if(this.publishFieldChange){
  // 			CoreStateStore.addValue(CoreStateStore.FIELD_EVENT_CHANGED, this.id);
  // 		}
  // 		this.isValueChanged = true;
  // 		this.setAttribute("changed", "true");
  // 	}
  // 	else {
  // 		if( this.getFieldValues() == undefined){
  // 			this.resetState();
  // 			CoreStateStore.removeValue(CoreStateStore.FIELD_EVENT_CHANGED, this.id);
  // 		}
  // 	}
  // }

  resetState() {
    super.resetState();
    if (this.getFieldValues() != undefined) {
      this.fieldInfos.fieldValue = this.getFieldValues().fieldValue;
    }
  }
  getFieldValues() {
    //dont need to change readoly fields
    if (this.fieldInfos.readonly) return;

    // check if the value changed
    let checkedInput = this.querySelector("input:checked");
    //debugger;

    if (
      checkedInput === undefined ||
      checkedInput === null ||
      checkedInput.value === this.fieldInfos.fieldValue
    )
      return;

    return {
      fieldValue: checkedInput.value,
      fieldType: this.fieldInfos.fieldType,
      fieldPath: this.fieldInfos.fieldPath,
    };
  }
}
window.customElements.define("core-radio-field", CoreRadioField);
