/**
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 20MAY19
 *
 * Purpose:
 * Javascript for the index page of the DnD character builder website.
 * Controls api access, creation of characters and some validation / calculation logic.
 **/

// XHR Request Variables
let loadAllCharsRequest = "";
let loadClassListRequest = "";
let loadRaceListRequest = "";
let loadClassRequest = "";
let loadRaceRequest = "";
let createCharacterRequest = "";
let verifyNameUniqueRequest = "";

let hitpoints = { level:null, hitdie:null, constitution:null };
let racialBonuses = [];

function init()
{
    // Stop form from submitting by default
    document.getElementById("newCharacterForm").onsubmit = function(event) {
        event.preventDefault;
        createCharacterAsync();
        return false;
    }
    
    // Validation setup
    characterFormValidationSetup();
    // Setup automatic calculation of HP
    hitPointsCalculator();
    // Get races
    loadRaceListAsync();
    // Get classes
    loadClassListAsync();
    // Load character list
    loadAllCharactersAsync();
}

function verifyNameUnique()
{
    if (document.getElementById("characterName") != "")
    {
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
            if (isInUseResponse == true) {
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

function setRacialBonusesAndTotals()
{
    let actualFieldIds = ["constitution", "dexterity", "strength", "charisma", "intelligence", "wisdom"];
    let bonusFieldIds = ["constitutionbonus", "dexteritybonus", "strengthbonus", "charismabonus", "intelligencebonus", "wisdombonus"];
	let totalFieldIds = ["constitutiontotal", "dexteritytotal", "strengthtotal", "charismatotal", "intelligencetotal", "wisdomtotal"];

    for (let i = 0; i < racialBonuses.length; i++)
    {
        document.getElementById(bonusFieldIds[i]).innerText = racialBonuses[i];

        let actualValue = document.getElementById(actualFieldIds[i]).value;
        if (isNaN(actualValue) || actualValue == "")
        {
            actualValue = 0;
        }

        document.getElementById(totalFieldIds[i]).innerText = parseInt(actualValue) + parseInt(racialBonuses[i]);
    }
}

function characterFormValidationSetup()
{
    let f1 = document.getElementById("constitution");
    let f2 = document.getElementById("dexterity");
    let f3 = document.getElementById("strength");
    let f4 = document.getElementById("charisma");
    let f5 = document.getElementById("intelligence");
    let f6 = document.getElementById("wisdom");

    let fields = [f1,f2,f3,f4,f5,f6];
    validateCount(fields);

    fields.forEach(function(item,index,array) {
        item.addEventListener("input", function (event) {
            validateCount(fields);
        }, false);
    });
}

// Each "field" i.e f[n] will be an "input" html tag
function validateCount(fields)
{
    let sum = 0;
    fields.forEach(function(item,index,array) {
        sum += parseInt(item.value);
    });
    // If sum == 20 -> valid
    if(sum == 20) {
        fields.forEach(function(item,index,array) {
            item.setCustomValidity("");
        });

        hitpoints.constitution = parseInt(fields[0].value);
        hitPointsCalculator();
        setRacialBonusesAndTotals();
    }
    else {
        fields.forEach(function(item,index,array) {
            item.setCustomValidity("Ability scores must sum to 20.");
        });

        hitpoints.constitution = null;
        hitPointsCalculator();
        setRacialBonusesAndTotals();
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
        hitpoints.constitution != null)
    {
        let hpValue = (parseInt(hitpoints.level) * parseInt(hitpoints.hitdie)) + parseInt(hitpoints.constitution);
        document.getElementById("hitpoints").value = hpValue;
    }
    else {
        document.getElementById("hitpoints").value = "";
    }
}

function raceSelected()
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
            setRacialBonusesAndTotals();
        } else {
            alert("Unable to load character race. Please try again. " + JSON.parse(loadRaceRequest.responseText));
        }
        loadRaceRequest = null;
    }
}

function classSelected()
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
        }
        else {
            alert("Unable to load character classes. " + JSON.parse(loadClassListRequest.responseText));
        }
        loadClassListRequest = null;
    }
}

