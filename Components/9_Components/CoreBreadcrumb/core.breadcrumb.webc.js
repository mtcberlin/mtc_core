class CoreBreadcrumb extends HTMLElement {
    
	constructor() {
		super();
		console.log("CoreBreadcrumb dran core select");
        this.data = [];
	}
    render(){
        console.log("CoreBreadcrumb render");
     
        const shadowRoot = this.attachShadow({mode: 'open'});
        shadowRoot.innerHTML = `
            <style>
              .breadcrumb-nav {
                font-size: .85em;
                margin: 0 0 2em;
                padding: .5em;
              }   
              .bc_border{
                background: #eee;
                border: 1px solid #ccc;
              }
              .breadcrumb-nav ol {
                list-style: none;
                margin: 0;
                padding: 0;
              }
              .breadcrumb-nav li {
                display: inline-block;
              }
              .breadcrumb-nav li:not(:last-child)::after {
                border-bottom: .75ch solid transparent;
                border-left: .75ch solid #000;
                border-top: .75ch solid transparent;
                content: "";
                display: inline-block;
                margin-left: .75ch;
                margin-right: .5ch;
              }
              .breadcrumb-nav a {
                display: inline-block;
                font-weight: bold;
              }
              .breadcrumb-nav a:focus,
              .breadcrumb-nav a[href]:hover {
                outline: 2px solid;
                outline-offset: 2px;
                text-decoration: none;
              }
              .breadcrumb-nav [aria-current] {
                color: inherit;
                font-weight: normal;
                text-decoration: none;
              }
              .breadcrumb-nav [aria-current][href]:hover,
              .breadcrumb-nav [aria-current]:focus {
                outline: 0px solid;
              }
            </style> 
            <nav class="breadcrumb-nav" aria-label="Breadcrumb">
                <ol>
                </ol>
            </nav>
        `;
        this.listroot = shadowRoot.querySelector("ol");

        var _that = this;
        this.maxEntries = this.data.length;

        this.data.forEach((obj, index, array) => this.createListEntry(  _that.listroot, _that.maxEntries, obj, index, array) );
    }

    createListEntry( rootelem, max, data, index, array ){
        if(data != null && data.Link != null && data.Name != null){
            let listelem = document.createElement('li');
            listelem.setAttribute('class', '');
            let linkelem = document.createElement('a');
            linkelem.setAttribute('href', data.Link);
            if(index == max-1){
                linkelem.setAttribute('aria-current', 'page');
            }
            linkelem.innerHTML = data.Name;
            listelem.appendChild(linkelem);
            rootelem.appendChild(listelem);    
        }
    }

    connectedCallback(){
        console.log("CoreBreadcrumb connectedCallback");
        if(this.getAttribute("data-links") != null){
            this.data = JSON.parse(this.getAttribute("data-links"));
        }

        this.render();
    }

}
window.customElements.define('core-breadcrumb', CoreBreadcrumb);