using PeakSWC.Configuration;
using System;
using System.ComponentModel.DataAnnotations;

namespace SampleNodes
{
    public class Root1 : BaseRootComponent
    {
        public Root1() { Id = "1"; }
        public string StringProp { get; set; } = "Root1";
        [Range(10, 1000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
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
