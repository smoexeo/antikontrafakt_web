document.getElementById('btnCancel').onclick = function () {
    document.getElementById('surname').required = false;
    document.getElementById('firstname').required = false;
    document.getElementById('mail-adress').required = false;
    document.getElementById('department').required = false;
    document.getElementById('request-type').required = false;
    document.getElementById('sp-adress').required = false;
    document.getElementById('message').required = false;
}

// не уверен, что это надо делать на кнопку отправки, но пусть пока будет
document.getElementById('btnSubmit').onclick = function () {
    document.getElementById('surname').required = true;
    document.getElementById('firstname').required = true;
    document.getElementById('mail-adress').required = true;
    document.getElementById('department').required = true;
    document.getElementById('request-type').required = true;
    document.getElementById('sp-adress').required = true;
    document.getElementById('message').required = true;
}

var selectorRequestType = document.getElementById('request-type');
selectorRequestType.onchange = function () {
    var val = selectorRequestType.value;

    if (val == "product") {
        document.getElementById('sp-adress-block').hidden = true;
        document.getElementById('sp-adress').required = false;
    } else if (val == "sale-point") {
        document.getElementById('sp-adress-block').hidden = false;
        document.getElementById('sp-adress').required = true;
    }
}