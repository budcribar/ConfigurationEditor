using PeakSWC.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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

        private PropertyNode? editModel = null;
        public PropertyNode? EditModel
        {
            get => editModel;
            set
            {
                SetValue(ref editModel, value);
            }
        }

        protected async virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == "SelectedId" && selectedId != null)
            {

                EditModel = null;
                SelectedRootComponent = await serializer.Read(selectedId);  // read based on id
                PropertyNodes = propertyIterator.Walk(SelectedRootComponent).ToList();
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void SetValue<T>(ref T backingFiled, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingFiled, value)) return;
            backingFiled = value;
            OnPropertyChanged(propertyName);
        }

        private PropertyIterator propertyIterator = new PropertyIterator();
        private IComponentSerializer<IRootComponent> serializer;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string PreviousValue { get; set; } = "";


        public List<Configuration.IComponent> Components { get; private set; } = new();

        public List<PropertyNode> PropertyNodes { get; private set; } = new();
        public ICollection<string> Identifiers { get; set; } = new List<string>();
        
        public List<ValidationResult> Errors { get; set; } = new();
        public Configuration.IComponent? SelectedComponent { get; set; } = null;
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

        public async Task Remove()
        {
            if (SelectedRootComponent == null)
                return;

            await serializer.Delete(SelectedRootComponent.Id);
            Identifiers = await serializer.ReadIds();
            EditModel = null;
            SelectedComponent = null;
            PropertyNodes = new();
            SelectedId = null;
            SelectedRootComponent = null;
        }
        public async void Change(object value, string name)
        {
            if (value == null)
            {
                EditModel = null;
                SelectedRootComponent = null;
                PropertyNodes = new();
            }
            else
            {
                EditModel = null;
                SelectedRootComponent = await serializer.Read((string)value);  // read based on id
                PropertyNodes = propertyIterator.Walk(SelectedRootComponent).ToList();
            }
            SelectedId = value as string;  
        }
        
        public async void Close(dynamic result)
        {
            if (SelectedComponent == null) return;
            if (SelectedRootComponent == null) return;
            if (result != null) return;
            if (selectedId == null) return;

            var x = EditModel?.Instance as IComponentComposite;

            if (x == null) return;

            x.Instances.Add(SelectedComponent);
            SelectedComponent.Parent = x;

            await serializer.Update(SelectedRootComponent.Id, SelectedRootComponent);
            SelectedRootComponent = await serializer.Read(selectedId);

            PropertyNodes = propertyIterator.Walk(SelectedRootComponent).ToList();
            EditModel = null;
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

            SelectedRootComponent = await serializer.Read(copy.Id);
            PropertyNodes = propertyIterator.Walk(SelectedRootComponent).ToList();
            selectedId = copy.Id;
        }

        public void InsertComponent(object value)
        {
            var x = value as Configuration.IComponent;
            if (EditModel == null) return;

            SelectedComponent = x;
        }


        public async Task OnInitializedAsync()
        {
            Components = ConfigurationComponent.GetAvailableComponents().ToList();

            Identifiers = await serializer.ReadIds();
            var id = Identifiers?.FirstOrDefault();
            if (id == null) return;

            if (Identifiers?.Contains(id) ?? false)
            {
                selectedId = id;
                SelectedRootComponent = await serializer.Read(selectedId);
                PropertyNodes = propertyIterator.Walk(SelectedRootComponent).ToList();
            }
        }
    }
}

