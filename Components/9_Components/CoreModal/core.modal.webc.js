class CoreModal extends HTMLElement {
  get visible() {
    return this.hasAttribute("visible");
  }

  set visible(value) {
    if (value) {
      this.setAttribute("visible", "");
    } else {
      this.removeAttribute("visible");
    }
  }
  get width() {
    if (this.hasAttribute("modal-width")) {
      return "width:" + this.getAttribute("modal-width");
    }
    return "";
  }

  get title() {
    return this.getAttribute("title");
  }

  set title(value) {
    this.setAttribute("title", value);
  }

  constructor() {
    super();
  }

  show(cb_ok, cb_cancel) {
    this.cb_ok = cb_ok;
    const cancelButton = this.shadowRoot.querySelector(".cancel");
    if (cb_cancel == null) {
      this.cb_cancel = cb_cancel;
      cancelButton.classList.add("hidden");
    } else {
      cancelButton.classList.remove("hidden");
    }
    this.visible = true;
  }

  close() {
    this.visible = false;
  }

  connectedCallback() {
    setTimeout(() => {
      this._connectedCallbackStart();
    });
  }

  _connectedCallbackStart() {
    this._render();
    this._attachEventHandlers();
  }

  static get observedAttributes() {
    return ["visible", "title"];
  }

  attributeChangedCallback(name, oldValue, newValue) {
    if (name === "title" && this.shadowRoot) {
      this.shadowRoot.querySelector(".title").textContent = newValue;
    }
    if (name === "visible" && this.shadowRoot) {
      if (newValue === null) {
        this.shadowRoot.querySelector(".wrapper").classList.remove("visible");
        this.dispatchEvent(new CustomEvent("close"));
      } else {
        this.shadowRoot.querySelector(".wrapper").classList.add("visible");
        this.dispatchEvent(new CustomEvent("open"));
      }
    }
  }

  _render() {
    const wrapperClass = this.visible ? "wrapper visible" : "wrapper";
    const container = document.createElement("div");
    container.innerHTML = `
        <style>
          .wrapper {
            position: fixed;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            background-color: gray;
            visibility: hidden;
            transform: scale(1.1);
            transition: visibility 0s linear .25s,opacity .25s 0s,transform .25s;
            z-index: 10000;
            background: rgba(0,0,0,0.5);
          }
          .visible {
            visibility: visible;
            transform: scale(1);
            transition: visibility 0s linear 0s,opacity .25s 0s,transform .25s;
          }
          .modal {
            opacity: 1;
            font-family: Helvetica;
            font-size: 14px;
            background-color: #fff;
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%,-50%);
            border-radius: 5px;
						height: 80%;
            min-width: 80%;
            max-width: 85%;
						max-height: 95%;
						overflow: hidden;
						display: flex;
						flex-direction: column;
					}
          .title {
						font-weight: bold;
						padding: 1rem 0.5rem;
            font-size: 18px;
						background-color:#ccc;
          }
					.content {
						padding: 0.8rem;
						flex: 1; 
						overflow-y: auto;
						}
						.hidden{
						display:none;
					}
          .button-container {
						padding: 1rem 0.5rem;
						background-color:#eee;
            text-align: right;
          }
          button {
            min-width: 80px;
            background-color: #848e97;
            border-color: #848e97;
            border-style: solid;
            border-radius: 2px;
            padding: 3px;
            color:white;
            cursor: pointer;
          }
          button:hover {
            background-color: #6c757d;
            border-color: #6c757d;
          }
        </style>
        <div class='${wrapperClass}'>
          <div class='modal' style='${this.width}'>
            <div class='title'><span>${this.title}</span></div>
            <div class='content'>
              <slot></slot>
            </div>
            <div class='button-container'>
              <button class='cancel btn-dark'>Cancel</button>
              <button class='ok btn-dark'>Okay</button>
            </div>
          </div>
        </div>`;

    const shadowRoot = this.attachShadow({ mode: "open" });
    shadowRoot.appendChild(container);
  }

  _attachEventHandlers() {
    const cancelButton = this.shadowRoot.querySelector(".cancel");
    cancelButton.addEventListener("click", (e) => {
      if (this.cb_cancel) {
        this.cb_cancel();
      }
      this.close();
      this.dispatchEvent(new CustomEvent("cancel"));
    });
    const okButton = this.shadowRoot.querySelector(".ok");

    okButton.addEventListener("click", (e) => {
      if (this.cb_ok) {
        this.cb_ok();
      }
      this.close();
      this.dispatchEvent(new CustomEvent("ok"));
    });
  }
}

window.customElements.define("core-modal", CoreModal);
