class CoreRadioEntry extends CoreHTMLElement {
  connectedCallback() {
    // workaround for chrom and safari
    // https://stackoverflow.com/questions/62962138/how-to-get-the-contents-of-a-custom-element
    setTimeout(() => {
      this._initNodes();
      this._readInfos();
      this._restoreState();
      this._addListener();
      this._render();
      this.refresh();
    });
  }

  _initNodes() {
    this._radio = this.querySelector("input[type='radio']");
    // console.log(this._radio);
  }

  _addListener() {
    this.clickedCustomEvent = new CustomEvent("radioitemclicked", {
      bubbles: true,
      detail: { id: this._radio.id },
    });

    //this.addEventListener("click", this._clicked.bind(this));
    this.addEventListener("click", this._clicked.bind(this));

    //this.observeList.push({attribute:"aria-checked", callback:this._groupChanged.bind(this)});
    //this.initObserver(this);
  }

  _restoreState() {}

  _readInfos() {}

  _render() {}

  _renderOption(option, field) {}

  update(id) {
    if (id == this._radio.id) {
      this.querySelector(".form-check").classList.add("selected");
    } else {
      this.querySelector(".form-check").classList.remove("selected");
    }
  }

  /*
	_groupChanged(){
		console.log("_groupChanged");
		if(this._radio.checked){
			this.querySelector(".form-check").classList.add("selected");
		} else{
			this.querySelector(".form-check").classList.remove("selected");
		}
	}*/

  _clicked() {
    console.log("Radio Item clicked: " + this._radio.id);

    if (this._radio.checked == true) {
      //this._radio.checked = false;
    } else {
      this._radio.checked = true;
      this.dispatchEvent(this.clickedCustomEvent);
    }
  }

  _dispatch() {}

  refresh() {
    if (!this._radio) return;
    if (this._radio.checked) {
      this._radio.ariaChecked = true;
      this.querySelector(".form-check").classList.add("selected");
    } else {
      this._radio.ariaChecked = false;
      this.querySelector(".form-check").classList.remove("selected");
    }
  }
}
window.customElements.define("core-radio-entry", CoreRadioEntry);
