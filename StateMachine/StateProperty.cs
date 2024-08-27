using System.Numerics;

namespace Vortex;

public enum EStatePropertyType
{
    PROP_None,
    PROP_Float,
    PROP_Int,
    PROP_String,
    PROP_Vec,
    PROP_Bool
}

public class StateProperty 
{
    public string Key { get; protected set; }
    public EStatePropertyType PropertyType { get; protected set; }

    public StateProperty(string key, EStatePropertyType propType)
    {
        Key = key;
        PropertyType = propType;
    }

    public bool IsValueEqual<T>(StateProperty other)
    {
        if(other is StateValue<T> otherVal && this is StateValue<T> thVal)
        {
            return otherVal.Value.Equals(thVal.Value);
        }

        return false;
    }
}

public class StateValue<T> : StateProperty
{
    public T Value;

    public StateValue(string key, T value, EStatePropertyType propType = EStatePropertyType.PROP_None) : base(key, propType)
    {
        Value = value;
    }
}