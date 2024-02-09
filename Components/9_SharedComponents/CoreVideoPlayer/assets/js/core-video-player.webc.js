/// <reference path="../../../../0_Base/corehtmlelement.webc.js" />

class CoreVideo {
	#videoBarMainHtml =
		`<!-- video bar  -->
			<div class="d-flex py-2 px-3 bg-core-blue align-items-center justify-content-between">
				<div class="d-flex align-items-center">
					<button class="btn me-3 p-0 js-play">
						<img src="/assets/icons/play-icon.svg"
							 alt="play"/>
					</button>
					<button class="btn me-3 p-0 js-pause d-none">
              			<img src="/assets/icons/pause-icon.svg" alt="pause">
					</button>
					<button class="btn me-3 p-0 js-seek">
						<img src="/assets/icons/skip-end-icon.svg"
							 alt="skip 10 seconds"/>
					</button>
					<div class="position-relative">
						<button class="btn me-3 p-0 js-playback-rate-settings-button">
							<img src="/assets/icons/speed-icon.svg"
								 alt="playback speed"/>
						</button>
						<div class="hidden position-absolute z-3 js-playback-rate-settings"
							 style="left: -25px; bottom: 175%;">
							<div class="d-flex flex-column align-items-center z-3 bg-core-blue rounded p-2"
								 style="width: 80px;">
								<button class="btn btn-core-blue playback-rate" data-playback-rate="0.5">0.5x</button>
								<button class="btn btn-core-blue playback-rate" data-playback-rate="0.75">0.75x</button>
								<button class="btn btn-core-blue playback-rate" data-playback-rate="1">Normal</button>
								<button class="btn btn-core-blue playback-rate" data-playback-rate="1.5">1.5x</button>
								<button class="btn btn-core-blue playback-rate" data-playback-rate="2">2x</button>
							</div>
						</div>
					</div>
					<p class="text-white">
						<div class="video-main-currentTime">
							<time class="time-elapsed">00:00</time>
							<span> / </span>
							<time class="time-duration">00:00</time>
						</div>
					</p>
				</div>
				<div class="d-flex align-items-center">
					<div class="position-relative">
						<button class="btn me-3 p-0 js-volume-settings-button">
							<img src="/assets/icons/volume-up-icon.svg"
								 alt="volume"/>
						</button>
						<div class="hidden position-absolute js-volume-settings"
							 style="left: -20px; bottom: 175%;">
							<div class="d-flex align-items-center justify-content-center bg-core-blue rounded p-2"
								 style="width: 70px; height: 150px;">
								<input class="volume-rate"
									   type="range"
									   name="volume"
									   id="volume-slider"
									   max="1"
									   min="0"
									   step="0.1"
									   style="transform: rotate(270deg);"/>
							</div>
						</div>
					</div>
					<div class="position-relative">
						<button class="btn me-3 p-0 js-accessibility-settings-button">
							<img src="/assets/icons/bootstrap-a11y-icon-white.svg"
								 alt="accessibility settings"/>
						</button>
						<div class="hidden position-absolute z-3 js-accessibility-settings"
							 style="left: -20px; bottom: 175%;">
							<div class="d-flex flex-column align-items-center z-3 bg-core-blue rounded p-2"
								 style="width: 70px;">
								<button class="btn btn-core-blue js-dgs-button">DGS</button>
								<button class="btn btn-core-blue js-ut-button">UT</button>
								<!--<button class="btn btn-core-blue">AD</button>
								<button class="btn btn-core-blue">LS</button>-->
							</div>
						</div>
					</div>
					<button class="btn p-0 js-fullscreen-main-button" >
						<img src="/assets/icons/fullscreen-icon.svg"
							 alt="full screen"/>
					</button>
				</div>
			</div>`;

