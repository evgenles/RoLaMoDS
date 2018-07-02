function toFormData(form, multipleFile = false) {
    var fdata = new FormData();
    var elements = form.querySelectorAll("input, select, textarea");
    for (var i = 0; i < elements.length; ++i) {
        var element = elements[i];

        var name = element.name;
        var value = element.value;

        if (name) {
            if (element.getAttribute("type") == "file") {
                if (!multipleFile) {
                    fdata.append(name, element.files[0]);
                }
                else {
                    for (var j = 0; j < element.files.length; ++j)
                        fdata.append(name, element.files[j])
                }
            }
            else {
                fdata.append(name, value);
            }
        }
    }
    return fdata;
}

function handleEvent(f, passedInElement) {
    return function (e) {
        f(e, passedInElement);
    };
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
    function makeFilter(contrast, brightness) {
        imgList.getElementsByClassName("ActiveImage")[0].style.filter = "contrast(" + contrast + "%) brightness(" + brightness + "%)";
    }

    var PreviewImage = document.getElementById("PreloadImage");
    var PreviewImageForm = PreviewImage.getElementsByTagName("form")[0];
    var ScaleSelector = PreviewImage.getElementsByClassName("RangeGrad")[0];
    var imgCanvas = document.getElementById('ImageCanvas').getElementsByTagName("canvas")[0];
    var ctx = imgCanvas.getContext('2d');
    function makeGrid() {
        ctx.clearRect(0, 0, imgCanvas.width, imgCanvas.height);
        // imgCanvas.style.backgroundImage = "url("+imgList.getElementsByClassName("ActiveImage")[0].src+") ";

        // imgList.style.display="none";
        // imgCanvas.style.display="block";
        // document.getElementById('ImageCanvas').style.display="block";

        ceilCount = Math.round(ScaleSelector.value / 2.5);
        var cellWidth = imgCanvas.width / ceilCount;
        var cellHeight = imgCanvas.height / ceilCount;
        ctx.beginPath();
        for (var i = 0; i < ceilCount; i++) {
            ctx.moveTo(i * cellWidth, 0);
            ctx.lineTo(i * cellWidth, imgCanvas.height);

            ctx.moveTo(0, i * cellHeight);
            ctx.lineTo(imgCanvas.width, i * cellHeight);
        }
        ctx.stroke();
    }
    var ScaleInput = PreviewImage.querySelector("input[name='Scale']");
    ScaleSelector.addEventListener("input", e => {
        makeGrid();
        ScaleInput.value = ScaleSelector.value;
    });
    ScaleSelector.addEventListener("change", e => {
        makeGrid();
        ScaleInput.value = ScaleSelector.value;
    });
    ScaleInput.addEventListener("input", e => {
        ScaleSelector.value = ScaleInput.value;
        makeGrid();
    });
    function insertActiveImageFromJSON(json) {
        if (json.code == 200) {

            location.href = "#PreloadImage";
            ctx.clearRect(0, 0, imgCanvas.width, imgCanvas.height);
            // var img = new Image();
            // img.onload = function() {
            //     ctx.drawImage(img, 0, 0);
            //   };
            // img.src =  encodeURI(json.data.resultImagePath);
            //ctx.drawImage(img,100,100);
            imgCanvas.style.backgroundImage = "url(" + encodeURI(json.data.resultImagePath) + ") ";

            PreviewImageForm.querySelector("input[name='URL']").value = json.data.resultImagePath;
            PreviewImageForm.querySelector("input[name='Longitude']").value = "";
            PreviewImageForm.querySelector("input[name='Latitude']").value = "";
            PreviewImageForm.querySelector("input[name='Scale']").value = "20";
        }
    }
    PreviewImageForm.addEventListener("submit", (e) => {
        e.preventDefault();
        fetch("/Main/UploadImageFromURL", {
            method: "POST",
            body: toJSONData(PreviewImageForm),
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            }
        }).then((response) => response.json())
            .then((json) => {
                json = JSON.parse(json);
                if (json.code == 200) {
                    var imgel = document.createElement("img");
                    imgel.setAttribute("src", json.data.resultImagePath);
                    imgel.classList.add("ActiveImage");
                    for (var i = 0; i < imgList.children.length; i++) {
                        imgList.children[i].classList.remove("ActiveImage");
                    }
                    imgList.insertAdjacentElement("afterbegin", imgel);
                    location.href = "#ImageFromLocal";

                }
                else {
                    //TODO: Error
                }
            });
    });
    formFile.addEventListener("submit", (e) => {
        e.preventDefault();
        fetch("/Main/UploadImageFromFile", {
            method: "POST",
            body: toFormData(formFile, false),
            credentials: 'same-origin',
        }).then((response) => response.json())
            .then((json) => {
                json = JSON.parse(json);
                insertActiveImageFromJSON(json);
                formFile.querySelector("#Image-Upload-Input ~ label > span").innerHTML = "Выбрать файл изображения";
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
                PreviewImageForm.querySelector("input[name='Longitude']").value = formMap.querySelector("input[name='Longitude']").value;
                PreviewImageForm.querySelector("input[name='Latitude']").value = formMap.querySelector("input[name='Latitude']").value;
            });
    });

    var signInForm = document.getElementById("SignIn").querySelector("form");
    var authorizeMessage = document.getElementById("SuccessAuthorize");

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
                if (json.code == 200) {
                    authorizeMessage.getElementsByClassName("ResultText")[0].innerHTML = json.data;
                    authorizeMessage.classList.add("Show");

                    location.href = "#SelectImage";
                    document.getElementsByClassName("UserClass")[0].innerText="Выйти";
                    setTimeout(() => {
                        authorizeMessage.classList.remove("Show")
                    }, 6000);

                }
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
    var oAuthRegisterForm = document.getElementById("OAuthRegister").querySelector("form");
    oAuthRegisterForm.addEventListener("submit", (e) => {
        e.preventDefault();
        fetch("/User/OAuthRegister", {
            method: "POST",
            body: toJSONData(oAuthRegisterForm),
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
        makeFilter(ContrastSelector.value, BrightnessSelector.value);
    });
    ContrastSelector.addEventListener("input", e => {
        makeFilter(ContrastSelector.value, BrightnessSelector.value);

    });
    BrightnessSelector.addEventListener("change", e => {
        makeFilter(ContrastSelector.value, BrightnessSelector.value);
    });
    ContrastSelector.addEventListener("change", e => {
        makeFilter(ContrastSelector.value, BrightnessSelector.value);

    });

    var TrainForm = document.getElementById("TrainModel").getElementsByTagName("form")[0];
    var messages = document.getElementsByClassName("Message");
    var message = messages[0];

    TrainForm.addEventListener("submit", (e) => {
        e.preventDefault();
        fetch("/Recognize/TrainModel", {
            method: "POST",
            body: toFormData(TrainForm, true),
            credentials: 'same-origin',
        }).then((response) => response.json())
            .then((json) => {
                json = JSON.parse(json);
                console.log(json);
                document.getElementById("ResultTrain").innerText = json.data * 100 + "%";
                document.getElementById("ResultTrainForm").classList.remove("Hide");
            });
        setTimeout(() => {
            message.classList.add("Show")
        }, 10000);
        setTimeout(() => {
            message.classList.remove("Show")
        }, 15000);
    });
    for (let ms = 0; ms < messages.length; ++ms) {
        messages[ms].querySelector("button[name=Close]")
            .addEventListener("click", handleEvent((e) => {
                messages[ms].classList.remove("Show");
            }, ms));
    }
    var TrainButton = document.getElementById("TrainModelMenuButton");
    var ModelSelect = TrainForm.querySelector("select[name=ModelURL]");
    var ModelClassSelect = TrainForm.querySelector("select[name=Class]");
    TrainButton.addEventListener("click", (e) => {
        ModelSelect.innerHTML = "<option value ='' selected disabled/>";
        fetch("/Recognize/GetModels", {
            method: "Get",
            credentials: 'same-origin',
        }).then((response) => response.json())
            .then((json) => {
                json = JSON.parse(json);
                console.log(json);
                if (json.code == 200) {
                    for (var i = 0; i < json.data.length; i++) {
                        let el = document.createElement("option");
                        el.value = json.data[i].URL;
                        el.innerText = json.data[i].Name;
                        ModelSelect.appendChild(el);
                    }
                }
            });
    });
    ModelSelect.addEventListener("change", e => {
        ModelClassSelect.innerHTML = "<option value ='' selected disabled/>";
        fetch("/Recognize/GetClassModels?URL=" + ModelSelect.value, {
            method: "Get",
            credentials: 'same-origin',
        }).then((response) => response.json())
            .then((json) => {
                json = JSON.parse(json);
                console.log(json);
                if (json.code == 200) {
                    for (var i = 0; i < json.data.length; i++) {
                        let el = document.createElement("option");
                        el.value = json.data[i].Id;
                        el.innerText = json.data[i].Name;
                        ModelClassSelect.appendChild(el);
                    }
                }
            });
    });
    var recognizeButton = document.getElementById("StartRecognizeButton");
    var recognizeResultPage = document.getElementById("RecognizePage");
    var SelectImageForm = document.getElementById("SelectImage").getElementsByTagName("form")[0];
    var SelectedImageURL = null;
    SelectImageForm.addEventListener("submit", e => {
        e.preventDefault();
        SelectedImageURL = imgList.getElementsByClassName("ActiveImage")[0].src;
    });
    recognizeButton.addEventListener("click", e => {
        recognizeResultPage.getElementsByClassName("Loading")[0].classList.remove("Hide");
        let resultRec = recognizeResultPage.getElementsByClassName("ResultRecognize")[0];
        resultRec.innerHTML = "";
        fetch("/Recognize/StartRecognize", {
            method: "POST",
            body: JSON.stringify({ ModelURL: "model1", SatelliteURL: SelectedImageURL }),
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            },
        }).then((response) => response.json())
            .then((json) => {
                json = JSON.parse(json);
                console.log(json);
                if (json.code == 200) {
                    var len = json.data.length;
                    let div =document.createElement("div");
                    div.innerHTML = "Результаты распознавания:";
                    let header = resultRec.appendChild(div);
                    var tbl = document.createElement("table");

                    for (var i = 0; i < len; i++) {
                        var tr = document.createElement("tr");
                        for (var j = 0; j < len; j++) {
                            var td = document.createElement("td");
                            var spn = document.createElement("div");
                            spn.innerText = "Обьект: " + json.data[i][j].Class;
                            td.appendChild(spn);
                            var spn = document.createElement("div");
                            spn.innerText = "Высота: " + json.data[i][j].Height;
                            td.appendChild(spn);
                            tr.appendChild(td);
                        }
                        tbl.appendChild(tr);
                    }
                    resultRec.appendChild(tbl);
                    recognizeResultPage.getElementsByClassName("Loading")[0].classList.add("Hide");

                }

            });
    });
});
