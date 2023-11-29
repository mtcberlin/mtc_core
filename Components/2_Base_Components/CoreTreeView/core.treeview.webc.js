class CoreTreeView extends CoreHTMLElement {
  _isConnected = false;

  constructor() {
    super();
    this._lastSelectedItemId = "";
    this._stopItemSelectedEventPropagation = true;
    this._childItemName = "core-treeview-item";
    this._selectableTypes = "";
  }

  connectedCallback() {
    setTimeout(() => {
      this.connectedCallbackStart();
    });
  }

  connectedCallbackStart() {
    if (!this._isConnected) {
      this._isConnected = true;

      this._createShadow();
      if (!this.hasAttribute("id")) {
        this.id = this.generateUUID();
      }
      if (this.hasAttribute("data-selectable-types")) {
        this._selectableTypes = this.getAttribute("data-selectable-types");
      }
      this.addEventListener("itemselected", (e) => {
        //console.log("itemselected: " + e.detail.id())
        if (this._stopItemSelectedEventPropagation) {
          e.stopPropagation();
        }
        this._propagateState(e.detail.id());
        this._lastSelectedItemId = e.detail.id();
      });

      this._tree = this.querySelector(".treeview");
      this._tree.id = "tree_" + this.id;
      this.role = "none";
    }
  }

  disconnectedCallback() {}

  static get observedAttributes() {
    return ["data-itempathids"];
  }

  attributeChangedCallback(name, oldValue, newValue) {
    if (name === "data-itempathids") {
      var allItems = this.querySelectorAll(this._childItemName);
      allItems.forEach((element) => {
        element.update();
      });
    }
  }

  reset() {
    var rootItem = this.querySelector(this._childItemName);
    if (rootItem) {
      rootItem.reset();
    }
  }

  _createShadow() {
    this.insertAdjacentHTML(
      "afterbegin",
      `
            <style>
            // .treeview {
            //     margin: 0;
            //     margin-bottom: 1em;
            //     padding: 0.5em 1em;
            // }            
            .treeview[hidden] {
                display: none;
            }
            </style>
            <nav class="js-tree-content">
                <ul id="" aria-labelledby="" role="tree" class="treeview p-0">
                    <div class="js-content"></div>
                </ul>
            </nav>
        `
    );
    this._addContentToSlot();
  }

  _addContentToSlot() {
    this.querySelector(".js-content").insertAdjacentElement(
      "afterbegin",
      this.querySelector("[type='root']")
    );
  }

  _propagateState(id) {
    if (this._lastSelectedItemId != "") {
      var item = document.querySelector(
        "[id='" + this._lastSelectedItemId + "']"
      );
      if (item != null) {
        item.deselect();
      }
    }
  }

  get selected() {
    return this._lastSelectedItemId;
  }

  toggle(open) {
    // don't do anything if the open state doesn't change
    if (open === this.openState) {
      return;
    }

    // update the internal state
    this.openState = open;

    // handle DOM updates
    this._btn.setAttribute("aria-expanded", `${open}`);
    this.setAttribute("expanded", `${open}`);

    if (open) {
      this._region.removeAttribute("hidden");
    } else {
      this._region.setAttribute("hidden", "");
    }
  }
}

window.customElements.define("core-treeview", CoreTreeView);
