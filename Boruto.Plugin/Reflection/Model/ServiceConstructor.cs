using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration.Internal;

namespace Boruto.Reflection.Model
{
    internal class ServiceConstructor
    {
        private static Dictionary<Type, ServiceConstructor> typeIndex = new Dictionary<Type, ServiceConstructor>();
        private static object locker = new object();

        internal System.Reflection.ConstructorInfo Constructor { get; private set; }
        internal ServiceConstructorArgument[] Parameters { get; private set; }

        internal static ServiceConstructor ForType(Type type)
        {
            lock (locker)
            {
                if (typeIndex.TryGetValue(type, out ServiceConstructor s))
                {
                    return s;
                }
                typeIndex[type] = new ServiceConstructor(type);
                return typeIndex[type];
            }
        }
        
        private ServiceConstructor(Type type)
        {
            if (type.IsInterface)
            {
                throw new Exceptions.InvalidServiceTypeException(type);
            }
            if (type.IsAbstract)
            {
                throw new Exceptions.InvalidServiceTypeException(type);
            }

            var constructors = type.GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).ToArray();
            if (constructors.Length > 1)
            {
                // ignore the public empty constructor if there is more than one public constructor
                constructors = constructors.Where(r => (r.GetParameters() ?? new System.Reflection.ParameterInfo[0]).Length > 0).ToArray();
            }

            if (constructors.Length == 0)
            {
                throw new Exceptions.InvalidServiceTypeException($"{ type.FullName } does not have a public constructor");
            }

            if (constructors.Length > 1)
            {
                throw new Exceptions.InvalidServiceTypeException($"{type.FullName} has more than one public constructor, taking arguments");
            }

            this.Constructor = constructors[0];
            this.Parameters = (this.Constructor.GetParameters() ?? new System.Reflection.ParameterInfo[0]).Select(r => new ServiceConstructorArgument(r)).ToArray();
        }
    }
}
