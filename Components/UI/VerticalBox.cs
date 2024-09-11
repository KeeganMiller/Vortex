using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using Vortex;

namespace Vortex;

public class VerticalBox : UIComponent
{
    public UIComponent? OwningUI { get; private set; }

    protected float _prevWidth;
    protected float _prevHeight;

    public EContentAlignmentHorizontal HorizontalAlignment { get; set; }
    public EContentAlignmentVertical VerticalAlignment { get; set; }

    public float VerticalAlignmentOffset { get; set; }
    public float HorizontalAlignmentOffset { get; set; }
    public bool UseChildrenWidth { get; set; } = false;
    public bool UseChildrenHeight { get; set; } = false;

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
                if(UseChildrenWidth && uiComp.Width > width)
                    width = uiComp.Width;


                if(prevComp != null)
                {
                    // If there is a previous component calculate the position
                    // Using the element size and spacing, multiplied by the amount of UI components
                    uiComp.Offset = new Vector2
                    {
                        X = 0,
                        Y = height
                    };
                    // Apply the positinal offset
                    if(OwningUI != null)
                        uiComp.Offset += GetAlignmentOffset(OwningUI);
                    
                    uiComp.Offset += new Vector2(PaddingLeft, PaddingTop);
                    uiComp.SetOriginAndAnchor(uiComp.GetOriginLocation(), uiComp.GetAnchorLocation());

                } else 
                {
                    // If there is not previous component, set the position to the top
                    uiComp.Offset = new Vector2
                    {
                        X = 0,
                        Y = 0
                    };

                    if(OwningUI != null)
                        uiComp.Offset += GetAlignmentOffset(OwningUI);

                    uiComp.Offset += new Vector2(PaddingLeft, PaddingTop);
                    uiComp.SetOriginAndAnchor(uiComp.GetOriginLocation(), uiComp.GetAnchorLocation());
                }


                height += uiComp.Height + PaddingTop;

                prevComp = uiComp;
                uiCompCount += 1;
            }
        }

        if(width != _prevWidth)
        {
            this.Width = width;
            SetOrigin(Origin);
            SetAnchor(Anchor);
        }

        if(height != _prevHeight)
        {
            this.Height = height;
            SetOrigin(Origin);
            SetAnchor(Anchor);
        }

        _prevHeight = height;
        _prevWidth = width;
    }

    /// <summary>
    /// Determines the offset to apply to the offset when positioning
    /// </summary>
    /// <param name="alignmentReference"></param>
    /// <returns></returns>
    public Vector2 GetAlignmentOffset(UIComponent alignmentReference)
    {
        if(alignmentReference == null)
            return Vector2.Zero;
    
        var offset = Vector2.Zero;
        
        switch(VerticalAlignment)
        {
            case EContentAlignmentVertical.VALIGN_Top:
                offset = new Vector2(offset.X, OwnerTransform.Position.Y + VerticalAlignmentOffset);
                break;
            case EContentAlignmentVertical.VALIGN_Middle:
                offset = new Vector2(offset.X, OwnerTransform.Position.Y + alignmentReference.Height / 2 + VerticalAlignmentOffset);
                break;
            case EContentAlignmentVertical.VALIGN_Bottom:
                offset = new Vector2(offset.X, OwnerTransform.Position.Y + alignmentReference.Height + VerticalAlignmentOffset);
                break;
        }

        switch(HorizontalAlignment)
        {
            case EContentAlignmentHorizontal.HALIGN_Left:
                offset = new Vector2(OwnerTransform.Position.X + HorizontalAlignmentOffset, offset.Y);
                break;
            case EContentAlignmentHorizontal.HALIGN_Middle:
                offset = new Vector2(OwnerTransform.Position.X + alignmentReference.Width / 2 + HorizontalAlignmentOffset, offset.Y);
                break;
            case EContentAlignmentHorizontal.HALIGN_Right:
                offset = new Vector2(OwnerTransform.Position.X + alignmentReference.Width + HorizontalAlignmentOffset, offset.Y);
                break;
        }

        return offset;
    }

}