	#videoBarDgsHtml =
		`<div class="d-flex py-2 px-3 bg-core-blue align-items-center justify-content-between">
			<div class="d-flex align-items-center">
				<button class="btn me-3 p-0 js-play">
					<img src="/assets/icons/play-icon.svg" alt="play"/>
				</button>
				<button class="btn me-3 p-0 js-pause d-none">
					<img src="/assets/icons/pause-icon.svg" alt="pause">
				</button>
				<button class="btn me-3 p-0 js-seek">
					<img src="/assets/icons/skip-end-icon.svg" alt="skip to end"/>
				</button>
				<!--<p class="text-white"><div class="video-dgs-currentTime">00:00 - <div class="video-dgs-duration">00:00</div></p>-->
			</div>
			<div class="d-flex">
				<div class="position-relative">
					<button class="btn me-3 p-0 js-dgs-settings-button">
						<img src="/assets/icons/settings-icon.svg" alt="settings"/>
					</button>
					<div class="hidden position-absolute js-dgs-settings" style="left: -120px; bottom: 150%;">
						<div class="d-flex align-items-end justify-content-center bg-core-blue rounded p-2"
							 style="width: 200px;">
							<div class="d-flex align-self-center w-25" style="transform: rotate(270deg);">
								<input class="volume-rate" type="range" name="volume" id="volume-slider" max="1" min="0" step="0.1"
									   style="transform: translateX(-60px);"/>
							</div>
							<div class="d-flex flex-column">
								<button class="btn btn-core-blue playback-rate" data-playback-rate="0.5">0.5x</button>
								<button class="btn btn-core-blue playback-rate" data-playback-rate="0.75">0.75x</button>
								<button class="btn btn-core-blue playback-rate" data-playback-rate="1">Normal</button>
								<button class="btn btn-core-blue playback-rate" data-playback-rate="1.5">1.5x</button>
								<button class="btn btn-core-blue playback-rate" data-playback-rate="2">2x</button>
							</div>
							<!--<div class="d-flex flex-column">
								<button class="btn btn-core-blue">DGS</button>
								<button class="btn btn-core-blue">UT</button>
								<button class="btn btn-core-blue">AD</button>
								<button class="btn btn-core-blue">LS</button>
		
							</div>-->
						</div>
		
					</div>
				</div>
				<div class="d-flex align-items-center">
					<button class="btn p-0 js-toggle-fullscreen-dgs">
						<img src="/assets/icons/fullscreen-icon.svg" alt="full screen"/>
					</button>
				</div>
			</div>
		</div>`;
	
	constructor(videoConfigDatasource, videoContainer, isMainVideo) {
		//console.log('coreVideo::constructor()');
		this.videoConfigDatasource = videoConfigDatasource;
		this.#resetVideoContainer(videoContainer);
		this.isMainVideo = isMainVideo;
	}
	
	// Instance fields
	isMainVideo;
	videoConfigDatasource;
	video;
	videoContainer;
	seekbarElement;
	timeElapsedElement;
	durationElement;
	dgsSettingsElement;
	playbackRateElement;
	volumeSettingsElement;
	accessibilitySettingsElement;
	
	buttonPlay;
	buttonPause;
	buttonSeek;
	buttonPlaybackRateSettings;
	buttonVolumeSettings;
	buttonAccessibilitySettings;
	buttonFullscreen;//TODO: js-fullscreen-dgs-button
	buttonDGS;
	buttonUt;
	dgsSettingsButton;
	playPromise;

