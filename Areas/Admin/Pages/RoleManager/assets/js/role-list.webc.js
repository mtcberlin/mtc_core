class RoleList extends HTMLElement {

	connectedCallback() {
		// workaround for chrom and safari
		// https://stackoverflow.com/questions/62962138/how-to-get-the-contents-of-a-custom-element
		setTimeout(() => {
			this.addListener();
		});
	}

	addListener() {
		this.querySelector("#AddRole").addEventListener("click", this.onAddRole.bind(this));
		this.querySelectorAll(".js-edit-role").forEach(ele => ele.addEventListener("click", this.onEditRole.bind(this)));
	}

	onAddRole() {
		this.querySelector(".js-add-role").classList.toggle("h-hidden", false);
	}

	onEditRole(e) {
		if(e.currentTarget) {
			const roleHash = e.currentTarget.dataset.roleName;
			this.querySelector(`.js-show-${roleHash}`).classList.toggle("h-hidden", true);
			this.querySelector(`.js-edit-${roleHash}`).classList.toggle("h-hidden", false);
		}
	}
}
window.customElements.define('role-list', RoleList);