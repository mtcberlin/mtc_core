class WebTreeView extends HTMLElement {

	constructor() {
		super();
		CoreStateStore.subscribe(CoreStateStore.PAGE_EDITOR_CONTENT_ID, this.onPageEditorContentIdChanged.bind(this));
	}

	connectedCallback() {
		// workaround for chrom and safari
		// https://stackoverflow.com/questions/62962138/how-to-get-the-contents-of-a-custom-element
		setTimeout(() => {
			this.addListener();
		});
	}

	addListener() {
		const elements = this.querySelectorAll(".form-editable");
		elements.forEach((e) => {e.addEventListener("click", (event) => {
			CoreStateStore.setValue(CoreStateStore.PAGE_EDITOR_REQUEST_ID, event.currentTarget.dataset.pageId);
		})});
	}

	onPageEditorContentIdChanged(pageId) {
		this.querySelectorAll(".selected").forEach((ele) => {ele.classList.toggle("selected", false)});
		this.querySelector('[data-page-id="' + pageId + '"]').classList.toggle("selected", true);
		console.log("MAROTZKE " + pageId);
	}
}
window.customElements.define('web-tree-view', WebTreeView);