	renderVideo(
		muted = true, 
		autoplay = false,
		hidden = false) {
		//console.log('coreVideo::renderVideo()');
		if( this.videoConfigDatasource.success ){
			this.#createVideoNode(muted, autoplay, hidden);
			if ( this.isMainVideo ){
				this.#appendProgressBar();
			}
			this.#appendVideoControlsBar();
			this.#appendVideoCopyrightAndSource();	
		}
		else {
			console.error("video infos success false!");
		}
		//this.#addEventListener();
	}
	#resetVideoContainer(videoContainer){
		//console.log('coreVideo::resetVideoContainer()');
		this.videoContainer = videoContainer;
		this.videoContainer.innerHTML = '';
		this.videoContainer.classList.add('d-none');
	}
	#createVideoNode(muted, autoplay, hidden){
		//console.log('coreVideo::createVideoNode()');
		let sources = this.isMainVideo? this.videoConfigDatasource.data.sources : this.videoConfigDatasource.data.dgsSources
		let sourceTag = "";
		for(let i = 0; i < sources.length; i++) {
			sourceTag += `<source src="${sources[i].src}" type="${sources[i].type}">`
		}
		let track = `
			<track
				label="Deutsch"
				kind="subtitles"
				srclang="de"
				src="${this.videoConfigDatasource.data.captionSrc}"
				default />
		`;
		
		this.video = document.createElement("video");
		this.video.style.width = "100%";
		this.video.controls = false;
		this.video.autoplay = autoplay;
		this.video.muted = muted;
		this.video.innerHTML = `${sourceTag}${track}
		
		Your browser does not support video.`;
		this.videoContainer.append(this.video);
		if(!hidden){
			this.videoContainer.classList.remove('d-none');
		}
		for (let i = 0; i < this.video.textTracks.length; i++) {
			this.video.textTracks[i].mode = "hidden";
		}
		//this.video.onloadedmetadata = this.#loadMetadata;
	}
	#appendProgressBar(){
		//console.log('coreVideo::appendProgressBar()');
		let progressBar = `
				<!-- progress bar  -->
				<div class="bg-blue laya-seekbar">
				<span></span>
				</div>
		`;
		this.videoContainer.insertAdjacentHTML('beforeend',progressBar);
		this.seekbarElement = this.videoContainer.getElementsByClassName('laya-seekbar')[0];
		this.buttonSeek =this.videoContainer.getElementsByClassName('js-seek')[0];
	}
	#appendVideoControlsBar(){
		//console.log('coreVideo::appendVideoControlsBar()');
		if (this.isMainVideo){
			this.videoContainer.insertAdjacentHTML('beforeend',this.#videoBarMainHtml);
			//main video controls =>
			this.buttonPlaybackRateSettings =this.videoContainer.getElementsByClassName('js-playback-rate-settings-button')[0];
			this.playbackRateElement =this.videoContainer.getElementsByClassName('js-playback-rate-settings')[0];
			this.buttonVolumeSettings =this.videoContainer.getElementsByClassName('js-volume-settings-button')[0];
			this.volumeSettingsElement =this.videoContainer.getElementsByClassName('js-volume-settings')[0];
			this.buttonAccessibilitySettings =this.videoContainer.getElementsByClassName('js-accessibility-settings-button')[0];
			this.accessibilitySettingsElement =this.videoContainer.getElementsByClassName('js-accessibility-settings')[0];
			this.durationElement = this.videoContainer.getElementsByClassName('time-duration')[0];
			this.timeElapsedElement = this.videoContainer.getElementsByClassName('time-elapsed')[0];
		}
		else{
			this.videoContainer.insertAdjacentHTML('beforeend',this.#videoBarDgsHtml);
			//DGS controls =>
			this.dgsSettingsButton = this.videoContainer.getElementsByClassName('js-dgs-settings-button')[0];
			this.dgsSettingsElement = this.videoContainer.getElementsByClassName('js-dgs-settings')[0];
		}
		//common controls =>
		this.buttonPlay = this.videoContainer.getElementsByClassName('js-play')[0];
		this.buttonPause =this.videoContainer.getElementsByClassName('js-pause')[0];
		this.buttonSeek =this.videoContainer.getElementsByClassName('js-seek')[0];
		
		this.buttonDgs =this.videoContainer.getElementsByClassName('js-dgs-button')[0];
		this.buttonUt =this.videoContainer.getElementsByClassName('js-ut-button')[0];
	}
	#appendVideoCopyrightAndSource(elementVideo, copyright, source){
		//console.log('coreVideo::appendProgressBar()');
		let sourceHtml = '<div class="fs-xs px-2 bg-light pt-1 pb-1">';
		if( copyright ){
			sourceHtml += `<div class="js-readout-content">© ${copyright}</div>`;
		}
		if ( source ){
			sourceHtml += `<div class="js-readout-content bg-light fs-xs"><span class="fw-bold">Source:</span> ${source}</div>`;
		}
		sourceHtml += '<div>';

		this.videoContainer.insertAdjacentHTML('beforeend',sourceHtml);
	}

	/*############
	PUBLIC EVENTS
	############*/
	onTogglePlayPause(event){
		//console.log('coreVideo::onTogglePlayPause()');
		if (this.video.paused || this.video.ended) {
			this.video.play();
		} else
		{
			this.video.pause();
		}
	}
	onTimeUpdate(event){
		//console.log('coreVideo::onTimeUpdate()');
		if (this.seekbarElement){
			if( this.timeElapsedElement ){
				this.timeElapsedElement.innerHTML=this.#getPrettyTimeFormat(this.video.currentTime);
			}
			let percentage = (this.video.currentTime /this.video.duration )*100;
			this.seekbarElement.firstElementChild.setAttribute("style", "width: "+ percentage +"%")	
		}
	}
	onEnded(event){
		//console.log('coreVideo::onEnded()');
		this.onTogglePlayPause(event);
		//in case main and dgs its duration arent equal:
		this.video.pause();
		this.video.pause();
		this.video.currentTime=0;
		this.video.currentTime=0;
	}

	onDgsSettings(event){
		//console.log('coreVideo::onDgsSettings()');
		this.dgsSettingsElement.classList.toggle('hidden');
	}
	onPlay(event){
		//console.log('coreVideo::onPlay()');
		this.buttonPlay.classList.add('d-none');
		this.buttonPause.classList.remove('d-none');
	}	
	onPause(event){
		//console.log('coreVideo::onPause()');
		this.buttonPlay.classList.remove('d-none');
		this.buttonPause.classList.add('d-none');
	}

	onToggleDgs(event){
		//console.log('coreVideo::onToggleDgs()');
	}

	onToggleUt(event){
		//console.log('coreVideo::onToggleUt()');
		for (let i = 0; i < this.video.textTracks.length; i++) {
			this.video.textTracks[i].mode = this.video.textTracks[i].mode === 'hidden' ? 'showing' : 'hidden'; 
		}
	}
	
	closeExcept(element, exceptElement){
		if ( !exceptElement || element!==exceptElement){
			element.classList.add('hidden');
		}
	}
	closeAllSettings(exceptToggleElement){
		if ( this.isMainVideo){
			this.closeExcept(this.playbackRateElement, exceptToggleElement);
			this.closeExcept(this.volumeSettingsElement, exceptToggleElement);
			this.closeExcept(this.accessibilitySettingsElement, exceptToggleElement);
			
		}
		else{
			this.closeExcept(this.dgsSettingsElement, exceptToggleElement);
			//this.dgsSettingsElement.classList.add('hidden');
		}
	}
	#loadMetadata(){
		console.log('coreVideo::loadMetadata()');
		this.durationElement.innerText = this.#getPrettyTimeFormat(this.video.duration);
	}
	
	
	get video(){
		return this.video;
	}

	/*############
	PRIVATE HELPERS
	############*/
	#getPrettyTimeFormat(seconds) {
		// Hours, minutes and seconds
		const hrs = Math.floor(seconds / 3600);
		const mins = Math.floor((seconds % 3600) / 60);
		const secs = Math.floor(seconds % 60);

		let ret = "";
		if (hrs>0){
			ret += this.#pad(hrs) + ":";
		}
		ret += this.#pad(mins) + ":" + this.#pad(secs);

		return ret;
	}
	
	#pad(num) {
		return ("0"+num).slice(-2)
	}
}
class CoreVideoPlayer extends CoreHTMLElement {
	_coreVideoMain = undefined;
	_coreVideoDgs = undefined;
	_coreVideoPlayer = undefined;
	
