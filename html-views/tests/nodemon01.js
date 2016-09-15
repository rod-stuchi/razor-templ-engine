var fs = require('fs');
//var file = fs.readFileSync('reg01.txt', 'utf-8').replace(new RegExp('<!--helper:(.*)-->([\\s\\S]*)<!--endhelper-->', 'g'), '$1');

var file = fs.readFileSync('reg01.txt', 'utf-8').replace(new RegExp('<!(?:--|--\\s)helper:([^~]+?)(?:\\s--|--)>[^~]*?<!(?:--|--\\s)endhelper(?:--|\\s--)>', 'g'), '$1');

console.log(file);