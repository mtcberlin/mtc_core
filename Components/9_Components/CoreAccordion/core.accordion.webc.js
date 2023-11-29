class CoreAccordion extends HTMLElement {
  constructor() {
    super();

    console.log("CoreAccordion constructor");
    this._createShadow();
  }

  connectedCallback() {
    setTimeout(() => {
      this._connectedCallbackStart();
    });
  }

  _connectedCallbackStart() {
    console.log("CoreAccordion connectedCallback");

    this.setAttribute("type", "accordion");

    this.allowMultiple = this.hasAttribute("data-allow-multiple");

    this.actionsNode = this.shadowRoot.querySelector(".actions");

    if (this.allowMultiple && this.actionsNode != null) {
      this.actionsNode.classList.remove("hidden");

      this.btnExpand = this.shadowRoot.querySelector("#btn_expand");
      this.btnCollapse = this.shadowRoot.querySelector("#btn_collapse");
      this._boundExpand = this._onBtnExpand.bind(this);
      this._boundCollapse = this._onBtnCollapse.bind(this);

      this.btnExpand.addEventListener("click", this._boundExpand);
      this.btnCollapse.addEventListener("click", this._boundCollapse);
    }

    this.addEventListener("expanded", (e) => {
      console.log("EVENT: " + e.detail.id() + " - " + e.detail.state());
      e.stopPropagation();
      this._propagateState(e.detail.id());
    });
  }

  _onBtnExpand() {
    console.log("_onBtnExpand");
    if (this.allowMultiple) {
      var panels = this.querySelectorAll(":scope > core-panel");
      panels.forEach((element) => {
        element.open();
      });
    }
  }

  _onBtnCollapse() {
    if (this.allowMultiple) {
      var panels = this.querySelectorAll(":scope > core-panel");
      panels.forEach((element) => {
        element.close();
      });
    }
  }

  _propagateState(id) {
    if (!this.allowMultiple) {
      console.log("propagateState");
      var panels = this.querySelectorAll(":scope > core-panel");
      panels.forEach((element) => {
        if (element.id != id) {
          element.close();
        }
      });
    }
  }

  /*
      static get observedAttributes() {
          return ['changed'];
      }

      attributeChangedCallback(name, oldValue, newValue) {
        console.log('CoreAccordion -> attribute changed callback being executed now');
        if (name === 'changed') {
        }
    }
    */

  disconnectedCallback() {
    //this.btnExpand.removeEventListener('click', this._boundExpand);
    //this.btnCollapse.removeEventListener('click', this._boundCollapse);
  }

  _createShadow() {
    let shadowRoot = this.attachShadow({ mode: "open" });
    shadowRoot.innerHTML = `
          <style>
          :host .title {
          }
          :host .actions {
          }
          :host .accordionbar{
            display: flex; 
            margin-bottom:0.6em;
            justify-content: space-between;
          }
          :host .hidden{
            display:none;
          }
          </style>
          <div>
          <div class="accordionbar">
            <div class="title"><slot name="title"></slot></div>
            <div class="actions hidden">
            <button id="btn_expand" type="button" class="btn btn-secondary">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-arrows-expand" viewBox="0 0 16 16">
                <path fill-rule="evenodd" d="M1 8a.5.5 0 0 1 .5-.5h13a.5.5 0 0 1 0 1h-13A.5.5 0 0 1 1 8zM7.646.146a.5.5 0 0 1 .708 0l2 2a.5.5 0 0 1-.708.708L8.5 1.707V5.5a.5.5 0 0 1-1 0V1.707L6.354 2.854a.5.5 0 1 1-.708-.708l2-2zM8 10a.5.5 0 0 1 .5.5v3.793l1.146-1.147a.5.5 0 0 1 .708.708l-2 2a.5.5 0 0 1-.708 0l-2-2a.5.5 0 0 1 .708-.708L7.5 14.293V10.5A.5.5 0 0 1 8 10z"/>
              </svg>          
            </button>
            <button  id="btn_collapse" type="button" class="btn btn-secondary">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-arrows-collapse" viewBox="0 0 16 16">
                <path fill-rule="evenodd" d="M1 8a.5.5 0 0 1 .5-.5h13a.5.5 0 0 1 0 1h-13A.5.5 0 0 1 1 8zm7-8a.5.5 0 0 1 .5.5v3.793l1.146-1.147a.5.5 0 0 1 .708.708l-2 2a.5.5 0 0 1-.708 0l-2-2a.5.5 0 1 1 .708-.708L7.5 4.293V.5A.5.5 0 0 1 8 0zm-.5 11.707-1.146 1.147a.5.5 0 0 1-.708-.708l2-2a.5.5 0 0 1 .708 0l2 2a.5.5 0 0 1-.708.708L8.5 11.707V15.5a.5.5 0 0 1-1 0v-3.793z"/>
              </svg>
            </button>
            </div>

          </div>
          <slot name="item"></slot>
          </div>
          `;
  }

  /*      
      _onTitleClick(e) { 
        if (e.target.slot === 'title') {
          this.selected = this.tabs.indexOf(e.target);
          e.target.focus();
        }
      }
      */

  /*
      _onKeyDown(e) {
        switch (e.code) {
          case 'ArrowUp':
          case 'ArrowLeft':
            e.preventDefault();
            var idx = this.selected - 1;
            idx = idx < 0 ? this.tabs.length - 1 : idx;
            this.tabs[idx].click();
            break;
          case 'ArrowDown':
          case 'ArrowRight':
            e.preventDefault();
            var idx = this.selected + 1;
            this.tabs[idx % this.tabs.length].click();
            break;
          default:
            break;
        }
      }
      */
}
window.customElements.define("core-accordion", CoreAccordion);
