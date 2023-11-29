class CoreSelect extends HTMLElement {
    
	constructor() {
		super();
		console.log("CoreSelect dran core select");
        this._title = "no title";
	}
    render(){
        //this.append(document.createElement("div"));
        //this.innerHTML = "schnuffel";
        console.log("CoreSelect render");
        //this.append(document.createElement("div"));
        this.innerHTML = "CoreSelect " + this._title;
    }

    connectedCallback(){
        console.log("CoreSelect connectedCallback");
        if(this.getAttribute("data-title") != null){
            this._title = this.getAttribute("data-title");
        }
        this.render();
    }

}
window.customElements.define('core-select', CoreSelect);