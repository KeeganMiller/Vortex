using System.Collections.Generic;
using Raylib_cs;
using Newtonsoft.Json;

namespace Vortex;

public static class Game
{
    private static WindowProperties _windowSettings;
    public static bool IsRunning = true;

    public static WindowProperties WindowSettings => _windowSettings;

    public static bool IsDebug = true;

    private const string DEBUG_ASSET_PATH = "../../../Assets/";
    private const string GENERAL_ASSET_PATH = "Assets/";

    public static void Initialize(string[] args)
    {
        if(args.Length > 0)
        {
            if(args[0] == "console")
            {
                IsDebug = false;
            }
        }
        
        if(_windowSettings == null)
        {
            _windowSettings = new WindowProperties
            {
                WindowWidth = 1280,
                WindowHeight = 720,
                WindowTitle = "Vortex Engine - v0.1"
            };
        }

        Raylib.InitWindow(_windowSettings.WindowWidth, _windowSettings.WindowHeight, _windowSettings.WindowTitle);
        Raylib.SetTargetFPS(60);

        // Handle Resource Creation
    }

    public static void Run()
    {
        while(!Raylib.WindowShouldClose() && IsRunning)
        {
            SceneManager.Update();

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);
            SceneManager.Draw();
            Raylib.EndDrawing();
        }
    }

    public static string GetAssetPath()
    {
        return IsDebug ? DEBUG_ASSET_PATH : GENERAL_ASSET_PATH;
    }
}

public class WindowProperties
{
    [JsonProperty] public int WindowWidth;
    [JsonProperty] public int WindowHeight;
    [JsonProperty] public string WindowTitle;
}
