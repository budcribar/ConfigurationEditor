// This file is to show how a library package may provide JavaScript interop features
// wrapped in a .NET API
function downloadFromUrl(options) {
    const anchorElement = document.createElement('a');
    anchorElement.href = options.url;
    anchorElement.download = options.fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
}
window.exampleJsFunctions = {
  showPrompt: function (message) {
    return prompt(message, 'Type anything here');
    },
  saveFile: function (filename, byteArray, contentType) {
      // Convert base64 string to numbers array.
      var numArray = atob(options.byteArray).split('').map(c => c.charCodeAt(0));

      // Convert numbers array to Uint8Array object.
      var uint8Array = new Uint8Array(numArray);

      // Wrap it by Blob object.
      var blob = new Blob([uint8Array], { type: options.contentType });

      // Create "object URL" that is linked to the Blob object.
      var url = URL.createObjectURL(blob);

      // Invoke download helper function that implemented in 
      // the earlier section of this article.
      downloadFromUrl({ url: url, fileName: options.fileName });

      // At last, release unused resources.
      URL.revokeObjectURL(url);
    }
};
