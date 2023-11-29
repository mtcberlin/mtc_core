/// <reference path="../../2_Base_Components/CoreTreeView/core.treeview.webc.js" />

class CoreContentEditorTreeView extends CoreTreeView {
    
    constructor() {
        super();
        this._childItemName = "core-contenteditortree-item";
    }
}

window.customElements.define('core-contenteditortreeview', CoreContentEditorTreeView);  