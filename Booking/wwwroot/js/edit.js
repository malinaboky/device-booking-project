document.addEventListener("DOMContentLoaded", checkPath);

function checkPath() {
    path = document.getElementById("old-pic").src;
}

function imageOnClick(path) {
    window.open(path);
    return false;
}

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