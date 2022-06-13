function func_getWindowSize() {

    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
}

function func_isRendered(name) {

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

// loadScript: returns a promise that completes when the script loads
window.loadScript = function (scriptPath) {
    // check list - if already loaded we can ignore
    if (loaded[scriptPath]) {
        console.log(scriptPath + " already loaded");
        // return 'empty' promise
        return new this.Promise(function (resolve, reject) {
            resolve();
        });
    }

    return new Promise(function (resolve, reject) {
        // create JS library script element
        var script = document.createElement("script");
        script.src = scriptPath;
        script.type = "text/javascript";
        console.log(scriptPath + " created");

        // flag as loading/loaded
        loaded[scriptPath] = true;

        // if the script returns okay, return resolve
        script.onload = function () {
            console.log(scriptPath + " loaded ok");
            resolve(scriptPath);
        };

        // if it fails, return reject
        script.onerror = function () {
            console.log(scriptPath + " load failed");
            reject(scriptPath);
        }

        // scripts will load at end of body
        document["body"].appendChild(script);
    });
}
// store list of what scripts we've loaded
loaded = [];