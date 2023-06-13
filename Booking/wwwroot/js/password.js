document.getElementById("password").addEventListener('input', checkPassword);

function changeVisibility() {
    var password = document.getElementById("password");
    if (password.type == "password") {
        password.type = "text;"
    }
    else {
        password.type = "password";
    }
}

function checkPassword() {
    document.getElementById("condition-list").classList.remove("enable");
    var letterNumber = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])/;
    var password = document.getElementById("password");
    var letterCheck = password.value.match(letterNumber);
    var lenCheck = password.value.length >= 8;
    if (letterCheck) {
        document.getElementById("letter").style.color = "#69CD1F";
    }
    else {
        document.getElementById("letter").style.color = "#FF0000";
    }
    if (lenCheck) {
        document.getElementById("length").style.color = "#69CD1F";
    }
    else {
        document.getElementById("length").style.color = "#FF0000";
    }
    if (letterCheck && lenCheck) {
        document.getElementById("button").disabled = false;
    }
    else {
        document.getElementById("button").disabled = true;
    }
}