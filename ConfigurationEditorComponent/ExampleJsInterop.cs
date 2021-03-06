using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace PeakSWC
{
    public class ExampleJsInterop
    {
        public static ValueTask<string> Prompt(IJSRuntime jsRuntime, string message)
        {
            
            // Implemented in exampleJsInterop.js
            return jsRuntime.InvokeAsync<string>(
                "exampleJsFunctions.showPrompt",
                message);
        }
        public static ValueTask SaveFile(IJSRuntime jsRuntime, string filename, byte[] fileContent, string contentType)
        {
           
            // Implemented in exampleJsInterop.js
            return jsRuntime.InvokeVoidAsync(
                "exampleJsFunctions.saveFile",
                filename, fileContent,contentType);
        }
    }
}
