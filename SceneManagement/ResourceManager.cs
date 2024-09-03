using System.Collections.Generic;
using Raylib_cs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using Microsoft.VisualBasic;
using Vortex.UI;

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
        foreach(var element in _elements.ToList())
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

    public Element GetElement(string elementName)
    {
        foreach(var element in _elements)
            if(element.Name == elementName)
                return element;

        return null;
    }

    public List<SpriteComponent> GetCameraRelativeSprites()
    {
        var sprites = new List<SpriteComponent>();
        foreach(var e in _elements)
        {
            if(e.IsCameraRelated)
            {
                var spriteComp = e.GetComponent<SpriteComponent>();
                if(spriteComp != null)
                    sprites.Add(spriteComp);
            }
        }

        return sprites;
    }

    public List<SpriteComponent> GetSprites()
    {
        var sprites = new List<SpriteComponent>();
        foreach(var e in _elements)
        {
            if(!e.IsCameraRelated)
            {
                var spritecomp = e.GetComponent<SpriteComponent>();
                if(spritecomp != null)
                    sprites.Add(spritecomp);
            }
        }

        return sprites;
    }

    public List<UIComponent> GetUiComponents()
    {
        var ui = new List<UIComponent>();
        foreach(var e in _elements)
        {
            var uiComp = e.GetComponent<UIComponent>();
            if(uiComp != null)
                ui.Add(uiComp);
        }

        return ui;
    }

    public void DrawElements()
    {
        foreach(var e in _elements)
        {
            if(e.IsActive && e.HasStarted && !e.IsCameraRelated)
                e.Draw();
        }
    }

    public void DrawElementsRelative()
    {
        foreach(var e in _elements)
        {
            if(e.IsActive && e.HasStarted && e.IsCameraRelated)
                e.Draw();
        }
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
                            case EAssetType.ASSET_Font:
                                assetData.AssetPath = Game.GetAssetPath() + assetData.AssetPath;
                                var font = new FontAsset(assetData);
                                font.Load();
                                if(font.IsValid)
                                {
                                    _assets.Add(font);
                                    Debug.Print($"Font: {font.AssetName} successfully loaded", EPrintMessageType.PRINT_Custom, ConsoleColor.Green);
                                } else 
                                {
                                    Debug.Print($"Font: {font.AssetName} failed to load", EPrintMessageType.PRINT_Error);
                                }
                                break;
                            case EAssetType.ASSET_Shader:
                                assetData.AssetPath = Game.GetAssetPath() + assetData.AssetPath;
                                var shader = new ShaderAsset(assetData);
                                shader.Load();
                                if(shader.IsValid)
                                {
                                    _assets.Add(shader);
                                    Debug.Print($"Shader: {shader.AssetName} sucessfully loaded", EPrintMessageType.PRINT_Custom, ConsoleColor.Green);
                                } else 
                                {
                                    Debug.Print($"Shader: {shader.AssetName} failed to load", EPrintMessageType.PRINT_Error);
                                }
                                break;
                            default:
                                Debug.Print($"Could not parse asset: {assetData.AssetName} of type: {assetData.AssetType}", EPrintMessageType.PRINT_Warning);
                                break;
                        }
                    }
                }
            }
        } else 
        {
            Debug.Print($"ResourceManager::LoadSceneResources -> Failed to find scene file: {this._sceneDataPath}", EPrintMessageType.PRINT_Error);
        }
    }
}