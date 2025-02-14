﻿using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;

namespace Vortex;

public enum ESceneLoadState
{
    SCENE_STATE_Idle,
    SCENE_STATE_Error,
    SCENE_STATE_Loading,
    SCENE_STATE_Loaded,
}

public abstract class Scene
{
    public string SceneId { get; } = Guid.NewGuid().ToString();
    public string SceneName;
    protected string _sceneDataPath { get; private set; }
    public ResourceManager Resources { get; }
    
    public ESceneLoadState CurrentSceneLoadState { get; private set; } = ESceneLoadState.SCENE_STATE_Idle;
    private bool _loadSceneAsync;

    public Scene(string name, string scenePath, bool loadAsync = false)
    {
        SceneName = name;
        _sceneDataPath = scenePath;
        Resources = new ResourceManager(this, Game.GetAssetPath() + scenePath);

        _loadSceneAsync = loadAsync;
    }

    public virtual void Enter()
    {
        if (Resources != null)
            Resources.Start();

        if(_loadSceneAsync)
            SceneLoad();
        else
            CurrentSceneLoadState = ESceneLoadState.SCENE_STATE_Loaded;
    }

    public async virtual Task SceneLoad()
    {
        BeginSceneLoading();
    }

    public void BeginSceneLoading() => CurrentSceneLoadState = ESceneLoadState.SCENE_STATE_Loading;
    public void EndSceneLoading() => CurrentSceneLoadState = ESceneLoadState.SCENE_STATE_Loaded;

    public virtual void DrawElements()
    {
        if(Resources != null)
            Resources.DrawElements();
    }

    public virtual void DrawElementsRelative()
    {
        if(Resources != null)
            Resources.DrawElementsRelative();
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