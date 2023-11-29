class CoreAdditionalField{

    _fldtype = "text";
    _valuetype = "string";
    _name = "";
    _label = "";
    _node = null;
    _fieldConfig = null;
    _slotMenu = null;
    _slotFields = null;
    _tmpValue = "";
    _changedEvent = null;
    isChanged = false;
    
    constructor(name, label, fldtype, valuetype, field, slotMenu, slotFields, changedEvent){
        this._name = name;
        this._label = label;
        this._fieldConfig = field;
        this._fldtype = fldtype;
        this._valuetype = valuetype;
        this._slotMenu = slotMenu;
        this._slotFields = slotFields;
        this._changedEvent = changedEvent;
    }

    resetState(){
        this._tmpValue =  this.value();
        this._setFieldChanged();
    }

    translate(key){
        if(CoreTranslationService){
            return CoreTranslationService.translate(key);
        }
        return key;
    }

    camelize(str) {
        return str.replace(/(?:^\w|[A-Z]|\b\w)/g, function(word, index) {
          return index === 0 ? word.toLowerCase() : word.toUpperCase();
        }).replace(/\s+/g, '');
    }

    renderMenuEntry(){
        var li = document.createElement("li");
        this._menuNode = document.createElement("input");
        this._menuNode.setAttribute('type', 'checkbox');
        this._menuNode.setAttribute('name', this._name);
        this._menuNode.setAttribute('id', this._name);
        var label = document.createElement("label");
        label.setAttribute('for', this._name);
        label.innerHTML = this.translate(this._label);
        li.append(this._menuNode);
        li.append(label);
        this._slotMenu.appendChild(li);
    }

    renderField(){
        this.fieldNode = document.createElement("div");
        this.fieldNode.setAttribute('class', 'c-fld hidden');

        var label = document.createElement("label");
        label.setAttribute('class', 'form-label c-fld-label');
        label.setAttribute('for', this._fieldConfig.fieldName + "_" + this._name);
        label.innerHTML = ( this._fieldConfig.displayName === null ? this._fieldConfig.fieldName : this._fieldConfig.displayName) + " (" + this.translate(this._label) + ")";
        this.fieldNode.append(label);

        const value = this._fieldConfig.fieldValue && this._fieldConfig.fieldValue[this.camelize(this._name)] ? this._fieldConfig.fieldValue[this.camelize(this._name)] : "";
        switch (this._fldtype) {
            case 'dgs':
                this._renderSelectMediaField(this.fieldNode, value);
                break;
            case 'rtf':
                this._renderRtfField(this.fieldNode, value);
                break;
            case 'multiline':
                this._renderMultilineTextField(this.fieldNode, value);
                break;
            default:
                this._renderTextField(this.fieldNode, value);
            }

        this._slotFields.appendChild(this.fieldNode);
    }

    _renderTextField(slot, value){
        this._field = document.createElement("input");
        this._field.setAttribute('type', 'text');
        this._field.setAttribute('class', 'form-control');
        this._field.setAttribute('name', this._fieldConfig.fieldName + "." + this._name);
        this._field.setAttribute('id', this._fieldConfig.fieldName + "_" + this._name);
        this._field.setAttribute('value', value);
        slot.append(this._field);
    }

    _renderSelectMediaField(slot, value){
        this._field = document.createElement("input");
        this._field.setAttribute('type', 'text');
        this._field.setAttribute('class', 'form-control hidden');
        this._field.setAttribute('name', this._fieldConfig.fieldName + "." + this._name);
        this._field.setAttribute('id', this._fieldConfig.fieldName + "_" + this._name);
        this._field.setAttribute('value', value);
        this._select = document.createElement("core-select-media");
        this._select.setAttribute('data-selectable-types', "video");
        this._select.setAttribute('data-fieldid', this._fieldConfig.fieldName + "_" + this._name);
        this._select.setAttribute('data-api-subtreeitems', "/api/core/assetlib/subitems");
        this._select.setAttribute('data-rootid', "22222222-2222-2222-2222-222222222222");
        this._select.append(this._field);
        slot.append(this._select);
    }

    _renderMultilineTextField(slot, value){
        this._field = document.createElement("textarea");
        this._field.setAttribute('class', 'form-control');
        this._field.setAttribute('rows', '4');
        this._field.setAttribute('name', this._fieldConfig.fieldName + "." + this._name);
        this._field.setAttribute('id', this._fieldConfig.fieldName + "_" + this._name);
        this._field.innerHTML = value;
        slot.append(this._field);
    }


    _renderRtfField(slot, value){
        this._field = document.createElement("textarea");
        this._field.setAttribute('class', 'form-control');
        this._field.setAttribute('name', this._fieldConfig.fieldName + "." + this._name);
        this._field.setAttribute('id', this._fieldConfig.fieldName + "_" + this._name);
        this._field.innerHTML = value;
        slot.append(this._field);
        this.rtfEditor = Jodit.make(this._field);
        this.rtfEditor.value = value;
    }
    
    init(){
        if (this._fieldConfig.fieldValue && this._fieldConfig.fieldValue[this.camelize(this._name)]) {
            this._menuNode.checked = true;
            this._menuChanged();
        }
        switch (this._fldtype) {
            case 'dgs':
                this._menuNode.addEventListener('change', this._menuChanged.bind(this));
                this._field.addEventListener('change', this._fieldChanged.bind(this));
                break;
            case 'rtf':
                this._menuNode.addEventListener('change', this._menuChanged.bind(this));
                //this._field.addEventListener('input', this._fieldChanged.bind(this));
                this.rtfEditor.events.on('change', this._fieldChanged.bind(this));
                break;
            default:
                this._menuNode.addEventListener('change', this._menuChanged.bind(this));
                this._field.addEventListener('input', this._fieldChanged.bind(this));
           }

        this._tmpValue = this.value();
    }

    _menuChanged(){
        if (this._menuNode.checked) {
            this.fieldNode.classList.remove("hidden");
            if (this._tmpValue && this._tmpValue != "") {
                this._field.value = this._tmpValue;
            }
        } else {
            this.fieldNode.classList.add("hidden");
            this._tmpValue = this.value();
            this._field.value = "";
            this._fieldChanged();
        }
    }

    _fieldChanged(){
        this._setFieldChanged();
        this._changedEvent();   
    }

    _setFieldChanged(){
        if(this._tmpValue !== this.value()){
            this.fieldNode.classList.add("fldchanged");
            this.isChanged = true;
        } else {
            this.fieldNode.classList.remove("fldchanged");
            this.isChanged = false;
        }
    }

    value(){
        var value = "";

        switch (this._fldtype) {
            case 'rtf':
                value = this._field.value !== "<p><br></p>" ? this._field.value : "";
                break;
            default:
                value =  this._field.value;
                break;
                
        }
        return value;
    }

    getValue(){
        var result = {};
        result = {
            fieldValue: this.value(),
            fieldType: this._valuetype,
            fieldPath: `${this._fieldConfig.fieldPath}.${this._name}`
        };
        return result;
    }
}
