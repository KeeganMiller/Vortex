using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using Vortex;

namespace Vortex.UI;

public class VerticalBox : UIComponent
{
    public float Spacing = 1f;                          // How much space is placed between the elements

    protected float _prevWidth;
    protected float _prevHeight;

    public override void Update(float dt)
    {
        base.Update(dt);

        int uiCompCount = 0;                        // Define a count of all the ui components
        UIComponent prevComp = null;                    // Store reference to the last ui component updated
        float width = 0;
        float height = 0;
        foreach(var element in Owner.GetChildren())
        {
            
            var uiComp = element.GetComponent<UIComponent>();                   // Get a ui component from the child element
            // Check the ui component is valid
            if(uiComp != null)
            {
                if(uiComp.Width > width)
                    width = uiComp.Width;

                height += uiComp.Height;

                if(prevComp != null)
                {
                    // If there is a previous component calculate the position
                    // Using the element size and spacing, multiplied by the amount of UI components
                    element.Transform.Position = new Vector2
                    {
                        X = OwnerTransform.Position.X,
                        Y = OwnerTransform.Position.Y + ((prevComp.Height + Spacing) * uiCompCount)
                    };
                } else 
                {
                    // If there is not previous component, set the position to the top
                    element.Transform.Position = new Vector2
                    {
                        X = OwnerTransform.Position.X,
                        Y = OwnerTransform.Position.Y
                    };
                }

                prevComp = uiComp;
                uiCompCount += 1;
            }
        }

        if(width != _prevWidth)
        {
            Width = width;
            _prevWidth = width;
            SetOrigin(Origin);
            SetAnchor(Anchor);
        }

        if(height != _prevHeight)
        {
            Height = height;
            _prevHeight = height;
            SetOrigin(Origin);
            SetAnchor(Anchor);
        }
    }

}