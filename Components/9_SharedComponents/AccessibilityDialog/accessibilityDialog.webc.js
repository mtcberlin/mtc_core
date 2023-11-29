///
/// AccessibilityDialog v1.0.3
///
class AccessibilityDialog extends HTMLElement {

	_currentSettings;
	_currentProfiles;
	_isProfileChangeDone = true;
	_isSettingsChangeDone = true;
	_isColorSettingsChangeDone = true;

	//ueberschreibung sample: {"Key":"SettingFontSize","Values":[{},{},{"Label":"510%","Value": "SCHNUFFEL"}]}
	_defaultConfig = {
		Profiles: [],
		Settings: [
			{
				Key: "SettingFontSize",
				Label: "SettingFontSize",
				Type: "radio",
				Enabled: true,
				Values: [
					{ Label: "70%", Value: "access-font-s", Group: "font-size" },
					{ Label: "100%", Value: "access-font-m", Group: "font-size" },
					{ Label: "150%", Value: "access-font-l", Group: "font-size" },
					{ Label: "200%", Value: "access-font-xl", Group: "font-size" },
				],
			},
			{
				Key: "SettingLineSpacing",
				Label: "SettingLineSpacing",
				Type: "radio",
				Enabled: true,
				Values: [
					{ Label: "1", Value: "access-linespacing-s", Group: "linespacing" },
					{ Label: "1,5", Value: "access-linespacing-m", Group: "linespacing" },
					{ Label: "2", Value: "access-linespacing-l", Group: "linespacing" },
				],
			},
			{
				Key: "SettingCharacterSpacing",
				Label: "SettingCharacterSpacing",
				Type: "radio",
				Enabled: true,
				Values: [
					{
						Label: "1",
						Value: "access-letterspacing-s",
						Group: "letterspacing",
					},
					{
						Label: "1,5",
						Value: "access-letterspacing-m",
						Group: "letterspacing",
					},
					{
						Label: "2",
						Value: "access-letterspacing-l",
						Group: "letterspacing",
					},
				],
			},
			{
				Key: "SettingWordSpacing",
				Label: "SettingWordSpacing",
				Type: "radio",
				Enabled: true,
				Values: [
					{
						Label: "Ohne",
						Value: "access-wordspacing-none",
						Group: "wordspacing",
					},
					{ Label: "0,5", Value: "access-wordspacing-s", Group: "wordspacing" },
					{ Label: "1", Value: "access-wordspacing-m", Group: "wordspacing" },
					{ Label: "1.5", Value: "access-wordspacing-l", Group: "wordspacing" },
				],
			},
			{
				Key: "SettingImageSaturation",
				Label: "SettingImageSaturation",
				Type: "radio",
				Enabled: true,
				Values: [
					{ Label: "0%", Value: "access-saturation-none", Group: "saturation" },
					{ Label: "35%", Value: "access-saturation-xs", Group: "saturation" },
					{ Label: "70%", Value: "access-saturation-s", Group: "saturation" },
					{ Label: "100%", Value: "access-saturation-m", Group: "saturation" },
					{ Label: "150%", Value: "access-saturation-l", Group: "saturation" },
					{ Label: "200%", Value: "access-saturation-xl", Group: "saturation" },
				],
			},
			{
				Key: "SettingButtonSize",
				Label: "SettingButtonSize",
				Type: "radio",
				Enabled: true,
				Values: [
					{ Label: "S", Value: "access-button-s", Group: "button-size" },
					{ Label: "M", Value: "access-button-m", Group: "button-size" },
					{ Label: "L", Value: "access-button-l", Group: "button-size" },
					{ Label: "XL", Value: "access-button-xl", Group: "button-size" },
				],
			},
			{
				Key: "SettingEasyLayout",
				Label: "",
				Enabled: true,
				Type: "checkbox",
				Values: [{ Label: "SettingEasyLayout", Value: "access-easy-layout" }],
			},
			{
				Key: "SettingHideImage",
				Label: "",
				Enabled: true,
				Type: "checkbox",
				Values: [{ Label: "SettingHideImage", Value: "access-hide-image" }],
			},
			{
				Key: "SettingHighContrast",
				Label: "",
				Enabled: true,
				Type: "checkbox",
				Values: [
					{ Label: "SettingHighContrast", Value: "access-high-contrast" },
				],
			},
			{
				Key: "SettingHeadlineBigger",
				Label: "",
				Type: "checkbox",
				Values: [
					{ Label: "SettingHeadlineBigger", Value: "access-headline-l" },
				],
			},
			{
				Key: "SettingHighlightLinks",
				Label: "",
				Enabled: true,
				Type: "checkbox",
				Values: [
					{ Label: "SettingHighlightLinks", Value: "access-highlight-links" },
				],
			},
			{
				Key: "SettingHighlightForm",
				Label: "",
				Enabled: true,
				Type: "checkbox",
				Values: [
					{ Label: "SettingHighlightForm", Value: "access-highlight-form" },
				],
			},
			{
				Key: "SettingDgs",
				Label: "",
				Enabled: true,
				Type: "checkbox",
				Values: [{ Label: "SettingDgs", Value: "access-dgs-video" }],
			},
			{
				Key: "SettingDialogFeedback",
				Label: "",
				Enabled: true,
				Type: "checkbox",
				Values: [
					{ Label: "SettingDialogFeedback", Value: "access-dialog-feedback" },
				],
			},
			{
				Key: "SettingBackgroundColor",
				Label: "",
				Enabled: true,
				Type: "color",
				Values: [{ Label: "SettingBackgroundColor", Target: "body" }],
			},
			{
				Key: "SettingDyslexiaFont",
				Label: "",
				Enabled: false,
				Type: "checkbox",
				Values: [
					{ Label: "SettingDyslexiaFont", Value: "access-open-dyslexic-font" },
				],
			},
			{
				Key: "SettingFontColor",
				Label: "",
				Enabled: true,
				Type: "color",
				Values: [{ Label: "SettingFontColor", Target: "font" }],
			},
		],
	};

	settingNumber = 0;
	settingWeight = 0;
	colorIndex = 0;

	constructor() {
		super();
		this._sessionColorKey = "accessibility-color-settings";
		this._settingsKey = "accessibility-settings";
		this._profileKey = "accessibility-active-profiles";

		this._profileCheckedSelector = ".js-profile:checked";
		this._settingCheckedSelector = ".js-setting:checked";
		this._appliedSettingWrapperClass = ".js-applied-settings-wrapper";
		this._colorSettingClass = ".js-setting-bg";
		this._settingClass = ".js-setting";
		this._settingWrapperClass = ".setting-wrapper";

		this._dataSettingsAttribute = "data-settings";
		this._dataSettingNumberAttribute = "data-setting-number";
		this._dataWeightAttribute = "data-weight";
		this._hasListenerClass = "js-has-listener";
	}

