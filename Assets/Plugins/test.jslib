mergeInto(LibraryManager.library, {

  Hello: function () {
    getSupply2();
  },

    Hello2: function () {
    var returnStr = getState();
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

});