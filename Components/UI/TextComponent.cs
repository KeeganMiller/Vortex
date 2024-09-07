using System.Collections.Generic;
using Raylib_cs;
using Vortex;

namespace Vortex;

public class TextComponent : UIComponent
{
    public Font NormalFont;
    public Shader FontShader;
    public string FontId { get; set; } = "";
    public string ShaderId { get; set; } = "";

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
    public Color FontColor = Color.Black;

    public override void Initialize(Element owner)
    {
        base.Initialize(owner);
        // Get and assign the shader
        if(!string.IsNullOrEmpty(ShaderId))
        {
            var shaderAsset = SceneManager.GlobalResources.GetAssetById<ShaderAsset>(ShaderId);
            if(shaderAsset != null && shaderAsset.IsValid)
                FontShader = shaderAsset.LoadedShader;
        }

        // Get and assign the font
        if(!string.IsNullOrEmpty(FontId))
        {
            var fontAsset = SceneManager.GlobalResources.GetAssetById<FontAsset>(FontId);
            if(fontAsset != null && fontAsset.IsValid)
                NormalFont = fontAsset.LoadedFont;
        }
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update(float dt)
    {
        base.Update(dt);
    }

    public override void Draw()
    {
        if(FontShader.Id > 0)
        {
            if(FontShader.Id > 0)
                Raylib.BeginShaderMode(FontShader);

            Raylib.DrawTextEx(NormalFont, Text, OwnerTransform.Position, FontSize, 1, FontColor);

            if(FontShader.Id > 0)
                Raylib.EndShaderMode();
        } else 
        {
            Raylib.DrawTextEx(NormalFont, Text, OwnerTransform.Position, FontSize, 1, FontColor);
        }
    }
}