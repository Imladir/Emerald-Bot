window.blazor = {
    getCharacterCount: function (elementName) {
        element = document.getElementById(elementName);
        return element.value.length;
    }
}