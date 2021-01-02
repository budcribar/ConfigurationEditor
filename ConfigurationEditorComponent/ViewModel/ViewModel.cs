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
        public PropertyNode? _editModel
        {
            get => editModel;
            set
            {
                SetValue(ref editModel, value);
            }
        }



        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {

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
       
        public string _savedValue {get;set;}


        public List<Configuration.IComponent> components { get; private set; } = new();

        public List<PropertyNode> nodes { get; private set; } = new();
        public IEnumerable<string> ids { get; set; } = new List<string>();
        
        



        public List<ValidationResult> Errors { get; set; } = new();
        public Configuration.IComponent? SelectedComponent { get; set; } = null;
        public IRootComponent? root { get; private set; }
        
        
        public ViewModel(IComponentSerializer<IRootComponent> serializer)
        {
            this.serializer = serializer;
        }

       

        public void Cancel()
        {
            _editModel.StringValue = _savedValue;
            Errors = new();
        }

        public Task Remove()
        {
            serializer.Delete(root.Id);
            _editModel = null;
            root = null;
            nodes = new();
            SelectedId = null;
            return Task.CompletedTask;
        }
        public async void Change(object value, string name)
        {
            

            if (value == null)
            {
                _editModel = null;
                root = null;
                nodes = new();
            }
            else
            {
                _editModel = null;
                root = await serializer.Read((string)value);
                nodes = propertyIterator.Walk(root).ToList();
            }
            SelectedId = value as string;  
        }
        public async void Close(dynamic result)
        {
            if (result != null && (bool)result && SelectedComponent != null)
            {
                var x = _editModel.Instance as IComponentComposite;

                if (x == null) return;

                x.Instances.Add(SelectedComponent);
                SelectedComponent.Parent = x;

               
                await serializer.Update(root.Id, root);
                root = await serializer.Read(selectedId);

                nodes = propertyIterator.Walk(root).ToList();
                _editModel = null;
            }
           
        }


        public async Task Save()
        {
            if (root.Validate().Count > 0)
            {
                Errors = root.Validate();
                _editModel.StringValue = _savedValue;
                return;
            }

            Errors = new();

            await serializer.Update(root.Id, root);
            ids = await serializer.ReadIds();

            if (!ids.Contains(selectedId))
                selectedId = null;

            //StateHasChanged();
        }

        public async void Duplicate()
        {
            var copy = root.DeepCopy();
            copy.Id = null;

            await serializer.Insert(copy);
           
            root = await serializer.Read(copy.Id);
            nodes = propertyIterator.Walk(root).ToList();
            selectedId = copy.Id;
            //OnInitialized();
        }

        public void InsertComponent(object value)
        {
            var x = value as Configuration.IComponent;
            if (_editModel == null) return;

            SelectedComponent = x;
        }


        public async Task OnInitializedAsync()
        {
            components = ConfigurationComponent.GetAvailableComponents().ToList();

            ids = await serializer.ReadIds();
            var id = ids?.FirstOrDefault();

            if (ids?.Contains(id) ?? false)
            {
                selectedId = id;
                root = await serializer.Read(selectedId);
                nodes = propertyIterator.Walk(root).ToList();
            }
        }
    }
}

