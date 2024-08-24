using System.Collections.Generic;
using Raylib_cs;
using Newtonsoft.Json;

namespace Vortex;

public static class Game
{
    private static WindowProperties _windowSettings;
    public static bool IsRunning = true;

    public const string ASSET_PATH = "../../../Assets/";

    public static void Initialize()
    {
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
            // Update here

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);
            Raylib.DrawText("Hello, World", 100, 100, 32, Color.Black);
            Raylib.EndDrawing();
        }
    }
}

public class WindowProperties
{
    [JsonProperty] public int WindowWidth;
    [JsonProperty] public int WindowHeight;
    [JsonProperty] public string WindowTitle;
}
