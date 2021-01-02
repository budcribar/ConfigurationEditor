using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PeakSWC.Configuration
{
    public interface IConfigurationSerializer : IComponentSerializer<IRootComponent> { }

    public interface IComponentSerializer<T> where T : class, IRootComponent {
        Task<IList<string>> ReadIds();
        Task<T> Read(string id);
        Task Insert(T component);
        Task Delete(string id);
        Task Update(string id, T component);
    }
}
