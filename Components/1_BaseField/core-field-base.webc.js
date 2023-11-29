class CoreFieldBase extends CoreHTMLElement {
  isValueChanged = false;
  publishFieldChange = true;
  initialValue = {};
  _isConnected = false;

  static get observedAttributes() {
    return ["changed"];
  }

  constructor() {
    super();
    if (!this.id) {
      this.id = this.generateUUID();
    }
    CoreStateStore.subscribe(
      CoreStateStore.EDITOR_EVENT_SAVED,
      this._eventFieldSaved.bind(this),
      null,
      this.id
    );
  }

  connectedCallback() {
    // workaround for chrom and safari
    // https://stackoverflow.com/questions/62962138/how-to-get-the-contents-of-a-custom-element
    setTimeout(() => {
      this._readInfos();
      this._restoreState();
      this._render();
      this._initNodes();
      this._addListener();

      this.initialValue = this.getFieldValues();
      this._isConnected = true;
      this.afterConnected();
    });
  }

  /**
   * after connected callback for the webcomponents
   */
  afterConnected() {}

  _readInfos() {
    if (this.dataset && this.dataset.data) {
      const data = JSON.parse(this.dataset.data, this.toCamelCase);
      this.fieldInfos = data.definition;
      this.fieldValues = data.values;
    } else {
      this.fieldInfos = {};
    }

    if (this.dataset && this.dataset.publishchange == "false") {
      this.publishFieldChange = false;
    }
  }

  _restoreState() {}

  _initNodes() {
    this._menuSlot = this.querySelector(".c-fld-af-menu");
    this._additionalFieldsSlot = this.querySelector(".c-fld-af-fields");
  }

  _addListener() {}

  _changeEvent() {
    this._setStateChanged();
  }

  _eventFieldSaved() {
    this.resetState();
    this.initialValue = this.getFieldValues();
  }

  disconnectedCallback() {}

  resetState() {
    this.isValueChanged = false;
    this.setAttribute("changed", "false");
  }

  attributeChangedCallback(name, oldValue, newValue) {
    if (this._isConnected === true) {
      if (name == "changed") {
        if (this.isValueChanged) {
          this.querySelector(".c-fld").classList.add("fldchanged");
        } else {
          this.querySelector(".c-fld").classList.remove("fldchanged");
        }
      }
    }
  }

  translate(key) {
    if (CoreTranslationService) {
      return CoreTranslationService.translate(key);
    }
    return key;
  }

  toCamelCase(key, value) {
    if (value && typeof value === "object") {
      for (var k in value) {
        if (/^[A-Z]/.test(k) && Object.hasOwnProperty.call(value, k)) {
          value[k.charAt(0).toLowerCase() + k.substring(1)] = value[k];
          delete value[k];
        }
      }
    }
    return value;
  }

  _render() {
    let field = this.fieldInfos;
    if (this.hasAdditionalFields) {
      this.innerHTML = `
				<div class='c-fld'>
					${this._renderField(field)}
          <div class="border bg-light rounded mb-4">
            ${this._renderMenu(field)}
            ${this._renderAdditionalFields(field)}
          </div>
				</div>
			`;
    } else {
      this.innerHTML = `
				<div class='c-fld'>
                	${this._renderField(field)}
            	</div>
			`;
    }
  }

  _renderField(field) {}

  _renderMenu(field) {
    return `<div><ul class='c-fld-af-menu m-2'></ul></div>`;
  }

  _renderAdditionalFields(field) {
    return `<div class="c-fld-af-fields m-2"></div>`;
  }

  _isValuesChanged() {
    return !(
      JSON.stringify(this.getFieldValues()) ===
      JSON.stringify(this.initialValue)
    );
  }

  _setStateChanged() {
    if (!this.isValueChanged) {
      if (this.publishFieldChange) {
        CoreStateStore.addValue(CoreStateStore.FIELD_EVENT_CHANGED, this.id);
      }
      this.isValueChanged = true;
      this.setAttribute("changed", "true");
    } else {
      if (!this._isValuesChanged()) {
        this.resetState();
        CoreStateStore.removeValue(CoreStateStore.FIELD_EVENT_CHANGED, this.id);
      }
    }
  }
}
