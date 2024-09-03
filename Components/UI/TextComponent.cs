using System.Collections.Generic;
using Raylib_cs;
using Vortex;

namespace Vortex.UI;

public class TextComponent : UIComponent
{
    public Font NormalFont;
    public Shader FontShader;

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
        }
    }
    public Color FontColor = Color.Black;

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
            Raylib.BeginShaderMode(FontShader);
            Raylib.DrawTextEx(NormalFont, Text, _ownerTransform.Position, FontSize, 1, FontColor);
            Raylib.EndShaderMode();
        } else 
        {
            Raylib.DrawTextEx(NormalFont, Text, _ownerTransform.Position, FontSize, 1, FontColor);
        }
    }
}