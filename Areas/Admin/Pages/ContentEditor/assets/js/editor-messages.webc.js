class EditorMessages extends CoreHTMLElement {

	messages;

	constructor() {
		super();
		CoreStateStore.subscribe(CoreStateStore.EDITOR_EVENT_SAVED, this._eventSaved.bind(this));
		CoreStateStore.subscribe(CoreStateStore.EDITOR_EVENT_SAVEFAILED, this._eventSaveFailed.bind(this));
		CoreStateStore.subscribe(CoreStateStore.EDITOR_EVENT_MODIFIED, this._eventModified.bind(this));
		CoreStateStore.subscribe(CoreStateStore.EDITOR_EVENT_OPENED, this._eventOpened.bind(this));
		CoreStateStore.subscribe(CoreStateStore.EDITOR_EVENT_WARNING, this._eventWarning.bind(this));
		CoreStateStore.subscribe(CoreStateStore.EDITOR_EVENT_CLEARED, this._eventCleared.bind(this));
		
		CoreStateStore.subscribe(CoreStateStore.EDITOR_EVENT_CLOSED, this.reset.bind(this));
	}

	connectedCallback() {
		// workaround for chrom and safari
		// https://stackoverflow.com/questions/62962138/how-to-get-the-contents-of-a-custom-element
		setTimeout(() => {
			this.restoreState();
			this.addListener();
			this.noOfRows = this.hasAttribute('data-rows') ? this.getAttribute('data-rows') : 1;
		});
	}

	addListener() {
		this.messages = this.querySelector(".messages");
	}

	restoreState() {
	}

	reset() {
		if (this.messages) {
			while (this.messages.firstChild) {
				this.messages.removeChild(this.messages.firstChild);
			}
		}
	}

	_eventSaved() {
		this.addInfo("Document Saved!");
	}

	_eventSaveFailed() {
		this.addError("Save failed!!!");
	}

	_eventModified(msg) {
		console.log("_eventModified");
		console.dir(msg);
		this.addWarning("Content changed, please save document!");

	}

	_eventOpened() {
		this.reset();
	}

	_eventWarning(msg) {
		this.addWarning(msg);
	}

	_eventCleared(msg) {
		this.reset();
	}

	addInfo(msg) {
		var t = this.querySelector('#info');
		var msgslot = t.content.querySelector(".message");
		msgslot.innerHTML = msg;
		var clone = document.importNode(t.content, true);
		if (this.noOfRows <= 1) {
			this.messages.replaceChildren();
		}
		this.messages.appendChild(clone);
	}

	addWarning(msg) {
		var t = this.querySelector('#warning');
		var msgslot = t.content.querySelector(".message");
		msgslot.innerHTML = msg;
		var clone = document.importNode(t.content, true);
		if (this.noOfRows <= 1) {
			this.messages.replaceChildren();
		}
		this.messages.appendChild(clone);
	}

	addError(msg) {
		var t = this.querySelector('#error');
		var msgslot = t.content.querySelector(".message");
		msgslot.innerHTML = msg;
		var clone = document.importNode(t.content, true);
		if (this.noOfRows <= 1) {
			this.messages.replaceChildren();
		}
		this.messages.appendChild(clone);

	}

}

window.customElements.define('editor-messages', EditorMessages);