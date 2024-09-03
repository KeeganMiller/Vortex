using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using Vortex;

namespace Vortex.UI;

public class VerticalBox : UIComponent
{
    public float Spacing = 1f;                          // How much space is placed between the elements

    public override void Update(float dt)
    {
        base.Update(dt);

        int uiCompCount = 0;                        // Define a count of all the ui components
        UIComponent prevComp = null;                    // Store reference to the last ui component updated
        foreach(var element in Owner.GetChildren())
        {
            var uiComp = element.GetComponent<UIComponent>();                   // Get a ui component from the child element
            // Check the ui component is valid
            if(uiComp != null)
            {
                if(prevComp != null)
                {
                    // If there is a previous component calculate the position
                    // Using the element size and spacing, multiplied by the amount of UI components
                    element.Transform.Position = new Vector2
                    {
                        X = _ownerTransform.Position.X,
                        Y = _ownerTransform.Position.Y * ((prevComp.Height + Spacing) * uiCompCount)
                    };
                } else 
                {
                    // If there is not previous component, set the position to the top
                    element.Transform.Position = new Vector2
                    {
                        X = _ownerTransform.Position.X,
                        Y = _ownerTransform.Position.Y + Spacing
                    };
                }

                prevComp = uiComp;
                uiCompCount += 1;
            }
        }
    }
}