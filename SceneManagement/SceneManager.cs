using System.Collections.Generic;
using System.Runtime.InteropServices;
using Raylib_cs;

namespace Vortex;

public static class SceneManager
{
    private static List<Scene> _activeScenes = new List<Scene>();
    public static ResourceManager GlobalResources = new ResourceManager(null, Game.GetAssetPath() + "GlobalResources.vt");

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
        foreach(var scene in _activeScenes)
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
        foreach(var scene in _activeScenes)
        {
            scene.Update(Raylib.GetFrameTime());
        }

        if(GlobalResources != null)
        {
            GlobalResources.Update(Raylib.GetFrameTime());
        }
    }

    public static void Draw()
    {
        foreach(var scene in _activeScenes)
            scene.DrawElements();

        GlobalResources.DrawElements();

        var sprites = new List<SpriteComponent>();
        foreach(var scene in _activeScenes)
        {
            var sceneSprites = scene.Resources.GetSprites();
            foreach(var s in sceneSprites)
                sprites.Add(s);
        }

        foreach(var s in GlobalResources.GetSprites())
        {
            sprites.Add(s);
        }

        var sortedSprites = SortSprites(sprites);
        foreach(var sprite in sortedSprites)
            sprite.Draw();

        foreach(var scene in _activeScenes)
        {
            var uiComps = scene.Resources.GetUiComponents();
            if(uiComps.Count > 0)
                foreach(var comp in uiComps)
                    comp.Draw();
        }

        var globalUiComps = GlobalResources.GetUiComponents();
        if(globalUiComps.Count > 0)
            foreach(var comp in globalUiComps)
                comp.Draw();
    }

    public static void DrawCameraRelated()
    {
        foreach(var scene in _activeScenes)
        {   
            scene.DrawElementsRelative();
        }
        
        GlobalResources.DrawElementsRelative();

        var sprites = new List<SpriteComponent>();
        foreach(var scene in _activeScenes)
        {
            var sceneSprites = scene.Resources.GetCameraRelativeSprites();
            foreach(var s in sceneSprites)
                sprites.Add(s);
        }

        foreach(var s in GlobalResources.GetCameraRelativeSprites())
        {
            sprites.Add(s);
        }

        var sortedSprites = SortSprites(sprites);
        foreach(var sprite in sortedSprites)
        {
            sprite.Draw();
        }
    }

    public static List<SpriteComponent> SortSprites(List<SpriteComponent> sprites)
    {
        var sortedSprites = new List<SpriteComponent>();

        while(sprites.Count != 0)
        {
            var highestSprite = sprites[0];
            for(var i = 1; i < sprites.Count; ++i)
            {
                if(sprites[i].ZIndex < highestSprite.ZIndex)
                {
                    highestSprite = sprites[i];
                }
            }

            sortedSprites.Add(highestSprite);
            sprites.Remove(highestSprite);
        }


        return sortedSprites;
    }
}