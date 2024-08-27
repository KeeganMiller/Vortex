using System.Collections.Generic;
using Raylib_cs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vortex;

public class ResourceManager
{
    public Scene Owner { get; }

    private List<Element> _elements = new List<Element>();
    private bool _hasElementToStart = false;

    private List<AssetData> _assets = new List<AssetData>();

    private string _sceneDataPath { get; }

    public ResourceManager(Scene owner, string sceneDataPath)
    {
        Owner = owner;
        _sceneDataPath = sceneDataPath;
    }

    public void Start()
    {
        LoadSceneResources();
    }

    public void Update(float dt)
    {
        foreach(var element in _elements)
        {
            if (element.HasStarted && element.IsActive)
                element.Update(dt);
        }

        if(_hasElementToStart)
        {
            foreach(var element in _elements)
            {
                if (!element.HasStarted && element.IsActive)
                    element.Start();
            }
        }
    }

    public void Draw()
    {
        foreach(var element in _elements)
            if(element.HasStarted && element.IsActive)
                element.Draw();
    }

    public void Stop()
    {
        foreach(var asset in _assets)
        {
            if (asset != null)
                asset.Unload();
        }
    }

    public bool AddElement(Element element)
    {
        if(element != null && !_elements.Contains(element)) 
        {
            _elements.Add(element);
            element.Initialize(this);
            _hasElementToStart = true;
            return true;
        }

        return false;
    }

    public bool RemoveElement(Element element)
    {
        if(element != null && _elements.Contains(element))
        {
            _elements.Remove(element);
            return true;
        }

        return false;
    }

    public T GetAsset<T>(string assetName) where T : AssetData
    {
        foreach(var asset in _assets)
        {
            if (asset.AssetName == assetName && asset is T assetAs)
                return assetAs;
        }

        return null;
    }

    public void LoadSceneResources()
    {
        if(File.Exists(_sceneDataPath))
        {
            using(var sr = new StreamReader(_sceneDataPath))
            {
                var jsonTokens = JArray.Parse(sr.ReadToEnd());
                foreach(var token in jsonTokens)
                {
                    var assetData = JsonConvert.DeserializeObject<AssetDataJson>(token.ToString());
                    if(assetData != null)
                    {
                        switch((EAssetType)assetData.AssetType)
                        {
                            case EAssetType.ASSET_Sprite:
                                assetData.AssetPath = Game.GetAssetPath() + assetData.AssetPath;
                                var sprite = new SpriteData(assetData);
                                sprite.Load();
                                if(sprite.IsValid)
                                {
                                    _assets.Add(sprite);
                                    Debug.Print($"Sprite: {sprite.AssetName} successfully loaded", EPrintMessageType.PRINT_Custom, ConsoleColor.Green);
                                } else 
                                {
                                    Debug.Print($"Sprite: {sprite.AssetName} failed to load", EPrintMessageType.PRINT_Error);
                                }
                                break;
                            default:
                                Debug.Print($"Could not parse asset: {assetData.AssetName} of type: {assetData.AssetType}", EPrintMessageType.PRINT_Warning);
                                break;
                        }
                    }
                }
            }
        }
    }
}