	_isRendered = false;	

	static get observedAttributes() {
		return ['data-video-id'];
	}

		connectedCallback() {
		setTimeout(() => {
			this._initialRender();
			this._loadVideoInformations();
		});
	}

	disconnectedCallback() {}

	_addEventListener() {
		this.querySelectorAll(".js-play").forEach((ele)=> ele.addEventListener("click", this._onTogglePlayPause.bind(this)));
		this.querySelectorAll(".js-pause").forEach((ele)=> ele.addEventListener("click", this._onTogglePlayPause.bind(this)));
		this.querySelectorAll(".js-seek").forEach((ele)=> ele.addEventListener("click", this._onSeekedVideo.bind(this)));
		this.querySelectorAll(".js-fullscreen-main-button").forEach((ele)=> ele.addEventListener("click", this._onToggleFullscreen.bind(this)));
		this.querySelectorAll(".js-fullscreen-dgs-button").forEach((ele)=> ele.addEventListener("click", this._onToggleFullscreen.bind(this)));
		this.querySelectorAll(".playback-rate").forEach((ele)=> ele.addEventListener("click", this._onPlaybackRateChanged.bind(this)));
		this.querySelectorAll(".volume-rate").forEach((ele)=> ele.addEventListener("click", this._onVolumeChanged.bind(this)));
		this.querySelectorAll(".laya-seekbar").forEach((ele)=> ele.addEventListener("click", this._onSeekbarClicked.bind(this)));

		document.addEventListener("keyup", this._onKeyup.bind(this));
		
		this._coreVideoMain.video.addEventListener('play',this._onPlay.bind(this));
		this._coreVideoMain.video.addEventListener('pause',this._onPause.bind(this));

		this._coreVideoMain.video.addEventListener('click', this._onTogglePlayPause.bind(this));
		this._coreVideoDgs.video.addEventListener('click', this._onTogglePlayPause.bind(this));
		this._coreVideoMain.video.addEventListener('ended',this._onEnded.bind(this));
		
		this._coreVideoMain.video.ontimeupdate = this._onTimeUpdate.bind(this);
		this._coreVideoMain.buttonPlaybackRateSettings.addEventListener('click', this._onTogglePlaybackRate.bind(this));
		this._coreVideoMain.buttonVolumeSettings.addEventListener('click', this._onToggleVolume.bind(this));
		this._coreVideoMain.buttonAccessibilitySettings.addEventListener('click', this._onToggleAccessibility.bind(this));
		this._coreVideoMain.buttonDgs.addEventListener('click', this._onToggleDgs.bind(this));
		this._coreVideoMain.buttonUt.addEventListener('click', this._onToggleUt.bind(this));
		
		this._coreVideoDgs.dgsSettingsButton.addEventListener('click', this._onDgsSettings.bind(this));
		
		
	}

