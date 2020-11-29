using PeakSWC.Configuration;
using System;

namespace SampleNodes
{
    public class Root1 : BaseRootComponent
    {
        public Root1() { Id = "1"; }
        public string StringProp { get; set; } = "Root1";
        public int IntProp { get; set; } = 101;
        public bool BoolProp { get; set; } = true;
    }

    public class Root2 : BaseRootComponent
    {
        public Root2() { Id = "2"; }
        public string StringProp { get; set; } = "Root2";
        public int IntProp1 { get; set; } = 1;
        public int IntProp2 { get; set; } = 2;
        public int IntProp3 => 3;

    }
}