	connectedCallback() {
		setTimeout(() => {
			this._connectedCallbackStart();
		});
	}

	_connectedCallbackStart() {
		this._readConfig();
		this._render();
		this._initNodes();
		this._attachEventHandlers();
		this._reloadData(() => {
			this._selectProfiles();
			this._restoreSelectedSettings();
			this._restoreColorSettings();
			this._restoreBodyClasses();
		});
	}

	_render() {
		this.innerHTML = `<dialog id="a11y-dialog">
		<div class="accessibility-dialog">
			<div class="accessbility-setting-container">
				<div class="a11y-icon-container">
					<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="40" height="40" viewBox="0 0 43.108 50.581" fill="black" aria-hidden="true">
					<defs>
						<clipPath id="clip-path">
							<rect id="Rechteck_2" data-name="Rechteck 2" width="40" height="40"/>
						</clipPath>
					</defs>
					<g id="Gruppe_18" data-name="Gruppe 18">
						<path id="Pfad_21" data-name="Pfad 21" d="M15.705,279.643a86.049,86.049,0,0,1-5.351,23.125,3.541,3.541,0,0,0,6.623,2.511,94.985,94.985,0,0,0,4.576-16.2,94.932,94.932,0,0,0,4.576,16.2,3.541,3.541,0,0,0,6.623-2.511,86.043,86.043,0,0,1-5.349-23.11,114.266,114.266,0,0,0,12.731-1.342A3.542,3.542,0,0,0,39,271.325a110.167,110.167,0,0,1-34.909,0,3.542,3.542,0,0,0-1.095,7,123.047,123.047,0,0,0,12.711,1.324Z" transform="translate(0 -256.985)" fill-rule="evenodd"/>
						<path id="Pfad_22" data-name="Pfad 22" d="M300.537,6.392A6.392,6.392,0,1,1,294.145,0a6.392,6.392,0,0,1,6.392,6.392" transform="translate(-272.592)" fill-rule="evenodd"/>
					</g>
				</svg>
					<button class="close-btn js-close-modal" aria-label='${this._labels.LabelClose
			}'>
						<svg xmlns="http://www.w3.org/2000/svg" width="50" height="50" fill="black" class="bi bi-x-lg"
							viewBox="0 0 16 16">
							<path
								d="M2.146 2.854a.5.5 0 1 1 .708-.708L8 7.293l5.146-5.147a.5.5 0 0 1 .708.708L8.707 8l5.147 5.146a.5.5 0 0 1-.708.708L8 8.707l-5.146 5.147a.5.5 0 0 1-.708-.708L7.293 8 2.146 2.854Z" />
						</svg>
					</button>
				</div>
				<h2 class="accessibility-dialog-header">${this._labels.AccessibilityDialogHeader
			}</h2>
				<div class="menue">
					<button class="js-reset reset-btn d-flex align-items-center justify-content-center" aria-label='${this._labels.ResetAllProfiles
			}'>
					<svg id="zurücksetzen" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 21.051 24.169" fill="black" class="me-2">
						<path data-name="Pfad 129" d="M0,13.643A10.526,10.526,0,1,0,11.717,3.191L12.912,2A1.169,1.169,0,0,0,11.258.342L8.139,3.461a1.169,1.169,0,0,0,0,1.654l3.119,3.119A1.17,1.17,0,1,0,12.912,6.58l-.995-.995A8.174,8.174,0,1,1,4.737,7.854,1.169,1.169,0,1,0,3.083,6.2,10.456,10.456,0,0,0,0,13.643Z" transform="translate(0 0)"/>
					</svg>
						${this._labels.LabelBtnReset}
					</button>
				</div>

				${this._config.Profiles != undefined && this._config.Profiles.length > 0
				? this._renderProfiles()
				: ""
			}

				<details class="js-applied-settings" open="">
					<summary>${this._labels.HeadlineActiveSettings}</summary>
					<div class="js-applied-settings-wrapper"></div>
				</details>

				${this._config.Settings != undefined && this._config.Settings.length > 0
				? this._renderSettings()
				: ""
			}
			</div>

			<div class="accessbility-close-container">
				<button class="reset-btn reset-btn-sticky js-close-modal d-flex align-items-center justify-content-center"
					aria-label='${this._labels.LabelClose}'>
					<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24.582 24.582" fill="black" class="me-2">
						<g id="schließen" transform="translate(1.061 1.061)">
							<line data-name="Linie 5" x2="22.46" y2="22.46" fill="none" stroke="#000" stroke-width="3"/>
							<line data-name="Linie 6" x1="22.46" y2="22.46" fill="none" stroke="#000" stroke-width="3"/>
						</g>
					</svg>
					${this._labels.LabelClose}
				</button>
			</div>
		</div>
	</dialog>`;
	}

	_renderProfiles() {
		return `<details open="">
		<summary>${this._labels.SectionTitleProfiles}</summary>
		<div class="js-profiles">
			${this._config.Profiles.map(this._renderProfile.bind(this)).join("")}
		</div>
	</details>`;
	}

	_renderProfile(profile) {
		return `<div class="setting-wrapper">
			<div class="a11y-settings-row justify-content-start">

				<div class="toggle-entry">
					<label class="toggle d-flex" aria-labelledby="${profile.Id}">
            <div class="me-3">
              <input id="${profile.Id
			}" class="toggle-checkbox js-profile" type="checkbox"
                data-settings="${profile.Settingclasses}"
                aria-label='${this._labels.DialogAriaLabelProfile
			} ${this._getLabel(profile.Name)} ${this._labels.DialogSelect}'
              >
              <span class="toggle-switch js-toggle-setting" aria-labelledby="${profile.Id
			}" tabindex="0"></span>
            </div>
            <div clas="d-flex flex-column">
				      <span class="toggle-label ms-0 fw-bold"
					      aria-label='${this._labels.DialogAriaLabelProfile} ${this._getLabel(
				profile.Name
			)}'>${this._getLabel(profile.Name)}
				      </span>
				      <p>${this._getLabel(profile.Description)}</p>
			      </div>
					</label>
				</div>

		   
			</div>
		</div>`;
	}

	_renderSettings() {
		return `<details open="">
		<summary>${this._labels.SectionTitleAllSettings}</summary>
		<div class="settings js-all-settings">
		${this._config.Settings.map((settingDef) => {
			if (settingDef.Enabled) {
				switch (settingDef.Type.toLowerCase()) {
					case "radio":
						return this._renderRadioSetting(settingDef);

					case "checkbox":
						return this._renderCheckboxSetting(settingDef);

					case "color":
						return this._renderColorSetting(settingDef);
				}
			}
		}).join("")}
		</div>
	</details>`;
	}

