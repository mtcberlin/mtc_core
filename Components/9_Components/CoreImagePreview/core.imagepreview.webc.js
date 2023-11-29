class CoreImagePreview extends CoreHTMLElement {
  _emptyGuid = "00000000-0000-0000-0000-000000000000";

  constructor() {
    super();

    this._createShadow();
  }

  connectedCallback() {
    this.setAttribute("slot", "item");

    setTimeout(() => {
      this.connectedCallbackStart();
    });
  }

  connectedCallbackStart() {
    if (!this.hasAttribute("id")) {
      this.id = this.generateUUID();
    }
    if (this.hasAttribute("imgid")) {
      this.show(this.getAttribute("imgid"));
    }
  }

  disconnectedCallback() {
    /*
		const tabsSlot = this.shadowRoot.querySelector('#tabsSlot');
		tabsSlot.removeEventListener('click', this._boundOnTitleClick);
		tabsSlot.removeEventListener('keydown', this._boundOnKeyDown);
		*/
  }

  static get observedAttributes() {
    return ["imgid"];
  }

  show(id) {
    this._id = id;
    if (id != "") {
      this._getImage();
      this.classList.remove("hidden");
    } else {
      this.hide();
    }
  }

  hide() {
    this.classList.add("hidden");
  }

  _getImage() {
    CoreStateStore.setValue(CoreStateStore.START_LOADER);

    this.ajax(
      {
        type: "GET",
        url: "/api/admin/media/image/previewinfo",
        data: { imgId: this._id },
        contentType: "application/json",
      },
      this._displayImage.bind(this),
      () => {
        CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
      }
    );
  }

  _displayImage(data) {
    if (data.success) {
      var img = this.shadowRoot.querySelector("img");
      img.src = data.imgPath;

      var width = this.shadowRoot.querySelector("#prev_width_val");
      width.innerHTML = data.width;
      var height = this.shadowRoot.querySelector("#prev_height_val");
      height.innerHTML = data.height;
      var alt = this.shadowRoot.querySelector("#prev_alt_val");
      alt.innerHTML = data.alt;
      var altsimple = this.shadowRoot.querySelector("#prev_altsimple_val");
      altsimple.innerHTML = data.altSimple;
    }

    CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
  }

  attributeChangedCallback(name, oldValue, newValue) {
    console.log(
      "CoreImagePreview -> " + name + " - " + oldValue + " - " + newValue
    );
    if (name === "imgid") {
      if (newValue != this.id) {
        this.show(newValue);
      }
    }
  }

  _createShadow() {
    let shadowRoot = this.attachShadow({ mode: "open" });
    shadowRoot.innerHTML = `
            <style>
            .preview{
                width:300px;
            }
            .meta{
                width:100%;
            }
            .row {
                display: flex;
            }  
            .column {
                flex: 50%;
            }
                </style>
            <div class="preview">
                <img src="" width="300px"/>
                <div class="meta">
                <div id="prev_width" class="row">
                    <div class="column">Width</div>
                    <div id="prev_width_val" class="column"></div>
                </div>
                <div id="prev_height" class="row">
                    <div class="column">Height</div>
                    <div id="prev_height_val" class="column"></div>
                </div>
                <div id="prev_alt" class="row">
                    <div class="column">Alt</div>
                    <div id="prev_alt_val" class="column"></div>
                </div>
                <div id="prev_altsimple" class="row">
                    <div class="column">Alt Einfache Sprache</div>
                    <div id="prev_altsimple_val" class="column"></div>
                </div>
            </div>
            </div>
        `;
  }
}

window.customElements.define("core-image-preview", CoreImagePreview);
