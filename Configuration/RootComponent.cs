
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakSWC.Configuration
{
    public class RootComponent : IRootComponent
    {
        public string Id { get; set; }
        public List<IComponent> Instances { get; set; }
        public string Name { get; set; }
        public IComponent Parent { get => null; set => throw new NotImplementedException(); }

        public string StringProp { get; set; }
        public int IntProp { get; set; }

        //  TODO
        public IRootComponent DeepCopy()
        {
            throw new NotImplementedException();
        }
    }
}
