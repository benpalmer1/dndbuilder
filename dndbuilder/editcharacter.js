/**
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 22MAY19
 *
 * Purpose:
 * Javascript for the editor page of the DnD character builder website.
 * Controls api access, editing of characters and some validation / calculation logic.
 * 
 * Due to difficulties with vanilla-JS in importing javascript files without some sort of kludge:
 * https://tutorials.technology/tutorials/How-to-include-a-JavaScript-file-in-another-JavaScript-file.html ->
 * In order to reduce complexity, many of the methods here are duplicated in index.js.
 * But, I argue worth it to avoid mistakes and it doesn't load any unnecessary methods.
 */

 // XHR Variables
let loadCharacterRequest = null;
let saveCharacterRequest = null;
let loadClassListRequest = null;
let loadRaceListRequest = null;
let loadRaceRequest = null;
let loadClassRequest = null;

// Globals
let characterId = null;
let initialName = "";
let listInfo = { characterClass:"", characterRace:"",
                 classListLoaded:false, raceListLoaded:false };
let hitpoints = { level:null, hitdie:null, constitution:null, bonus:null };
let racialBonuses = [];

function initEditCharacter()
{
    // Stop form from submitting by default
    document.getElementById("editCharacterForm").onsubmit = function(event) {
        event.preventDefault;
        saveCharacterAsync();
        return false;
    }

    // Get character ID from query params
    var params = new URLSearchParams(location.search);
	if (params.has('id'))
  	{
		if (params.get('id') != "")
		{
            characterId = parseInt(params.get('id'));
            document.getElementById("viewcharacterid").value = characterId;
            loadCharacterByIdAsync(characterId);
		}
	}
	else {
		alert("No character id in the URL.\n\nPress 'OK' to select a character from the homepage.");
		backToHome();
    }
}

function loadCharacterByIdAsync(id)
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
			document.getElementById("viewcharacterid").value = charResponse["Id"];
			document.getElementById("characterName").value = charResponse["Name"];
			document.getElementById("characterAge").value = charResponse["Age"];
			document.getElementById("characterGender").value = charResponse["Gender"];
			document.getElementById("characterBiography").value = charResponse["Biography"];
            document.getElementById("characterLevel").value = charResponse["Level"];
            document.getElementById("spellcaster").value = charResponse["CharacterClass"]["Spellcaster"];

            // Name validation, ignore initial name
            initialName = charResponse["Name"];
            
            // Set HTML select to correct option
            listInfo.characterRace = charResponse["Race"]["Name"];
			listInfo.characterClass = charResponse["CharacterClass"]["Name"];
            
            // Hitpoints calculation setup
            hitpoints.hitdie = parseInt(charResponse["CharacterClass"]["HitDie"]);
            hitpoints.level = parseInt(charResponse["Level"]);
            hitpoints.constitution = parseInt(charResponse["AbilityScores"][0]);
            hitpoints.bonus = parseInt(charResponse["Race"]["RacialBonuses"][0]);

            // Ability scores values & validation
            racialBonuses = charResponse["Race"]["RacialBonuses"];
            setRacialBonusesAndTotals(charResponse["AbilityScores"]);
            abilityScoreValidationSetup();
                
            // Initial setup methods
            hitPointsCalculator();
            loadRaceListAsync();
            loadClassListAsync();
        }
        else {
			alert("Unable to find character: " + JSON.parse(loadCharacterRequest.responseText) + "\nClick 'OK' to go back to home.");
			backToHome();
        }
        loadCharacterRequest = null;
    }
}

