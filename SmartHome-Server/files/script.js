var App = function(){
	var statusLabels = new Array("Wyłączone", "Włączone");
	var buttonsLabels = new Array("Włącz", "Wyłącz");
	
	var messages = new Array();
	messages["http"] = "Uruchomiono serwer HTTP";
	messages["L11"] = "Włączono światło w salonie";
	messages["L10"] = "Wyłączono światło w salonie";
	messages["L21"] = "Włączono światło w kuchni";
	messages["L20"] = "Wyłączono światło w kuchni";
	messages["L31"] = "Włączono światło w sypialni";
	messages["L30"] = "Wyłączono światło w sypialni";
	messages["L41"] = "Włączono światło na korytarzu";
	messages["L40"] = "Wyłączono światło na korytarzu";
	messages["L51"] = "Włączono światło w łazience";
	messages["L50"] = "Wyłączono światło w łazience";
	messages["L61"] = "Włączono światło w ogrodzie";
	messages["L60"] = "Wyłączono światło w ogrodzie";
	messages["R1"] = "Włączono telewizor";
	messages["R0"] = "Wyłączono telewizor";
	messages["S11"] = "Otworzono okna";
	messages["S10"] = "Zamknięto okna";
	messages["S21"] = "Otworzono drzwi wejściowe";
	messages["S20"] = "Zamknięto drzwi wejściowe";
	messages["S31"] = "Otwarto bramę wjazdową";
	messages["S30"] = "Zamknięto bramę wjazdową";
	messages["AL1"] = "Włączono alarm";
	messages["AL0"] = "Wyłączono alarm";
	messages["AB1"] = "Uruchomiono alarm";
	messages["APR1"] = "Włączono reakcję na czujnik ruchu";
	messages["APR0"] = "Wyłączono reakcję na czujnik ruchu";
	messages["APO1"] = "Włączono reakcję na czujnik odległości";
	messages["APO0"] = "Wyłączono reakcję na czujnik odległości";
	messages["D"] = "Połączono ze SMART HOME";
	
	function messageContent(code){
		if(code in messages){
			return messages[code];
		} else {
			return code;
		}
	}
	
	function sensorValueUnit(code){
		switch(code){
			case "PT":
			case "PTMIN":
			case "PTMAX":
				return " &deg;C";
			case "PC":
			case "PW":
				return " %";
			case "PO":
				return " cm";
			default:
				return "";
		}
	}

	var Ui = function(contrCodes, sensCodes){
		var Controls = function(codes){
			this.controls = new Array();
			this.update = function(data){
				for(var key in data){
					if(key in this.controls){
						this.controls[key].status.innerHTML = statusLabels[data[key]];
						this.controls[key].button.innerHTML = buttonsLabels[data[key]];
						this.controls[key].button.className = "row_button";
						if(data[key] != 0){
							this.controls[key].button.className += " but_on";
						}
						this.controls[key].value = data[key];
					}
				}
			}
			var ControlUnit = function(code){
				this.status = document.getElementById(code+"_status");
				this.button = document.getElementById(code+"_button");
				this.button.onclick = function(){
					setControl(this.id.split("_")[0]);
				}
			}
			for(var i in codes){
				this.controls[codes[i]] = new ControlUnit(codes[i]);
			}
		}
		var Sensors = function(codes){
			this.sensors = new Array();
			this.update = function(data){
				for(var key in data){
					this.sensors[key].innerHTML = data[key]+sensorValueUnit(key);
				}
			}
			for(var i in codes){
				this.sensors[codes[i]] = document.getElementById(codes[i]);
			}
		}
		var Messages = function(){
			this.messagebox = document.getElementById('com');
			this.update = function(data){
				var content = "";
				for(var i = data.length-1; i >= 0; i--){
					content += '<div class="com_row"><div class="com_label">';
					content += data[i].Date;
					if(data[i].Type == "p"){
						content += '</div><div class="com_status com_bold">';
					} else {
						content += '</div><div class="com_status">';
					}
					content += messageContent(data[i].Content);
					content += "</div></div>";
				}
				this.messagebox.innerHTML = content;
			}
		}
		this.controls = new Controls(contrCodes);
		this.sensors = new Sensors(sensCodes);
		this.messages = new Messages();
		this.update = function(data){
			this.controls.update(data["Controls"]);
			this.sensors.update(data["Sensors"]);
			this.messages.update(data["Messages"]);
		}
	}
	var ui;
	var lastchange = "0";

	function getContent(){
		var xmlhttp = new XMLHttpRequest();
		var url = "http://localhost/api/content/get?lastchange=\""+lastchange+"\"";
		xmlhttp.onreadystatechange = function() {
			if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
				var data = JSON.parse(xmlhttp.responseText);
				ui.update(data);
				lastchange = data["LastChange"];
				getContent();
			}
		};
		xmlhttp.open("GET", url, true);
		xmlhttp.send();
	}
	
	function setControl(code){
		var xmlhttp = new XMLHttpRequest();
		var url = "http://localhost/api/controls/set?"+code+"=";
		if(ui.controls.controls[code].value == 0){
			url += "1";
		} else {
			url += "0";
		}
		xmlhttp.open("GET", url, true);
		xmlhttp.send();
	}
	
	this.init = function(){
		ui = new Ui(
			new Array("L1", "L2", "L3", "L4", "L5", "L6", "R", "S1", "S2", "S3", "AL", "APR", "APO"),
			new Array("PT", "PTMIN", "PTMAX", "PW", "PC", "PO", "PR")
		);
		getContent();
	}
}
var app = new App();
window.onload = app.init;