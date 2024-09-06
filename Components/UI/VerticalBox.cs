using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using Vortex;

namespace Vortex;

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

                height += uiComp.Height + Spacing;

                if(prevComp != null)
                {
                    // If there is a previous component calculate the position
                    // Using the element size and spacing, multiplied by the amount of UI components
                    uiComp.Offset = new Vector2
                    {
                        X = 0,
                        Y = (prevComp.Height + Spacing) * uiCompCount
                    };
                    uiComp.SetOriginAndAnchor(uiComp.GetOriginLocation(), uiComp.GetAnchorLocation());

                } else 
                {
                    // If there is not previous component, set the position to the top
                    uiComp.Offset = new Vector2
                    {
                        X = 0,
                        Y = 0
                    };
                    uiComp.SetOriginAndAnchor(uiComp.GetOriginLocation(), uiComp.GetAnchorLocation());
                }

                prevComp = uiComp;
                uiCompCount += 1;
            }
        }

        if(width != _prevWidth)
        {
            Width = width;
            _prevWidth = width;
            SetOrigin(_origin);
            SetAnchor(_anchor);
        }

        if(height != _prevHeight)
        {
            Height = height;
            _prevHeight = height;
            SetOrigin(_origin);
            SetAnchor(_anchor);
        }
    }

}