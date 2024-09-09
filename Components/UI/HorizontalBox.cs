using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace Vortex;

public enum EContentAlignmentHorizontal
{
    HALIGN_None = 0,
    HALIGN_Left = 1,
    HALIGN_Middle = 2,
    HALIGN_Right = 3
}

public enum EContentAlignmentVertical
{
    VALIGN_None = 0,
    VALIGN_Top = 1,
    VALIGN_Middle = 2,
    VALIGN_Bottom = 3
}

public class HorizontalBox : UIComponent
{
    public UIComponent? OwningUI { get; private set; }
    public string? OwningUiCompId { get; set; }

    protected float _prevWidth;
    protected float _prevHeight;

    public EContentAlignmentHorizontal HorizontalAlignment;
    public EContentAlignmentVertical VerticalAlignment;

    public int HorizontalAlignmentValue
    {
        get => (int)HorizontalAlignment;
        set => HorizontalAlignment = (EContentAlignmentHorizontal)value;
    }

    public int VerticalAlignmentValue
    {
        get => (int)VerticalAlignment;
        set => VerticalAlignment = (EContentAlignmentVertical)value;
    }

    public float VerticalAlignmentOffset { get; set; }
    public float HorizontalAlignmentOffset { get; set; }
    public bool UseChildrenWidth { get; set; } = false;
    public bool UseChildrenHeight { get; set; } = false;

    public override void Initialize(Element owner)
    {
        base.Initialize(owner);
        if(!string.IsNullOrEmpty(OwningUiCompId))
        {
            OwningUI = (UIComponent)Owner.GetComponent(OwningUiCompId);
        }
    }

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
                if(UseChildrenHeight && uiComp.Height > height)
                    height = uiComp.Height;

                if(prevComp != null)
                {
                    // If there is a previous component calculate the position
                    // Using the element size and spacing, multiplied by the amount of UI components
                    uiComp.Offset = new Vector2
                    {
                        X = width,
                        Y = 0
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

                width += uiComp.Width + PaddingLeft;

                prevComp = uiComp;
                uiCompCount += 1;
            }
        }

        if(width != _prevWidth)
        {
            this.Width = width;
            SetOrigin(_origin);
            SetAnchor(_anchor);
        }

        if(height != _prevHeight)
        {
            this.Height = height;
            SetOrigin(_origin);
            SetAnchor(_anchor);
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