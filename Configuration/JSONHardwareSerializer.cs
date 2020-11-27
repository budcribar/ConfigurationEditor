using System;
using System.Collections.Generic;
using System.Text;
//using System.Text.Json;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PeakSWC.Configuration
{
   
    public class JSONConfigurationSerializer : JSONHardwareSerializer<IRootComponent>, IConfigurationSerializer
    {
        public JSONConfigurationSerializer() : base(null) { }
    }

    public class JSONHardwareSerializer<TRoot> : IComponentSerializer<TRoot> where TRoot : class, IRootComponent
    {
        public const string FILENAME = "configuration.json";

        // TODO async and add error handling
        private string path;
        private string Path => path ?? FILENAME;
        private IList<TRoot> roots = null;
        private IList<TRoot> proxiedRoots = null;

        // https://blog.stephencleary.com/2012/08/asynchronous-lazy-initialization.html


        public IList<TRoot> RootsProxy
        {
            get
            {
               
                return Roots;
            }
        }

        
        public IList<TRoot> Roots
        {
            get
            {
                if (roots == null)
                {
                    if (File.Exists(Path))
                    {
                        roots = JsonConvert.DeserializeObject<List<TRoot>>(File.ReadAllText(Path), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
                    }
                    else roots = new List<TRoot>();
                    
                }

                return roots;
            }
            set
            {
                roots = value;
            }
        }

        public JSONHardwareSerializer(string path=FILENAME) {         
            this.path = path;      
        }

        public Task Insert(TRoot root) {
            root.Id = Guid.NewGuid().ToString();
            Roots.Add(root);
            return Write();
        }

        public Task Delete(string id) { 
            Roots.Remove(Roots.FirstOrDefault(x => x.Id == id));
            return Write();
        }

       
        public Task Update(string id, TRoot fans)
        {
            var fansIndex = Roots.IndexOf(Roots.FirstOrDefault(x => x.Id == id));
            Roots[fansIndex] = fans;
            return Write();
        }
       
        public Task Write()
        {
            proxiedRoots = null;
            var json = JsonConvert.SerializeObject(roots, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, PreserveReferencesHandling = PreserveReferencesHandling.All });
            
            File.WriteAllText(Path, json);
            return Task.CompletedTask;
        }

        public Task<TRoot> Read(string id)
        {
            return Task.Run(() => Roots.FirstOrDefault(x => x.Id == id));
        }
        public Task<IList<string>> ReadIds()
        {
            return Task.Run(() => (IList<string>)Roots.Select(x => x.Id).ToList());
        }
    }
}
