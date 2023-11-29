class UserList extends HTMLElement {
	
	connectedCallback() {
		// workaround for chrom and safari
		// https://stackoverflow.com/questions/62962138/how-to-get-the-contents-of-a-custom-element
		setTimeout(() => {
			this.addListener();
		});
	}

	addListener() {
		this.querySelector("#AddUser").addEventListener("click", this.onAddUser.bind(this));
		this.querySelectorAll(".js-edit-user").forEach(ele => ele.addEventListener("click", this.onEditUser.bind(this)));
	}

	onAddUser() {
		this.querySelector(".js-add-user").classList.toggle("h-hidden", false);
	}

	onEditUser(e) {
		if(e.currentTarget) {
			const userHash = e.currentTarget.dataset.userName;
			this.querySelector(`.js-show-${userHash}`).classList.toggle("h-hidden", true);
			this.querySelector(`.js-edit-${userHash}`).classList.toggle("h-hidden", false);
		}
	}

}
window.customElements.define('user-list', UserList);