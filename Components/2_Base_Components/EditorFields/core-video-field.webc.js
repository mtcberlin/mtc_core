/// <reference path="../../1_BaseField/core-field-base.webc.js" />
class CoreVideoField extends CoreFileField {
  files = [];
  vodViewUploadPreview;
  vodUploadTrigger;
  videos = [];

  constructor() {
    super();
    CoreStateStore.subscribe(
      CoreStateStore.EDITOR_EVENT_SAVED,
      this._showAndRegisterDeleteBtn.bind(this)
    );
  }

  afterConnected() {
    if (this.fieldInfos.fieldValue != "") {
      this.files = this.fieldInfos.fieldValue;
    }

    this.vodViewUploadPreview = this.querySelector(
      ".js-vod-view-upload-preview"
    );
    this.vodUploadTrigger = this.querySelector(".js-upload-file");
  }

  _addListener() {
    this.querySelector(`#input-${this.id}`).addEventListener(
      "change",
      this._uploadFile.bind(this)
    );
    this.querySelector(".js-upload-file").addEventListener(
      "keyup",
      this._triggerUploadFile.bind(this)
    );
    this._registerPanelEvents();
    this._showAndRegisterDeleteBtn();
    this._allowPlayOneVideoAtTime();
  }

  _registerPanelEvents() {
    const panel = this.closest("core-panel");
    if (panel) {
      panel.addEventListener("click", this._onPanelClick.bind(this));
    }
  }

  _onPanelClick(e) {
    const expand = e.currentTarget.getAttribute("expanded");
    const activeVideo = this.querySelector(".js-video.is-active-video");
    if (expand === "false" && activeVideo) {
      activeVideo.pause();
    }
  }

  _allowPlayOneVideoAtTime() {
    this.videos = this.querySelectorAll(".js-video");
    if (this.videos.length > 0) {
      this.videos.forEach((video) => {
        video.addEventListener("play", this._stopOtherVideos.bind(this));
      });
    }
  }

  _stopOtherVideos(e) {
    const prevVideo = this.querySelector(".js-video.is-active-video");
    if (prevVideo) {
      prevVideo.classList.remove("is-active-video");
    }
    e.currentTarget.classList.add("is-active-video");

    this.videos.forEach((video) => {
      if (!video.classList.contains("is-active-video")) {
        video.pause();
      } else {
        video.play();
      }
    });
  }

  _triggerUploadFile(e) {
    if (e.keyCode === 13) {
      e.preventDefault();
      this.querySelector(`#input-${this.id}`).click();
    }
  }

  _showAndRegisterDeleteBtn() {
    const saveBtns = this.querySelectorAll(".js-delete-vod");
    if (saveBtns.length > 0) {
      saveBtns.forEach((btn) => {
        this._registerOnBtnDelete(btn);
        btn.classList.remove("d-none");
      });
    }
  }

  _registerOnBtnDelete(btn) {
    if (!btn.classList.contains("has-listener")) {
      btn.addEventListener("click", this._onVodRemove.bind(this));
      btn.classList.add("has-listener");
    }
  }

  _onUploadSuccess(result) {
    if (result.success) {
      const vodContainer = this.querySelector(".js-vod-container");
      const deleteBtns = vodContainer.querySelectorAll(".js-delete-vod");
      deleteBtns.forEach((btn) => btn.classList.remove("has-listener"));

      vodContainer.innerHTML += this._renderVodBlock(
        result.mimeType,
        result.fileName,
        result.fileSize
      );
      this._reinitAfterAppendingNewVodBlock(vodContainer);
      this.files.push({
        mimeType: result.mimeType,
        filename: result.fileName,
        fileSize: result.fileSize,
      });
      this.querySelector(".js-fileinput").value = "";
      this._setStateChanged();
    }
  }

  _reinitAfterAppendingNewVodBlock(vodView) {
    //reinit delete event of existing delete btns
    vodView.querySelectorAll(".js-delete-vod").forEach((btn) => {
      if (!btn.classList.contains("d-none")) {
        this._registerOnBtnDelete(btn);
      }
    });

    this._allowPlayOneVideoAtTime();
  }

  _onUploadProgressStart(file, event) {
    this._setUploadPreviewFileInformation(file);
    this.vodViewUploadPreview.classList.remove("d-none");
    this.vodUploadTrigger.classList.add("d-none");
  }

