using System.Collections.Generic;
using Raylib_cs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using Microsoft.VisualBasic;
namespace Vortex;

public class ResourceManager
{
    public Scene Owner { get; }                         // Reference to the scene that owns this resource manager

    private List<Element> _elements = new List<Element>();                      // List of all the elements
    private bool _hasElementToStart = false;

    private List<AssetData> _assets = new List<AssetData>();                        // List of all the loaded assets for this manager

    private string _sceneDataPath { get; }                          // Where the scene file data is stored

    public ResourceManager(Scene owner, string sceneDataPath)
    {
        Owner = owner;
        _sceneDataPath = sceneDataPath;
    }

    public void Start()
    {
        VortexSceneReader.ParseFile(_sceneDataPath, this);
    }

    public void Update(float dt)
    {
        // Update all the elements
        foreach(var element in _elements.ToList())
        {
            if (element.HasStarted && element.IsActive)
                element.Update(dt);
        }

        // Start any elements that require it
        foreach(var element in _elements)
        {
            if (!element.HasStarted && element.IsActive)
                element.Start();
        }
    }

    /// <summary>
    /// Finds the element with the specified name
    /// </summary>
    /// <param name="elementName">Name of the element to find</param>
    /// <returns>Reference to the element found</returns>
    public Element GetElement(string elementName)
    {
        foreach(var element in _elements)
            if(element.Name == elementName)
                return element;

        return null;
    }

    /// <summary>
    /// Finds the element in this manager with the id passed in
    /// </summary>
    /// <param name="id">Id of the element to find</param>
    /// <returns>Reference to that element</returns>
    public Element? GetElementById(string id)
    {
        foreach(var element in _elements)
        {
            if(element.ObjectId == id)
                return element;
        }

        return null;
    }

    /// <summary>
    /// Gets all of the sprites that are related to the camera
    /// </summary>
    /// <returns>List of camera relative sprites</returns>
    public List<SpriteComponent> GetCameraRelativeSprites()
    {
        var sprites = new List<SpriteComponent>();                  // Pre-define list
        // Loop through each element
        foreach(var e in _elements.ToList())
        {
            // Check if the element is Camera relative
            if(e.IsCameraRelated)
            {
                var spriteComp = e.GetComponent<SpriteComponent>();                 // Get the sprite component from the element (if it exist)
                // Check that the sprite component is valid
                // and add it to the list of sprite components
                if(spriteComp != null)
                    sprites.Add(spriteComp);
            }
        }

        // Return the list of sprites
        return sprites;
    }

    /// <summary>
    /// Gets a list of all the sprite components that aren't camera relative
    /// </summary>
    /// <returns>List of non camera relative sprites</returns>
    public List<SpriteComponent> GetSprites()
    {
        var sprites = new List<SpriteComponent>();                  // Pre-define the list of sprites
        //  Loop through each element
        foreach(var e in _elements.ToList())
        {
            // Check that the element isn't camera relative
            if(!e.IsCameraRelated)
            {
                // Get the sprite component on the element
                var spritecomp = e.GetComponent<SpriteComponent>();
                // Check that the sprite component exist
                // and add it to the list of sprites
                if(spritecomp != null)
                    sprites.Add(spritecomp);
            }
        }

        // Return the list of non camera relative sprites
        return sprites;
    }

    /// <summary>
    /// Gets a list of all the UI Components in this manager
    /// </summary>
    /// <returns>List of UI components</returns>
    public List<UIComponent> GetUiComponents()
    {
        var ui = new List<UIComponent>();               // Define the list
        // Loop through each of the elements
        foreach(var e in _elements)
        {
            // Get the UI Component
            var uiComp = e.GetComponent<UIComponent>();
            // If the ui component is valid add it to the list
            if(uiComp != null)
                ui.Add(uiComp);
        }

        // Return the list of UI Components
        return ui;
    }

    /// <summary>
    /// Handles calling the draw method on all the elements
    /// so that we can draw other elements/components
    /// </summary>
    public void DrawElements()
    {
        foreach(var e in _elements.ToList())
        {
            if(e.IsActive && e.HasStarted && !e.IsCameraRelated)
                e.Draw();
        }
    }

    /// <summary>
    /// Handles calling the draw method on all the elements that are camera relative
    /// </summary>
    public void DrawElementsRelative()
    {
        foreach(var e in _elements.ToList())
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

    /// <summary>
    /// Adds a new element to this manager
    /// </summary>
    /// <param name="element">Element to add</param>
    /// <returns>If the element was added</returns>
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

    /// <summary>
    /// Removes an element from this manager
    /// </summary>
    /// <param name="element">Element to remove</param>
    /// <returns>If the element was removed</returns>
    public bool RemoveElement(Element element)
    {
        if(element != null && _elements.Contains(element))
        {
            _elements.Remove(element);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the assets based on the asset name
    /// and handles casting it to the required type
    /// </summary>
    /// <typeparam name="T">Required type (AssetData Parent)</typeparam>
    /// <param name="assetName">Name of the asset to find</param>
    /// <returns>Found asset</returns>
    public T GetAsset<T>(string assetName) where T : AssetData
    {
        foreach(var asset in _assets)
        {
            if (asset.AssetName == assetName && asset is T assetAs)
                return assetAs;
        }

        return null;
    }

    public T GetAssetById<T>(int id) where T : AssetData
    {
        foreach(var asset in _assets)
        {
            if(asset.AssetId == id && asset is T assetAs)
                return assetAs;
        }

        return null;
    }

/*
    /// <summary>
    /// Handles loading all of the assets for this scene
    /// Generally called within the start method
    /// </summary>
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
*/

    public void AddLoadedAsset(AssetData asset)
    {
        _assets.Add(asset);
    }

    public void RemoveLoadedAsset(AssetData asset)
    {
        _assets.Remove(asset);
    }

    public void RemoveLoadedAsset(string name)
    {
        foreach(var asset in _assets)
        {
            if(asset.AssetName == name)
            {
                _assets.Remove(asset);
                return;
            }
        }
    }

    public List<Element> GetAllElements() => _elements;
}