	_loadVideoInformations() {
		if(this.dataset.videoId != ""){
			this.ajax({
				type: "GET",
				url: "/api/corevideoplayer/getvideoconfig",
				data: { "videoId": this.dataset.videoId },
				contentType: 'application/json'
			}, this._render.bind(this), this._videoInfoError.bind(this));	
		}
	}

	attributeChangedCallback(name, oldValue, newValue) {
		if (name === 'data-video-id') {
			this._id = newValue;
			this._loadVideoInformations();
		}
	}

	_videoInfoError() {
		console.log("video infos error");
	}

	_initialRender(){
		this.innerHTML = `
		<!-- main video -->
		<div class="col d-flex flex-column me-lg-5">
			<section class="mb-3" aria-label="Video Komponente">
				<div class="js-main-video d-flex flex-column"></div>			
			</section>
		</div>
		<!-- /main video -->
		<!-- DGS video -->
		<div class=" col-lg-4" aria-hidden="true">
			<section>
				<div class="w-100 js-dgs-video d-flex flex-column">
			</section>
		</div>
		<!-- /DGS video -->`;
	}

	_render(result) {
		debugger;
		if (result.success) {
			let videoMainContainer = this.querySelector(".js-main-video");
			let videoDgsContainer = this.querySelector(".js-dgs-video");
			
			this._coreVideoMain = new CoreVideo(result, videoMainContainer, true);
			this._coreVideoDgs = new CoreVideo(result, videoDgsContainer, false);

			this._coreVideoMain.renderVideo(false, false);
			this._coreVideoDgs.renderVideo(true, false, true);

			this._addEventListener();
		}
		else{
			console.error("video infos success false!");
		}
	}
	_pad(num) {
		return ("0"+num).slice(-2)
	}
	
	_getOffset(element)
	{
		if (!element.getClientRects().length)
		{
			return { top: 0, left: 0 };
		}

		let rect = element.getBoundingClientRect();
		let win = element.ownerDocument.defaultView;
		return (
			{
				top: rect.top + win.scrollY,
				left: rect.left + win.scrollX
			});
	}
	/** EVENTS */
	
	_onKeyup(event){
		//console.log(`_onKeyup - code: ${event.code}` );
		switch (event.code) {
			case 'Escape':
				this._coreVideoMain.closeAllSettings();
				this._coreVideoDgs.closeAllSettings();
				break;
			case 'Space':
				this.onPlay(event);
				break;
			//default:
		}
	}
	_onTogglePlayPause(event) {
		this._coreVideoMain?.onTogglePlayPause(event);
		this._coreVideoDgs?.onTogglePlayPause(event);
	}
	
