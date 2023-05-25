document.getElementById("pruebaHola").click();
window.addEventListener("load", function () {
    history.pushState({ noBack: true }, null, location.href);
});

window.addEventListener("popstate", function () {
    history.pushState({ noBack: true }, null, location.href);
});
