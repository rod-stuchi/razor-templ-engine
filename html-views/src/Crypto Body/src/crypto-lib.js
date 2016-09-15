var Core = require('crypto-js/core');
var AES = require('crypto-js/aes');
var SHA512 = require('crypto-js/sha512');
var PBKDF2 = require('crypto-js/pbkdf2');

var exp = module.exports = {};

var pageIsLoaded = true;
//razorsafe:var cttCipher = [@Raw(Model.CipherText)];
var cttCipher =
['ENziOg+9AFujWMlK0gkdCa6iHA45W6g5xGylUEiaUQJSPU+XU/HPNjTGCEZX9ONJRJV8Af+12hQnUb52',
'VrVCS8Ce+pkWyXxywOoEbuXQN9PcOnIlocSveNSIqbrYfq9vYaExW8mq4o/WiTLsmxauipP76UHWAgEr',
'Fkvgx92IT9NXFV7UIr/5SMmx0+H8U3+QPkATkZxzw4hpS1QZbDfC+ewfxjE7JBijg2Qjhi5ZRsI='];
//endrazorsafe

exp.decrypt = function () {
    if (!pageIsLoaded) {
        alert('Aguarde a p\xe1gina carregar.');
        return;
    }

    var txtpass = document.getElementById('txt-pass');
    var passHash = Core.SHA512(txtpass.value);
    var txtCipher = cttCipher.join('');
    //razor:var iniVector = Core.enc.Hex.parse("@Model.InitializationVector");
    var iniVector = Core.enc.Hex.parse('7121402377706F696A2063C3A1312D32');
    //endrazor
    var passHashB64 = passHash.toString(Core.enc.Base64);
    //razor:var salt = Core.enc.Utf8.parse("@Model.Salt");
    var salt = Core.enc.Utf8.parse('insight123resultxyz');
    //endrazor
    var PBKDF2Hash = Core.PBKDF2(passHashB64.toString(Core.enc.Utf8), salt, {
        keySize: 8,
        iterations: 1e3
    });
    var cipherParam = Core.lib.CipherParams.create({
        ciphertext: Core.enc.Base64.parse(txtCipher)
    });
    try {
        var encText = Core.AES.decrypt(cipherParam, PBKDF2Hash, {
            mode: Core.mode.CBC,
            iv: iniVector,
            padding: Core.pad.Pkcs7
        });

        var decryptText = encText.toString(Core.enc.Utf8);

        openTabDecrypt(decryptText);

        return true;

    } catch (error) {
        console.log(error);
        var pww = document.getElementById('pww-info');
        pww.className = 'pwwinfo';
        txtpass.className = 'error';

        return false;
    }
};

function openTabDecrypt(html) {
    var txtp = document.getElementById('txt-pass');
    txtp.value = '';
    changeCounter('0');

    var tab;
    if ((navigator.appName.indexOf('Netscape') == -1)) {
        tab = window;
    } else {
        tab = window.open('', 'RodStu', 'toolbar= 0,location= 0, directories=1,status=1, menubar=1,scrollbars=1,resizable=1,width=1024,height=728');
    }
    tab.document.open('text/html');
    tab.document.clear();
    tab.document.write(html);
    tab.document.close();
}

function changeCounter(v) {
    var counter = document.getElementById('counter');
    counter.innerHTML = v;
}

function mgmtTxtHashEvents(event) {
    var txtp = document.getElementById('txt-pass');
    changeCounter(txtp.value.length);

    var pwrong = document.getElementById('pww-info');
    pwrong.className = 'nodisplay';
    txtp.className = '';

    var ev = event.keyCode ? event.keyCode : event.which;
    13 == ev && exp.decrypt();
    event.preventDefault();
}

exp.checkPageLoad = function () {
    pageIsLoaded = true;
    var txtp = document.getElementById('txt-pass');
    txtp.addEventListener ? txtp.addEventListener('keyup', function (a) {
        mgmtTxtHashEvents(a);
    }, !1) : txtp.attachEvent('onkeyup', function (a) {
        mgmtTxtHashEvents(a);
    });
};

window.secure = exp;

$(document).ready(function () {
    $('a[rel^="prettyPhoto"]').prettyPhoto({
        animation_speed: 'fast', /* fast/slow/normal */
        // slideshow: 5000, /* false OR interval time in ms */
        // autoplay_slideshow: false, /* true/false */
        // opacity: 0.80, /* Value between 0 and 1 */
        // show_title: true, /* true/false */
        // allow_resize: true, /* Resize the photos bigger than viewport. true/false */
        // default_width: 500,
        // default_height: 344,
        // counter_separator_label: '/', /* The separator for the gallery counter 1 "of" 2 */
        // theme: 'pp_default', /* light_rounded / dark_rounded / light_square / dark_square / facebook */
        // horizontal_padding: 20, /* The padding on each side of the picture */
        // hideflash: false, /* Hides all the flash object on a page, set to TRUE if flash appears over prettyPhoto */
        // wmode: 'opaque', /* Set the flash wmode attribute */
        // autoplay: true, /* Automatically start videos: True/False */
        // modal: false, /* If set to true, only the close button will close the window */
        deeplinking: false, /* Allow prettyPhoto to update the url to enable deeplinking. */
        // overlay_gallery: true, /* If set to true, a gallery will overlay the fullscreen image on mouse over */
        // keyboard_shortcuts: true, /* Set to false if you open forms inside prettyPhoto */
        // changepicturecallback: function () { }, /* Called everytime an item is shown/changed */
        // callback: function () { }, /* Called when prettyPhoto is closed */
        // ie6_fallback: true,
        iframe_markup: '<iframe src ="{path}" width="{width}" height="{height}" allowfullscreen="true" frameborder="no"></iframe>',
        social_tools: false
    });
});
