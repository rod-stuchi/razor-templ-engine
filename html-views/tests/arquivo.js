var AES = require('crypto-js/aes'),
    SHA512 = require('crypto-js/sha512'),
    PBKDF2 = require('crypto-js/PBKDF2'),
    Core = require('crypto-js/core');

var ___loaded = true,
    cttCipher = [];

function doIt() {
    if (!___loaded) {
        alert("Aguarde a p\xe1gina carregar.");
        return;
    }

    var txtpass = document.getElementById('txt-pass'),
        passHash = Core.SHA512(txtpass.value),
        txtCipher = cttCipher.join(''),
        iniVector = Core.enc.Hex.parse('1F0213B24D1FAC5A68CABDCA784A14EB'), //("@Model.InitializationVector"),
        passHashB64 = passHash.toString(Core.enc.Base64),
        salt = Core.enc.Utf8.parse('49DD9DC99FD34820B0F54C127DC22348'), //("@Model.Salt"),
        PBKDF2Hash = Core.PBKDF2(passHashB64.toString(Core.enc.Utf8), salt, {
            keySize: 8,
            iterations: 1e3
        }),
        cipherParam = Core.lib.CipherParams.create({
            ciphertext: Core.enc.Base64.parse(txtCipher)
        });
    try {
        var encText = Core.AES.decrypt(cipherParam, PBKDF2Hash, {
                mode: Core.mode.CBC,
                iv: iniVector,
                padding: Core.pad.Pkcs7
            }),
            decryptText = encText.toString(Core.enc.Utf8);

        console.log(decryptText.substring(0, 100));

        OpenTabDecrypt(decryptText);
    } catch (j) {
        console.log(j);
        var kk = document.getElementById('pww-info');
        kk.className = 'pwwinfo';
        txtpass.className = 'error';
    }
}

function OpenTabDecrypt(a) {
    var tx = document.getElementById('txt-pass');
    tx.value = '';
    var b;
    b = -1 == navigator.appName.indexOf('Netscape') ? window : window.open('', 'RodStu', 'toolbar= 0,location= 0, directories=1,status=1, menubar=1,scrollbars=1,resizable=1,width=1024,height=728'),
        b.document.open('text/html'),
        b.document.clear(),
        b.document.write(a),
        b.document.close();
}

function mgmtTxtHashEvents(a) {
    var b = document.getElementById('txt-pass'),
        c = document.getElementById('counter');
    c.innerHTML = b.value.length;
    var d = document.getElementById('pww-info');
    d.className = 'nodisplay';
    b.className = '';

    var ev = a.keyCode ? a.keyCode : a.which;
    13 == ev && doIt();
    a.preventDefault();
}

function checkLoad() {
    ___loaded = !0;
    var a = document.getElementById('txt-pass');
    a.addEventListener ? a.addEventListener('keyup', function(a) {
        mgmtTxtHashEvents(a);
    }, !1) : a.attachEvent('onkeyup', function(a) {
        mgmtTxtHashEvents(a);
    });
}