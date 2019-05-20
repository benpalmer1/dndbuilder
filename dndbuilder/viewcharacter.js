/**
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 20MAY19
 *
 * Purpose:
 * Javascript for the view character page on the DnD character builder website.
**/

// XHR Values
let loadCharacterRequest = "";

// Process query params
function init()
{
	var params = new URLSearchParams(location.search);

	if (params.has('id'))
  	{
		if (params.get('id') != "")
		{
			loadCharacterById(params.get('id'));
		}
	}
	else {
		alert("No character id in the URL.\n\nPress 'OK' to select a character from the homepage.");
		backToHome();
	}
}

function loadCharacterById(id)
{
	loadCharacterRequest = new XMLHttpRequest();
	loadCharacterRequest.open("GET", "/api/characters/"+id, true);
	loadCharacterRequest.onreadystatechange = loadCharacterCallback;
	loadCharacterRequest.send();	
}

function loadCharacterCallback()
{
	if (loadCharacterRequest.readyState == 4)
    { 
        if (loadCharacterRequest.status == 200)
        {
						let charResponse = JSON.parse(loadCharacterRequest.responseText);
						document.getElementById("editcharacterid").value = charResponse["Id"];
						document.getElementById("charname").innerText = charResponse["Name"];
						document.getElementById("charage").innerText = charResponse["Age"];
						document.getElementById("chargender").innerText = charResponse["Gender"];
						document.getElementById("charbiography").innerText = charResponse["Biography"];
						document.getElementById("charlevel").innerText = charResponse["Level"];
						document.getElementById("charhitpoints").innerText = charResponse["HitPoints"];
						document.getElementById("racename").innerText = charResponse["Race"]["Name"];
						setRacialBonusesAndTotals(charResponse["AbilityScores"], charResponse["Race"]["RacialBonuses"]);
						document.getElementById("classname").innerText = charResponse["CharacterClass"]["Name"];
						document.getElementById("classhitdie").innerText = charResponse["CharacterClass"]["HitDie"];
						document.getElementById("classspellcaster").innerText = charResponse["CharacterClass"]["Spellcaster"];
        }
        else {
						alert("Unable to find character: " + JSON.parse(loadCharacterRequest.responseText) + "\nClick 'OK' to go back to home.");
						backToHome();
        }
        loadCharacterRequest = null;
    }
}

function setRacialBonusesAndTotals(actualvalues, racialBonuses)
{
    let actualFieldIds = ["constitution", "dexterity", "strength", "charisma", "intelligence", "wisdom"];
    let bonusFieldIds = ["constitutionbonus", "dexteritybonus", "strengthbonus", "charismabonus", "intelligencebonus", "wisdombonus"];
		let totalFieldIds = ["constitutiontotal", "dexteritytotal", "strengthtotal", "charismatotal", "intelligencetotal", "wisdomtotal"];

    for (let i = 0; i < racialBonuses.length; i++)
    {
				document.getElementById(bonusFieldIds[i]).innerText = racialBonuses[i];
				
        let actualValue = actualvalues[i];
        if (isNaN(actualValue) || actualValue == "")
        {
					actualValue = 0;
        }
				document.getElementById(actualFieldIds[i]).innerText = actualValue;

        document.getElementById(totalFieldIds[i]).innerText = parseInt(actualValue) + parseInt(racialBonuses[i]);
    }
}

function backToHome()
{
	location.href = "/dndbuilder/index.html";
}