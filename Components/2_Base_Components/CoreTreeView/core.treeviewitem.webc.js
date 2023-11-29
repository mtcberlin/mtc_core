class CoreTreeViewItem extends CoreHTMLElement {
	_isConnected = false;

	constructor() {
		super();

		this._rootNodeName = "core-treeview";
		this._nodeName = "core-treeview-item";
		this._treeRoot = null;

		this._type = "page"; // page,pdf,image,folder,audio,video
		this._selectableTypes = "";

		this._hasSubItems = true;
		this._subItems = [];
		this._getSubItemsApiPath = "";
		this._getItemInfoApiPath = "";

		this._contentUrl = "";
		this._contentLoaded = null;
		this._isExpanded;
		this._isExpandable = false;

		this.eventSelected = new CustomEvent("itemselected", {
			bubbles: true,
			detail: { state: () => this._isExpanded, id: () => this.id },
		});

		this.keyCode = Object.freeze({
			TAB: 9,
			RETURN: 13,
			SPACE: 32,
			PAGEUP: 33,
			PAGEDOWN: 34,
			END: 35,
			HOME: 36,
			LEFT: 37,
			UP: 38,
			RIGHT: 39,
			DOWN: 40,
		});
	}

	connectedCallback() {
		this._createShadow();
		this._title = this.getAttribute("title");
		this._type = this.getAttribute("type").toLowerCase() || "page";

		if (this.hasAttribute("data-selectable-types")) {
			this._selectableTypes = this.getAttribute("data-selectable-types");
		}

		this._treeRoot = this.closest(this._rootNodeName);

		if (
			this._treeRoot &&
			this._treeRoot.hasAttribute("data-api-subtreeitems")
		) {
			this._getSubItemsApiPath = this._treeRoot.getAttribute(
				"data-api-subtreeitems"
			);
			this._getItemInfoApiPath =
				this._treeRoot.getAttribute("data-api-iteminfo");
		}

		this.parentId = this.getAttribute("data-parentid");

		setTimeout(() => {
			this.connectedCallbackStart();
		});
	}

	connectedCallbackStart() {
		if (!this._isConnected) {
			this._isConnected = true;

			if (!this.hasAttribute("id")) {
				this.id = "id_" + this.generateUUID();
			}

			this._initNodes();
			this._initAria();
			this._addListener();
			this._initActions();
			this._initType();
			this._initSubItems();
			this._initExpandedState();
		}
	}

	_initAria() {
		this.role = "none";
		this._header.setAttribute("tabIndex", "0");
	}

	_initType() {
		switch (this._type) {
			case "page":
				this._initPage();
				break;
			case "root":
				this._initRoot();
				break;
			case "image":
				this._initImage();
				break;
			case "pdf":
				this._initPdf();
				break;
			case "folder":
			case "contentfolder":
			case "webroot":
			case "baseitem":
				this._initFolder();
				break;
			case "audio":
				this._initAudio();
				break;
			case "video":
				this._initVideo();
				break;
			default:
				console.log("Sorry, we are out of " + this._type + ".");
		}
	}

	_initNodes() {
		this._header = this.querySelector(".header");
		this._content = this.querySelector(".content");
		this._nodeTitle = this.querySelector(".title");
		this._expand = this.querySelector("#expand");
		this._collapse = this.querySelector("#collapse");
		this._actions = this.querySelector(".actions");

		this._nodeTitle.innerHTML = this._title;
	}

	_initActions() {
		if (this.hasAttribute("data-insert-options")) {
			this._insertOptions = JSON.parse(
				this.getAttribute("data-insert-options")
			);

			if (this._insertOptions && this._insertOptions.length > 0) {
				var options = "";
				this._insertOptions.forEach(
					(element) =>
						(options += `<li><a id="${element.insertType}" class="dropdown-item">${element.displayName}</a></li>`)
				);
				this._actions.innerHTML = `
				<div id="dropdown" class="dropdown">
					<button id="ddb" class="btn js-dropdown-btn outline-blue-focus" type="button" data-bs-toggle="dropdown" aria-expanded="false">
            <svg style="pointer-events: none" id="ddb-icon" xmlns="http://www.w3.org/2000/svg" width="20" height="20" fill="#404040" class="bi bi-plus-circle-fill" viewBox="0 0 16 16">
              <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zM8.5 4.5a.5.5 0 0 0-1 0v3h-3a.5.5 0 0 0 0 1h3v3a.5.5 0 0 0 1 0v-3h3a.5.5 0 0 0 0-1h-3v-3z"/>
            </svg>
          </button>
					<ul class="dropdown-menu">
						${options}
					</ul>
				</div>	
				`;
				const myDropdown = this.querySelector(".dropdown");

				// myDropdown.addEventListener('show.bs.dropdown', event => {
				// 	console.log("dropdown show");
				// })

				myDropdown.addEventListener("click", (event) => {
					event.stopPropagation();
					console.log("dropdown clicked");
					// const icon = this.querySelector("#ddb-icon");
					// console.log(icon);
					if (event.target.id != "ddb") {
						// ToDo: boostrap way
						const menu = this.querySelector(".dropdown-menu");
						menu.classList.remove("show");
						const btn = this.querySelector(".js-dropdown-btn");
						btn.setAttribute("aria-expanded", "false");
						this._onAddTreeItem(event.target.id);
					}
				});
			}
		}
	}
	_onAddTreeItem() { }

	_initPage() {
		this._isExpandable = true;
		this.querySelector("#type_page").classList.remove("hidden");
	}

	_initRoot() {
		this._isExpandable = true;
		this.querySelector("#type_root").classList.remove("hidden");
	}

	_initFolder() {
		this._isExpandable = true;
		this.querySelector("#type_folder").classList.remove("hidden");
	}

	_initImage() {
		this.querySelector("#type_image").classList.remove("hidden");
	}

	_initPdf() {
		this.querySelector("#type_pdf").classList.remove("hidden");
	}

	_initAudio() {
		this.querySelector("#type_audio").classList.remove("hidden");
	}

	_initVideo() {
		this.querySelector("#type_video").classList.remove("hidden");
	}

	_initSubItems() {
		this._hasSubItems = this.dataset.hasSubitems.toLowerCase() == "true";
		if (this._hasSubItems == false) {
			this.querySelector(".tree-toggle").classList.add("hidden");
		} else {
			this.querySelector(".tree-toggle").classList.remove("hidden");
		}
	}

	_initExpandedState() {
		if (this._isExpandable) {
			if (this._treeRoot.hasAttribute("data-itempathids")) {
				if (this._treeRoot.getAttribute("data-itempathids").includes(this.id)) {
					this.setAttribute("expanded", "true");
				}
			}

			// get expanded State from store
			let shouldBeExpanded = JSON.parse(
				CoreStateStore.getValue(
					CoreStateStore.PAGE_EDITOR_TREE_ITEM_EXPANDED + "-" + this.id
				)
			);
			if (shouldBeExpanded != null && shouldBeExpanded === true) {
				// ToDo: find a better way to handle boolean values in store
				this.toggle(true);
			} else {
				if (this.hasAttribute("expanded")) {
					var open = this.getAttribute("expanded") === "true";
					this.toggle(open);
				} else {
					if (!this._isExpanded) {
						this.toggle(false);
					}
				}
			}
		}
	}

	_addListener() {
		console.log("_addListener: " + this.id);
		this.querySelector(".tree-toggle").addEventListener(
			"click",
			this.onButtonClick.bind(this)
		);
		this.querySelector(".header").addEventListener(
			"click",
			this.onHeaderClick.bind(this)
		);
		this._header.addEventListener("keydown", this.handleKeydown.bind(this));


		this.addEventListener("dragstart", (e) => {
			//e.preventDefault();
			e.stopPropagation();
			console.log("start");
			e.dataTransfer.setData("text", this.id);
		});
		this.addEventListener("dragover", (e) => {
			e.preventDefault();
			e.stopPropagation();
			const top = this._header.offsetHeight / 3
			if (e.offsetY < top) {
				this._header.style.setProperty('border-top', '4px solid red', 'important');
				this._header.style.borderBottom = "initial";
			} else if(e.offsetY > top * 2) {
				this._header.style.borderTop = "initial";
				this._header.style.setProperty('border-bottom', '4px solid red', 'important');
			} else {
				this._header.style.borderTop = "initial";
				this._header.style.borderBottom = "initial";
			}
		});
		this.addEventListener("dragleave", (e) => {
			e.preventDefault();
			e.stopPropagation();

			this._header.style.borderTop = "initial";
			this._header.style.borderBottom = "initial";
		});
		this.addEventListener("drop", (e) => {
			e.stopPropagation();
			
			this._header.style.borderTop = "initial";
			this._header.style.borderBottom = "initial";

			if(this.closest(`[id="${e.dataTransfer.getData("text")}"]`) != null) {
				alert("Sie können das element nicht in sich selbst verschieben.");
				return;
			}

			const parent = this.closest(`[id="${this.parentId}"]`);
			const top = this._header.offsetHeight / 3
			if (e.offsetY < top) {
				parent.reorderChildren(e.dataTransfer.getData("text"), this.id, "before");
				console.log("before");
			} else if(e.offsetY > top * 2) {
				parent.reorderChildren(e.dataTransfer.getData("text"), this.id, "after");
				console.log("after");
			} else {
				this.reorderChildren(e.dataTransfer.getData("text"), this.id, "child");
				console.log("child");
			}
			
		});
	}

	reorderChildren(sourceId, targetId, position) {
		let newSortList = [];
		let i = 1;
		this._subItems.forEach((item) => {
			if(item.id === targetId) {
				if(position === "before") {
					newSortList.push({
						pageId: sourceId,
						parentId: this.id,
						sort: i
					});
					i++;
				}
				
				newSortList.push({
					pageId: targetId,
					parentId: this.id,
					sort: i
				});
				i++;

				if(position === "after") {
					newSortList.push({
						pageId: sourceId,
						parentId: this.id,
						sort: i
					});
					i++;
				}
			} else if(item.id !== sourceId) {
				newSortList.push({
					pageId: item.id,
					parentId: this.id,
					sort: i
				});
				i++;
			}
		});

		if(position === "child") {
			newSortList.push({
				pageId: sourceId,
				parentId: this.id,
				sort: i
			});
		}
		
		this.ajax(
			{
				type: "POST",
				url: "/api/core/contenteditor/reorder",
				data: JSON.stringify({pages: newSortList}),
				contentType: "application/json",
			},
			() => {
				window.location.reload();
			}, () => { }
		);
	}

	disconnectedCallback() {
		this.querySelector(".tree-toggle").removeEventListener(
			"click",
			this.onButtonClick.bind(this)
		);
		this.querySelector(".header").removeEventListener(
			"click",
			this.onHeaderClick.bind(this)
		);

		this._disconnectActions();

		//this._header.removeEventListener('keydown', this.handleKeydown.bind(this));
	}

	_disconnectActions() { }

	_refresh() {
		this.refreshNode();
	}

	refreshNode() {
		this._loadSubItems();
	}

	select() {
		if (!this._header.hasAttribute("selected")) {
			//console.log("select: " + this.id);
			const state = CoreStateStore.setValue(
				"treeview-item-selected",
				{ assetId: this.id, assetType: this._type },
				false
			);
			//console.log("state: " + state);
			if (!state.canceled) {
				// selected darf nicht gesetzt werden, muss über subscribe
				this._header.setAttribute("selected", this.id);
				this.dispatchEvent(this.eventSelected);
			}
		}
	}

	deselect() {
		//console.log("deselect");
		this._header.setAttribute("selected", "");
		this._header.removeAttribute("selected");
		//console.dir(this._header);
	}

	focusElement() {
		//console.log("focusElement: " + this.id);
		this._header.focus({ focusVisible: true });
	}

	close() {
		this.toggle(false);
	}

	open() {
		this.toggle(true);
	}

	update() {
		this._initExpandedState();
	}

	reset() {
		//console.log("reset: " + this.id);
		this.toggle(false);
		var allItems = this.querySelectorAll(this._nodeName);
		allItems.forEach((element) => {
			element.remove();
		});
	}

	_createShadow() {
		this.innerHTML = `
    <li id="" aria-labelledby="" role="treeitem" class="treeviewitem">
      <button type="button" class="header btn btn-white d-flex align-items-center justify-content-between border-bottom border-1 w-100 rounded-0 px-0 py-2 bg-white-hover outline-blue-focus">
        <div class="left d-flex">
          <div class="tree-toggle">
            <div id="expand">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-chevron-up ms-3 me-2 rotate-90"
                viewBox="0 0 16 16">
                <path fill-rule="evenodd"
                  d="M7.646 4.646a.5.5 0 0 1 .708 0l6 6a.5.5 0 0 1-.708.708L8 5.707l-5.646 5.647a.5.5 0 0 1-.708-.708l6-6z" />
              </svg>
            </div>
            <div id="collapse">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-chevron-down ms-3 me-2"
                viewBox="0 0 16 16">
                <path fill-rule="evenodd"
                  d="M1.646 4.646a.5.5 0 0 1 .708 0L8 10.293l5.646-5.647a.5.5 0 0 1 .708.708l-6 6a.5.5 0 0 1-.708 0l-6-6a.5.5 0 0 1 0-.708z" />
              </svg>
            </div>
          </div>
          <div class="tree-icon me-2">
            <div id="type_root" class="hidden">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 448 512">
                <!--! Font Awesome Pro 6.4.0 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license (Commercial License) Copyright 2023 Fonticons, Inc. -->
                <path
                  d="M210.6 5.9L62 169.4c-3.9 4.2-6 9.8-6 15.5C56 197.7 66.3 208 79.1 208H104L30.6 281.4c-4.2 4.2-6.6 10-6.6 16C24 309.9 34.1 320 46.6 320H80L5.4 409.5C1.9 413.7 0 419 0 424.5c0 13 10.5 23.5 23.5 23.5H192v32c0 17.7 14.3 32 32 32s32-14.3 32-32V448H424.5c13 0 23.5-10.5 23.5-23.5c0-5.5-1.9-10.8-5.4-15L368 320h33.4c12.5 0 22.6-10.1 22.6-22.6c0-6-2.4-11.8-6.6-16L344 208h24.9c12.7 0 23.1-10.3 23.1-23.1c0-5.7-2.1-11.3-6-15.5L237.4 5.9C234 2.1 229.1 0 224 0s-10 2.1-13.4 5.9z" />
              </svg>
            </div>
            <div id="type_page" class="hidden">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-file-earmark mx-2"
                viewBox="0 0 16 16">
                <path
                  d="M14 4.5V14a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V2a2 2 0 0 1 2-2h5.5L14 4.5zm-3 0A1.5 1.5 0 0 1 9.5 3V1H4a1 1 0 0 0-1 1v12a1 1 0 0 0 1 1h8a1 1 0 0 0 1-1V4.5h-2z" />
              </svg>
            </div>
            <div id="type_image" class="hidden">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-image mx-2"
                viewBox="0 0 16 16">
                <path d="M6.002 5.5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0z" />
                <path
                  d="M2.002 1a2 2 0 0 0-2 2v10a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V3a2 2 0 0 0-2-2h-12zm12 1a1 1 0 0 1 1 1v6.5l-3.777-1.947a.5.5 0 0 0-.577.093l-3.71 3.71-2.66-1.772a.5.5 0 0 0-.63.062L1.002 12V3a1 1 0 0 1 1-1h12z" />
              </svg>
            </div>
            <div id="type_pdf" class="hidden">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor"
                class="bi bi-file-earmark-pdf mx-2" viewBox="0 0 16 16">
                <path
                  d="M14 14V4.5L9.5 0H4a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2zM9.5 3A1.5 1.5 0 0 0 11 4.5h2V14a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1h5.5v2z" />
                <path
                  d="M4.603 14.087a.81.81 0 0 1-.438-.42c-.195-.388-.13-.776.08-1.102.198-.307.526-.568.897-.787a7.68 7.68 0 0 1 1.482-.645 19.697 19.697 0 0 0 1.062-2.227 7.269 7.269 0 0 1-.43-1.295c-.086-.4-.119-.796-.046-1.136.075-.354.274-.672.65-.823.192-.077.4-.12.602-.077a.7.7 0 0 1 .477.365c.088.164.12.356.127.538.007.188-.012.396-.047.614-.084.51-.27 1.134-.52 1.794a10.954 10.954 0 0 0 .98 1.686 5.753 5.753 0 0 1 1.334.05c.364.066.734.195.96.465.12.144.193.32.2.518.007.192-.047.382-.138.563a1.04 1.04 0 0 1-.354.416.856.856 0 0 1-.51.138c-.331-.014-.654-.196-.933-.417a5.712 5.712 0 0 1-.911-.95 11.651 11.651 0 0 0-1.997.406 11.307 11.307 0 0 1-1.02 1.51c-.292.35-.609.656-.927.787a.793.793 0 0 1-.58.029zm1.379-1.901c-.166.076-.32.156-.459.238-.328.194-.541.383-.647.547-.094.145-.096.25-.04.361.01.022.02.036.026.044a.266.266 0 0 0 .035-.012c.137-.056.355-.235.635-.572a8.18 8.18 0 0 0 .45-.606zm1.64-1.33a12.71 12.71 0 0 1 1.01-.193 11.744 11.744 0 0 1-.51-.858 20.801 20.801 0 0 1-.5 1.05zm2.446.45c.15.163.296.3.435.41.24.19.407.253.498.256a.107.107 0 0 0 .07-.015.307.307 0 0 0 .094-.125.436.436 0 0 0 .059-.2.095.095 0 0 0-.026-.063c-.052-.062-.2-.152-.518-.209a3.876 3.876 0 0 0-.612-.053zM8.078 7.8a6.7 6.7 0 0 0 .2-.828c.031-.188.043-.343.038-.465a.613.613 0 0 0-.032-.198.517.517 0 0 0-.145.04c-.087.035-.158.106-.196.283-.04.192-.03.469.046.822.024.111.054.227.09.346z" />
              </svg>
            </div>
            <div id="type_folder" class="hidden">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" fill="currentColor" class="bi bi-folder2 mx-2"
                viewBox="0 0 16 16">
                <path
                  d="M1 3.5A1.5 1.5 0 0 1 2.5 2h2.764c.958 0 1.76.56 2.311 1.184C7.985 3.648 8.48 4 9 4h4.5A1.5 1.5 0 0 1 15 5.5v7a1.5 1.5 0 0 1-1.5 1.5h-11A1.5 1.5 0 0 1 1 12.5v-9zM2.5 3a.5.5 0 0 0-.5.5V6h12v-.5a.5.5 0 0 0-.5-.5H9c-.964 0-1.71-.629-2.174-1.154C6.374 3.334 5.82 3 5.264 3H2.5zM14 7H2v5.5a.5.5 0 0 0 .5.5h11a.5.5 0 0 0 .5-.5V7z" />
              </svg>
            </div>
            <div id="type_audio" class="hidden">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-speaker mx-2"
                viewBox="0 0 16 16">
                <path
                  d="M12 1a1 1 0 0 1 1 1v12a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1h8zM4 0a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h8a2 2 0 0 0 2-2V2a2 2 0 0 0-2-2H4z" />
                <path
                  d="M8 4.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5zM8 6a2 2 0 1 0 0-4 2 2 0 0 0 0 4zm0 3a1.5 1.5 0 1 0 0 3 1.5 1.5 0 0 0 0-3zm-3.5 1.5a3.5 3.5 0 1 1 7 0 3.5 3.5 0 0 1-7 0z" />
              </svg>
            </div>
            <div id="type_video" class="hidden">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-film mx-2"
                viewBox="0 0 16 16">
                <path
                  d="M0 1a1 1 0 0 1 1-1h14a1 1 0 0 1 1 1v14a1 1 0 0 1-1 1H1a1 1 0 0 1-1-1V1zm4 0v6h8V1H4zm8 8H4v6h8V9zM1 1v2h2V1H1zm2 3H1v2h2V4zM1 7v2h2V7H1zm2 3H1v2h2v-2zm-2 3v2h2v-2H1zM15 1h-2v2h2V1zm-2 3v2h2V4h-2zm2 3h-2v2h2V7zm-2 3v2h2v-2h-2zm2 3h-2v2h2v-2z" />
              </svg>
            </div>
          </div>
          <div class="title text-start"></div>
        </div>
        <div class="right">
          <div class="actions">
          </div>
        </div>
      </button>
      <ul role="group" class="content">
        <slot></slot>
      </ul>
    </li>
    `;
	}

	_setFocusToPreviousItem() {
		var elem = this.previousElementSibling;
		if (elem) {
			if (elem._isExpanded) {
				var childs = elem.querySelectorAll(this._nodeName);
				console.log("childs: " + childs.length);
				if (childs.length > 0) {
					childs[childs.length - 1].focusElement();
				}
			} else {
				elem.focusElement();
			}
		} else {
			var prev = this.closest("li").parentNode;

			if (prev) {
				prev.focusElement();
			}
		}
	}

	_setFocusToNextItem() {
		console.log("_setFocusToNextItem isexpanded: " + this._isExpanded);
		if (this._isExpanded) {
			var items = this.querySelectorAll(this._nodeName);
			console.dir(items);
			if (items.length > 0) {
				items[0].focusElement();
			}
		} else {
			var item = this.nextElementSibling;
			if (item) {
				item.focusElement();
			} else {
				var next = this.parentNode.nextElementSibling;
				if (next) {
					next.focusElement();
				}
			}
		}
	}

	onButtonClick(e) {
		this.toggle(!this._isExpanded);
		e.stopPropagation();
	}

	onHeaderClick(e) {
		e.stopPropagation();
		this.select();
	}

	_renderSubItems(subItems) {
		while (this._content.lastElementChild) {
			this._content.removeChild(this._content.lastElementChild);
		}

		subItems.forEach((subItem) => {
			this._createSubItem(subItem);
		});
	}

	_createSubItem(subItem) {
		const item = document.createElement(this._nodeName);
		item.setAttribute("draggable", this._isDraggable);
		item.setAttribute("title", subItem.name);
		item.setAttribute("id", subItem.id);
		item.setAttribute("data-parentid", subItem.parentId);
		item.setAttribute("type", subItem.type);
		item.dataset.selectableTypes = this._selectableTypes;
		item.dataset.hasSubitems = subItem.hasSubItems;
		if (subItem.insertOptions) {
			item.dataset.insertOptions = JSON.stringify(subItem.insertOptions);
		}
		this._content.append(item);
	}

	handleKeydown(event) {
		console.log("KEY: " + event);
		if (
			event.altKey ||
			event.ctrlKey ||
			event.metaKey ||
			event.keyCode == this.keyCode.TAB
		) {
			return;
		}

		var tgt = event.currentTarget,
			flag = false,
			char = event.key,
			clickEvent;

		switch (event.keyCode) {
			case this.keyCode.SPACE:
			case this.keyCode.RETURN:
				// Create simulated mouse event to mimic the behavior of ATs
				// and let the event handler handleClick do the housekeeping.
				try {
					clickEvent = new MouseEvent("click", {
						view: window,
						bubbles: true,
						cancelable: true,
					});
				} catch (err) {
					/*
								if (document.createEvent) {
								// DOM Level 3 for IE 9+
								clickEvent = document.createEvent('MouseEvents');
								clickEvent.initEvent('click', true, true);
								}
							*/
				}
				tgt.dispatchEvent(clickEvent);
				flag = true;
				break;

			case this.keyCode.UP:
				console.log("key up: ");
				this._setFocusToPreviousItem();
				flag = true;
				break;

			case this.keyCode.DOWN:
				console.log("key down: ");
				this._setFocusToNextItem();
				flag = true;
				break;

			case this.keyCode.RIGHT:
				console.log("key right: ");
				console.log("_isExpandable" + this._isExpandable);
				console.log("_isExpanded" + this._isExpanded);

				if (this._isExpandable) {
					if (this._isExpanded) {
						this._setFocusToNextItem();
						//this.toggle(false);
					} else {
						//this.tree.expandTreeitem(this);
						this.toggle(true);
					}
				}
				flag = true;
				break;

			case this.keyCode.LEFT:
				if (this._isExpandable) {
					if (this._isExpanded) {
						//this._setFocusToNextItem(this);
						this.toggle(false);
					} else {
						this._setFocusToPreviousItem(this);
						//this._expandTreeitem();
						//this.toggle(true);
					}
				} else {
					this._setFocusToPreviousItem(this);
				}
				/*
					  if (this.isExpandable && this.isExpanded()) {
						//this.tree.collapseTreeitem(this);
						flag = true;
					  }
					  else {
						if (this.inGroup) {
						  //this.tree.setFocusToParentItem(this);
						  flag = true;
						}
					  }
					  */
				break;

			case this.keyCode.HOME:
				//this.tree.setFocusToFirstItem();
				flag = true;
				break;

			case this.keyCode.END:
				//this.tree.setFocusToLastItem();
				flag = true;
				break;

			default:
				//if (isPrintableCharacter(char)) {
				//  printableCharacter(this);
				//}
				break;
		}
		event.preventDefault();
	}

	toggle(open) {
		if (!this._isExpandable || open === this._isExpanded) {
			return;
		}

		// update the internal state
		this._isExpanded = open;

		CoreStateStore.setValue(
			CoreStateStore.PAGE_EDITOR_TREE_ITEM_EXPANDED + "-" + this.id,
			this._isExpanded
		);

		// handle DOM updates
		this.setAttribute("aria-expanded", `${open}`);
		this.setAttribute("expanded", `${open}`);

		if (open) {
			this._loadSubItems();
			this._content.classList.remove("hidden");
			this._expand.classList.add("hidden");
			this._collapse.classList.remove("hidden");
		} else {
			this._content.classList.add("hidden");
			this._expand.classList.remove("hidden");
			this._collapse.classList.add("hidden");
		}
	}

	_loadSubItems() {
		CoreStateStore.setValue(CoreStateStore.START_LOADER);

		if (this._getSubItemsApiPath == "") {
			return;
		}
		var data = {
			parentId: this.id,
		};
		if (this._selectableTypes != "") {
			data.type = this._selectableTypes;
		}
		this.ajax(
			{
				type: "GET",
				url: this._getSubItemsApiPath,
				data: data,
				contentType: "application/json",
			},
			this._onSubItemsSuccess.bind(this),
			() => {
				CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
			}
		);
	}

	_updateData() {
		this.ajax(
			{
				type: "GET",
				url: this._getItemInfoApiPath,
				data: { id: this.id },
				contentType: "application/json",
			},
			this._updateDataSuccess.bind(this),
			() => {
				CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
			}
		);
	}

	_onSubItemsSuccess(data) {
		if (data.success) {
			//this.data = data.subItems;
			if (data.subItems.length > 0) {
				this.dataset.hasSubitems = "true";
			}
			this._subItems = data.subItems;
			this._renderSubItems(data.subItems);

			this._initSubItems();
		}
		CoreStateStore.setValue(CoreStateStore.STOP_LOADER);
	}
}

window.customElements.define("core-treeview-item", CoreTreeViewItem);
