
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakSWC.Configuration
{
    public class RootComponent : BaseRootComponent
    {

        public string StringProp { get; set; } = "test";
        public int IntProp { get; set; }

    }
}
