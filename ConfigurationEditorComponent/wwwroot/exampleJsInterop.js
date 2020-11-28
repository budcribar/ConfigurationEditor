// This file is to show how a library package may provide JavaScript interop features
// wrapped in a .NET API

window.exampleJsFunctions = {
  showPrompt: function (message) {
    return prompt(message, 'Type anything here');
    },
  saveFile: function (filename, fileContent) {
      var link = document.createElement('a');
      link.download = filename;
      link.href = "data:text/plain;charset=utf-8," + encodeURIComponent(fileContent)
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    }
};
