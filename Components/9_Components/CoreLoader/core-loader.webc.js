class CoreLoader extends CoreHTMLElement {

	loadersStarted = 0;

	constructor() {
		super();
		CoreStateStore.subscribe(CoreStateStore.START_LOADER, this.onStartLoader.bind(this));
		CoreStateStore.subscribe(CoreStateStore.STOP_LOADER, this.onEndLoader.bind(this));
	}

	onStartLoader() {
		if(this.loadersStarted == 0) {
			this.style.display = "block";
		}
		this.loadersStarted++;
	}

	onEndLoader() {
		this.loadersStarted--;
		if(this.loadersStarted <= 0) {
			this.style.display = "none";
			this.loadersStarted = 0;
		}
	}

}
window.customElements.define('core-loader', CoreLoader);