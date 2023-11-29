class CoreTranslationService {
    
    static _isInitialized = false;
	static _translations = {};

    static _readTranslations(){
        var elem = document.querySelector(".js-translations");
        if(elem){
            this._translations = JSON.parse(elem.dataset.translations);
        }
    }

	static translate(key) {
        if(!this._isInitialized){
            this._readTranslations();
        }

        return this._translations[key] != undefined ? this._translations[key] : key;
    }

}