function abilityScoreValidationSetup()
{
    let f1 = document.getElementById("constitution");
    let f2 = document.getElementById("dexterity");
    let f3 = document.getElementById("strength");
    let f4 = document.getElementById("charisma");
    let f5 = document.getElementById("intelligence");
    let f6 = document.getElementById("wisdom");

    let fields = [f1,f2,f3,f4,f5,f6];
    validateAbilityScoresCount(fields);

    fields.forEach(function(item,index,array) {
        item.addEventListener("input", function (event) {
            validateAbilityScoresCount(fields);
        }, false);
    });
}
// Each "field" i.e f[n] will be an "input" html tag.
// Called initially and on input of any ability scores field.
function validateAbilityScoresCount(fields)
{
    let sum = 0;
    fields.forEach(function(item,index,array) {
        sum += parseInt(item.value);
    });

    // If sum == 20, valid
    if(sum == 20)
    {
        fields.forEach(function(item,index,array) {
            item.setCustomValidity("");
        });
        hitpoints.constitution = parseInt(fields[0].value);
        hitpoints.bonus = racialBonuses[0];
        hitPointsCalculator();
        setRacialBonusesAndTotals();
    }
    else
    {
        fields.forEach(function(item,index,array) {
            item.setCustomValidity("Ability scores must sum to 20.");
        });
        hitpoints.constitution = null;
        hitpoints.bonus = null;
        hitPointsCalculator();
        setRacialBonusesAndTotals();
    }
}

function backToHome()
{
    location.href = "/dndbuilder/index.html";
}

// Show the correct class and race of the character on screen
function classOrRaceListLoaded()
{
    if (listInfo.classListLoaded == true && listInfo.raceListLoaded == true)
    {
        let classSelector = document.getElementById("charClassSelect");
        for (let i = 0; i < classSelector.options.length; ++i) {
            if (classSelector.options[i].text == listInfo.characterClass)
            {
                classSelector.options[i].selected = true;
            }
        }

        let raceSelector = document.getElementById("charRaceSelect");
        for (let i = 0; i < raceSelector.options.length; ++i) {
            if (raceSelector.options[i].text == listInfo.characterRace)
            {
                raceSelector.options[i].selected = true;
            }
        }
    }
}

function setRacialBonusesAndTotals(actualValues)
{
    let actualFieldIds = ["constitution", "dexterity", "strength", "charisma", "intelligence", "wisdom"];
    let bonusFieldIds = ["constitutionbonus", "dexteritybonus", "strengthbonus", "charismabonus", "intelligencebonus", "wisdombonus"];
	let totalFieldIds = ["constitutiontotal", "dexteritytotal", "strengthtotal", "charismatotal", "intelligencetotal", "wisdomtotal"];

    for (let i = 0; i < racialBonuses.length; i++)
    {
        document.getElementById(bonusFieldIds[i]).innerText = racialBonuses[i];

        let actualValue = 0;
        if (actualValues != null)
        {
            actualValue = actualValues[i];
            document.getElementById(actualFieldIds[i]).value  = actualValue;
        }
        else {
            actualValue = document.getElementById(actualFieldIds[i]).value;
            if (isNaN(actualValue) || actualValue == "")
            {
                actualValue = 0;
            }
        }

        document.getElementById(totalFieldIds[i]).innerText = parseInt(actualValue) + parseInt(racialBonuses[i]);
    }
}

function hitPointsCalculator()
{
    let level = parseInt(document.getElementById("characterLevel").value);

    if (level >= 0 && level <= 20)
    {
        hitpoints.level = level;
    }
    else {
        hitpoints.level = null;
    }

    if (hitpoints.hitdie != null &&
        hitpoints.level != null &&
        hitpoints.constitution != null &&
        hitpoints.bonus != null)
    {
        let hpValue = (parseInt(hitpoints.level) * parseInt(hitpoints.hitdie)) +
                       parseInt(hitpoints.constitution) + parseInt(hitpoints.bonus);
        document.getElementById("hitpoints").value = hpValue;
    }
    else {
        document.getElementById("hitpoints").value = "";
    }
}

