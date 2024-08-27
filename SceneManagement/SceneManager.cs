using System.Collections.Generic;
using Raylib_cs;

namespace Vortex;

public static class SceneManager
{
    private static List<Scene> _activeScenes = new List<Scene>();
    public static ResourceManager GlobalResources = new ResourceManager(null, "GlobalResources.vt");

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
        foreach (var scene in _activeScenes)
            scene.Draw();

        if(GlobalResources != null)
            GlobalResources.Draw();
    }

    public static void DrawCameraRelated()
    {
        foreach(var scene in _activeScenes)
            scene.DrawCameraRelated();

        if(GlobalResources != null)
            GlobalResources.DrawCameraRelated();
    }
}