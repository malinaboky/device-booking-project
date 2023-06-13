document.addEventListener("DOMContentLoaded", countCheckboxesOs);
document.addEventListener("DOMContentLoaded", countCheckboxesTag);
document.addEventListener("DOMContentLoaded", countCheckboxesType);


function countCheckboxesOs() {
    lenOs = $("input:checkbox[id^='SelectedOs']").length;
    countOs = 0;
}

function changeAllOs() {
    let selectAll = document.getElementById("SelectAllOs");
    for (let i = 0; i < lenOs; i++) {
        document.getElementById("SelectedOs" + i).checked = selectAll.checked;
        if (selectAll.checked) {
            countOs += 1;
        }
        else {
            countOs -= 1;
        }
    }
}

function turnSelectAllOs(id) {
    let selectAll = document.getElementById("SelectAllOs");
    let deviceCheckbox = document.getElementById("SelectedOs" + id);
    if (deviceCheckbox.checked) {
        countOs += 1;
    }
    else {
        countOs -= 1;
    }
    var allChecked = lenOs === countOs;
    selectAll.checked = allChecked;
}

function countCheckboxesTag() {
    lenTag = $("input:checkbox[id^='SelectedTag']").length;
    countTag = 0;
}

function changeAllTag() {
    let selectAll = document.getElementById("SelectAllTag");
    for (let i = 0; i < lenTag; i++) {
        document.getElementById("SelectedTag" + i).checked = selectAll.checked;
        if (selectAll.checked) {
            countTag += 1;
        }
        else {
            countTag -= 1;
        }
    }
}

function turnSelectAllTag(id) {
    let selectAll = document.getElementById("SelectAllTag");
    let deviceCheckbox = document.getElementById("SelectedTag" + id);
    if (deviceCheckbox.checked) {
        countTag += 1;
    }
    else {
        countTag -= 1;
    }
    var allChecked = lenTag === countTag;
    selectAll.checked = allChecked;
}

function countCheckboxesType() {
    lenType = $("input:checkbox[id^='SelectedType']").length;
    countType = 0;
}

function changeAllType() {
    let selectAll = document.getElementById("SelectAllType");
    for (let i = 0; i < lenType; i++) {
        document.getElementById("SelectedType" + i).checked = selectAll.checked;
        if (selectAll.checked) {
            countType += 1;
        }
        else {
            countType -= 1;
        }
    }
}

function turnSelectAllType(id) {
    let selectAll = document.getElementById("SelectAllType");
    let deviceCheckbox = document.getElementById("SelectedType" + id);
    if (deviceCheckbox.checked) {
        countType += 1;
    }
    else {
        countType -= 1;
    }
    var allChecked = lenType === countType;
    selectAll.checked = allChecked;
}

function saveOs(type, id) {
    let tag = document.getElementById(type + id);
    let nameOfTag = tag.value;

    const item = {
        Id: id,
        Name: nameOfTag
    };

    fetch("TagPage/ChangeOs", {
        method: 'POST',
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(item)
    })
        .then((response) => response.json())
        .then((data) => {
            let messange = document.getElementById("messange");
            if (data.messange == "Ok") {
                tag.style.backgroundColor = "#93C54B";
            }
            else {
                tag.style.backgroundColor = "#FF6C65";
            }
            messange.innerHTML = data.messange;
            $("#messange").fadeIn();
            setTimeout(function () { $("#messange").fadeOut(); }, 2000);
        })
        .catch(error => console.error('Unable to add item.', error));
}

function saveTag(type, id) {
    let tag = document.getElementById(type + id);
    let nameOfTag = tag.value;

    const item = {
        Id: id,
        Name: nameOfTag
    };

    fetch("TagPage/ChangeTag", {
        method: 'POST',
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(item)
    })
        .then((response) => response.json())
        .then((data) => {
            let messange = document.getElementById("messange");
            if (data.messange == "Ok") {
                tag.style.backgroundColor = "#93C54B";
            }
            else {            
                tag.style.backgroundColor = "#FF6C65";
            }
            messange.innerHTML = data.messange;
            $("#messange").fadeIn();
            setTimeout(function () { $("#messange").fadeOut(); }, 2000); 
        })
        .catch(error => console.error('Unable to add item.', error));
}

function saveType(type, id) {
    let tag = document.getElementById(type + id);
    let nameOfTag = tag.value;

    const item = {
        Id: id,
        Name: nameOfTag
    };

    fetch("TagPage/ChangeType", {
        method: 'POST',
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(item)
    })
        .then((response) => response.json())
        .then((data) => {
            let messange = document.getElementById("messange");
            if (data.messange == "Ok") {
                tag.style.backgroundColor = "#93C54B";
            }
            else {
                tag.style.backgroundColor = "#FF6C65";
            }
            messange.innerHTML = data.messange;
            $("#messange").fadeIn();
            setTimeout(function () { $("#messange").fadeOut(); }, 2000);
        })
        .catch(error => console.error('Unable to add item.', error));
}