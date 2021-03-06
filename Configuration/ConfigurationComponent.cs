﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PeakSWC.Configuration
{
    public class ConfigurationComponent
    { 
        public static IEnumerable<IComponent> GetAvailableComponents()
        {
            var type = typeof(IComponent);
            var instances = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract && !p.IsGenericType).Where(c => c.GetConstructor(Type.EmptyTypes) != null).Select(x => Activator.CreateInstance(x) as IComponent).ToList();

            List<IComponent> results = instances?.Where(x => x != null).Select(e => e!).ToList() ?? new List<IComponent>();
            results.ForEach(x => x.Name = x.GetType().Name);

            return results;

        }
    }
}
