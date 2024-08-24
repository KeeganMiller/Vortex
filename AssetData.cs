using System.Collections.Generic;
using Raylib_cs;
using Newtonsoft.Json;

namespace Vortex;

public enum EAssetType
{
    ASSET_Sprite
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