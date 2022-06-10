function func_getWindowSize() {

    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
}

function func_isRendered (name) {

    var element = document.getElementById(name);
    if (typeof element !== "undefined" && element != null) {
        return true;
    }
    else {
        return false;
    }
}

var delay = 300;
var timer = null;

//Javascript
window.addEventListener('load', function () {
    clearTimeout(timer);
    timer = setTimeout(function () {
        func_getWindowSize();
    }, delay);
});

window.addEventListener('resize', function () {
    clearTimeout(timer);
    timer = setTimeout(function () {
        func_getWindowSize();
    }, delay);
});

var func_BoundingClientRect = {

    adjust_height: function (name) {

        var element = document.getElementById(name);

        if (typeof element !== "undefined" && element != null) {

            var height = window.innerHeight;

            var rect = this.get(name);

            rect.height = height - rect.top;

            element.style.height = rect.height;

            console.log("height:" + rect.height);
        }
    },

    // 너무 느림 ㅠㅠ
    get: function (name) {

        var element = document.getElementById(name);

        if (typeof element !== "undefined" && element != null) {

            return element.getBoundingClientRect();
        }

        return null;
    },

    top: function (name) {

        var element = document.getElementById(name);

        if (typeof element !== "undefined" && element != null) {

            return element.offsetTop;
        }

        return -1;
    },
    height: function (name) {

        var element = document.getElementById(name);

        if (typeof element !== "undefined" && element != null) {

            return element.offsetHeight;
        }

        return -1;
    },
}


// Html2Canvas
async function Screenshot (id, name) {
    var img = "";
    await html2canvas(document.querySelector("#" + id)).then(canvas => img = canvas.toDataURL("image/png"));
    var d = document.createElement("a");
    d.href = img;
    d.download = name + ".png";
    d.click();
    return img;
}