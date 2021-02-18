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
   
    public class JSONConfigurationSerializer : JSONComponentSerializer<IRootComponent>, IConfigurationSerializer
    {
        public JSONConfigurationSerializer() : base() { }
    }

    public class JSONComponentSerializer<TRoot> : IComponentSerializer<TRoot> where TRoot : class, IRootComponent
    {
        public const string FILENAME = "configuration.json";

        // TODO async and add error handling
        private readonly string path;
        public string Path => path ?? FILENAME;
        //public string Path => FILENAME;
        private IList<TRoot>? roots = null;

        // https://blog.stephencleary.com/2012/08/asynchronous-lazy-initialization.html


        public IList<TRoot> Roots
        {
            get
            {
                if (roots == null)
                {
                    if (File.Exists(Path))
                    {
                        try
                        {
                            roots = JsonConvert.DeserializeObject<List<TRoot>>(File.ReadAllText(Path), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }) ?? new List<TRoot>();
                        }
                        catch (Exception) { }

                        if (roots == null)
                            roots = new List<TRoot>();
                        
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

        public JSONComponentSerializer(string path=FILENAME) {         
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
