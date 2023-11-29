
class CoreAssetTreeView extends CoreTreeView {

    constructor() {
        super();
        console.log("CoreAssetTreeView ");
        this._childItemName = "core-assettree-item";
    }
    
    _addContentToSlot(){
        this.querySelector(".js-content").insertAdjacentElement("afterbegin", this.querySelector("[title='Root']"));
    }

}

window.customElements.define('core-assettreeview', CoreAssetTreeView);  