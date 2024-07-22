namespace Boruto.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class IfAttribute : Attribute
    {
        public IfAttribute(Type type)
        {
            if (!typeof(IMethodCondition).IsAssignableFrom(type))
            {
                throw new Exceptions.InvalidTypeException(type, typeof(IMethodCondition));
            }
            this.Type = type;
        }

        public Type Type { get; private set; }
    }
}
