class CoreMediaPreview extends CoreHTMLElement {
  _emptyGuid = "00000000-0000-0000-0000-000000000000";
  _mediaType = "image";
  _isConnected = false;

  static get observedAttributes() {
    return ["mediaid"];
  }

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
    this.imageNode = this.shadowRoot.querySelector(".preview_img");
    this.videoNode = this.shadowRoot.querySelector(".preview_video");

    if (!this.hasAttribute("id")) {
      this.id = this.generateUUID();
    }
    // if (!this.hasAttribute("mediatype")) {
    // 	this._mediaType = this.getAttribute("mediatype");
    // }
    if (this.hasAttribute("mediaid")) {
      this._id = this.getAttribute("mediaid");
      if (this._id != "") {
        this._getMedia();
      }
    } else {
      this.hide();
    }
    this._isConnected = true;
  }

  disconnectedCallback() {
    /*
		const tabsSlot = this.shadowRoot.querySelector('#tabsSlot');
		tabsSlot.removeEventListener('click', this._boundOnTitleClick);
		tabsSlot.removeEventListener('keydown', this._boundOnKeyDown);
		*/
  }

  show() {
    if (this._id != "") {
      this.classList.remove("hidden");
    } else {
      this.hide();
    }
  }

  hide() {
    this.classList.add("hidden");
  }

  showImage() {
    this.imageNode.classList.remove("hidden");
    this.videoNode.classList.add("hidden");
  }

  showVideo() {
    this.videoNode.classList.remove("hidden");
    this.imageNode.classList.add("hidden");
  }

  _getMedia() {
    CoreStateStore.setValue(CoreStateStore.START_LOADER);
    var url = "/api/admin/media/previewinfo";
    this.ajax(
      {
        type: "GET",
        url: url,
        data: { mediaId: this._id },
        contentType: "application/json",
      },
      this._getDataSuccess.bind(this),
      () => {
        CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
      }
    );
  }

  _getDataSuccess(data) {
    this.data = data;
    if (data.success) {
      if (data.mimeType == "image") {
        this._mediaType = "image";
        if (this.data.img != null) {
          this._displayImage();
          this.show();
        }
      } else if (data.mimeType == "video") {
        this._mediaType = "video";
        this._displayVideo();
        this.show();
      }
    } else {
      this.hide();
    }
    CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
  }

  _displayImage() {
    var img = this.shadowRoot.querySelector("img");
    img.src = this.data.imgPath;

    var width = this.shadowRoot.querySelector("#prev_width_val");
    width.innerHTML = this.data.img.width;
    var height = this.shadowRoot.querySelector("#prev_height_val");
    height.innerHTML = this.data.img.height;
    var alt = this.shadowRoot.querySelector("#prev_alt_val");
    alt.innerHTML = this.data.img.alt.value;
    var altsimple = this.shadowRoot.querySelector("#prev_altsimple_val");
    altsimple.innerHTML = this.data.img.altSimple;
    this.showImage();
  }

  _displayVideo() {
    var video = this.shadowRoot.querySelector("core-video-player");
    video.setAttribute("data-video-id", this.data.video.id);
    this.showVideo();
  }

  attributeChangedCallback(name, oldValue, newValue) {
    if (this._isConnected === true && name === "mediaid") {
      if (newValue != this._id) {
        this._id = newValue;
        this._getMedia();
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
			.hidden{
				display:none;
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
					<div class="preview preview_img hidden" style="margin-bottom: 1rem;">
						<img src="" width="300px"/>
						<div class="meta">
							<div id="prev_width" class="row" style="border-bottom: lightgray solid 1px">
								<div class="column" style="font-weight: bold;">Width</div>
								<div id="prev_width_val" class="column" style="text-align: end;"></div>
							</div>
							<div id="prev_height" class="row" style="border-bottom: lightgray solid 1px">
								<div class="column" style="font-weight: bold;">Height</div>
								<div id="prev_height_val" class="column" style="text-align: end;"></div>
							</div>
							<div id="prev_alt" class="row" style="border-bottom: lightgray solid 1px">
								<div class="column" style="font-weight: bold";>Alt</div>
								<div id="prev_alt_val" class="column" style="text-align: end;"></div>
							</div>
							<div id="prev_altsimple" class="row" style="border-bottom: lightgray solid 1px">
								<div class="column" style="font-weight: bold;">Alt Einfache Sprache</div>
								<div id="prev_altsimple_val" class="column" style="text-align: end;"></div>
							</div>
						</div>
					</div>
				<div class="preview preview_video hidden">
					<core-video-player class='d-flex flex-column' data-video-id=""></core-video-player>
				<div class="meta">
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

window.customElements.define("core-media-preview", CoreMediaPreview);
