// This file is to show how a library package may provide JavaScript interop features
// wrapped in a .NET API

window.exampleJsFunctions = {
  showPrompt: function (message) {
    return prompt(message, 'Type anything here');
    },
  saveFile: function (fileName, byteArray, contentType) {
      // Convert base64 string to numbers array.
      var numArray = atob(byteArray).split('').map(c => c.charCodeAt(0));

      // Convert numbers array to Uint8Array object.
      var uint8Array = new Uint8Array(numArray);

      // Wrap it by Blob object.
      var blob = new Blob([uint8Array], { type: contentType });

      // Create "object URL" that is linked to the Blob object.
      var url = URL.createObjectURL(blob);

      // Invoke download helper function that implemented in 
      // the earlier section of this article.
      //exampleJsFunctions.downloadFromUrl(url, fileName);

      const anchorElement = document.createElement('a');
      anchorElement.href = url;
      anchorElement.download = fileName ?? '';
      anchorElement.click();
      anchorElement.remove();

      // At last, release unused resources.
      URL.revokeObjectURL(url);
    }
};
