using System.Collections.Generic;
using Raylib_cs;
using Vortex;

namespace Vortex.UI;

public class FontComponent : UIComponent
{
    public Font NormalFont;
    public Shader FontShader;


    public int FontSize = 16;
    public string Text = "";
    public Color FontColor = Color.Black;

    public override void Start()
    {
        base.Start();
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