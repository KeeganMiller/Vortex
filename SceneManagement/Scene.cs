using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;

namespace Vortex;

public abstract class Scene
{
    public string SceneId { get; } = Guid.NewGuid().ToString();
    public string SceneName;
    protected string _sceneDataPath { get; private set; }
    public ResourceManager Resources { get; }

    public Scene(string name, string scenePath)
    {
        SceneName = name;
        _sceneDataPath = scenePath;
        Resources = new ResourceManager(this, Game.GetAssetPath() + scenePath);
    }

    public virtual void Enter()
    {
        if (Resources != null)
            Resources.Start();
    }

    public virtual void Exit()
    {
        if (Resources != null)
            Resources.Stop();
    }

    public virtual void Update(float dt)
    {
        if (Resources != null)
            Resources.Update(dt);
    }
}