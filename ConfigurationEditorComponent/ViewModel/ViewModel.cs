using PeakSWC.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using IComponent = PeakSWC.Configuration.IComponent;

namespace PeakSWC.ConfigurationEditor
{
    public class ViewModel : IViewModel, INotifyPropertyChanged
    {
       
        private string? selectedId = null;
        public string? SelectedId
        {
            get => selectedId;
            set
            {
               
                SetValue(ref selectedId, value);
            }
        }
        public IComponent? selectedComponent = null;
        public IComponent? SelectedComponent
        {
            get => selectedComponent;
            set
            {

                SetValue(ref selectedComponent, value);
            }
        } 

        private PropertyNode? editModel = null;
        public PropertyNode? EditModel
        {
            get => editModel;
            set
            {
                SetValue(ref editModel, value);
            }
        }

        public IList<PropertyNode> ExpandedNodes { get; set; } = new List<PropertyNode>();

        private IEnumerable<PropertyNode> propertyNodes = new List<PropertyNode>();
        
        public IEnumerable<PropertyNode> PropertyNodes
        {
            get => propertyNodes;
            set
            {
                SetValue(ref propertyNodes, value);
            }
        }

        protected async virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName == "SelectedId" && selectedId != null)
            {

                EditModel = null;
                SelectedRootComponent = await serializer.Read(selectedId);  // read based on id
                PropertyNodes = propertyIterator.Walk(SelectedRootComponent).ToList();
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void SetValue<T>(ref T backingFiled, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingFiled, value)) return;
            backingFiled = value;
            OnPropertyChanged(propertyName);
        }

        private readonly PropertyIterator propertyIterator = new();
        private readonly IComponentSerializer<IRootComponent> serializer;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string PreviousValue { get; set; } = "";


        public List<Configuration.IComponent> Components { get; private set; } = new();

        



        public ICollection<string> Identifiers { get; set; } = new List<string>();
        
        public List<ValidationResult> Errors { get; set; } = new();
       
        public IRootComponent? SelectedRootComponent { get; private set; }
        
        
        public ViewModel(IComponentSerializer<IRootComponent> serializer)
        {
            this.serializer = serializer;
        }

        public void Cancel()
        {
            if (EditModel == null)
                return;
            EditModel.StringValue = PreviousValue;
            Errors = new();
        }

        public async Task RemoveFromList()
        {
            if (SelectedRootComponent == null)
                return;

            if (SelectedComponent == null)
                return;

            if (SelectedComponent.Parent is not IComponentComposite composite) return;
            composite.Instances.Remove(SelectedComponent);

            await serializer.Update(SelectedRootComponent.Id, SelectedRootComponent);

            EditModel = EditModel?.Parent;

            PropertyNodes = propertyIterator.Walk(SelectedRootComponent).ToList();
            SelectedComponent = SelectedComponent.Parent;          
        }

        public async Task Remove()
        {
            if (SelectedRootComponent == null)
                return;

            await serializer.Delete(SelectedRootComponent.Id);
            Identifiers = await serializer.ReadIds();
            EditModel = null;
            SelectedComponent = null;
            PropertyNodes = new List<PropertyNode>();
            SelectedId = null;
            SelectedRootComponent = null;
        }
        //public async void Change(object value, string name)
        //{
        //    if (value == null)
        //    {
        //        EditModel = null;
        //        SelectedRootComponent = null;
        //        PropertyNodes = new List<PropertyNode>();
        //    }
        //    else
        //    {
        //        EditModel = null;
        //        SelectedRootComponent = await serializer.Read((string)value);  // read based on id
        //        PropertyNodes = propertyIterator.Walk(SelectedRootComponent).ToList();
        //    }
        //    SelectedId = value as string;  
        //}
        
        public async void Close(dynamic result)
        {
            if (SelectedComponent == null) return;
            if (SelectedRootComponent == null) return;
            if (!Convert.ToBoolean(result)) return;
            if (selectedId == null) return;

            if (EditModel?.Instance is not IComponentComposite x) return;

            x.Instances.Add(SelectedComponent);
            SelectedComponent.Parent = x;

            await serializer.Update(SelectedRootComponent.Id, SelectedRootComponent);
            PropertyNodes = propertyIterator.Walk(SelectedRootComponent).ToList();

            EditModel = null;
            SelectedRootComponent = await serializer.Read(selectedId);
        }


        public async Task Save()
        {
            if (EditModel != null)
            {
                if (EditModel.Validate != ValidationResult.Success)
                {
                    Errors = new() { EditModel.Validate };
                    if (EditModel != null)
                        EditModel.StringValue = PreviousValue;
                    return;
                }
            }
                

            if (SelectedRootComponent?.Validate().Count > 0)
            {
                Errors = SelectedRootComponent.Validate();
                if (EditModel != null)
                    EditModel.StringValue = PreviousValue;
                return;
            }

            Errors = new();

            if (SelectedRootComponent != null)
                await serializer.Update(SelectedRootComponent.Id, SelectedRootComponent);
            Identifiers = await serializer.ReadIds();

            if (selectedId != null && !Identifiers.Contains(selectedId))
                selectedId = null;
        }

        public async void Duplicate()
        {
            if (SelectedRootComponent == null) return;
            var copy = SelectedRootComponent.DeepCopy();
            copy.Id = "";

            await serializer.Insert(copy);
            Identifiers = await serializer.ReadIds();

            SelectedId = copy.Id;
        }

        public void LinkComponent(object value)
        {
            var x = value as Configuration.IComponent;
            if (EditModel == null) return;

            SelectedComponent = x;
        }

        public void CopyComponent(object value)
        {
            var x = value as Configuration.IComponent;
            if (EditModel == null) return;

            SelectedComponent = x?.DeepCopy();
        }


        public async Task OnInitializedAsync()
        {
            Components = ConfigurationComponent.GetAvailableComponents().ToList();

            Identifiers = await serializer.ReadIds();
            var id = Identifiers?.FirstOrDefault();
            if (id == null) return;

            SelectedId = id;
        }
    }
}

