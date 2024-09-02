﻿using System.Collections.Generic;
using Raylib_cs;
using Newtonsoft.Json;

namespace Vortex;

public enum EAssetType
{
    ASSET_Sprite = 0,
    ASSET_Font = 1,
    ASSET_Shader = 2
}

public abstract class AssetData
{
    public string AssetName;
    public string AssetPath;
    public EAssetType AssetType;

    public AssetData(AssetDataJson data)
    {
        this.AssetName = data.AssetName;
        this.AssetPath = data.AssetPath;
        this.AssetType = (EAssetType)data.AssetType;
    }

    public abstract void Load();
    public abstract void Unload();
}

public class AssetDataJson
{
    [JsonProperty] public string AssetName;
    [JsonProperty] public string AssetPath;
    [JsonProperty] public int AssetType;
}

public class SpriteData : AssetData
{
    public Texture2D Texture { get; private set; }
    public bool IsValid => Texture.Id > 0;

    public SpriteData(AssetDataJson data) : base(data)
    {
    }

    public override void Load()
    {
        if(File.Exists(AssetPath))
        {
            var tex = Raylib.LoadTexture(AssetPath);
            if(tex.Id > 0)
            {
                Texture = tex;
            } else
            {
                Debug.Print($"Failed to load asset: {this.AssetName} at path: {this.AssetPath}", EPrintMessageType.PRINT_Error);
            }
        }
        else
        {
            Debug.Print($"Failed to find file at location: {this.AssetPath}", EPrintMessageType.PRINT_Error);
        }
    }

    public override void Unload()
    {
        if (IsValid)
            Raylib.UnloadTexture(Texture);
    }
}

public class FontAsset : AssetData
{
    public Font LoadedFont { get; private set; }
    public bool IsValid => LoadedFont.Texture.Id > 0;

    public FontAsset(AssetDataJson data) : base(data)
    {
    }

    public override void Load()
    {
        unsafe 
        { 
            var fileSize = 0;
            var fileData = Raylib.LoadFileData(AssetPath, ref fileSize); 
            Font fontDefault = new Font
            {
                BaseSize = 16,
                GlyphCount = 95,
                Glyphs = Raylib.LoadFontData(fileData, (int)fileSize, 16, null, 95, FontType.Default)
            };

            Image atlas = Raylib.GenImageFontAtlas(fontDefault.Glyphs, &fontDefault.Recs, 95, 16, 4, 0);
            fontDefault.Texture = Raylib.LoadTextureFromImage(atlas);
            Raylib.UnloadImage(atlas);
            Raylib.SetTextureFilter(fontDefault.Texture, TextureFilter.Bilinear);
            LoadedFont = fontDefault;
        }
    }

    public override void Unload()
    {
        if(LoadedFont.Texture.Id > 0)
            Raylib.UnloadTexture(LoadedFont.Texture);
    }
}

public class ShaderAsset : AssetData
{
    public Shader LoadedShader {get; private set;}
    public bool IsValid => LoadedShader.Id > 0; 
    public ShaderAsset(AssetDataJson data) : base(data)
    {
    }

    public override void Load()
    {
        LoadedShader = Raylib.LoadShader(null, AssetPath);
    }

    public override void Unload()
    {
        if(LoadedShader.Id > 0)
            Raylib.UnloadShader(LoadedShader);
    }
}