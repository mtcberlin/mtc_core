/// <reference path="../../2_Base_Components/CoreTreeView/core.treeview.webc.js" />

class CoreSelectTreeView extends CoreTreeView {
    constructor() {
        super();
        this._stopItemSelectedEventPropagation = false;
        this._childItemName = "core-selecttree-item";
    }
    
    _addContentToSlot(){
        this.querySelector(".js-content").insertAdjacentElement("afterbegin", this.querySelector("[title='Root']"));
    }

}
window.customElements.define('core-selecttreeview', CoreSelectTreeView);  