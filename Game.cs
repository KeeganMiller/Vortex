using System.Collections.Generic;
using Raylib_cs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Numerics;

namespace Vortex;

public static class Game
{
    private static WindowProperties _windowSettings;
    public static bool IsRunning = true;

    public static WindowProperties WindowSettings => _windowSettings;

    public static bool IsConsole = false;                    // flag if the game is started via a console with arguments

    private const string DEBUG_ASSET_PATH = "../../../Assets/";
    private const string GENERAL_ASSET_PATH = "Assets/";

    private static bool _isInitialized = false;
    public static Camera2D CameraRef;

    public static void Initialize(string[] args)
    {
        if(args.Length > 0)
        {
            foreach(var arg in args)
            {
                switch(arg)
                {
                    case "console":
                        IsConsole = true;
                        break;
                    case "no-debug":
                        Debug.DebugEnabled = false;
                        break;
                    default:
                        Console.WriteLine("Failed to launch application with argument");
                        return;
                }
            }
        }

        if(File.Exists(GetAssetPath() + "Run.vt"))
        {
            using(var sr = new StreamReader(GetAssetPath() + "Run.vt"))
            {
                var tokens = JArray.Parse(sr.ReadToEnd());
                foreach(var token in tokens)
                {
                    var runSettings = JsonConvert.DeserializeObject<RunSettingsJson>(token.ToString());
                    if(runSettings != null)
                    {
                        if(runSettings.Category == "WindowSettings")
                            _windowSettings = JsonConvert.DeserializeObject<WindowProperties>(token.ToString());

                    }
                }
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

        _isInitialized = true;

        CameraRef = new Camera2D();

        SceneManager.GlobalResources.Start();

        // Handle Resource Creation
    }

    public static void Run()
    {
        if(!_isInitialized)
            return;

        while(!Raylib.WindowShouldClose() && IsRunning)
        {
            SceneManager.Update();

            Raylib.BeginDrawing();
            
            Raylib.ClearBackground(Color.White);
            Raylib.BeginMode2D(CameraRef);
            SceneManager.DrawCameraRelated();
            Raylib.EndMode2D();

            SceneManager.Draw();
            Raylib.EndDrawing();
        }

        SceneManager.GlobalResources.Stop();
        Raylib.CloseWindow();
    }

    public static string GetAssetPath()
    {
        return IsConsole ? GENERAL_ASSET_PATH : DEBUG_ASSET_PATH;
    }

    public static Vector2 GetPositionBasedOnCamera(Vector2 position)
    {
        return position + CameraRef.Target;
    }
}

public class WindowProperties
{
    [JsonProperty] public int WindowWidth;
    [JsonProperty] public int WindowHeight;
    [JsonProperty] public string WindowTitle;
}

public class RunSettingsJson
{
    [JsonProperty] public string Category;
}