  _setUploadPreviewFileInformation(file) {
    this.vodViewUploadPreview.querySelector(".js-file-name").innerText =
      file.name;
    this.vodViewUploadPreview.querySelector(".js-file-mimetype").innerText =
      file.type;
    const fileSize = this.vodViewUploadPreview.querySelector(".js-file-size");

    if (file.size !== "") {
      fileSize.innerText = Math.round(file.size / 1000000);
    } else {
      fileSize.innerText = "";
    }
  }

  _onUploadProgress(e) {
    const pct = Math.round((e.loaded / e.total) * 100);
    this._updateProgressbar(pct);
  }

  _onUploadProgressFinish() {
    this._setUploadPreviewFileInformation({ name: "", type: "", size: "" });
    this._updateProgressbar(0);
    this.vodViewUploadPreview.classList.add("d-none");
    this.vodUploadTrigger.classList.remove("d-none");
  }

  _updateProgressbar(value) {
    const progressbar =
      this.vodViewUploadPreview.querySelector(".js-progress-bar");
    progressbar.style.width = `${value}%`;
    progressbar.innerText = `${value}%`;
  }
  _render() {
    const field = this.fieldInfos;

    let sources = "";
    if (field.fieldValue && field.fieldValue.length > 0) {
      field.fieldValue.forEach((source) => {
        sources += this._renderVodBlock(
          source.mimeType,
          source.filename,
          source.fileSize
        );
      });
    }

    this.innerHTML = `
			${this._renderVodView(field, sources)}`;
  }

  _renderVodView(field, sources) {
    return `
			<div class="js-vod-view c-fld">
				<label>${
          field.displayName === null ? field.fieldName : field.displayName
        }</label>
				<div>${sources}</div>
				<div class="js-vod-container"></div>
				${this._renderUploadPreview()}
				${this._renderVodInput()}
			</div>`;
  }

  _renderVodInput() {
    return `
		<div class="fld-group js-vod-input pt-3 pb-3">
			<input type="file" accept=".mp4,.ogv,.webm" class="js-fileinput control d-none d-none" id="input-${
        this.id
      }" name="${this.id}" />
			<input type="button" class="js-upload-file btn btn-dark" tabindex="0" value="${this.translate(
        "Select Video"
      )}" onclick="document.getElementById('input-${this.id}').click();" />
		</div>`;
  }

