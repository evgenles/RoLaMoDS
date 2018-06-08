function toFormData(form) {
    var fdata = new FormData();
    var elements = form.querySelectorAll("input, select, textarea");
    for (var i = 0; i < elements.length; ++i) {
        var element = elements[i];

        var name = element.name;
        var value = element.value;

        if (name) {
            if (element.getAttribute("type") == "file") {
                fdata.append(name, element.files[0])
            }
            else {
                fdata.append(name, value);
            }
        }
    }
    return fdata;
}

function toJSONData(form) {
    var data = {};
    var elements = form.querySelectorAll("input, select, textarea");
    for (var i = 0; i < elements.length; ++i) {
        var element = elements[i];

        var name = element.name;
        var value = element.value;

        if (name)
            if (element.type == "checkbox" || element.type == "radio")
                data[name] = element.checked;
            else
                data[name] = value;
    }
    return JSON.stringify(data);
}

document.addEventListener("DOMContentLoaded", () => {
    location = location.hash || "#PageEnteringForm"
    document.getElementById("Image-Upload-Input").addEventListener("change", (e) => {
        filename = e.target.value.split('\\').pop();;
        if (filename)
            document.querySelector("#Image-Upload-Input ~ label > span").innerHTML = filename;
    });
    var formFile = document.getElementById("ImageFromLocal").querySelector("form");
    var formURL = document.getElementById("ImageFromURL").querySelector("form");
    var formMap = document.getElementById("ImageFromMaps").querySelector("form");
    var imgList = document.getElementById("ImageList");
    document.getElementById("ImagePrevLeft").addEventListener("click", () => {
        images = imgList.querySelectorAll("img");
        for (var i = 0; i < images.length; i++) {
            if (images[i].classList.contains("ActiveImage")) {
                images[i].classList.remove("ActiveImage");
                images[i > 0 ? i - 1 : images.length - 1].classList.add("ActiveImage");
                break;
            }
        }
    });

    document.getElementById("ImageNextRight").addEventListener("click", () => {
        images = imgList.querySelectorAll("img");
        for (var i = 0; i < images.length; i++) {
            if (images[i].classList.contains("ActiveImage")) {
                images[i].classList.remove("ActiveImage");
                images[i < images.length - 1 ? i + 1 : 0].classList.add("ActiveImage");
                break;
            }
        }
    });
    function makeFilter(contrast,brightness) {
        imgList.getElementsByClassName("ActiveImage")[0].style.filter = "contrast(" + contrast + "%) brightness(" + brightness + "%)";
    }

    function insertActiveImageFromJSON(json) {
        var imgel = document.createElement("img");
        imgel.setAttribute("src", json.data.resultImagePath);
        imgel.classList.add("ActiveImage");
        for (var i = 0; i < imgList.children.length; i++) {
            imgList.children[i].classList.remove("ActiveImage");
        }
        imgList.insertAdjacentElement("afterbegin", imgel);
    }

    formFile.addEventListener("submit", (e) => {
        e.preventDefault();
        fetch("/Main/UploadImageFromFile", {
            method: "POST",
            body: toFormData(formFile),
            credentials: 'same-origin',
        }).then((response) => response.json())
            .then((json) => {
                json = JSON.parse(json);
                insertActiveImageFromJSON(json);
            });
    });

    formURL.addEventListener("submit", (e) => {
        e.preventDefault();
        fetch("/Main/UploadImageFromURL", {
            method: "POST",
            body: toJSONData(formURL),
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            }
        }).then((response) => response.json())
            .then((json) => {
                json = JSON.parse(json);
                insertActiveImageFromJSON(json);
            });
    });

    formMap.addEventListener("submit", (e) => {
        e.preventDefault();
        fetch("/Main/UploadImageFromMap", {
            method: "POST",
            body: toJSONData(formMap),
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            }
        }).then((response) => response.json())
            .then((json) => {
                json = JSON.parse(json);
                insertActiveImageFromJSON(json);
            });
    });

    var signInForm = document.getElementById("SignIn").querySelector("form");
    signInForm.addEventListener("submit", (e) => {
        e.preventDefault();
        fetch("/User/SignIn", {
            method: "POST",
            body: toJSONData(signInForm),
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            }
        }).then((response) => response.json())
            .then((json) => {
                json = JSON.parse(json);
                console.log(json);
            });
    });

    var signUpForm = document.getElementById("SignUp").querySelector("form");
    signUpForm.addEventListener("submit", (e) => {
        e.preventDefault();
        fetch("/User/Register", {
            method: "POST",
            body: toJSONData(signUpForm),
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            }
        }).then((response) => response.json())
            .then((json) => {
                json = JSON.parse(json);
                console.log(json);
            });
    });
    var BrightnessSelector = document.getElementById("BrightnessSelector");
    var ContrastSelector = document.getElementById("ContrastSelector");
    BrightnessSelector.addEventListener("input", e => {
        makeFilter(ContrastSelector.value,BrightnessSelector.value);
    });
    ContrastSelector.addEventListener("input", e => {       
         makeFilter(ContrastSelector.value,BrightnessSelector.value);

    });
    BrightnessSelector.addEventListener("change", e => {
        makeFilter(ContrastSelector.value,BrightnessSelector.value);
    });
    ContrastSelector.addEventListener("change", e => {       
         makeFilter(ContrastSelector.value,BrightnessSelector.value);

    });
    var ScaleSelector = document.getElementById("ImageFromLocal").getElementsByClassName("RangeGrad")[0];
    var imgCanvas = document.getElementById('ImageCanvas').getElementsByTagName("canvas")[0];
    var ctx = imgCanvas.getContext('2d');
    ScaleSelector.addEventListener("input", e => {
        ctx.clearRect(0, 0, imgCanvas.width, imgCanvas.height);
        imgCanvas.style.backgroundImage = "url("+imgList.getElementsByClassName("ActiveImage")[0].src+") ";

        imgList.style.display="none";
        imgCanvas.style.display="block";
        document.getElementById('ImageCanvas').style.display="block";
      
        ceilCount = Math.round(ScaleSelector.value / 5);
        var cellWidth = imgCanvas.width / ceilCount;
        var cellHeight = imgCanvas.height / ceilCount;
        ctx.beginPath();    
        for (var i = 0; i<ceilCount;i++){
            ctx.moveTo(i*cellWidth, 0); 
            ctx.lineTo(i*cellWidth, imgCanvas.height); 
        
            ctx.moveTo(0, i*cellHeight); 
            ctx.lineTo(imgCanvas.width, i*cellHeight);
        }
       ctx.stroke();
    });
});