	_onPlay(event){
		this._coreVideoMain?.onPlay(event);
		this._coreVideoDgs?.onPlay(event);
		this._coreVideoMain.closeAllSettings();
		this._coreVideoDgs.closeAllSettings();
	}
	
	_onPause(event){
		this._coreVideoMain?.onPause(event);
		this._coreVideoDgs?.onPause(event);
		this._coreVideoMain.closeAllSettings();
		this._coreVideoDgs.closeAllSettings();
	}
	
	_onSeekedVideo() {
		const newTime = this._coreVideoMain.video.currentTime + 5;
		this._coreVideoMain.video.currentTime = newTime;
		this._coreVideoDgs.video.currentTime = newTime;
		this._coreVideoMain.closeAllSettings();
		this._coreVideoDgs.closeAllSettings();
	}

	_onPlaybackRateChanged(event){
		this._coreVideoMain.video.playbackRate = Number(event.target.dataset.playbackRate);
		this._coreVideoDgs.video.playbackRate = Number(event.target.dataset.playbackRate);
		this._coreVideoMain.closeAllSettings();
		this._coreVideoDgs.closeAllSettings();
	}

	_onTogglePlaybackRate(event){
		this._coreVideoMain.playbackRateElement.classList.toggle('hidden');
		this._coreVideoMain.closeAllSettings(this._coreVideoMain.playbackRateElement);
		this._coreVideoDgs.closeAllSettings();
	}

	_onVolumeChanged(event){
		this._coreVideoMain.video.volume = event.target.value;
		this._coreVideoMain.volumeSettingsElement.classList.add('hidden');
		this._coreVideoDgs.dgsSettingsElement.classList.add('hidden');
		this._coreVideoMain.closeAllSettings();
		this._coreVideoDgs.closeAllSettings();
	}

	_onToggleVolume(event){
		this._coreVideoMain.closeAllSettings(this._coreVideoMain.volumeSettingsElement);
		this._coreVideoDgs.closeAllSettings();		
		this._coreVideoMain.volumeSettingsElement.classList.toggle('hidden');
	}

	_onToggleAccessibility(){
		this._coreVideoMain.closeAllSettings(this._coreVideoMain.accessibilitySettingsElement);
		this._coreVideoDgs.closeAllSettings();
		this._coreVideoMain.accessibilitySettingsElement.classList.toggle('hidden');
	}

	_onToggleFullscreen(event){
		this._coreVideoMain.closeAllSettings();
		this._coreVideoDgs.closeAllSettings();
		
		this._coreVideoPlayer=event.target.closest("core-video-player");

		if (!document.fullscreenElement) {
			this._coreVideoPlayer.requestFullscreen();
		} else if (document.exitFullscreen) {
			document.exitFullscreen();
		}
	}
	_onEnded(event){
		this._coreVideoMain.onEnded(event);
		this._coreVideoDgs.onEnded(event);
	}

	_onTimeUpdate( event ){
		this._coreVideoMain.onTimeUpdate(event);
		this._coreVideoDgs.onTimeUpdate(event);
	}

	_onSeekbarClicked(event) {
		const videoDuration = Math.round(this._coreVideoMain.video.duration);
		const skipTo = Math.round((event.offsetX / this._coreVideoMain.videoContainer.clientWidth) * videoDuration);
		this._coreVideoMain.video.currentTime = skipTo;
		this._coreVideoDgs.video.currentTime = skipTo;
		this._coreVideoMain.closeAllSettings();
		this._coreVideoDgs.closeAllSettings();
	}
	
	_onToggleDgs(event){
		this._coreVideoMain.onToggleDgs(event);
		this._coreVideoDgs.videoContainer.classList.toggle('d-none');
		this._coreVideoMain.closeAllSettings();
		this._coreVideoDgs.closeAllSettings();
	}

	_onToggleUt(event){
		this._coreVideoMain.onToggleUt(event);
		this._coreVideoMain.closeAllSettings();
		this._coreVideoDgs.closeAllSettings();
		
	}

	_onDgsSettings(event){
		this._coreVideoMain.closeAllSettings(this._coreVideoDgs.dgsSettingsButton);
		this._coreVideoDgs.closeAllSettings();
		this._coreVideoDgs.onDgsSettings(event);
	}
}
window.customElements.define('core-video-player', CoreVideoPlayer);