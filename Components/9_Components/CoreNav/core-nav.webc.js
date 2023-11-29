class CoreNav extends CoreHTMLElement {
  constructor() {
    super();
  }

  connectedCallback() {
    // workaround for chrom and safari
    // https://stackoverflow.com/questions/62962138/how-to-get-the-contents-of-a-custom-element
    setTimeout(() => {
      this._initNodes();
    });
  }

  _initNodes() {
    var linkNodes = this.querySelectorAll(".nav-link");

    linkNodes.forEach((element) => {
      this._checkActive(element);
    });
  }

  _checkActive(element) {
    var dochref = document.location.pathname;
    var url = element.getAttribute("href").split("?");

    if (url[0] == dochref) {
      this._setActive(element);
    }
  }

  _setActive(element) {
    element.parentNode.classList.add("active");
  }
}
window.customElements.define("core-nav", CoreNav);
