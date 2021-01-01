
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PeakSWC.Configuration
{
    public class RootComponent : BaseRootComponent
    {
        public string StringProp { get; set; } = "test";

        [Range(1, 10, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int IntProp { get; set; }

    }
}
