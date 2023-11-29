class CoreStateStore {

	static _states = {};

	static notify(stateName, value, state) {
		if (this._states[stateName] && this._states[stateName].length > 0) {
			this._states[stateName].forEach(obj => {
				//console.log("notify: " + value + " / " + obj.requiredValue );
				if (obj.requiredValue === undefined ||obj.requiredValue === null || obj.requiredValue === value) {
					obj.listener(value, state);
				}
			});
		}
	}

	/**
	 * 
	 * @param {string} stateName 
	 * @param {void(value, state)} listener 
	 */
	static unSubscribe(stateName, id) {		
		if(this._states[stateName] && id != undefined){
			this._states[stateName] = this._states[stateName].filter( function(elem){
				return elem.id != id;
			});
		}
	}
	

	/**
	 * 
	 * @param {string} stateName 
	 * @param {void(value, state)} listener 
	 * @param {string|number|boolean} requiredValue 
	 */
	static subscribe(stateName, listener, requiredValue, id) {
		if (!this._states[stateName]) {
			this._states[stateName] = [];
		}
		this._states[stateName].push({ id, listener, requiredValue });
	}

	/**
	 * 
	 * @param {string} stateName 
	 * @param {any} value 
	 * @param {boolean} store 
	 */
	static setValue(stateName, value, store = true) {
		//console.log("setValue: " + value);
		let state = {
			canceled: false
		}
		if (store && value !== undefined) {
			sessionStorage.setItem(stateName, value);
		}
		this.notify(stateName, value, state);
		return state;
	}

	/**
	 * Returns the current stored value
	 * @param {string} stateName
	 */
	static getValue(stateName) {
		return sessionStorage.getItem(stateName);
	}

	/**
	 * 
	 * @param {string} stateName 
	 * @param {any} value 
	 */
	static addValue(stateName, value) {
		let state = {
			canceled: false
		}
		let oldValue = sessionStorage.getItem(stateName);
		if(oldValue === null || oldValue === "undefined" || oldValue === undefined || oldValue === "") {
			oldValue = [];
		}else {
			oldValue = JSON.parse(oldValue);
		}
		oldValue.push(value);
		sessionStorage.setItem(stateName, JSON.stringify(oldValue));

		this.notify(stateName, {value: oldValue, added: value}, state);
		return state;
	}

	/**
	 * 
	 * @param {string} stateName 
	 * @param {any} value 
	 * @param {boolean} store 
	 */
	static removeValue(stateName, value) {
		let oldValue = JSON.parse(sessionStorage.getItem(stateName, value));
		// TODO, scheinbar wenn array mit einem wert, dann gibt filter kein array zurück
		let newValue = oldValue.filter(function (ele) {
			return ele != value;
		});
		var concatedValue = [].concat(newValue);
		//console.dir(concatedValue);
		sessionStorage.setItem(stateName, JSON.stringify(concatedValue));

		this.notify(stateName, { oldValue: oldValue, value: newValue, removed: value});
	}

	/**
	 * 
	 * @param {string} stateName 
	 * @param {any} value 
	 * @param {boolean} store 
	 */
	 static clearValue(stateName) {
		let oldValue = sessionStorage.getItem(stateName);
		sessionStorage.setItem(stateName, []);
		this.notify(stateName, { value: oldValue });
	}


	static START_LOADER = "start-loader";
	static STOP_LOADER = "stop-loader";

	static REFRESH = "refresh";


	static PAGE_EDITOR_REQUEST_ID = "page-editor-request-id";
	static PAGE_EDITOR_CONTENT_ID = "page-editor-content-id";
	static PAGE_EDITOR_CONTENT_TYPE = "page-editor-content-type";
	static PAGE_EDITOR_CONTENT_LANG = "page-editor-content-lang";

	static ASSET_EDITOR_CONTENT_ID = "asset-editor-content-id";

	static EDITOR_EVENT_CLOSED = "editor-event-closed";
	static EDITOR_EVENT_SAVED = "editor-event-saved";
	static EDITOR_EVENT_SAVEFAILED = "editor-event-save-failed";
	static EDITOR_EVENT_OPENED = "editor-event-opened";
	static EDITOR_EVENT_MODIFIED = "editor-event-modified";
	static EDITOR_EVENT_WARNING = "editor-event-warning";
	static EDITOR_EVENT_CLEARED = "editor-event-cleared";



	static PAGE_EDITOR_SELECTED_TAB = "page-editor-selected-tab";
	static PAGE_EDITOR_IS_PANEL_EXPANDED_PREFIX = "page-editor-panel-";
	
	static FIELD_EVENT_CHANGED = "field-event-changed";

	static PAGE_EDITOR_TREE_ITEM_EXPANDED = "page-editor-tree-item-expanded";
	static PAGE_EDITOR_TREE_EVENT_DO_REFRESH = "page-editor-tree-event-do-refresh";

}