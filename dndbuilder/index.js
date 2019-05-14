/**
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 15MAY19
 *
 * Purpose:
 * Javascript for the index page of the DnD character builder website.
 **/

// ------ GLOBAL VARS

// XHR Request Variables
let stubGETreq = null;
let stubPOSTreq = null;

// ------ END GLOBAL VARS ------

function init() {
    addCharacterTest();
    addCharacterTest();
    addCharacterTest();
}

// ------ GET REQUEST  ------
function stubGETAsync() {
    stubGETreq = new XMLHttpRequest();

    stubGETreq.open("GET", "getexamplestub", true);
    stubGETreq.onreadystatechange = stubGETcallback;
    stubGETreq.send();
}

function stubGETcallback() {
    if (stubGETreq.readyState == 4) {
        if (stubGETreq.status == 200) {
            let retVal = stubGETreq.responseText;
            // do something with the retVal
        }
        else {
            alert("Unable to do something: " + stubGETreq.responseText);
        }
        stubGETreq = null;
    }
}


// ------ POST REQUEST ------
function loadImageAsync(xPos, yPos, zoomVal) {
    stubPOSTreq = new XMLHttpRequest();

    stubPOSTreq.open("POST", "postExampleStub", true);
    stubPOSTreq.onreadystatechange = stubPOSTcallback;
    stubPOSTreq.setRequestHeader("Content-type", "application/json");

    let message = {"val1":1, "val2":2, "val3":3};
    stubPOSTreq.send(JSON.stringify(message));
}

function stubPOSTcallback() {
    if (stubPOSTreq.readyState == 4) {
        if (stubPOSTreq.status == 200) {
            let retVal = JSON.parse(stubPOSTreq.responseText);

            // do something with the returned data.
        }
        else {
            alert("Unable to do something. Response:\n" + stubPOSTreq.responseText);
        }
    }
}

// Load all characters in database
function loadAllCharactersAsync()
{
    loadAllCharsRequest = new XMLHttpRequest();

    loadAllCharsRequest.open("GET", "/characters/", true);
    loadAllCharsRequest.onreadystatechange = loadAllCharactersCallback;
    loadAllCharsRequest.send();
}
function loadAllCharactersCallback()
{
    if (loadAllCharsRequest.readyState == 4)
    {
        if (loadAllCharsRequest.status == 200)
        {
            let retVal = JSON.parse(loadAllCharsRequest.responseText);
            for (let character in retVal)
            {
                if (retVal.hasOwnProperty(character))
                {
                    addCharacterToList(character["id"], character["name"], character["race"], character["class"], character["level"]);
                }
            }
        }
        else {
            alert("Unable to load characters. Server response:\n" + loadAllCharsRequest.responseText);
        }
        loadAllCharsRequest = null;
    }
}

function addCharacterTest()
{
    addCharacterToList(1,"Erufu","Night Elf","Druid",1);
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
    let viewHidName = document.createElement("input");
    viewHidName.setAttribute("type", "hidden");
    viewHidName.setAttribute("value", name);
    viewHidName.setAttribute("name", "name");
    viewForm.appendChild(viewButton);
    viewForm.appendChild(viewHidId);
    viewForm.appendChild(viewHidName);
    viewNode.appendChild(viewForm);
    viewNode.setAttribute("class", "col col5");

    newCharacterNode.appendChild(nameNode);
    newCharacterNode.appendChild(raceNode);
    newCharacterNode.appendChild(classNode);
    newCharacterNode.appendChild(levelNode);
    newCharacterNode.appendChild(viewNode);

    document.getElementById("characterList").appendChild(newCharacterNode);
}
