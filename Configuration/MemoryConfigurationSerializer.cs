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
   
    public class MemoryConfigurationSerializer : MemoryConfigurationSerializer<IRootComponent>, IConfigurationSerializer
    {
        public MemoryConfigurationSerializer() : base() { }
    }

    public class MemoryConfigurationSerializer<TRoot> : IComponentSerializer<TRoot> where TRoot : class, IRootComponent
    {
        // TODO
        public string data = "";
        private IList<TRoot>? roots = null;

        // https://blog.stephencleary.com/2012/08/asynchronous-lazy-initialization.html


        public IList<TRoot> Roots
        {
            get
            {
                if (roots == null)
                {
                    try
                    {
                        roots = JsonConvert.DeserializeObject<List<TRoot>>(data, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }) ?? new List<TRoot>();
                    }
                    catch (Exception) {
                        roots = new List<TRoot>();
                    }
                                  
                }

                return roots;
            }
            set
            {
                roots = value;
            }
        }

        public MemoryConfigurationSerializer() {         
            
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
            data = JsonConvert.SerializeObject(roots, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, PreserveReferencesHandling = PreserveReferencesHandling.All });
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
