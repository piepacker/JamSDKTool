mergeInto(LibraryManager.library, {
  SessionID: function () {
    var returnStr = window.sessionStorage.getItem("sessionID");
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

  UserSessionID: function () {
    var returnStr = window.sessionStorage.getItem("userSessionID");
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

  ClusterName: function () {
    var returnStr = window.sessionStorage.getItem("clusterName");
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

  ConnectionHost: function () {
    var returnStr = window.sessionStorage.getItem("connectionHost");
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

  DisplayName: function () {
    var returnStr = window.sessionStorage.getItem("displayName");
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

  PlayerIdx: function () {
    return parseInt(window.sessionStorage.getItem("playerIdx"));
  },
});
