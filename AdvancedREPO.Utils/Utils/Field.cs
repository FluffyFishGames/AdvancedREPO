using System;
using System.Reflection;
using System.Linq.Expressions;

namespace AdvancedREPO.Utils
{
    /// <summary>
    /// Aims to speed up setting and getting field values by using expressions.
    /// More elaborated ways to speeding up can be implemented later by using IL.Emit and ref values
    /// </summary>
    /// <typeparam name="C">The class this field is attached to</typeparam>
    /// <typeparam name="T">The type of the field</typeparam>
    public class Field<C, T>
    {
        private FieldInfo FieldInfo;
        public Field(FieldInfo f)
        {
            if (f.FieldType != typeof(T))
                throw new ArgumentException($"Type of field {f.DeclaringType.FullName}::{f.Name} is not matching with type of Field<{typeof(T).FullName}>");
            FieldInfo = f;
        }

        private Func<C, T> Getter;
        private Action<C, T> Setter;

        public T GetValue(C instance)
        {
            if (Getter == null)
            {
                var instanceExpression = Expression.Parameter(typeof(C));
                var fieldExpression = Expression.Field(instanceExpression, FieldInfo);
                Getter = Expression.Lambda<Func<C, T>>(fieldExpression, instanceExpression).Compile();
            }
            return Getter.Invoke(instance);
        }

        public void SetValue(C instance, T value)
        {
            if (Setter == null)
            {
                var instanceExpression = Expression.Parameter(typeof(C));
                var valueExpression = Expression.Parameter(typeof(T));
                var fieldExpression = Expression.Field(instanceExpression, FieldInfo);
                var assignExpression = Expression.Assign(fieldExpression, valueExpression);
                Setter = Expression.Lambda<Action<C, T>>(assignExpression, instanceExpression, valueExpression).Compile();
            }
            Setter.Invoke(instance, value);
        }
    }
}
