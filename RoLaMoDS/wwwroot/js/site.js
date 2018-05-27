function toFormData(form) {
    var fdata = new FormData();
    document.getElementById("")
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

document.addEventListener("DOMContentLoaded", () => {
    location = location.hash || "#PageEnteringForm"
    document.getElementById("Image-Upload-Input").addEventListener("change", (e) => {
        filename = e.target.value.split('\\').pop();;
        if (filename)
            document.querySelector("#Image-Upload-Input ~ label > span").innerHTML = filename;
    });
    var form = document.getElementById("ImageFromLocal").querySelector("form");
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

    form.addEventListener("submit", (e) => {
        e.preventDefault();
        fetch("/Main/UploadMap", {
            method: "POST",
            body: toFormData(form)
        }).then((response) => response.json())
            .then((json) => {
                json = JSON.parse(json)
                var imgel = document.createElement("img");
                imgel.setAttribute("src", json.data.resultImagePath);
                imgel.classList.add("ActiveImage");
                for(var i = 0; i<imgList.children.length;i++){
                    imgList.children[i].classList.remove("ActiveImage");
                }
                imgList.insertAdjacentElement("afterbegin",imgel);
            });
    });
});
