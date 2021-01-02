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
        public void Change(object value, string name);
        public void Cancel();
        public Task Remove();
        public Task Save();
        public void Close(dynamic result);
        public Task OnInitializedAsync();
        public void InsertComponent(object value);
        public PropertyNode? _editModel { get; set; }
        public List<PropertyNode> nodes { get; }
        public List<Configuration.IComponent> components { get; }
        public IEnumerable<string> ids { get; }
        public string? SelectedId { get; set; }
        public IRootComponent? root { get;  }
        public string _savedValue { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
