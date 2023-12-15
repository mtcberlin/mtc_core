/// <reference path="../../../../../../Components/0_Base/corehtmlelement.webc.js" />

class UserList extends CoreHTMLElement {
	
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
		this.querySelectorAll(".js-update").forEach(ele => ele.addEventListener("click", this.updateUser.bind(this)));
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

	updateUser(e) {
		const line = e.currentTarget.closest("tr");
		this.ajax({
			type: "POST",
			url: "/api/core/user/update",
			data: JSON.stringify({ 
				"userId": line.querySelector('[name="userId"]').value,
				"userName": line.querySelector('[name="userName"]').value,
				"firstName": line.querySelector('[name="firstName"]').value,
				"lastName": line.querySelector('[name="lastName"]').value,
				"email": line.querySelector('[name="email"]').value,
				"roles": line.querySelector('[name="roles"]').value,
				"newPw": line.querySelector('[name="newPw"]').value,
				"isActive": line.querySelector('[name="isActive"]').checked
			}),
			contentType: 'application/json'
		}, () => {
			window.location.reload();
		}, () => { 
			
		});
	}

}
window.customElements.define('user-list', UserList);