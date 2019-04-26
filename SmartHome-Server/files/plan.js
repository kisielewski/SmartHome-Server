var PlanApp = function(){
	var controls = new Array();
	var colorOff = "#840303";
	var colorLOff = "#008422";
	var colorOn = "#ff4d00";
	var colorLOn = "#2af702";

	var lastchange = "0";
	var data;

	var Controls = function(){
		this["L1"] = new Object();
		this["L1"].el = new Array(document.getElementById("L1"), document.getElementById("L1label"));
		this["L2"] = new Object();
		this["L2"].el = new Array(document.getElementById("L2"), document.getElementById("L2label"));
		this["L3"] = new Object();
		this["L3"].el = new Array(document.getElementById("L3"), document.getElementById("L3label"));
		this["L4"] = new Object();
		this["L4"].el = new Array(document.getElementById("L4"), document.getElementById("L4label"));
		this["L5"] = new Object();
		this["L5"].el = new Array(document.getElementById("L5"), document.getElementById("L5label"));
		this["L6"] = new Object();
		this["L6"].el = new Array(
			document.getElementById("L61"),
			document.getElementById("L62"),
			document.getElementById("L63"),
			document.getElementById("L64"),
			document.getElementById("L6label")
		);
		this["R"] = new Object();
		this["R"].el = new Array(document.getElementById("R"), document.getElementById("Rlabel"));
		this["S1"] = new Object();
		this["S1"].el = new Array(
			document.getElementById("S11"),
			document.getElementById("S12"),
			document.getElementById("S13"),
			document.getElementById("S14"),
			document.getElementById("S1label")
		);
		this["S2"] = new Object();
		this["S2"].el = new Array(document.getElementById("S2"), document.getElementById("S2label"));
		this["S3"] = new Object();
		this["S3"].el = new Array(document.getElementById("S3"), document.getElementById("S3label"));
		for(var key in this){
			for(var i = 0; i < this[key].el.length; ++i){
				this[key].el[i].onclick = function(){
					var code = this.id[0];
					if(this.id[0] != 'R' && this.id.length > 1){
						code += this.id[1];
					}
					setControl(code);
				}
			}
		}
	}

	function getControls(){
		var xmlhttp = new XMLHttpRequest();
		var url = "/api/content/get?lastchange=\""+lastchange+"\"";
		xmlhttp.onreadystatechange = function() {
			if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
				var data = JSON.parse(xmlhttp.responseText);
				lastchange = data["LastChange"];
				update(data["Controls"]);
				getControls();
			}
		};
		xmlhttp.open("GET", url, true);
		xmlhttp.send();
	}
	
	function setControl(code){
		var xmlhttp = new XMLHttpRequest();
		var url = "/api/controls/set?"+code+"=";
		if(controls[code].value == 0){
			url += "1";
		} else {
			url += "0";
		}
		xmlhttp.open("GET", url, true);
		xmlhttp.send();
	}

	function update(data){
		for(var key in data){
			if(key in controls){
				var color;
				if(key[0] == 'L'){
					if(data[key] == 0){
						color = colorLOff;
					} else {
						color = colorLOn;
					}
				} else {
					if(data[key] == 0){
						color = colorOff;
					} else {
						color = colorOn;
					}
				}
				for(var i = 0; i < controls[key].el.length; ++i){
					controls[key].el[i].style.fill = color;
					controls[key].value = data[key];
				}
			}
		}
	}
	
	this.init = function(){
		document.getElementById("fullScreen").onclick = function(){
			this.style.display = "none";
			var i = document.body;
			if (i.requestFullscreen) {
				i.requestFullscreen();
			} else if (i.webkitRequestFullscreen) {
				i.webkitRequestFullscreen();
			} else if (i.mozRequestFullScreen) {
				i.mozRequestFullScreen();
			} else if (i.msRequestFullscreen) {
				i.msRequestFullscreen();
			}
			screen.orientation.lock("portrait-primary");
		}
		controls = new Controls();
		getControls();
	}
}
var planApp = new PlanApp();
window.onload = planApp.init;