/**
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 22MAY19
 *
 * Purpose:
 * Javascript for the view character page on the DnD character builder website.
**/

// XHR Values
let loadCharacterRequest = "";

// Globals
let characterId = null;
let jsonChar = null;

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
			jsonChar = charResponse;
			characterId = charResponse["Id"];

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

function deleteCharacterAsync()
{
	let confirmDelete = confirm("Are you sure you wish to delete this character?");
	if (confirmDelete == true)
	{
		deleteCharacterRequest = new XMLHttpRequest();
		deleteCharacterRequest.open("GET", "/api/characters/delete/"+characterId, true);
		deleteCharacterRequest.onreadystatechange = deleteCharacterCallback;

		deleteCharacterRequest.send();
	}
}
function deleteCharacterCallback()
{
	if (deleteCharacterRequest.readyState == 4)
    { 
        if (deleteCharacterRequest.status == 200)
        {
			let response = JSON.parse(deleteCharacterRequest.responseText);

			if (response == true)
			{
				alert("Character successfully deleted.");
				backToHome();
			}
			else {
				alert("Unable to delete character: " + JSON.parse(deleteCharacterRequest.responseText));
			}
        }
        else {
			alert("Unable to delete character: " + JSON.parse(deleteCharacterRequest.responseText));
        }
        deleteCharacterRequest = null;
    }
}

// Browser support as per https://stackoverflow.com/a/33542499
function downloadCharacter()
{
	if (jsonChar != null)
	{
		let serializer = new XMLSerializer();
		let characterAsXmlString = serializer.serializeToString(createXMLCharacter());

		let blob = new Blob([characterAsXmlString], {type: 'text/xml'});
		let filename = "DnDCharacter.xml"

		if (window.navigator.msSaveOrOpenBlob) {
			window.navigator.msSaveBlob(blob, filename);
		}
		else {
			var elem = window.document.createElement('a');
			elem.href = window.URL.createObjectURL(blob);
			elem.download = filename;        
			document.body.appendChild(elem);
			elem.click();        
			document.body.removeChild(elem);
		}
	}
}

function createXMLCharacter()
{
	let xmldoc = document.implementation.createDocument(null, null, null);
	xmldoc.appendChild(xmldoc.createProcessingInstruction('xml', 'version="1.0"'));

	let xmlcharacter = xmldoc.createElement("DnDCharacter");
	let xmlid = xmldoc.createElement("Id");
	xmlid.appendChild(xmldoc.createTextNode(jsonChar["Id"]));
	let xmlname = xmldoc.createElement("Name");
	xmlname.appendChild(xmldoc.createTextNode(jsonChar["Name"]));
	let xmlage = xmldoc.createElement("Age");
	xmlage.appendChild(xmldoc.createTextNode(jsonChar["Age"]));
	let xmlgender = xmldoc.createElement("Gender");
	xmlgender.appendChild(xmldoc.createTextNode(jsonChar["Gender"]));
	let xmlbiography = xmldoc.createElement("Biography");
	xmlbiography.appendChild(xmldoc.createTextNode(jsonChar["Biography"]));
	let xmllevel = xmldoc.createElement("Level");
	xmllevel.appendChild(xmldoc.createTextNode(jsonChar["Level"]));
	let xmlhitpoints = xmldoc.createElement("Hitpoints");
	xmlhitpoints.appendChild(xmldoc.createTextNode(jsonChar["HitPoints"]));

	let xmlrace = xmldoc.createElement("Race");
	let xmlracename = xmldoc.createElement("Name");
	xmlracename.appendChild(xmldoc.createTextNode(jsonChar["Race"]["Name"]));
	let xmlraceracialbonuses = xmldoc.createElement("RacialBonuses");
	xmlraceracialbonuses.appendChild(xmldoc.createTextNode(jsonChar["Race"]["RacialBonuses"]));
	xmlrace.appendChild(xmlracename);
	xmlrace.appendChild(xmlraceracialbonuses);
	
	let xmlcharacterclass = xmldoc.createElement("CharacterClass");
	let xmlcharacterclassname = xmldoc.createElement("Name");
	xmlcharacterclassname.appendChild(xmldoc.createTextNode(jsonChar["CharacterClass"]["Name"]));
	let xmlcharacterclasshitdie = xmldoc.createElement("HitDie");
	xmlcharacterclasshitdie.appendChild(xmldoc.createTextNode(jsonChar["CharacterClass"]["HitDie"]));
	let xmlcharacterclassspellcaster = xmldoc.createElement("Spellcaster");
	xmlcharacterclassspellcaster.appendChild(xmldoc.createTextNode(jsonChar["CharacterClass"]["Spellcaster"]));
	xmlcharacterclass.appendChild(xmlcharacterclassname);
	xmlcharacterclass.appendChild(xmlcharacterclasshitdie);
	xmlcharacterclass.appendChild(xmlcharacterclassspellcaster);
	
	let xmlabilityscores = xmldoc.createElement("AbilityScores");
	xmlabilityscores.appendChild(xmldoc.createTextNode(jsonChar["AbilityScores"]));

	// Build character
	xmlcharacter.appendChild(xmlid);
	xmlcharacter.appendChild(xmlname);
	xmlcharacter.appendChild(xmlage);
	xmlcharacter.appendChild(xmlgender);
	xmlcharacter.appendChild(xmlbiography);
	xmlcharacter.appendChild(xmllevel);
	xmlcharacter.appendChild(xmlhitpoints);
	xmlcharacter.appendChild(xmlrace);
	xmlcharacter.appendChild(xmlcharacterclass);
	xmlcharacter.appendChild(xmlabilityscores);

	let xmldec = xmldoc.createDocumentFragment()

	// Add character to doc
	xmldoc.appendChild(xmlcharacter);
	return xmldoc;
}