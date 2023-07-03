export function rand(length) {
  // https://stackoverflow.com/a/47496558/810360
  //[...Array(length)].map(() => Math.random().toString(36)[2])

  // https://stackoverflow.com/a/46536578/810360
  var char;
  var arr = [];
  var length = length || 32;
  var alphaNumeric = [
    48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 65, 66, 67, 68, 69, 70, 71, 72, 73,
    74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 97, 98,
    99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113,
    114, 115, 116, 117, 118, 119, 120, 121, 122,
  ];
  do {
    char = ~~(Math.random() * 128);
    if (alphaNumeric.indexOf(char) !== -1) {
      arr.push(String.fromCharCode(char));
    }
  } while (arr.length < length);

  return arr.join("");
}
