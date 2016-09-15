var fs = require('fs');
//var file = fs.readFileSync('reg01.txt', 'utf-8').replace(new RegExp('<!--helper:(.*)-->([\\s\\S]*)<!--endhelper-->', 'g'), '$1');

var file = fs.readFileSync('reg02.txt', 'utf-8').replace(new RegExp('\\/(?:\\/|\\/\\s)razor:([^~]*?=)([^~]*?;)[^*]+?\\/(?:\\/|\\s)endrazor', 'g'), '$1 "______________$2______________";');

console.log(file);