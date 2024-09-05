using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using Raylib_cs;

namespace Vortex;

public static class SceneManager
{
    private static List<Scene> _activeScenes = new List<Scene>();
    public static ResourceManager GlobalResources = new ResourceManager(null, null);

    public static void AddScene(Scene scene)
    {
        if (!_activeScenes.Contains(scene))
        {
            _activeScenes.Add(scene);
            scene.Enter();
        }
    }

    public static void RemoveScene(Scene scene)
    {
        if(_activeScenes.Contains(scene))
        {
            _activeScenes.Remove(scene);
            scene.Exit();
        }
    }

    public static void RemoveScene(string sceneName)
    {
        foreach(var scene in _activeScenes.ToList())
        {
            if(scene.SceneName ==  sceneName)
            {
                RemoveScene(scene);
                return;
            }
        }
    }

    public static void Update()
    {
        foreach(var scene in _activeScenes.ToList())
        {
            if(scene.CurrentSceneLoadState == ESceneLoadState.SCENE_STATE_Loaded)
                scene.Update(Raylib.GetFrameTime());
        }

        if(GlobalResources != null)
        {
            GlobalResources.Update(Raylib.GetFrameTime());
        }
    }

    public static void Draw()
    {
        

        var sprites = new List<SpriteComponent>();
        foreach(var scene in _activeScenes.ToList())
        {
            if(scene.CurrentSceneLoadState == ESceneLoadState.SCENE_STATE_Loaded)
            {
                var sceneSprites = scene.Resources.GetSprites();
                foreach(var s in sceneSprites)
                    sprites.Add(s);
            }
        }

        foreach(var s in GlobalResources.GetSprites())
        {
            sprites.Add(s);
        }

        var sortedSprites = SortSprites(sprites);
        foreach(var sprite in sortedSprites)
            if(sprite.Owner.IsActive && sprite.IsActive && sprite.HasStarted)
                sprite.Draw();

    }

    public static void DrawCameraRelated()
    {
        

        var sprites = new List<SpriteComponent>();
        foreach(var scene in _activeScenes.ToList())
        {
            if(scene.CurrentSceneLoadState == ESceneLoadState.SCENE_STATE_Loaded)
            {
                var sceneSprites = scene.Resources.GetCameraRelativeSprites();
                foreach(var s in sceneSprites)
                    sprites.Add(s);
                }
        }

        foreach(var s in GlobalResources.GetCameraRelativeSprites())
        {
            sprites.Add(s);
        }

        var sortedSprites = SortSprites(sprites);
        foreach(var sprite in sortedSprites)
            if(sprite.Owner.IsActive && sprite.IsActive && sprite.HasStarted)
                sprite.Draw();
    }

    public static void DrawElementsRelative()
    {
        foreach(var scene in _activeScenes.ToList())
        {   
            if(scene.CurrentSceneLoadState == ESceneLoadState.SCENE_STATE_Loaded)
                scene.DrawElementsRelative();
        }
        
        GlobalResources.DrawElementsRelative();
    }

    public static void DrawElements()
    {
        foreach(var scene in _activeScenes.ToList())
            if(scene.CurrentSceneLoadState == ESceneLoadState.SCENE_STATE_Loaded)
                scene.DrawElements();

        GlobalResources.DrawElements();
    }

    public static void DrawUiElements()
    {
        var uiList = new List<UIComponent>();
        foreach(var scene in _activeScenes.ToList())
        {
            if(scene.CurrentSceneLoadState == ESceneLoadState.SCENE_STATE_Loaded)
            {
                foreach(var comp in scene.Resources.GetUiComponents())
                {
                    uiList.Add(comp);
                }
            }
        }

        foreach(var comp in GlobalResources.GetUiComponents())
            uiList.Add(comp);

        var sortedUiElements = SortUi(uiList);
        
        foreach(var comp in sortedUiElements)
        {
            if(comp.IsActive && comp.Owner.IsActive && comp.HasStarted)
                comp.Draw();
        }
    }

    public static List<SpriteComponent> SortSprites(List<SpriteComponent> sprites)
    {
        var sortedSprites = new List<SpriteComponent>();

        while(sprites.Count != 0)
        {
            var lowestSprite = sprites[0];
            for(var i = 1; i < sprites.Count; ++i)
            {
                if(sprites[i].ZIndex < lowestSprite.ZIndex)
                {
                    lowestSprite = sprites[i];
                }
            }

            sortedSprites.Add(lowestSprite);
            sprites.Remove(lowestSprite);
        }


        return sortedSprites;
    }

    public static List<UIComponent> SortUi(List<UIComponent> comps)
    {
        var sortedUiComps = new List<UIComponent>();

        while(comps.Count != 0)
        {
            var highest = comps[0];
            for(var i = 1; i < comps.Count; ++i)
            {
                if(comps[i].ZIndex < highest.ZIndex)
                    highest = comps[i];
            }

            sortedUiComps.Add(highest);
            comps.Remove(highest);
        }


        return sortedUiComps;
    }

    public static List<ResourceManager> GetAllResourceManagers()
    {
        var resources = new List<ResourceManager>();
        foreach(var scene in _activeScenes)
            if(scene.CurrentSceneLoadState == ESceneLoadState.SCENE_STATE_Loaded)
                resources.Add(scene.Resources);

        return resources;
    }
}