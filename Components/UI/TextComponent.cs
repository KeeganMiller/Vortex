using System.Collections.Generic;
using Raylib_cs;
using Vortex;

namespace Vortex;

public class TextComponent : UIComponent
{
    public Font NormalFont;
    public Font HoverFont;
    private Font _activeFont;
    public Shader FontShader;
    private string NormalFontId { get; set; } = "";
    private string HoverFontId { get; set; } = "";
    private string ShaderId { get; set; } = "";

    private int _fontSize = 16;
    public int FontSize
    {
        get => _fontSize;
        set 
        {
            _fontSize = value;
            var componentSize = Raylib.MeasureTextEx(NormalFont, Text, FontSize, 1);
            Width = componentSize.X;
            Height = componentSize.Y;
            SetOriginAndAnchor(_origin, _anchor);
        }
    }

    private string _text = "";
    public string Text 
    {
        get => _text;
        set 
        {
            _text = value;
            var componentSize = Raylib.MeasureTextEx(NormalFont, value, FontSize, 1);
            Width = componentSize.X;
            Height = componentSize.Y;
            SetOriginAndAnchor(_origin, _anchor);
        }
    }
    public Color FontColor { get; set; } = Color.Black;

    public override void Constructor(ResourceManager resources)
    {
        base.Constructor(resources);
        // Get and assign the shader
        if(!string.IsNullOrEmpty(ShaderId))
        {
            var shaderAsset = SceneManager.GlobalResources.GetAssetById<ShaderAsset>(ShaderId);
            if(shaderAsset != null && shaderAsset.IsValid)
                FontShader = shaderAsset.LoadedShader;
        }

        // Get and assign the font
        if(!string.IsNullOrEmpty(NormalFontId))
        {
            var fontAsset = SceneManager.GlobalResources.GetAssetById<FontAsset>(NormalFontId);
            if(fontAsset != null)
            {
                NormalFont = fontAsset.LoadedFont;
                _activeFont = NormalFont;
            }
        }

        if(!string.IsNullOrEmpty(HoverFontId))
        {
            var fontAsset = SceneManager.GlobalResources.GetAssetById<FontAsset>(HoverFontId);
            if(fontAsset != null)
            {
                HoverFont = fontAsset.LoadedFont;
                OnMouseEnter += () => _activeFont = HoverFont;
                OnMouseExit += () => _activeFont = NormalFont;
            }
        }

        CalculateTextSize();
    }

    public override void Start()
    {
        base.Start();
        _activeFont = NormalFont;
    }

    public override void Update(float dt)
    {
        base.Update(dt);
    }

    public override void Draw()
    {
        base.Draw();
        
        if(Owner.Transform == null)
            return;

        if(FontShader.Id > 0)
            Raylib.BeginShaderMode(FontShader);

        if(NormalFont.Texture.Id > 0)
            Raylib.DrawTextEx(_activeFont, Text, Owner.Transform.Position, FontSize, 1, FontColor);
        else
            Raylib.DrawTextEx(new Font(), Text, Owner.Transform.Position, FontSize, 1, FontColor);

        if(FontShader.Id > 0)
            Raylib.EndShaderMode();
    }

    /// <summary>
    /// Manually recalculates the size of the size (width, height)
    /// </summary>
    public void CalculateTextSize()
    {
        var componentSize = Raylib.MeasureTextEx(NormalFont, _text, FontSize, 1);
        Width = componentSize.X;
        Height = componentSize.Y;
        SetOriginAndAnchor(_origin, _anchor);
    }
}