using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;

namespace Vortex;

public abstract class Scene
{
    public string SceneId { get; } = Guid.NewGuid().ToString();
    public string SceneName;
    protected string _sceneDataPath { get; private set; }

    public Scene(string name, string scenePath)
    {
        SceneName = name;
        _sceneDataPath = scenePath;
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public virtual void Update(float dt)
    {

    }

    public virtual void Draw()
    {

    }
}