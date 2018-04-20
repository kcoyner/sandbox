const { DateTime } = require('luxon');

// var ts = DateTime.local(2017, 5, 15, 8, 30);
// const dt = DateTime.fromMillis(ts);
// console.log('dt: ', dt)

var d = new Date('2018-03-17');
var n = d.getTime();

var d = new Date('2018-03-18');
var m = d.getTime();

console.log('n: ', n)
console.log('m: ', m)