  _renderUploadPreview() {
    return `
		<div class=" d-flex flex-column flex-md-row js-vod-node mb-2">
          <div class="js-vod-view-upload-preview col bg-light p-4 me-md-1 d-none">
            <div class="d-flex flex-column flex-md-row">
                <div class="me-md-4">
                  <svg class="me-md-4" width="100%" height="100%" viewBox="0 0 300 179" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <rect opacity="0.4" width="300" height="179" fill="#73BD3A" fill-opacity="0.51"/>
                    <path d="M131.203 103V71.2738H110L152.065 38L189 71.2738H168.139V103H131.203Z" fill="#606060" stroke="black"/>
                    <path d="M79.1364 137.972V126.364H82.1534V146H79.1364V142.676H78.9318C78.4716 143.673 77.7557 144.521 76.7841 145.22C75.8125 145.911 74.5852 146.256 73.1023 146.256C71.875 146.256 70.7841 145.987 69.8295 145.45C68.875 144.905 68.125 144.087 67.5795 142.996C67.0341 141.896 66.7614 140.511 66.7614 138.841V126.364H69.7784V138.636C69.7784 140.068 70.179 141.21 70.9801 142.062C71.7898 142.915 72.821 143.341 74.0739 143.341C74.8239 143.341 75.5866 143.149 76.3622 142.766C77.1463 142.382 77.8026 141.794 78.331 141.001C78.8679 140.209 79.1364 139.199 79.1364 137.972ZM87.6793 153.364V126.364H90.5941V129.483H90.9521C91.1737 129.142 91.4805 128.707 91.8725 128.179C92.2731 127.642 92.8441 127.165 93.5856 126.747C94.3356 126.321 95.3498 126.108 96.6282 126.108C98.2816 126.108 99.739 126.521 101 127.348C102.262 128.175 103.246 129.347 103.953 130.864C104.661 132.381 105.015 134.17 105.015 136.233C105.015 138.312 104.661 140.115 103.953 141.641C103.246 143.158 102.266 144.334 101.013 145.169C99.7603 145.996 98.3157 146.409 96.6793 146.409C95.418 146.409 94.408 146.2 93.6495 145.783C92.891 145.357 92.3072 144.875 91.8981 144.338C91.489 143.793 91.1737 143.341 90.9521 142.983H90.6964V153.364H87.6793ZM90.6452 136.182C90.6452 137.665 90.8626 138.973 91.2972 140.107C91.7319 141.232 92.3668 142.114 93.2021 142.753C94.0373 143.384 95.06 143.699 96.2702 143.699C97.5316 143.699 98.5842 143.366 99.4279 142.702C100.28 142.028 100.919 141.125 101.346 139.991C101.78 138.849 101.998 137.58 101.998 136.182C101.998 134.801 101.784 133.557 101.358 132.449C100.941 131.332 100.306 130.45 99.4535 129.803C98.6097 129.146 97.5487 128.818 96.2702 128.818C95.043 128.818 94.0117 129.129 93.1765 129.751C92.3413 130.365 91.7106 131.226 91.2844 132.334C90.8583 133.433 90.6452 134.716 90.6452 136.182ZM112.634 119.818V146H109.617V119.818H112.634ZM126.137 146.409C124.364 146.409 122.809 145.987 121.471 145.143C120.141 144.3 119.102 143.119 118.352 141.602C117.61 140.085 117.239 138.312 117.239 136.284C117.239 134.239 117.61 132.453 118.352 130.928C119.102 129.402 120.141 128.217 121.471 127.374C122.809 126.53 124.364 126.108 126.137 126.108C127.91 126.108 129.461 126.53 130.79 127.374C132.129 128.217 133.168 129.402 133.91 130.928C134.66 132.453 135.035 134.239 135.035 136.284C135.035 138.312 134.66 140.085 133.91 141.602C133.168 143.119 132.129 144.3 130.79 145.143C129.461 145.987 127.91 146.409 126.137 146.409ZM126.137 143.699C127.484 143.699 128.592 143.354 129.461 142.663C130.33 141.973 130.974 141.065 131.391 139.94C131.809 138.815 132.018 137.597 132.018 136.284C132.018 134.972 131.809 133.749 131.391 132.615C130.974 131.482 130.33 130.565 129.461 129.866C128.592 129.168 127.484 128.818 126.137 128.818C124.79 128.818 123.683 129.168 122.813 129.866C121.944 130.565 121.3 131.482 120.883 132.615C120.465 133.749 120.256 134.972 120.256 136.284C120.256 137.597 120.465 138.815 120.883 139.94C121.3 141.065 121.944 141.973 122.813 142.663C123.683 143.354 124.79 143.699 126.137 143.699ZM145.419 146.46C144.174 146.46 143.045 146.226 142.031 145.757C141.017 145.28 140.211 144.594 139.615 143.699C139.018 142.795 138.72 141.705 138.72 140.426C138.72 139.301 138.941 138.389 139.385 137.69C139.828 136.983 140.42 136.429 141.162 136.028C141.903 135.628 142.721 135.33 143.616 135.134C144.52 134.929 145.427 134.767 146.339 134.648C147.532 134.494 148.5 134.379 149.241 134.303C149.991 134.217 150.537 134.077 150.877 133.881C151.227 133.685 151.402 133.344 151.402 132.858V132.756C151.402 131.494 151.056 130.514 150.366 129.815C149.684 129.116 148.649 128.767 147.26 128.767C145.819 128.767 144.69 129.082 143.872 129.713C143.054 130.344 142.478 131.017 142.146 131.733L139.282 130.71C139.794 129.517 140.475 128.588 141.328 127.923C142.189 127.25 143.126 126.781 144.14 126.517C145.163 126.244 146.169 126.108 147.157 126.108C147.788 126.108 148.512 126.185 149.331 126.338C150.157 126.483 150.954 126.786 151.721 127.246C152.497 127.706 153.14 128.401 153.652 129.33C154.163 130.259 154.419 131.503 154.419 133.062V146H151.402V143.341H151.248C151.044 143.767 150.703 144.223 150.225 144.709C149.748 145.195 149.113 145.608 148.321 145.949C147.528 146.29 146.561 146.46 145.419 146.46ZM145.879 143.75C147.072 143.75 148.078 143.516 148.896 143.047C149.723 142.578 150.345 141.973 150.762 141.232C151.189 140.49 151.402 139.71 151.402 138.892V136.131C151.274 136.284 150.993 136.425 150.558 136.553C150.132 136.672 149.637 136.778 149.075 136.872C148.521 136.957 147.98 137.034 147.451 137.102C146.931 137.162 146.51 137.213 146.186 137.256C145.402 137.358 144.669 137.524 143.987 137.754C143.314 137.976 142.768 138.312 142.35 138.764C141.941 139.207 141.737 139.812 141.737 140.58C141.737 141.628 142.125 142.42 142.9 142.957C143.684 143.486 144.677 143.75 145.879 143.75ZM167.34 146.409C165.704 146.409 164.259 145.996 163.006 145.169C161.754 144.334 160.773 143.158 160.066 141.641C159.359 140.115 159.005 138.312 159.005 136.233C159.005 134.17 159.359 132.381 160.066 130.864C160.773 129.347 161.758 128.175 163.019 127.348C164.281 126.521 165.738 126.108 167.391 126.108C168.67 126.108 169.68 126.321 170.421 126.747C171.171 127.165 171.742 127.642 172.134 128.179C172.535 128.707 172.846 129.142 173.067 129.483H173.323V119.818H176.34V146H173.425V142.983H173.067C172.846 143.341 172.531 143.793 172.121 144.338C171.712 144.875 171.129 145.357 170.37 145.783C169.612 146.2 168.602 146.409 167.34 146.409ZM167.749 143.699C168.96 143.699 169.982 143.384 170.817 142.753C171.653 142.114 172.288 141.232 172.722 140.107C173.157 138.973 173.374 137.665 173.374 136.182C173.374 134.716 173.161 133.433 172.735 132.334C172.309 131.226 171.678 130.365 170.843 129.751C170.008 129.129 168.977 128.818 167.749 128.818C166.471 128.818 165.406 129.146 164.553 129.803C163.71 130.45 163.075 131.332 162.648 132.449C162.231 133.557 162.022 134.801 162.022 136.182C162.022 137.58 162.235 138.849 162.661 139.991C163.096 141.125 163.735 142.028 164.579 142.702C165.431 143.366 166.488 143.699 167.749 143.699ZM182.285 146V126.364H185.302V146H182.285ZM183.819 123.091C183.231 123.091 182.724 122.891 182.298 122.49C181.88 122.089 181.671 121.608 181.671 121.045C181.671 120.483 181.88 120.001 182.298 119.601C182.724 119.2 183.231 119 183.819 119C184.407 119 184.91 119.2 185.327 119.601C185.754 120.001 185.967 120.483 185.967 121.045C185.967 121.608 185.754 122.089 185.327 122.49C184.91 122.891 184.407 123.091 183.819 123.091ZM193.845 134.188V146H190.828V126.364H193.743V129.432H193.998C194.458 128.435 195.157 127.634 196.095 127.028C197.032 126.415 198.243 126.108 199.725 126.108C201.055 126.108 202.218 126.381 203.216 126.926C204.213 127.463 204.988 128.281 205.542 129.381C206.096 130.472 206.373 131.852 206.373 133.523V146H203.356V133.727C203.356 132.185 202.956 130.983 202.154 130.122C201.353 129.253 200.254 128.818 198.856 128.818C197.893 128.818 197.032 129.027 196.274 129.445C195.524 129.862 194.931 130.472 194.497 131.273C194.062 132.074 193.845 133.045 193.845 134.188ZM219.812 153.773C218.355 153.773 217.102 153.585 216.054 153.21C215.006 152.844 214.132 152.358 213.433 151.753C212.743 151.156 212.193 150.517 211.784 149.835L214.188 148.148C214.46 148.506 214.805 148.915 215.223 149.375C215.641 149.844 216.212 150.249 216.936 150.589C217.669 150.939 218.628 151.114 219.812 151.114C221.398 151.114 222.706 150.73 223.737 149.963C224.768 149.196 225.284 147.994 225.284 146.358V142.369H225.028C224.807 142.727 224.491 143.17 224.082 143.699C223.682 144.219 223.102 144.683 222.344 145.092C221.594 145.493 220.58 145.693 219.301 145.693C217.716 145.693 216.293 145.318 215.031 144.568C213.778 143.818 212.786 142.727 212.053 141.295C211.328 139.864 210.966 138.125 210.966 136.08C210.966 134.068 211.32 132.317 212.027 130.825C212.734 129.325 213.719 128.166 214.98 127.348C216.241 126.521 217.699 126.108 219.352 126.108C220.631 126.108 221.645 126.321 222.395 126.747C223.153 127.165 223.733 127.642 224.134 128.179C224.543 128.707 224.858 129.142 225.08 129.483H225.386V126.364H228.301V146.562C228.301 148.25 227.918 149.622 227.151 150.679C226.392 151.744 225.369 152.524 224.082 153.018C222.804 153.521 221.381 153.773 219.812 153.773ZM219.71 142.983C220.92 142.983 221.943 142.706 222.778 142.152C223.614 141.598 224.249 140.801 224.683 139.761C225.118 138.722 225.335 137.477 225.335 136.028C225.335 134.614 225.122 133.365 224.696 132.283C224.27 131.2 223.639 130.352 222.804 129.739C221.969 129.125 220.938 128.818 219.71 128.818C218.432 128.818 217.366 129.142 216.514 129.79C215.67 130.437 215.036 131.307 214.609 132.398C214.192 133.489 213.983 134.699 213.983 136.028C213.983 137.392 214.196 138.598 214.622 139.646C215.057 140.686 215.696 141.504 216.54 142.101C217.392 142.689 218.449 142.983 219.71 142.983Z" fill="black"/>
                  </svg>
                </div>
                <div class="d-flex-column mt-2 mt-md-0 col col-md-4">
                  <div>Name: <span class="js-file-name"></span></div>
                  <div>Type: <span class="js-file-mimetype"></span></div>
                  <div>Größe: <span class="js-file-size"></span> MB</div>
                  <div class="pb-2 pt-4">
                    <div class="progress">
                      <div class="progress-bar bg-success js-progress-bar" role="progressbar" style="width:0%;" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100"></div>
                    </div>
                  </div>
                </div>
            </div>
          </div>
        </div>`;
  }

