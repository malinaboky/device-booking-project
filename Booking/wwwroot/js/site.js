document.addEventListener("DOMContentLoaded", countCheckboxes);
document.addEventListener("DOMContentLoaded", truncate);


function countCheckboxes() {
    lenOfCheckboxes = $("input:checkbox[id^='Selected']").length;
    countOfCheckboxes = 0;
    path = document.getElementById("old-pic").src;
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

function setStay() {
    let flag = document.getElementById("stay");
    flag.value = true;
}

function imageOnClick(path) {
    window.open(path);
    return false;
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

const chooseFile = document.getElementById("choose-file");
const imgPreview = document.getElementById("img-preview");

chooseFile.addEventListener("change", function () {
    getImgData();
});

function getImgData() {
    const files = chooseFile.files[0];
    if (files) {
        const fileReader = new FileReader();
        fileReader.readAsDataURL(files);
        fileReader.addEventListener("load", function () {
            imgPreview.style.display = "block";
            imgPreview.innerHTML =
                '<img src="' + this.result + '" height = "200" />';
        });
    }
}

function removeNewImg() {
    imgPreview.innerHTML =
        `<a title="Нажмите, для просмотра в полном размере" href="#" onclick="imageOnClick('${path}')" class="img-link">
             <img src = "${path}" height = "200" id="old-pic"/>
        </a >`
}