using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PeakSWC.Configuration
{
    [AttributeUsage(AttributeTargets.All, Inherited = true)]
    public class EditIgnoreAttribute : Attribute
    {
        public EditIgnoreAttribute() { }
    }
    public interface IRootComponent : IComponentComposite
    {
        IRootComponent DeepCopy();

        [EditIgnore]
        [JsonProperty(PropertyName = "id")] // Must be lower case for database
        string Id { get; set; }
    }

    public interface IComponent
    {
        string Name { get; set; }
        [EditIgnore]
        public IComponent? Parent { get; set; }
    }

    public interface IComponentComposite : IComponent
    {
        List<IComponent> Instances { get; set; }
    }
}
