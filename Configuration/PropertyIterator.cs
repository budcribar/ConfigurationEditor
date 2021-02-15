using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;


namespace PeakSWC.Configuration
{
    public class PropertyNode
    {
        private readonly string[] BaseTypes = new string[] { "UInt16", "String", "Byte", "Int32", "Guid", "Boolean" };
        private PropertyInfo Property { get; set; }
        public PropertyNode(object instance, string typeName, string name, PropertyInfo pi, List<PropertyNode> children, bool isEnumerable)
        {
            Instance = instance;
            TypeName = typeName;
            Name = name;
            Property = pi;
            Children = children;
            IsEnumerable = isEnumerable;
        }
       
        public object Instance { get; }
        public string TypeName { get; }
        public string Name { get; }
        public bool IsEnumerable { get; }
        public IEnumerable<PropertyNode> Children { get; }
        public PropertyNode? Parent { get; }
        public bool CanWrite { get { return Property.CanWrite; } }
        public bool IsClass
        {
            get { return !BaseTypes.Contains(TypeName); }
        }

        public ValidationResult Validate { get; private set; } = ValidationResult.Success; 

        public string StringValue { get { return Property.GetValue(Instance)?.ToString() ?? ""; } set {
                Validate = ValidationResult.Success;

                try
                {
                    if (TypeName == "UInt16")
                        Property.SetValue(Instance, UInt16.Parse(value));
                    else if (TypeName == "String")
                        Property.SetValue(Instance, value);
                    else if (TypeName == "Byte")
                        Property.SetValue(Instance, byte.Parse(value));
                    else if (TypeName == "Int32")
                        Property.SetValue(Instance, Int32.Parse(value));
                    else if (TypeName == "Guid")
                        Property.SetValue(Instance, Guid.Parse(value));
                    else if (TypeName == "Boolean")
                        Property.SetValue(Instance, Boolean.Parse(value));
                    else throw new Exception("Type " + TypeName + " not supported");
                }
                catch (Exception ex)
                {
                    Validate = new ValidationResult(ex.Message);
                }
               
            } }

        /// <summary>
        /// Converts a string into the specified type. If conversion was successful, parsed property will be of the correct type and method will return true.
        /// If conversion fails it will return false and parsed property will be null.
        /// This method supports the 8 data types that are valid navigation parameters in Blazor. Passing a string is also safe but will be returned as is because no conversion is neccessary.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="s"></param>
        /// <param name="result">The parsed object of the type specified. This will be null if conversion failed.</param>
        /// <returns>True if s was converted successfully, otherwise false</returns>
        public static bool TryParse(Type type, string s, out object? result)
        {
            bool success;

            if (type == typeof(string))
            {
                result = s;
                success = true;
            }
            else if (type == typeof(int))
            {
                success = int.TryParse(s, out var parsed);
                result = parsed;
            }
            else if (type == typeof(Guid))
            {
                success = Guid.TryParse(s, out var parsed);
                result = parsed;
            }
            else if (type == typeof(bool))
            {
                success = bool.TryParse(s, out var parsed);
                result = parsed;
            }
            else if (type == typeof(DateTime))
            {
                success = DateTime.TryParse(s, out var parsed);
                result = parsed;
            }
            else if (type == typeof(decimal))
            {
                success = decimal.TryParse(s, out var parsed);
                result = parsed;
            }
            else if (type == typeof(double))
            {
                success = double.TryParse(s, out var parsed);
                result = parsed;
            }
            else if (type == typeof(float))
            {
                success = float.TryParse(s, out var parsed);
                result = parsed;
            }
            else if (type == typeof(long))
            {
                success = long.TryParse(s, out var parsed);
                result = parsed;
            }
            else
            {
                result = null;
                success = false;
            }
            return success;
        }
        
        public object? Value
        {
            get { return Property?.GetValue(Instance); }
            set
            {
                
                    Property?.SetValue(Instance, value);
               
            }
        }


        
    }


    public class PropertyIterator
    {
       
        public IEnumerable<PropertyNode> Walk(object element)
        {
            foreach (var p in element.GetType().GetProperties().OrderBy(n => n.Name))
            {
                List<PropertyNode> children = new();
                bool isEnumerable = false;
                if (p.Name == "Instances")
                {
                    children = (p.GetValue(element) as List<IComponent>)?.Select(x => new PropertyNode(x, p.PropertyType.Name, x.Name, p, Walk(x).ToList(), isEnumerable)).ToList() ?? new List<PropertyNode>();
                    isEnumerable = true;
                }

                if (!p.CustomAttributes.Any(x => x.AttributeType.Name == "EditIgnoreAttribute"))
                    yield return new PropertyNode (element, p.PropertyType.Name, p.Name, p, children,isEnumerable );
            }
        }
    }
}
