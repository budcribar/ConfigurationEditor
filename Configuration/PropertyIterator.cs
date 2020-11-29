using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


namespace PeakSWC.Configuration
{
    public class PropertyNode
    {
        public PropertyNode(object instance, string typeName, string name, PropertyInfo pi, List<PropertyNode> children)
        {
            Instance = instance;
            TypeName = typeName;
            Name = name;
            Property = pi;
            Children = children;
        }

        public object Instance { get; }
        public string TypeName { get; }
        public string Name { get; }
        public List<PropertyNode> Children { get; }
        public PropertyNode? Parent { get; }
        public bool CanWrite { get { return Property.CanWrite; } }
        public string StringValue { get { return Property?.GetValue(Instance)?.ToString() ?? ""; } set {
                if (TypeName == "UInt16")
                    Property?.SetValue(Instance, UInt16.Parse(value));
                else if (TypeName == "String")
                    Property?.SetValue(Instance, value);
                else if (TypeName == "Byte")
                    Property?.SetValue(Instance, byte.Parse(value));
                else if (TypeName == "Int32")
                    Property?.SetValue(Instance, Int32.Parse(value));
                else if (TypeName == "Guid")
                    Property?.SetValue(Instance, Guid.Parse(value));
                else if (TypeName == "Boolean")
                    Property?.SetValue(Instance, Boolean.Parse(value));
                else throw new Exception("Type " + TypeName + " not supported");
            } }


        public object? Value
        {
            get { return Property?.GetValue(Instance); }
            set
            {
                
                    Property?.SetValue(Instance, value);
               
            }
        }


        private PropertyInfo Property { get; set; }
    }


    public class PropertyIterator
    {
       
        public IEnumerable<PropertyNode> Walk(object element)
        {
            foreach (var p in element.GetType().GetProperties().OrderBy(n => n.Name))
            {
                List<PropertyNode> children = new List<PropertyNode>();
                if (p.Name == "Instances")
                    children = (p.GetValue(element) as List<IComponent>)?.Select(x => new PropertyNode(x, p.PropertyType.Name, x.Name, p, Walk(x).ToList())).ToList() ?? new List<PropertyNode>();

                if (!p.CustomAttributes.Any(x => x.AttributeType.Name == "EditIgnoreAttribute"))

                    yield return new PropertyNode (element, p.PropertyType.Name, p.Name, p, children );
            }
        }
    }
}
