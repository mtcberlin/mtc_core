class CoreReadOut extends HTMLElement {

	speakInstanz = undefined;
	textToRead = [];
	_speakRate = 1;
	_voice = null;
	_rootClass = "js-component";
	_isPaused = false;

	constructor() {
		super();
	}

	connectedCallback() {
		setTimeout(() => {
			this._connectedCallbackStart();
		}, 100);
	}

	_connectedCallbackStart() {
		this._render();
		this._attachEventHandlers();

		this.hide(".js-play");
		this.hide(".js-pause");
		if (this.hasAttribute("data-root") && this.getAttribute("data-root") != "") {
			this._rootClass = this.getAttribute("data-root");
		}
	}

	getTextToRead() {
		const component = this.closest("." + this._rootClass);
		if (component) {
			this.textToRead = [];
			var nodes = component.querySelectorAll(".js-readout-content");
			this._prepareReadOuts(nodes);
		}
		if (!component || this.textToRead.length == 0) {
			this.hide(".js-start");
		}

	}

	_prepareReadOuts(nodes) {
		if (nodes.length > 0) {
			nodes.forEach(ele => {
				if (ele.offsetParent === null) {
					return;
				}
				if (ele.nodeName === "#text") {
					var span = document.createElement('span');
					span.innerText = this._cleanTextNode(ele.nodeValue);
					if (span.innerText !== "" && span.innerText !== " ") {
						ele.parentNode.insertBefore(span, ele);
						ele.parentNode.removeChild(ele);
						this.textToRead.push(span);
					}
				} else {
					this._prepareReadOuts(ele.childNodes);
				}
			});
		}
	}

	_cleanTextNode(textNode) {
		return textNode.replaceAll("\n", "").replaceAll("\t", "").replace(/\s+/g, ' ');
	}

	_render() {
		this.insertAdjacentHTML("afterbegin", `
		<div class="speak">
			<button type="button" class="btn px-2 js-start outline-pink-focus" aria-label="Speak Text"><svg xmlns="http://www.w3.org/2000/svg" width="26" height="26" viewBox="0 0 21.555 33.794"><path data-name="Pfad 101" d="M8.344,0-1.55,7.8H-13.211V26H-1.55l9.894,7.8Z" transform="translate(13.211)"></path></svg></button>
			<button type="button" class="btn px-1 js-play d-none outline-pink-focus" aria-label="Play speak text"><svg xmlns="http://www.w3.org/2000/svg" width="30" height="30" fill="currentColor" class="bi bi-play-btn" viewBox="0 0 16 16"><path d="M6.79 5.093A.5.5 0 0 0 6 5.5v5a.5.5 0 0 0 .79.407l3.5-2.5a.5.5 0 0 0 0-.814l-3.5-2.5z"></path><path d="M0 4a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2v8a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2V4zm15 0a1 1 0 0 0-1-1H2a1 1 0 0 0-1 1v8a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1V4z"></path></svg></button>
			<button type="button" class="btn px-1 js-pause d-none outline-pink-focus" aria-label="Pause speak text"><svg xmlns="http://www.w3.org/2000/svg" width="30" height="30" fill="currentColor" class="bi bi-pause-btn" viewBox="0 0 16 16"><path d="M6.25 5C5.56 5 5 5.56 5 6.25v3.5a1.25 1.25 0 1 0 2.5 0v-3.5C7.5 5.56 6.94 5 6.25 5zm3.5 0c-.69 0-1.25.56-1.25 1.25v3.5a1.25 1.25 0 1 0 2.5 0v-3.5C11 5.56 10.44 5 9.75 5z"></path><path d="M0 4a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2v8a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2V4zm15 0a1 1 0 0 0-1-1H2a1 1 0 0 0-1 1v8a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1V4z"></path></svg></button>
		</div>
	  `);
	}

	_attachEventHandlers() {
		const startButton = this.querySelector(".js-start");
		startButton.addEventListener('click', e => {
			e.preventDefault();
			var synth = window.speechSynthesis;
			if (synth.speaking) {
				synth.cancel();
				synth.dispatchEvent(new Event("cancel"));
			}
			this.getTextToRead();
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

	_getSpeakRate() {
		var speed = "";

		document.documentElement.classList.forEach(cls => {
			if (cls.indexOf("audio-rate-") >= 1) {
				speed = cls;
			};
		});
		if (speed != "") {
			switch (speed) {
				case 'access-audio-rate-70':
					this._speakRate = 0.7;
					break;
				case 'access-audio-rate-80':
					this._speakRate = 0.8;
					break;
				case 'access-audio-rate-90':
					this._speakRate = 0.9;
					break;
				case 'access-audio-rate-110':
					this._speakRate = 1.1;
					break;
				case 'access-audio-rate-120':
					this._speakRate = 1.2;
					break;
				default:
					this._speakRate = 1;
			}
		} else {
			this._speakRate = 1;
		}
	}

	_speak() {
		if (this.textToRead.length) {
			this._isPaused = false;
			this._startSpeechInstanz(0);
			this.hide(".js-play");
			this.show(".js-pause");
		}
	}

	_startSpeechInstanz(currentElement) {
		this._getSpeakRate();
		var synth = window.speechSynthesis;

		synth.cancel();
		synth.dispatchEvent(new Event("cancel"));

		this.speakInstanz = new SpeechSynthesisUtterance(this.textToRead[currentElement].innerText);
		this.speakInstanz.rate = this._speakRate;
		if (this._voice != null) {
			//this.speakInstanz.voice = speechSynthesis.getVoices()[75];
			this.speakInstanz.voice = this._voice;
		}

		this.speakInstanz.addEventListener("boundary", (e) => {
			this._markCurrentSpeak(e.charIndex, e.charLength, currentElement);
		});

		this.speakInstanz.addEventListener("end", () => {
			this._markCurrentSpeak(0, 0, currentElement);
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

		if (!this._isPaused) {
			this._isPaused = false;
			synth.speak(this.speakInstanz);
		}
	}

	_pause() {
		if (this.speakInstanz) {
			this._isPaused = true;
			this.show(".js-play");
			this.hide(".js-pause");

			var synth = window.speechSynthesis;
			synth.pause(this.speakInstanz);
		}
	}

	_play() {
		if (this.speakInstanz) {
			this._isPaused = false;
			this.hide(".js-play");
			this.show(".js-pause");

			var synth = window.speechSynthesis;
			if (synth.speaking) {
				synth.resume(this.speakInstanz);
			} else {
				synth.speak(this.speakInstanz);
			}
		}
	}

	_markCurrentSpeak(startIndex, length, currentElement) {
		let openingTag = '<span style="background-color:#ffaaaa">';
		let closingTag = '</span>';
		if (startIndex === 0 && length === 0) {
			openingTag = "";
			closingTag = "";
		}

		const clone = this.textToRead[currentElement].cloneNode(true);
		startIndex = clone.innerText.indexOf(" ") == 0 ? startIndex + 1 : startIndex;

		const newHTML = clone.innerText.slice(0, startIndex)
			+ openingTag + clone.innerText.slice(startIndex, startIndex + length) + closingTag
			+ clone.innerText.slice(startIndex + length);
		this.textToRead[currentElement].innerHTML = newHTML;
	}

	hide(selector) {
		if (selector !== undefined) {
			this.querySelectorAll(selector).forEach(ele => ele.classList.toggle("d-none", true));
		} else {
			this.classList.toggle("d-none", true);
		}
	}

	show(selector) {
		if (selector !== undefined) {
			this.querySelectorAll(selector).forEach(ele => ele.classList.toggle("d-none", false));
		} else {
			this.classList.toggle("d-none", false);
		}
	}

}

window.customElements.define('core-readout', CoreReadOut);