	_renderRadioSetting(settingDef) {
		let i = 0;
		return `<div class="setting-wrapper" data-setting-number="${this.settingNumber
			}">
		<div class="subheader"
			aria-label='${this._labels.DialogAriaLabelSetting} ${this._getLabel(
				settingDef.Label
			)}'>
			${this._getLabel(settingDef.Label)}</div>
		
      <fieldset>
        <legend aria-label="${this._getLabel(settingDef.Label)}"></legend>
        ${settingDef.Values.map((setting) => {
				let result = `<div class="">
            <input type="radio" id="${setting.Group
					}-${i}" class="js-setting" name="${setting.Group}"
              data-settings="${setting.Value}" data-weight="${this.settingWeight
					}"
              aria-label='${this._labels.DialogAriaLabelSetting
					} ${this._getLabel(settingDef.Label)} ${this._getLabel(
						setting.Label
					)} ${this._labels.DialogSelect}'>
            <label for="${setting.Group}-${i}">${this._getLabel(
						setting.Label
					)}</label>
          </div>`;
				i++;
				this.settingNumber++;
				this.settingWeight++;
				return result;
			}).join("")}
      </fieldset>

		<button class="reset-setting-btn js-reset-setting d-flex align-items-center justify-content-center"
			aria-label='${this._labels.DialogAriaLabelSetting} ${this._getLabel(
				settingDef.Label
			)} ${this._labels.DialogReset}'>
		<svg id="zurücksetzen" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 21.051 24.169" fill="black" class="me-2">
			<path data-name="Pfad 129" d="M0,13.643A10.526,10.526,0,1,0,11.717,3.191L12.912,2A1.169,1.169,0,0,0,11.258.342L8.139,3.461a1.169,1.169,0,0,0,0,1.654l3.119,3.119A1.17,1.17,0,1,0,12.912,6.58l-.995-.995A8.174,8.174,0,1,1,4.737,7.854,1.169,1.169,0,1,0,3.083,6.2,10.456,10.456,0,0,0,0,13.643Z" transform="translate(0 0)"/>
		</svg>
		${this._labels.DialogReset}
		</button>
	</div>`;
	}

	_renderCheckboxSetting(settingDef) {
		return `<div class="setting-wrapper" data-setting-number="${this.settingNumber
			}">

		<div class="subheader">${this._getLabel(settingDef.Label)}</div>
		  ${settingDef.Values.map((setting) => {
				let result = `<div class="a11y-settings-row">

      <div class="toggle-entry">
        <label class="toggle" aria-label="${this._labels.DialogAriaLabelSetting
					} ${this._getLabel(setting.Label)} ${this._labels.DialogSelect}">
          <input class="toggle-checkbox js-setting" type="checkbox"
            data-settings="${setting.Value}"
            aria-label='${this._labels.DialogAriaLabelSetting} ${this._getLabel(
						setting.Label
					)} ${this._labels.DialogSelect}' />
          <span class="toggle-switch js-toggle-setting" tabindex="0"></span>
          <span class="toggle-label"
          aria-label='${this._labels.DialogAriaLabelSetting} ${this._getLabel(
						setting.Label
					)}'>${this._getLabel(setting.Label)}</span>
        </label>
      </div>

		</div>
		<button class="reset-setting-btn js-reset-setting d-flex align-items-center justify-content-center"
				aria-label='${this._labels.DialogAriaLabelSetting} ${this._getLabel(
						setting.Label
					)} ${this._labels.DialogReset}'>
			<svg id="zurücksetzen" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 21.051 24.169" fill="black" class="me-2">
				<path data-name="Pfad 129" d="M0,13.643A10.526,10.526,0,1,0,11.717,3.191L12.912,2A1.169,1.169,0,0,0,11.258.342L8.139,3.461a1.169,1.169,0,0,0,0,1.654l3.119,3.119A1.17,1.17,0,1,0,12.912,6.58l-.995-.995A8.174,8.174,0,1,1,4.737,7.854,1.169,1.169,0,1,0,3.083,6.2,10.456,10.456,0,0,0,0,13.643Z" transform="translate(0 0)"/>
			</svg>
			${this._labels.DialogReset}
			</button>`;
				this.settingNumber++;
				return result;
			}).join("")}
	</div>`;
	}

	_renderColorSetting(settingDef) {
		return `<div class="setting-wrapper" data-setting-number="${this.settingNumber
			}">

		<div class="subheader">${settingDef.Label}</div>
		${settingDef.Values.map((setting) => {
				let result = `<label for="color-${this.colorIndex}"
			aria-label='${this._labels.DialoagAriaLabelColorSetting} ${setting.Label}'>${setting.Label}</label>

		<div class="color-input-section">
			<input type="color" id="color-${this.colorIndex}" class="js-setting-bg js-color-${this.colorIndex}"
				name="color-${this.colorIndex}" data-target="${setting.Target}"
				aria-label='${this._labels.DialoagAriaLabelColorSetting} ${setting.Label} ${this._labels.DialogSelect}'>
			<button class="reset-setting-btn-color js-reset-setting-color d-flex align-items-center justify-content-center w-100"
				aria-label='${this._labels.DialoagAriaLabelColorSetting} ${setting.Label} ${this._labels.DialogReset}'>
				<svg id="zurücksetzen" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 21.051 24.169" fill="black" class="me-2">
					<path data-name="Pfad 129" d="M0,13.643A10.526,10.526,0,1,0,11.717,3.191L12.912,2A1.169,1.169,0,0,0,11.258.342L8.139,3.461a1.169,1.169,0,0,0,0,1.654l3.119,3.119A1.17,1.17,0,1,0,12.912,6.58l-.995-.995A8.174,8.174,0,1,1,4.737,7.854,1.169,1.169,0,1,0,3.083,6.2,10.456,10.456,0,0,0,0,13.643Z" transform="translate(0 0)"/>
				</svg>
				${this._labels.DialogReset}
			</button>
		</div>`;
				this.colorIndex++;
				this.settingNumber++;
				return result;
			}).join("")}
	</div>`;
	}

	_getLabel(name) {
		return this._labels[name] != undefined ? this._labels[name] : name;
	}

	_initNodes() {
		this._dialog = this.querySelector("#a11y-dialog");
		this._allSettings = this.querySelector(".js-all-settings");
		this._appliedSettings = this.querySelector(".js-applied-settings");
		this._resetAllBtns = this.querySelectorAll(".js-reset");
		this._resetSettingsBtns = this.querySelectorAll(".js-reset-setting");
		this._profileBtns = this.querySelectorAll(".js-profile");
		this._settingBtns = this.querySelectorAll(this._settingClass);
		this._settingColorBtns = this.querySelectorAll(this._colorSettingClass);
		this._resetSettingColorBtns = this.querySelectorAll(
			".js-reset-setting-color"
		);
		this._closeModalBtns = this.querySelectorAll(".js-close-modal");
		this._toggleSettingBtns = this.querySelectorAll(".js-toggle-setting");
		this._btnOpen = document.getElementById(this.dataset.btnid);
		this._appliedSettingWrapper = this.querySelector(
			this._appliedSettingWrapperClass
		);
	}

	_readConfig() {
		this._easyUrl = this.getAttribute("data-easy-url");
		this._normalUrl = this.getAttribute("data-normal-url");
		this._savePath = this.getAttribute("data-save-path");
		this._loadPath = this.getAttribute("data-load-path");
		this._labels = JSON.parse(this.dataset.labels);
		if (this.dataset.config && this.dataset.config.length > 0) {
			this._config = this._mergeConfig(JSON.parse(this.dataset.config));
		} else {
			this._config = this._defaultConfig;
		}
	}

	_mergeConfig(customConfig) {
		customConfig.Settings.forEach((value) => {
			var match = this._defaultConfig.Settings.find((setting) => {
				return setting.Key === value.Key;
			});
			if (match === undefined) {
				this._defaultConfig.Settings.push(value);
			} else {
				this._mergeProperty(match, value);
			}
		});
		customConfig.Profiles.forEach((value) => {
			var match = this._defaultConfig.Profiles.find((setting) => {
				return setting.Id === value.Id;
			});
			if (match === undefined) {
				this._defaultConfig.Profiles.push(value);
			} else {
				this._mergeProperty(match, value);
			}
		});

		return this._defaultConfig;
	}

	_mergeProperty(match, value) {
		Object.keys(match).forEach((key) => {
			if (value[key] != undefined) {
				if (Array.isArray(match[key]) || typeof match[key] === "object") {
					this._mergeProperty(match[key], value[key]);
				} else {
					match[key] = value[key];
				}
			}
		});
	}

	_attachEventHandlers() {
		if (this._btnOpen) {
			this._btnOpen.addEventListener("click", (e) => {
				this.config();
			});
		}

		this._resetAllBtns.forEach((el) =>
			el.addEventListener("click", this._onResetAllSettings.bind(this))
		);
		this._resetSettingsBtns.forEach((el) =>
			el.addEventListener("click", this._onResetSetting.bind(this))
		);
		this._profileBtns.forEach((el) =>
			el.addEventListener("change", this._onProfileChange.bind(this))
		);
		this._settingBtns.forEach((el) =>
			el.addEventListener("change", this._onSettingChange.bind(this))
		);
		this._settingColorBtns.forEach((el) =>
			el.addEventListener("input", this._onColorBgChange.bind(this))
		);
		this._resetSettingColorBtns.forEach((el) =>
			el.addEventListener("click", this._onResetColor.bind(this))
		);
		this._closeModalBtns.forEach((el) =>
			el.addEventListener("click", this._closeModal.bind(this))
		);
		this._toggleSettingBtns.forEach((el) =>
			el.addEventListener("keydown", this._toggleSetting.bind(this))
		);

		this.querySelectorAll(
			`${this._settingClass}[${this._dataSettingsAttribute}="access-hide-image"]`
		).forEach((el) => {
			if (!el.classList.contains(this._hasListenerClass)) {
				el.addEventListener("change", this._onHideImages.bind(this));
				el.classList.add(this._hasListenerClass);
			}
		});

		this.querySelectorAll(
			`${this._settingClass}[data-settings="access-easy-lang"]`
		).forEach((el) => {
			if (!el.classList.contains(this._hasListenerClass)) {
				el.addEventListener("change", this._onEasyLangChange.bind(this));
				el.classList.add(this._hasListenerClass);
			}
		});

		let observer = new MutationObserver((event) => {
			if (event[0].attributeName == "open" && event[0].target.open) {
				this._beepOpen();
			} else if (event[0].attributeName == "open" && !event[0].target.open) {
				this._beepClose();
			}
		});

		this.querySelectorAll("dialog").forEach((ele) => {
			observer.observe(ele, { attributes: true });
		});
	}

	_closeModal() {
		this._dialog.close();
	}

	_toggleSetting(e) {
		if (e.keyCode === 32) {
			e.preventDefault();
			const checkbox = e.currentTarget
				.closest(".toggle")
				.querySelector('input[type="checkbox"]');
			if (checkbox) {
				checkbox.click();
			}
		}
	}

	_onProfileChange(e) {
		this._isProfileChangeDone = false;
		this._storeActiveProfiles();
		const profile = e.currentTarget;
		this._mergeProfileSettings(profile);
	}

	_storeActiveProfiles() {
		const profileIds = [];
		this.querySelectorAll(this._profileCheckedSelector).forEach((p) =>
			profileIds.push(p.id)
		);
		this._setStoredProfiles(profileIds);
	}

	_onResetSetting(e) {
		const settingWrapper = e.currentTarget.closest(
			`${this._settingWrapperClass}`
		);
		if (settingWrapper) {
			const selectedSetting = settingWrapper.querySelector(
				`${this._settingCheckedSelector}`
			);
			if (selectedSetting) {
				const setting = selectedSetting.getAttribute(
					this._dataSettingsAttribute
				);
				let bodyClasses = Array.from(document.body.classList);
				let accessibilityClasses = bodyClasses.filter((c) =>
					c.startsWith("access-")
				);
				accessibilityClasses = accessibilityClasses.filter((c) => {
					return c !== setting;
				});
				const newBodyClasses = bodyClasses.filter((c) => c !== setting);
				this._setBodyClasses(newBodyClasses);
				this._storeAccessibilitySettings(accessibilityClasses);

				const settingNumber = settingWrapper.getAttribute(
					`${this._dataSettingNumberAttribute}`
				);
				this._appliedSettings
					.querySelector(
						`[${this._dataSettingNumberAttribute}='${settingNumber}']`
					)
					.remove();

				const originalSetting = this._allSettings.querySelector(
					`[${this._dataSettingNumberAttribute}='${settingNumber}']`
				);
				const settingElem = originalSetting.querySelector(this._settingClass);

				if (
					settingElem.type === "checkbox" &&
					settingElem.classList.contains(this._hasListenerClass)
				) {
					settingElem.click();
				} else {
					if (settingElem.type === "radio" || settingElem.type === "checkbox") {
						originalSetting.querySelector(
							this._settingCheckedSelector
						).checked = false;
					}
				}
			}
		}
	}

	_onSettingChange(e) {
		const element = e.currentTarget;
		const elemSetting = element.getAttribute(
			this._dataSettingsAttribute
		);

		if (elemSetting) {
			let bodyClasses = Array.from(document.body.classList);

			const notAccessibilityClasses = bodyClasses.filter(
				(c) => !c.startsWith("access-")
			);
			if (element.type === "radio") {
				const radioGroupe = this.querySelectorAll(
					`${this._settingClass}[name="${element.name}"]`
				);
				const settingsToMerge = Array.from(radioGroupe).map((r) =>
					r.getAttribute(this._dataSettingsAttribute)
				);
				this._currentSettings = this._currentSettings.filter(
					(s) => !settingsToMerge.includes(s)
				);
				this._currentSettings.push(elemSetting);
			}

			if (element.type === "checkbox") {
				if (!element.checked) {
					this._currentSettings = this._currentSettings.filter((c) => {
						return c !== elemSetting;
					});
				}
				if (
					element.checked &&
					!this._currentSettings.includes(elemSetting)
				) {
					this._currentSettings.push(elemSetting);
				}
			}

			if (!e.currentTarget.preventSave) {
				this._storeAccessibilitySettings(this._currentSettings);
			}

			delete e.currentTarget.preventSave;

			this._appendSettingToAppliedSettings(element);
			bodyClasses = this._currentSettings.concat(notAccessibilityClasses);
			this._setBodyClasses(bodyClasses);
		} else {
			console.warn(`No setting configuration for elem ${element.name}`);
		}
	}

	_appendSettingToAppliedSettings(settingElem) {
		const settingWrapper = settingElem.closest(`${this._settingWrapperClass}`);
		if (settingWrapper) {
			const clonedSettingWrapper = settingWrapper.cloneNode(true);
			const clonedSettings = Array.from(
				this._appliedSettingWrapper.childNodes
			).filter(
				(node) =>
					node.getAttribute(`${this._dataSettingNumberAttribute}`) ==
					clonedSettingWrapper.getAttribute(
						`${this._dataSettingNumberAttribute}`
					)
			);

			if (clonedSettings.length === 0) {
				this._cloneSettingAndAppendToAppliedSettings(clonedSettingWrapper);
			}

			this._syncAppliedAndAllSettings(settingElem);
			this._registerAppliedSettings();
		}
	}

	_cloneSettingAndAppendToAppliedSettings(clonedSettingWrapper) {
		clonedSettingWrapper.querySelectorAll(this._settingClass).forEach((s) => {
			s.id = `${s.id}-clone`;
			s.name = `${s.name}-clone`;
		});

		clonedSettingWrapper
			.querySelectorAll("label")
			.forEach((l) => (l.htmlFor = `${l.htmlFor}-clone`));
		this._appliedSettingWrapper.append(clonedSettingWrapper);
	}

	_syncAppliedAndAllSettings(target) {
		const settingWrapper = target.closest(`${this._settingWrapperClass}`);

		if (target.type === "radio") {
			const dataSettings = target.getAttribute(this._dataSettingsAttribute);
			this._allSettings.querySelector(
				`[data-settings='${dataSettings}']`
			).checked = true;
			this._appliedSettings.querySelector(
				`[data-settings='${dataSettings}']`
			).checked = true;
		}

		if (target.type === "checkbox" && !target.checked) {
			const settingNumber = settingWrapper.getAttribute(
				`${this._dataSettingNumberAttribute}`
			);
			this._appliedSettings
				.querySelector(
					`[${this._dataSettingNumberAttribute}='${settingNumber}']`
				)
				.remove();

			const originalSetting = this._allSettings.querySelector(
				`[${this._dataSettingNumberAttribute}='${settingNumber}']`
			);
			originalSetting.querySelector(this._settingClass).checked = false;
		}

		if (target.type === "color") {
			const targetAttribute = target.getAttribute("data-target");
			this._allSettings.querySelector(
				`.js-setting-bg[data-target='${targetAttribute}']`
			).value = target.value;
			this._appliedSettings.querySelector(
				`.js-setting-bg[data-target='${targetAttribute}']`
			).value = target.value;
		}
	}

	_registerAppliedSettings() {
		this._registerAppliedSetting(
			this._settingClass,
			"change",
			this._onSettingChange.bind(this)
		);
		this._registerAppliedSetting(
			".js-reset-setting",
			"click",
			this._onResetSetting.bind(this)
		);
		this._registerAppliedSetting(
			this._colorSettingClass,
			"input",
			this._onColorBgChange.bind(this)
		);
		this._registerAppliedSetting(
			".js-reset-setting-color",
			"click",
			this._onResetColor.bind(this)
		);
		this._registerAppliedSetting(
			".js-toggle-setting",
			"keydown",
			this._toggleSetting.bind(this)
		);

		const hideImagesClonesBtn = this._appliedSettingWrapper.querySelector(
			`${this._settingClass}[${this._dataSettingsAttribute}="access-hide-image"]`
		);
		if (hideImagesClonesBtn) {
			hideImagesClonesBtn.addEventListener(
				"change",
				this._onHideImages.bind(this)
			);
			hideImagesClonesBtn.addEventListener(
				"change",
				this._onSettingChange.bind(this)
			);
		}
		const accessEasyLangBtn = this._appliedSettingWrapper.querySelector(
			`${this._settingClass}[${this._dataSettingsAttribute}="access-easy-lang"]`
		);
		if (accessEasyLangBtn) {
			accessEasyLangBtn.addEventListener(
				"change",
				this._onEasyLangChange.bind(this)
			);
			accessEasyLangBtn.addEventListener(
				"change",
				this._onSettingChange.bind(this)
			);
		}
	}

	_registerAppliedSetting(selector, eventType, listener) {
		this._appliedSettingWrapper.querySelectorAll(selector).forEach((el) => {
			if (!el.classList.contains(this._hasListenerClass)) {
				el.addEventListener(eventType, listener);
				el.classList.add(this._hasListenerClass);
			}
		});
	}

	_onColorBgChange(e) {
		const colorElem = e.currentTarget;
		const targetToColor = colorElem.getAttribute("data-target");

		if (targetToColor) {
			const colorSetting = {
				target: targetToColor,
				color: e.currentTarget.value,
				id: e.currentTarget.id,
			};
			this._changeTargetColor(colorSetting.target, colorSetting.color);
			this._appendSettingToAppliedSettings(colorElem);
			this._storeAccessibilityColorSettings(colorSetting);
		} else {
			console.warn(`No target color definition for ${colorElem.name}`);
		}
	}

	_changeTargetColor(target, color) {
		switch (target.toLowerCase()) {
			case "body":
				document.body.style.backgroundColor = color;
				break;
			case "font":
				document.body.style.color = color;
				break;
		}
	}

	_removeCbFromAppliedSettingsAndReset(target) {
		const settingNumber = target
			.closest(`${this._settingWrapperClass}`)
			.getAttribute(`${this._dataSettingNumberAttribute}`);
		this._appliedSettingWrapper
			.querySelector(`[${this._dataSettingNumberAttribute}='${settingNumber}']`)
			.remove();
		target.checked = false;
		const newBodyClasses = Array.from(document.body.classList).filter(
			(c) => c !== target.getAttribute(this._dataSettingsAttribute)
		);
		this._setBodyClasses(newBodyClasses);
	}

	_mergeProfileSettings(profile) {
		const settings = profile
			.getAttribute(this._dataSettingsAttribute)
			.split(" ");
		if (settings && settings.length) {
			this._isSettingsChangeDone = false;
			profile.checked
				? this._selectProfileAndAssociatedSettings(settings)
				: this._deselectProfileAndAssociatedSettings(settings);
		}
	}

	_setBodyClasses(bodyClasses) {
		const bodyClassesString = bodyClasses.toString().replaceAll(",", " ");
		document.body.classList = bodyClassesString;
	}

	_selectProfileAndAssociatedSettings(settings) {
		settings.forEach((profileSetting, index) => {
			const preventSave = settings.length - 1 !== index;
			const targetSettingElem = this._allSettings.querySelector(
				`[${this._dataSettingsAttribute}='${profileSetting}']`
			);
			if (targetSettingElem && !targetSettingElem.checked) {
				const settingWeight = targetSettingElem.getAttribute(
					`${this._dataWeightAttribute}`
				);
				if (settingWeight) {
					const settingGroup = targetSettingElem.closest(
						`${this._settingWrapperClass}`
					);
					if (settingGroup) {
						const previousSetting = settingGroup.querySelector(
							`${this._settingCheckedSelector}`
						);
						if (previousSetting) {
							const previousSettingWeight = previousSetting.getAttribute(
								`${this._dataWeightAttribute}`
							);
							if (
								previousSettingWeight &&
								settingWeight > previousSettingWeight
							) {
								targetSettingElem.preventSave = preventSave;
								targetSettingElem.click();
							}
						} else {
							targetSettingElem.preventSave = preventSave;
							targetSettingElem.click();
						}
					}
				} else {
					targetSettingElem.preventSave = preventSave;
					targetSettingElem.click();
				}
			}
		});
	}

	_deselectProfileAndAssociatedSettings(settings) {
		const activeProfiles = this.querySelectorAll(this._profileCheckedSelector);
		if (activeProfiles) {
			let activeSettings = Array.from(activeProfiles).map((p) =>
				p.getAttribute(this._dataSettingsAttribute)
			);
			activeSettings = [
				...new Set(activeSettings.toString().replaceAll(",", " ").split(" ")),
			];

			const settingsToRemove = settings.filter(
				(p) => !activeSettings.includes(p)
			);
			if (settingsToRemove && settingsToRemove.length > 0) {
				settingsToRemove.forEach((setting, index) => {
					const preventSave = settingsToRemove.length - 1 !== index;
					const el = this._allSettings.querySelector(
						`input[${this._dataSettingsAttribute}='${setting}']`
					);
					if (el.checked) {
						if (el.type === "checkbox") {
							el.preventSave = preventSave;
							el.click();
						}
						if (el.type === "radio") {
							const settingGroupValues = Array.from(
								el
									.closest(`${this._settingWrapperClass}`)
									.querySelectorAll(this._settingClass)
							).map((i) => i.getAttribute(this._dataSettingsAttribute));
							const setting = el.getAttribute(this._dataSettingsAttribute);
							const settingNumber = el
								.closest(`${this._settingWrapperClass}`)
								.getAttribute(`${this._dataSettingNumberAttribute}`);
							const activeSetting = settingGroupValues.filter((i) =>
								activeSettings.includes(i)
							);

							if (activeSetting.length === 0) {
								this.querySelector(".js-applied-settings")
									.querySelector(
										`[${this._dataSettingNumberAttribute}='${settingNumber}']`
									)
									.remove();

								this._allSettings.querySelector(
									`input[${this._dataSettingsAttribute}='${setting}']`
								).checked = false;

								const newBodyClasses = Array.from(
									document.body.classList
								).filter((c) => c !== setting);
								this._setBodyClasses(newBodyClasses);

								this._currentSettings = this._currentSettings.filter(
									(s) => s !== setting
								);
								if (!preventSave) {
									this._storeAccessibilitySettings(this._currentSettings);
								}
							} else {
								const targetElement = this._allSettings
									.querySelector(
										`input[${this._dataSettingsAttribute}='${activeSetting}']`
									);
								targetElement.preventSave = preventSave;
								targetElement.click();
							}
						}
					}
				});
			}
		}
	}

	_storeAccessibilitySettings(accessibilityClasses) {
		const accessibilitySettings = [];
		if (accessibilityClasses && accessibilityClasses.length > 0) {
			accessibilityClasses.forEach((setting) =>
				accessibilitySettings.push(setting)
			);
		}

		this._setStoredSettings(accessibilitySettings);
	}

	_storeAccessibilityColorSettings(colorSetting) {
		this._currentColorSettings = this._currentColorSettings.filter(
			(s) => s.id !== colorSetting.id
		);
		this._currentColorSettings.push(colorSetting);
		localStorage.setItem(
			this._sessionColorKey,
			JSON.stringify(this._currentColorSettings)
		);

		this._isColorSettingsChangeDone = true;
		if (this._savePath && this._isSettingsChangeDone && this._isProfileChangeDone) {
			const data = {
				[this._settingsKey]: JSON.stringify(this._currentSettings),
				[this._profileKey]: JSON.stringify(this._currentProfiles),
				[this._sessionColorKey]: JSON.stringify(this._currentColorSettings)
			};

			this._ajax({
				type: "POST",
				url: this._savePath,
				data: JSON.stringify(data),
				contentType: 'application/json'
			}, () => { }, () => { });
		}
	}

	_onResetAllSettings() {
		this._isColorSettingsChangeDone = false;
		this._isProfileChangeDone = false;
		this._isSettingsChangeDone = false;
		this._setStoredProfiles([]);
		this._setStoredSettings([]);
		this._deselectProfiles();
		this._deselectSettings();
		this._resetColorSettings();
		this._appliedSettingWrapper.innerHTML = "";
	}

	_deselectSettings() {
		this._allSettings
			.querySelectorAll(this._settingClass)
			.forEach((settingElem) => {
				const setting = settingElem.getAttribute(this._dataSettingsAttribute);
				if (setting && settingElem.checked) {
					settingElem.checked = false;
					document.body.classList.remove(setting);
				}
			});
	}

	_resetColorSettings() {
		this._isColorSettingsChangeDone = false;
		const colorSettings = this.querySelectorAll(this._colorSettingClass);
		colorSettings.forEach((e, index) => {
			const colorElem = this.querySelector(`#${e.id}`);
			this._resetColor(colorElem, colorSettings.length - 1 === index);
		});
	}

	_onResetColor(e) {
		const colorElem = e.currentTarget
			.closest(`${this._settingWrapperClass}`)
			.querySelector(this._colorSettingClass);
		this._resetColor(colorElem, true);
	}

	_resetColor(colorElem, store) {
		const targetToColor = colorElem.getAttribute("data-target");
		if (targetToColor && targetToColor.length > 0) {
			const colorSetting = {
				target: targetToColor,
				color: "",
				id: colorElem.id,
			};
			this._changeTargetColor(targetToColor, "");

			if (store) {
				this._storeAccessibilityColorSettings(colorSetting);
			} else {
				this._currentColorSettings = this._currentColorSettings.filter(
					(s) => s.id !== colorSetting.id
				);
				this._currentColorSettings.push(colorSetting);
			}

			const settingNumber = colorElem
				.closest(`${this._settingWrapperClass}`)
				.getAttribute(`${this._dataSettingNumberAttribute}`);
			const appliedSettingNo = this._appliedSettingWrapper.querySelector(
				`[${this._dataSettingNumberAttribute}='${settingNumber}']`
			);
			if (appliedSettingNo) {
				appliedSettingNo.remove();
				this._allSettings.querySelector(`#${colorElem.id}`).value = "#000000";
			}
		} else {
			console.warn(`No target for color elem  ${colorElem.id}`);
		}
	}

	_onHideImages(e) {
		const images = document.getElementsByTagName("img");
		if (images && images.length > 0) {
			let imageState = "";

			if (e.currentTarget.checked) {
				imageState = "none";
			} else {
				imageState = "block";
			}

			let images_length = images.length;
			for (let i = 0; i < images_length; i++) {
				images[i].style.setProperty("display", imageState, "important");
			}
		}
	}

	_onEasyLangChange(e) {
		if (this._dialog) {
			if (
				e.currentTarget.checked &&
				window.location.pathname !== this._easyUrl
			) {
				window.location.pathname = this._easyUrl;
			}

			if (!e.currentTarget.checked) {
				window.location.pathname = this._normalUrl;
			}
		}
	}

	_refreshDialogOptions() {
		const accessibilitySettings = this._currentSettings;
		const profiles = this.querySelectorAll(".accessibility-profil");
		if (profiles.length > 0) {
			profiles.forEach((profile) => {
				let activeProfiles = true;
				profile.value.split(" ").forEach((profileClass) => {
					if (accessibilitySettings.indexOf(profileClass) === -1) {
						activeProfiles = false;
					}
				});

				profile.checked = activeProfiles;
			});
		}

		if (accessibilitySettings.length > 0) {
			accessibilitySettings.forEach((setting) => {
				const ele = this.querySelector(`[value="${setting}"]`);
				this.querySelectorAll(`[name="${ele.name}"]`).forEach(
					(grEle) => (grEle.checked = false)
				);
				ele.checked = true;
			});
		}
	}

	_restoreColorSettings() {
		if (this._currentColorSettings && this._currentColorSettings.length > 0) {
			this._currentColorSettings.forEach((colorSetting) => {
				if (colorSetting.color && colorSetting.color.length > 0) {
					const el = this._allSettings.querySelector(
						`input[data-target='${colorSetting.target}']`
					);
					el.value = colorSetting.color;
					this._appendSettingToAppliedSettings(el);
					this._changeTargetColor(colorSetting.target, colorSetting.color);
				}
			});
		}
	}

	_deselectProfiles() {
		const checkedProfiles = document.querySelectorAll(
			".js-profiles input.js-profile:checked"
		);
		if (checkedProfiles && checkedProfiles.length > 0) {
			checkedProfiles.forEach((p) => (p.checked = false));
		}
	}

	_selectProfiles() {
		const profiles = this._currentProfiles;
		if (profiles && profiles.length !== 0) {
			profiles.forEach((profileId) => {
				const profileElem = this.querySelector(`#${profileId}`);
				if (profileElem) {
					profileElem.checked = "checked";
				}
			});
		}
	}

	_restoreSelectedSettings() {
		const settings = this._currentSettings;
		if (settings && settings.length > 0) {
			settings.forEach((setting) => {
				const settingElem = this._allSettings.querySelector(
					`input[${this._dataSettingsAttribute}='${setting}']`
				);
				if (
					settingElem &&
					!settingElem.classList.contains(this._hasListenerClass)
				) {
					settingElem.setAttribute("checked", "checked");
					this._appendSettingToAppliedSettings(settingElem);
				} else {
					settingElem.click();
				}
			});
		}
	}

	_restoreBodyClasses() {
		let bodyClasses = Array.from(document.body.classList);
		const accessibilitySettings = this._currentSettings;
		if (accessibilitySettings.length > 0) {
			accessibilitySettings.forEach((setting) => bodyClasses.push(setting));
		}
		this._setBodyClasses(bodyClasses);
	}

	config() {
		this._dialog.showModal();
	}

	_beepOpen() {
		if (document.body.classList.contains("access-dialog-feedback")) {
			var beep = new Audio(
				"data:audio/wav;base64,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qJrx4KOQjahZxWbcZgztj2c49nKmkId44S71j0c8eV9yDK6uPRzx5X18eDvjvQ6yKo9ZSS6l//8elePK/Lf//IInrOF/FvDoADYAGBMGb7FtErm5MXMlmPAJQVgWta7Zx2go+8xJ0UiCb8LHHdftWyLJE0QIAIsI+UbXu67dZMjmgDGCGl1H+vpF4NSDckSIkk7Vd+sxEhBQMRU8j/12UIRhzSaUdQ+rQU5kGeFxm+hb1oh6pWWmv3uvmReDl0UnvtapVaIzo1jZbf/pD6ElLqSX+rUmOQNpJFa/r+sa4e/pBlAABoAAAAA3CUgShLdGIxsY7AUABPRrgCABdDuQ5GC7DqPQCgbbJUAoRSUj+NIEig0YfyWUho1VBBBA//uQZB4ABZx5zfMakeAAAAmwAAAAF5F3P0w9GtAAACfAAAAAwLhMDmAYWMgVEG1U0FIGCBgXBXAtfMH10000EEEEEECUBYln03TTTdNBDZopopYvrTTdNa325mImNg3TTPV9q3pmY0xoO6bv3r00y+IDGid/9aaaZTGMuj9mpu9Mpio1dXrr5HERTZSmqU36A3CumzN/9Robv/Xx4v9ijkSRSNLQhAWumap82WRSBUqXStV/YcS+XVLnSS+WLDroqArFkMEsAS+eWmrUzrO0oEmE40RlMZ5+ODIkAyKAGUwZ3mVKmcamcJnMW26MRPgUw6j+LkhyHGVGYjSUUKNpuJUQoOIAyDvEyG8S5yfK6dhZc0Tx1KI/gviKL6qvvFs1+bWtaz58uUNnryq6kt5RzOCkPWlVqVX2a/EEBUdU1KrXLf40GoiiFXK///qpoiDXrOgqDR38JB0bw7SoL+ZB9o1RCkQjQ2CBYZKd/+VJxZRRZlqSkKiws0WFxUyCwsKiMy7hUVFhIaCrNQsKkTIsLivwKKigsj8XYlwt/WKi2N4d//uQRCSAAjURNIHpMZBGYiaQPSYyAAABLAAAAAAAACWAAAAApUF/Mg+0aohSIRobBAsMlO//Kk4soosy1JSFRYWaLC4qZBYWFRGZdwqKiwkNBVmoWFSJkWFxX4FFRQWR+LsS4W/rFRb/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////VEFHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAU291bmRib3kuZGUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMjAwNGh0dHA6Ly93d3cuc291bmRib3kuZGUAAAAAAAAAACU="
			);
			beep.play();
		}
	}

	_beepClose() {
		if (document.body.classList.contains("access-dialog-feedback")) {
			var beep = new Audio(
				"data:audio/mpeg;base64,/+MYxAAEaAIEeUAQAgBgNgP/////KQQ/////Lvrg+lcWYHgtjadzsbTq+yREu495tq9c6v/7vt/of7mna9v6/btUnU17Jun9/+MYxCkT26KW+YGBAj9v6vUh+zab//v/96C3/pu6H+pv//r/ycIIP4pcWWTRBBBAMXgNdbRaABQAAABRWKwgjQVX0ECmrb///+MYxBQSM0sWWYI4A++Z/////////////0rOZ3MP//7H44QEgxgdvRVMXHZseL//540B4JAvMPEgaA4/0nHjxLhRgAoAYAgA/+MYxAYIAAJfGYEQAMAJAIAQMAwX936/q/tWtv/2f/+v//6v/+7qTEFNRTMuOTkuNVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV"
			);
			beep.play();
		}
	}

	_getStoredSettings() {
		return new Promise((resolve) => {
			if (this._loadPath) {
				this._ajax({
					type: "GET",
					url: this._loadPath,
					data: { key: this._settingsKey },
					contentType: 'application/json'
				}, (result) => {
					resolve(JSON.parse(result.data) ?? []);
				}, () => { })
			} else {
				resolve(JSON.parse(localStorage.getItem(this._settingsKey)) ?? []);
			}
		});
	}

	_getStoredColorSettings() {
		return new Promise((resolve) => {
			if (this._loadPath) {
				this._ajax({
					type: "GET",
					url: this._loadPath,
					data: { key: this._sessionColorKey },
					contentType: 'application/json'
				}, (result) => {
					resolve(JSON.parse(result.data) ?? []);
				}, () => { })
			} else {
				resolve(JSON.parse(localStorage.getItem(this._sessionColorKey)) ?? []);
			}
		});
	}

	_setStoredSettings(accessibilitySettings) {
		this._currentSettings = accessibilitySettings;
		localStorage.setItem(this._settingsKey, JSON.stringify(accessibilitySettings));

		this._isSettingsChangeDone = true;
		if (this._savePath && this._isProfileChangeDone && this._isColorSettingsChangeDone) {
			const data = {
				[this._settingsKey]: JSON.stringify(this._currentSettings),
				[this._profileKey]: JSON.stringify(this._currentProfiles),
				[this._sessionColorKey]: JSON.stringify(this._currentColorSettings)
			};

			this._ajax({
				type: "POST",
				url: this._savePath,
				data: JSON.stringify(data),
				contentType: 'application/json'
			}, () => { }, () => { });
		}
	}

	_getStoredProfiles() {
		return new Promise((resolve) => {
			if (this._loadPath) {
				this._ajax({
					type: "GET",
					url: this._loadPath,
					data: { key: this._profileKey },
					contentType: 'application/json'
				}, (result) => {
					resolve(JSON.parse(result.data) ?? []);
				}, () => { })
			} else {
				resolve(JSON.parse(localStorage.getItem(this._profileKey)) ?? []);
			}
		});
	}

	_setStoredProfiles(profileIds) {
		this._currentProfiles = profileIds;
		localStorage.setItem(this._profileKey, JSON.stringify(profileIds));

		this._isProfileChangeDone = true;
		if (this._savePath && this._isSettingsChangeDone && this._isColorSettingsChangeDone) {
			const data = {
				[this._settingsKey]: JSON.stringify(this._currentSettings),
				[this._profileKey]: JSON.stringify(this._currentProfiles),
				[this._sessionColorKey]: JSON.stringify(this._currentColorSettings)
			};

			this._ajax({
				type: "POST",
				url: this._savePath,
				data: JSON.stringify(data),
				contentType: 'application/json'
			}, () => { }, () => { });
		}
	}

	async _reloadData(callback) {
		this._currentProfiles = await this._getStoredProfiles();
		this._currentSettings = await this._getStoredSettings();
		this._currentColorSettings = await this._getStoredColorSettings();
		callback();
	}

	_ajax(options, successCallback, errorCallback, uploadStartCallback, uploadProgressCallback, uploadFinishCallback) {
		var xhr = new XMLHttpRequest();
		xhr.responseType = options.responseType ? options.responseType : 'json';
		xhr.onreadystatechange = () => {
			if (xhr.readyState == XMLHttpRequest.DONE) {   // XMLHttpRequest.DONE == 4
				if (xhr.status == 200) {
					successCallback(xhr.response);
				}
				else if (xhr.status == 400) {
					errorCallback(xhr.response);
				}
				else {
					errorCallback();
				}
			}
		};

		if (!options.type || options.type.toLowerCase() === "get") {
			options.url = this._updateUrlParameter(options.data, options.url);
		}

		xhr.open(options.type ? options.type : "GET", options.url, true);

		if (options.contentType) {
			xhr.setRequestHeader("Content-Type", options.contentType);
		}

		if (options.type && options.type.toLowerCase() === "post") {
			xhr.send(options.data);
		} else {
			xhr.send();
		}

		return xhr;
	}

	_updateUrlParameter(param, url) {
		url = url || window.location.href;
		const parsedUri = new URL(url, window.location.href);
		for (const key in param) {
			if (param[key] instanceof Array) {
				param[key].forEach((element) => {
					parsedUri.searchParams.append(key, element);
				});
			} else if (param[key] != undefined) {
				parsedUri.searchParams.set(key, param[key]);
			}
		}

		return parsedUri.toString();
	}
}
window.customElements.define("accessibility-dialog", AccessibilityDialog);
