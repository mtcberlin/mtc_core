class ContentPackagesEditor extends CoreHTMLElement {

	saveBtn;
	rowTemplate;

	constructor() {
		super();
	}

	connectedCallback() {
		// workaround for chrom and safari
		// https://stackoverflow.com/questions/62962138/how-to-get-the-contents-of-a-custom-element
		setTimeout(() => {
			this.addListener();
			this.readTemplate();
			this.getPackageNames();
		});
	}

	readTemplate() {
		this.rowTemplate = this.querySelector("#row-template").content;
	}

	addListener() {
		this.saveBtn = this.querySelector("#submitBtn");
		this.saveBtn.addEventListener("click", this.uploadPackage.bind(this));
	}

	uploadPackage() {

		var input = document.getElementById("zip");
		var files = input.files;
		var formData = new FormData();
		formData.append("zip", files[0]);

		this.ajax({
			type: "POST",
			url: "/api/core/contentpackages/upload",
			data: formData,

		}, (result) => {
			if (result.success) {
				this.getPackageNames();
			}
		}, () => {

		});
	}

	getPackageNames() {
		this.ajax({
			type: "GET",
			url: "/api/admin/contentpackage/packages"
		}, (result) => {
			if (result.success) {
				this._renderResultList(result.data);
			}
		}, () => {

		});
	}

	delete(e) {
		e.preventDefault();
		const name = e.currentTarget.dataset.name;
		this.ajax({
			type: "DELETE",
			url: "/api/admin/contentpackage/delete",
			data: { name: name }
		}, (result) => {
			if (result.success) {
				this._renderResultList(result.data);
			}
		}, () => {

		});
	}

	_renderResultList(data) {
		let resultlist = document.querySelector(".js-result-list");
		resultlist.innerHTML = "";
		data.forEach((filename) => {
			const result = this.rowTemplate.cloneNode(true);
			result.querySelector(".js-name").innerText = filename;
			result.querySelector(".js-delete").dataset.name = filename;
			result.querySelector(".js-delete").addEventListener("click", this.delete.bind(this));
			result.querySelector(".js-download").dataset.name = filename;
			result.querySelector(".js-download").addEventListener("click", this.download.bind(this));

			resultlist.appendChild(result);
		});
	}

	download(e) {
		e.preventDefault();
		const name = e.currentTarget.dataset.name;
		window.open(`/api/admin/contentpackage/download?name=${name}`, '_blank').focus();
	}
}
window.customElements.define('contentpackages-editor', ContentPackagesEditor);