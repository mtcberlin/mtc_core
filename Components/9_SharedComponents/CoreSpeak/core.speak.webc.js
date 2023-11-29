/// <reference path="../../0_Base/corehtmlelement.webc.js" />
class CoreSpeak extends CoreHTMLElement {

	speakInstanz = undefined;
	textToRead = [];

	constructor() {
		super();
	}

	connectedCallback() {
		setTimeout(() => {
			this._connectedCallbackStart();
		});

	}

	_connectedCallbackStart() {
		this._render();
		this._attachEventHandlers();
		const component = this.closest(".js-component");
		if (component) {
			component.querySelectorAll(".js-speak-content").forEach(ele => {
				this.textToRead.push(ele.cloneNode(true));
			});
		}

		if(!component || this.textToRead.length == 0) {
			this.hide(".js-start");
		}
	}

	_render() {
		this.hide(".js-play");
		this.hide(".js-pause");
	}

	_attachEventHandlers() {
		const startButton = this.querySelector(".js-start");
		startButton.addEventListener('click', e => {
			e.preventDefault();
			this._speak();
		});

		const pauseButton = this.querySelector(".js-pause");
		pauseButton.addEventListener('click', e => {
			e.preventDefault();
			this._pause();
		});

		const playButton = this.querySelector(".js-play");
		playButton.addEventListener('click', e => {
			e.preventDefault();
			this._play();
		});
	}

	_speak() {
		if (this.textToRead.length) {
			this._startSpeechInstanz(0);

			this.hide(".js-play");
			this.show(".js-pause");
		}
	}

	_startSpeechInstanz(currentElement) {
		var synth = window.speechSynthesis;
		synth.cancel();
		synth.dispatchEvent(new Event("cancel"));

		this.speakInstanz = new SpeechSynthesisUtterance(this.textToRead[currentElement].innerText);

		this.speakInstanz.addEventListener("boundary", (e) => {
			this._markCurrentSpeak(e.charIndex, e.charLength, currentElement);
		});

		this.speakInstanz.addEventListener("end", () => {
			this._getSpeakElementByIndex(currentElement).innerHTML = this.textToRead[currentElement].innerHTML;
			if (currentElement + 1 < this.textToRead.length) {
				this._startSpeechInstanz(currentElement + 1);
			} else {
				this.hide(".js-play");
				this.hide(".js-pause");
			}
		});

		synth.addEventListener("cancel", () => {
			this._markCurrentSpeak(0, 0, currentElement);
		}, { once: true });

		synth.speak(this.speakInstanz);
	}

	_pause() {
		if (this.speakInstanz) {
			this.show(".js-play");
			this.hide(".js-pause");

			var synth = window.speechSynthesis;
			synth.pause(this.speakInstanz);
		}
	}

	_play() {
		if (this.speakInstanz) {
			this.hide(".js-play");
			this.show(".js-pause");

			var synth = window.speechSynthesis;
			synth.resume(this.speakInstanz);
		}
	}

	_markCurrentSpeak(startIndex, length, currentElement) {
		const openingTag = '<span style="background-color:#ffaaaa">';
		const closingTag = '</span>';
		
		const newHTML = this.textToRead[currentElement].innerText.slice(0, startIndex)
			+ openingTag + this.textToRead[currentElement].innerText.slice(startIndex, startIndex + length) + closingTag
			+ this.textToRead[currentElement].innerText.slice(startIndex + length);
		this._getSpeakElementByIndex(currentElement).innerHTML = newHTML;
	}

	_getSpeakElementByIndex(index) {
		return this.closest(".js-component").querySelectorAll(".js-speak-content")[index];
	}

}

window.customElements.define('core-speak', CoreSpeak);