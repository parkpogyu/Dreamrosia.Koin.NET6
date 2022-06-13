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
