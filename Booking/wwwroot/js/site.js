document.addEventListener("DOMContentLoaded", countCheckboxes);
document.addEventListener("DOMContentLoaded", truncate);


function countCheckboxes() {
    lenOfCheckboxes = $("input:checkbox[id^='Selected']").length;
    countOfCheckboxes = 0;
}

function changeAll() {
    let selectAll = document.getElementById("SelectAll");
    for (let i = 0; i < lenOfCheckboxes; i++) {
        document.getElementById("Selected" + i).checked = selectAll.checked;
        if (selectAll.checked) {
            countOfCheckboxes += 1;
        }
        else {
            countOfCheckboxes -= 1;
        }
    }
}

function turnSelectAll(id) { 
    let selectAll = document.getElementById("SelectAll");
    let deviceCheckbox = document.getElementById("Selected" + id);
    if (deviceCheckbox.checked) {
        countOfCheckboxes += 1;
    }
    else {
        countOfCheckboxes -= 1;
    }
    var allChecked = lenOfCheckboxes === countOfCheckboxes;
    selectAll.checked = allChecked;
}

function removeSelected() {
    document.getElementById("ReviewerId").selectedIndex = -1;
}

function truncate() {
    var texts = document.getElementsByClassName("column");
    for (let i in texts) {
        if (texts[i].innerHTML.length > 250)
            texts[i].innerHTML = texts[i].innerText.substring(0, 250) + '...';
    }
};