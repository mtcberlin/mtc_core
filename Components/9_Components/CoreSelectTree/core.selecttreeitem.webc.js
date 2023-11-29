/// <reference path="../../2_Base_Components/CoreTreeView/core.treeviewitem.webc.js" />

class CoreSelectTreeItem extends CoreTreeViewItem {

	constructor() {
		super();
		this._nodeName = "core-selecttree-item";
		this._rootNodeName = "core-selecttreeview";
	}

	_initActions(){}

	_createSubItem(subItem) {
		const item = document.createElement(this._nodeName);
		item.setAttribute("title", subItem.name);
		item.setAttribute("id", subItem.id);
		item.setAttribute("data-parentid", subItem.parentId);
		item.setAttribute("type", subItem.type);
		item.dataset.selectableTypes = this._selectableTypes;
		item.dataset.hasSubitems = subItem.hasSubItems;
		if(subItem.insertOptions){
			item.dataset.insertOptions = JSON.stringify(subItem.insertOptions);
		}
		this._content.append(item);
	}
	_disconnectActions() {
		//this.shadowRoot.querySelector(".js-add-page").removeEventListener('click', this.onAddPageClick.bind(this));
	}

}

window.customElements.define('core-selecttree-item', CoreSelectTreeItem);  