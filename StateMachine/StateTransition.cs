using System.Collections.Generic;
using System.Numerics;

namespace Vortex;

public class StateTransition
{
    public State NextState;
    public List<StateProperty> RequiredProperties;

    /// <summary>
    /// Checks if any properties are equal
    /// </summary>
    /// <param name="currentProps">Reference to the properties on the state machine</param>
    /// <returns>If we can transition to this state</returns>
    public bool CanTransition(List<StateProperty> currentProps)
    {
        var hasCheckedProperty = false;
        
        foreach(var reqProp in RequiredProperties)
        {
            foreach(var curProp in currentProps)
            {
                if(curProp.Key == reqProp.Key)
                {
                    hasCheckedProperty = true;
                    switch(curProp.PropertyType)
                    {
                        case EStatePropertyType.PROP_Float:
                            if(!curProp.IsValueEqual<float>(reqProp))
                                return false;
                            break;
                        case EStatePropertyType.PROP_Int:
                            if(!curProp.IsValueEqual<int>(reqProp))
                                return false;
                            break;
                        case EStatePropertyType.PROP_String:
                            if(!curProp.IsValueEqual<string>(reqProp))
                                return false;
                            break;
                        case EStatePropertyType.PROP_Vec:
                            if(!curProp.IsValueEqual<Vector2>(reqProp))
                                return false;
                            break;
                        case EStatePropertyType.PROP_Bool:
                            if(!curProp.IsValueEqual<bool>(reqProp))
                                return false;
                            break;
                    }
                }
            }
        }

        return hasCheckedProperty;
    }
}