  _renderVodBlock(mimeType, filename, fileSize) {
    return `
        <div class="d-flex js-vod-node mb-3 border border-dark rounded mb-4">

					<div class="col d-flex p-3">

						<div class="d-flex flex-column flex-xl-row align-items-center justify-content-start">
							<video class="js-video me-xl-2 container-fluid mb-3 mb-xl-0" style="max-width: 350px; max-height: 300px;" controls>
								<source src="/uploads/${
                  this.itemId
                }/${filename}" type="${mimeType}">Your browser does not support ${mimeType}.
							</video>
              
              <div class="me-xl-2 mb-2 mb-xl-0">
                <div class="d-flex flex-column mb-1">
                  <div class="bg-dark-gray text-white rounded-top fw-bold px-2 py-1 fs-sm">
                    ${this.translate("Name")} 
                  </div>
                  <div class="bg-white rounded-bottom border border-dark-gray px-2 py-1">
                    <span class="js-file-type">${filename}</span>
                  </div>
                </div>

                <div class="d-flex flex-column mb-1">
                  <div class="bg-dark-gray text-white rounded-top fw-bold px-2 py-1 fs-sm">
                    ${this.translate("Typ")} 
                  </div>
                  <div class="bg-white rounded-bottom border border-dark-gray px-2 py-1">
                    <span class="js-file-type">${mimeType}</span>
                  </div>
                </div>
                
                <div class="d-flex flex-column mb-1">
                  <div class="bg-dark-gray text-white rounded-top fw-bold px-2 py-1 fs-sm">
                    ${this.translate("Size")} 
                  </div>
                  <div class="bg-white rounded-bottom border border-dark-gray px-2 py-1">
                    <span class="js-file-type">${fileSize} MB</span>
                  </div>
                </div>
              </div>
              
              </div>
              <div class="d-flex ms-auto">
                <button class="btn align-self-start outline-blue-focus rounded-0 px-4 ms-md-1 mt-2 mt-md-0 js-delete-vod d-none" data-filename="${filename}">
                  <img src="/admin/img/icons/icon_trash.png" height="24" weight="24" alt="trash icon">
                </button>
              
              </div>

          </div>

				</div>
    `;
  }

  getFieldValues() {
    //dont need to change readoly fields
    if (this.fieldInfos.readonly) return;

    return {
      fieldValue: JSON.stringify(this.files),
      fieldType: this.fieldInfos.fieldType,
      fieldPath: this.fieldInfos.fieldPath,
    };
  }

  _onVodRemove(e) {
    let filename = e.currentTarget.dataset["filename"];
    if (filename) {
      this.files = this.files.filter((file) => {
        return file.filename != filename;
      });

      e.currentTarget.closest(".js-vod-node").remove();
      this._setStateChanged();
    }
  }
}
window.customElements.define("core-video-field", CoreVideoField);