function createJsonCharacter(id, name, age, gender, biography, level, racename, raceBonuses,
                             classname, classhitdie, classspellcaster,
                             constitution, dexterity, strength, charisma, intelligence, wisdom)
{
    var newCharacter =
    {
        Id: id,
        Name: name,
        Age: age,
        Gender: gender,
        Biography: biography,
        Level: level,
        Race: { Name: racename, RacialBonuses:raceBonuses },
        CharacterClass: { Name: classname, HitDie:classhitdie, Spellcaster: classspellcaster },
        AbilityScores: [
            constitution,
            dexterity,
            strength,
            charisma,
            intelligence,
            wisdom ]
    };

    return newCharacter;
}

// Load all races
function loadRaceListAsync()
{
    loadRaceListRequest = new XMLHttpRequest();
    loadRaceListRequest.open("GET", "/api/races/simple", true);
    loadRaceListRequest.onreadystatechange = loadRaceListCallback;
    loadRaceListRequest.send();
}
function loadRaceListCallback()
{
    if (loadRaceListRequest.readyState == 4)
    { 
        if (loadRaceListRequest.status == 200)
        {
            let jsonRaces = Object.keys(JSON.parse(loadRaceListRequest.responseText));
            let raceSelect = document.getElementById("charRaceSelect");
            for (let i = 0; i < jsonRaces.length; i++)
            {
                let option = document.createElement("option");
                option.innerText = jsonRaces[i];
                raceSelect.appendChild(option);
            }
            document.getElementById("charRaceSelect").value = "";
            listInfo.raceListLoaded = true;
            classOrRaceListLoaded();
        } else {
            alert("Unable to load character races. Please try reloading. " + JSON.parse(loadRaceListRequest.responseText));
        }
        loadRaceListRequest = null;
    }
}

// Load all classes
function loadClassListAsync()
{
    loadClassListRequest = new XMLHttpRequest();
    loadClassListRequest.open("GET", "/api/classes/simple", true);
    loadClassListRequest.onreadystatechange = loadClassListCallback;
    loadClassListRequest.send();
}
function loadClassListCallback()
{
    if (loadClassListRequest.readyState == 4) { 
        if (loadClassListRequest.status == 200) {
            let jsonClasses = Object.keys(JSON.parse(loadClassListRequest.responseText));
            let classSelect = document.getElementById("charClassSelect");
            for (let i = 0; i < jsonClasses.length; i++) {
                let option = document.createElement("option");
                option.innerText = jsonClasses[i];
                classSelect.appendChild(option)
            }

            document.getElementById("charClassSelect").value = "";
            listInfo.classListLoaded = true;
            classOrRaceListLoaded();
        }
        else {
            alert("Unable to load character classes. " + JSON.parse(loadClassListRequest.responseText));
        }
        loadClassListRequest = null;
    }
}

function raceSelectedAsync()
{
    let id = parseInt(document.getElementById("charRaceSelect").selectedIndex);

    loadRaceRequest = new XMLHttpRequest();
    loadRaceRequest.open("GET", "/api/races/"+id, true);
    loadRaceRequest.onreadystatechange = raceSelectedCallback;
    loadRaceRequest.send();
}
function raceSelectedCallback()
{
    if (loadRaceRequest.readyState == 4)
    { 
        if (loadRaceRequest.status == 200)
        {
            let jsonRace = JSON.parse(loadRaceRequest.responseText);

            racialBonuses = jsonRace["RacialBonuses"];
            hitpoints.bonus = parseInt(racialBonuses[0]);
            setRacialBonusesAndTotals();
            hitPointsCalculator();
        } else {
            alert("Unable to load character race. Please try again. " + JSON.parse(loadRaceRequest.responseText));
        }
        loadRaceRequest = null;
    }
}


