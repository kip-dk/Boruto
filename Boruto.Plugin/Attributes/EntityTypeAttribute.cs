namespace Boruto.Attributes
{
    using System;
    /// <summary>
    /// For steps supporting multi entity types, decorate the method with one ore more logical names to be supported
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true)]
    class EntityTypeAttribute : Attribute
    {
        public EntityTypeAttribute(Type type)
        {
            if (type.IsSubclassOf(typeof(Microsoft.Xrm.Sdk.Entity)))
            {
                this.Type = type;
                var rootType = type;
                while (rootType.BaseType != typeof(Microsoft.Xrm.Sdk.Entity))
                {
                    rootType = rootType.BaseType;
                }
                this.LogicalName = ((Microsoft.Xrm.Sdk.Entity)System.Activator.CreateInstance(rootType)).LogicalName;
            }
            else
            {
                throw new Exceptions.TypeNotEntityType(type);
            }

        }

        public Type Type
        {
            get; private set;
        }

        public string LogicalName { get; private set; }
    }
}
