
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PeakSWC.Configuration
{
    public class BaseRootComponent : IRootComponent
    {
        public List<ValidationResult> Validate()
        {
            ValidationContext context = new(this, null, null);
            List<ValidationResult> validationResults = new();
            Validator.TryValidateObject(this, context, validationResults, true);
            return validationResults;
        }

        [EditIgnore]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public List<IComponent> Instances { get; set; } = new List<IComponent>();
        public string Name { get; set; } = "";
        public IComponent? Parent { get; set; } = null;

        //  TODO
        public IRootComponent DeepCopy()
        {
            return JsonConvert.DeserializeObject<IRootComponent>(JsonConvert.SerializeObject(this, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, PreserveReferencesHandling = PreserveReferencesHandling.All }), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }) ?? new BaseRootComponent();
        }
    }
}