function classSelectedAsync()
{
    let id = parseInt(document.getElementById("charClassSelect").selectedIndex);

    loadClassRequest = new XMLHttpRequest();
    loadClassRequest.open("GET", "/api/classes/"+id, true);
    loadClassRequest.onreadystatechange = classSelectedCallback;
    loadClassRequest.send();
}
function classSelectedCallback()
{
    if (loadClassRequest.readyState == 4)
    { 
        if (loadClassRequest.status == 200)
        {
            let jsonClass = JSON.parse(loadClassRequest.responseText);
            // determine if spellcaster or not.
            document.getElementById("spellcaster").value = jsonClass["Spellcaster"];
            // set hitpoints object for class hit-die.
            hitpoints.hitdie = parseInt(jsonClass["HitDie"]);
            hitPointsCalculator();
        } else {
            hitpoints.hitdie = null;
            hitPointsCalculator();
            alert("Unable to load character class. Please try again. "+ JSON.parse(loadClassRequest.responseText));
        }
        loadClassRequest = null;
    }
}

function saveCharacterAsync()
{
    saveCharacterRequest = new XMLHttpRequest();
    saveCharacterRequest.open("POST", "/api/characters/edit", true);
    saveCharacterRequest.setRequestHeader("Content-type", "application/json");
    saveCharacterRequest.onreadystatechange = saveCharacterCallback;

    let message = createJsonCharacter(
        id = characterId,
        name = document.getElementById("characterName").value,
        age = document.getElementById("characterAge").value,
        gender = document.getElementById("characterGender").value,
        biography = document.getElementById("characterBiography").value,
        level = document.getElementById("characterLevel").value,
        racename = document.getElementById("charRaceSelect").value,
        raceBonuses = racialBonuses,
        classname = document.getElementById("charClassSelect").value,
        classhitdie = hitpoints.hitdie,
        classspellcaster = document.getElementById("spellcaster").value,
        constitution = document.getElementById("constitution").value,
        dexterity = document.getElementById("dexterity").value,
        strength = document.getElementById("strength").value,
        charisma = document.getElementById("charisma").value,
        intelligence = document.getElementById("intelligence").value,
        wisdom = document.getElementById("wisdom").value,
    );
    saveCharacterRequest.send(JSON.stringify(message));
}
function saveCharacterCallback()
{
    if (saveCharacterRequest.readyState == 4)
    { 
        if (saveCharacterRequest.status == 200)
        {
            let charResponse = JSON.parse(saveCharacterRequest.responseText);
            if (charResponse == true)
            {
                alert("Character saved successfully.");
            }
            else {
                alert("Unable to save character: \n" + JSON.parse(saveCharacterRequest.responseText));
            }
        }
        else {
            alert("Unable to save character: \n" + JSON.parse(saveCharacterRequest.responseText));
        }
        saveCharacterRequest = null;
    }
}

function verifyNameUniqueAsync()
{
    if (document.getElementById("characterName").value != "" &&
        document.getElementById("characterName").value != initialName)
    {
        document.getElementById("characterName").setCustomValidity("");
        verifyNameUniqueRequest = new XMLHttpRequest();

        verifyNameUniqueRequest.open("POST", "/api/characters/exists", true);
        verifyNameUniqueRequest.setRequestHeader("Content-type", "application/json");
        verifyNameUniqueRequest.onreadystatechange = verifyNameUniqueCallback;

        verifyNameUniqueRequest.send(JSON.stringify(document.getElementById("characterName").value));
    }
}
function verifyNameUniqueCallback()
{
    if (verifyNameUniqueRequest.readyState == 4)
    { 
        if (verifyNameUniqueRequest.status == 200)
        {
            let isInUseResponse = JSON.parse(verifyNameUniqueRequest.responseText);

            if (isInUseResponse == true
                && document.getElementById("characterName").value != initialName)
            {
                document.getElementById("characterName").setCustomValidity("Name is in use.");
                document.getElementById("characterName").reportValidity();
            } else {
                document.getElementById("characterName").setCustomValidity("");
            }
        }
        else {
            alert("Unable to search for name. \n" + JSON.parse(verifyNameUniqueRequest.responseText));
        }
        verifyNameUniqueRequest = null;
    }
}