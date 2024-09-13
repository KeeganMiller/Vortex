using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using Vortex;

namespace Vortex;

public class TextComponent : UIComponent
{
    protected FontAsset? _normalFont;
    protected FontAsset? _hoverFont;
    public FontAsset? NormalFont
    {
        get => _normalFont;
        set 
        {
            _normalFont = value;
            CalculateTextSize();
        }
    }

    public FontAsset? HoverFont
    {
        get => _hoverFont;
        set 
        {
            _hoverFont = value;
        }
    }
    protected Font _activeFont;
    
    protected ShaderAsset? _fontShader { get; set; }
    public ShaderAsset _fontShaderAsset 
    {
        get => _fontShader;
        set 
        {
            _fontShader = value;
            CalculateTextSize();
        }
    }

    protected int _fontSize = 16;
    public int FontSize
    {
        get => _fontSize;
        set 
        {
            _fontSize = value;
            CalculateTextSize();
            SetOriginAndAnchor(Origin, Anchor);
        }
    }

    protected string _text = "";
    public string Text 
    {
        get => _text;
        set 
        {
            _text = value;
            CalculateTextSize();
            SetOriginAndAnchor(Origin, Anchor);
        }
    }
    public Color FontColor { get; set; } = Color.Black;

    public override void Constructor(ResourceManager resources)
    {
        base.Constructor(resources);
        CalculateTextSize();

        if(_hoverFont != null && _hoverFont.IsValid)
        {
            OnMouseEnter += () => this._activeFont = _hoverFont!.LoadedFont;
            OnMouseExit += () => this._activeFont = _normalFont!.LoadedFont;
        }

        Owner.Transform.ScaleUpdateEvent += CalculateTextSize;
    }

    public override void Start()
    {
        base.Start();
        if(_normalFont != null)
            _activeFont = _normalFont.LoadedFont;

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

        var usingShader = false;

        if(_fontShader != null &&_fontShader.LoadedShader.Id > 0)
        {
            Raylib.BeginShaderMode(_fontShader.LoadedShader);
            usingShader = true;
        }

        if(_normalFont != null && _normalFont.LoadedFont.Texture.Id > 0)
            Raylib.DrawTextEx(_activeFont, Text, Owner.Transform.Position, FontSize, 1, FontColor);
        else
            Raylib.DrawText(Text, (int)Owner.Transform.Position.X, (int)Owner.Transform.Position.Y, FontSize, FontColor);

        if(usingShader)
            Raylib.EndShaderMode();
    }

    /// <summary>
    /// Manually recalculates the size of the size (width, height)
    /// </summary>
    public void CalculateTextSize()
    {
        if(NormalFont == null || !NormalFont.IsValid || Owner == null)
            return;

        var componentSize = Raylib.MeasureTextEx(NormalFont.LoadedFont, _text, FontSize, 1);
        Width = componentSize.X / Owner.Transform.Scale.X;
        Height = componentSize.Y / Owner.Transform.Scale.Y;
        SetOriginAndAnchor(Origin, Anchor);
    }

    public Font GetNormalFont()
    {
        return NormalFont != null && NormalFont.IsValid ? NormalFont.LoadedFont : new Font();
    }

    public Font GetHoverFont()
    {
        return HoverFont != null && HoverFont.IsValid ? HoverFont.LoadedFont : new Font();
    }
}