using PeakSWC.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeakSWC.ConfigurationEditor
{
    public interface IViewModel
    {
        public List<ValidationResult> Errors { get; set; }
        public Configuration.IComponent? SelectedComponent { get; set; }
        public void Duplicate();
        public void Cancel();
        public Task Remove();
        public Task Save();
        public void Close(dynamic result);
        public Task OnInitializedAsync();
        public void InsertComponent(object value);
        public PropertyNode? EditModel { get; set; }
        public List<PropertyNode> PropertyNodes { get; }
        public List<Configuration.IComponent> Components { get; }
        public ICollection<string> Identifiers { get; }
        public string? SelectedId { get; set; }
        public IRootComponent? SelectedRootComponent { get;  }
        public string PreviousValue { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
