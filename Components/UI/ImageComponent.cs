using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;

namespace Vortex;

public class ImageComponent : UIComponent
{
    public Texture2D NormalImage;
    public Texture2D HoverImage;

    private int _normalImageId = -1;
    private int _hoverImageId = -1;

    public int NormalImageId 
    {
        get => _normalImageId;
        set 
        {
            var texture = SceneManager.GlobalResources.GetAssetById<SpriteData>(value);
            if(texture != null && texture.IsValid)
            {
                _normalImageId = value;
                NormalImage = texture.Texture;
            }
        }
    }

    public int HoverImageId
    {
        get => _normalImageId;
        set 
        {
            var texture = SceneManager.GlobalResources.GetAssetById<SpriteData>(value);
            if(texture != null && texture.IsValid)
            {
                HoverImage = texture.Texture;
                _hoverImageId = value;
            }
        }
    }

    public Texture2D ActiveImage { get; private set; }
    public Color Tint = Color.White;
    public Vector2 ImageRotationOrigin = Vector2.Zero;

    public bool IsClickable = false;

    public Action OnClick;

    public override void Initialize(Element owner)
    {
        base.Initialize(owner);
        if(NormalImageId > -1)
        {
            var asset = Owner.Owner.GetAssetById<SpriteData>(NormalImageId);
            if(asset != null && asset.IsValid)
                NormalImage = asset.Texture;
        }

        if(HoverImageId > -1)
        {
            var asset = Owner.Owner.GetAssetById<SpriteData>(HoverImageId);
            if(asset != null && asset.IsValid)
                HoverImage = asset.Texture;
        }
    }

    public override void Start()
    {
        base.Start();
        OnMouseEnter += () => {
            if(HoverImage.Id > 0)
                SetActiveImage(HoverImage);
        };
        OnMouseExit += () => {
            if(NormalImage.Id > 0)
                SetActiveImage(NormalImage);
        };


        OwnerTransform.ScaleUpdateEvent += UpdateImageSize;
        SetActiveImage(NormalImage);

        // Enable the cursor properties
        if(IsClickable)
        {
            OnMouseEnter += () => { Raylib.SetMouseCursor(MouseCursor.PointingHand); };
            OnMouseExit += () => { Raylib.SetMouseCursor(MouseCursor.Default); };
        }
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        if(IsClickable && IsMouseOver)
        {
            if(Input.IsMouseButtonClicked(EMouseButton.MOUSE_Left))
                OnClick?.Invoke();
        }
    }

    /// <summary>
    /// Handles setting the size of the image
    /// </summary>
    private void UpdateImageSize()
    {
        this.Width = ActiveImage.Width * OwnerTransform.Scale.X;
        this.Height = ActiveImage.Height * OwnerTransform.Scale.Y;
    }

    public override void Draw()
    {
        base.Draw();

        // Make sure the image is valid
        if(ActiveImage.Id > 0)
        {
            var source = new Rectangle(0.0f, 0.0f, ActiveImage.Width, ActiveImage.Height);              // Create the source rect
            var dest = new Rectangle(Owner.Transform.Position, new Vector2(Width, Height));                 // Create the destination rect
            // Draw Texture
            Raylib.DrawTexturePro(ActiveImage, source, dest, ImageRotationOrigin, Owner.Transform.Rotation, Tint);
        }
    }

    private void SetActiveImage(Texture2D image)
    {
        ActiveImage = image;
    }
}