function createCharacterAsync()
{
    createCharacterRequest = new XMLHttpRequest();
    createCharacterRequest.open("POST", "/api/characters/add", true);
    createCharacterRequest.setRequestHeader("Content-type", "application/json");
    createCharacterRequest.onreadystatechange = createCharacterCallback;

    let message = createJsonCharacter(
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
    createCharacterRequest.send(JSON.stringify(message));
}
function createCharacterCallback()
{
    if (createCharacterRequest.readyState == 4)
    { 
        if (createCharacterRequest.status == 200)
        {
            let charResponse = JSON.parse(createCharacterRequest.responseText);
            if (charResponse == true)
            {
                alert("Character added successfully.");

                // Clear input, reset bonuses/total info && reload character list
                clearCharacterInput();
                setRacialBonusesAndTotals();
                loadAllCharactersAsync();
            }
            else {
                alert("Unable to save character: \n" + JSON.parse(createCharacterRequest.responseText));
            }
        }
        else {
            alert("Unable to save character: \n" + JSON.parse(createCharacterRequest.responseText));
        }
        createCharacterRequest = null;
    }
}

function createJsonCharacter(name, age, gender, biography, level, racename, raceBonuses,
                             classname, classhitdie, classspellcaster,
                             constitution, dexterity, strength, charisma, intelligence, wisdom)
{
    var newCharacter =
    {
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

function clearCharacterInput()
{
    document.getElementById("characterName").value = "";
    document.getElementById("characterAge").value = "";
    document.getElementById("characterGender").value = "";
    document.getElementById("characterBiography").value = "";
    document.getElementById("characterLevel").value = "";
    document.getElementById("charRaceSelect").value = "";
    document.getElementById("charClassSelect").value = "";
    document.getElementById("spellcaster").value = "";
    document.getElementById("hitpoints").value = "";
    document.getElementById("constitution").value = "";
    document.getElementById("dexterity").value = "";
    document.getElementById("strength").value = "";
    document.getElementById("charisma").value = "";
    document.getElementById("intelligence").value = "";
    document.getElementById("wisdom").value = "";
}

// Load all characters in database
function loadAllCharactersAsync()
{
    loadAllCharsRequest = new XMLHttpRequest();
    loadAllCharsRequest.open("GET", "/api/characters/simple", true);
    loadAllCharsRequest.onreadystatechange = loadAllCharactersCallback;
    loadAllCharsRequest.send();
}
function loadAllCharactersCallback()
{
    if (loadAllCharsRequest.readyState == 4)
    { 
        if (loadAllCharsRequest.status == 200)
        {
            let jsonCharacters = JSON.parse(loadAllCharsRequest.responseText);
            document.getElementById("charactersFromDb").innerHTML = "";
            for (let i = 0; i < jsonCharacters.length; i++)
            {
                let character = jsonCharacters[i];
                addCharacterToList(character["Id"], character["Name"], character["Race"], character["CharacterClass"], character["Level"]);
            }
        }
        else {
            alert("Unable to load characters. " + JSON.parse(loadAllCharsRequest.responseText));
        }
        loadAllCharsRequest = null;
    }
}

function addCharacterToList(id, name, race, charClass, level)
{
    let newCharacterNode = document.createElement("div");
    newCharacterNode.setAttribute("class", "characterRow");

    let nameNode = document.createElement("span");
    let raceNode = document.createElement("span");
    let classNode = document.createElement("span");
    let levelNode = document.createElement("span");
    let viewNode  = document.createElement("span");

    // name
    nameNode.innerText = name;
    nameNode.setAttribute("class", "col col1");

    // race
    raceNode.innerText = race;
    raceNode.setAttribute("class", "col col2");

    // class
    classNode.innerText = charClass;
    classNode.setAttribute("class", "col col3");

    // level
    levelNode.innerText = level;
    levelNode.setAttribute("class", "col col4");

    // view button
    let viewForm = document.createElement("form");
    viewForm.setAttribute("action", "character.html");
    viewForm.setAttribute("method", "GET");
    let viewButton = document.createElement("button");
    viewButton.innerText = "View";
    viewButton.setAttribute("type", "submit");
    let viewHidId = document.createElement("input");
    viewHidId.setAttribute("type", "hidden");
    viewHidId.setAttribute("value", id);
    viewHidId.setAttribute("name", "id");
    viewForm.appendChild(viewButton);
    viewForm.appendChild(viewHidId);
    viewNode.appendChild(viewForm);
    viewNode.setAttribute("class", "col col5");

    newCharacterNode.appendChild(nameNode);
    newCharacterNode.appendChild(raceNode);
    newCharacterNode.appendChild(classNode);
    newCharacterNode.appendChild(levelNode);
    newCharacterNode.appendChild(viewNode);

    document.getElementById("charactersFromDb").appendChild(newCharacterNode);
}