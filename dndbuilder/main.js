/**
 * Name: Benjamin Nicholas Palmer
 * Student ID: 17743075
 * Class: Distributed Computing (COMP3008)
 * Date Last Updated: 22APR19
 *
 * Purpose:
 * Javascript for the DnD character builder website.
 **/

// ------ GLOBAL VARS

// XHR Request Variables
let stubGETreq = null;
let stubPOSTreq = null;

// ------ END GLOBAL VARS ------

function initStub() {

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