//function SetSort(id) {
//    id = "header" + id;
//    let header = document.getElementById(id);
//    let headerName = document.querySelector(`#${id} > div > input`).getAttribute("placeholder");
//    if (header.classList.contains("desc")) {
//        header.classList.remove("desc");
//        header.classList.add("asc");
//        Sort(headerName, "asc");
//        return;
//    }
//    if (header.classList.contains("asc")) {
//        header.classList.remove("asc");
//        Sort(headerName, "");
//        return
//    }
//    header.classList.add("desc");
//    Sort(headerName, "desc");
//}

//function Sort(name, filterMethod) {
//    const item = {
//        name: name,
//        filter: filterMethod
//    };

//    fetch("DevicePage/SortDevices", {
//        method: 'POST',
//        headers: {
//            "Content-Type": "application/json"
//        },
//        body: JSON.stringify(item)
//    })
//        .then(response => this.fetchData())
//        .catch(error => console.error('Unable to add item.', error));
//}

//function GetFilter() {
//    MvcGridPopup.show();
//}