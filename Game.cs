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
    public static string DefaultNamespace = "Uprising";

    private static bool _isInitialized = false;
    public static Camera2D CameraRef;
    public static Color BackgroundColor;

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

        CameraRef = new Camera2D
        {
            Target = new Vector2(0, 0),
            Zoom = 1.0f
        };

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

            if(Raylib.IsWindowResized())
            {
                WindowSettings.ResizeWindow(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
            }

            Raylib.BeginDrawing();
            Raylib.ClearBackground(BackgroundColor);

            SceneManager.Draw();
            SceneManager.DrawElements();


            Raylib.BeginMode2D(CameraRef);


            SceneManager.DrawCameraRelated();
            SceneManager.DrawElementsRelative();

            Raylib.EndMode2D();
            

            SceneManager.DrawUiElements();

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
    private int _windowWidth;
    private int _windowHeight;
    [JsonProperty] public int WindowWidth
    {
        get => _windowWidth;
        set => _windowWidth = value;
    }

    [JsonProperty] public int WindowHeight
    {
        get => _windowHeight;
        set => _windowHeight = value;
    }
    [JsonProperty] public string WindowTitle;

    public System.Action<int, int> WindowResizeEvent;

    public void ResizeWindow(int width, int height)
    {
        _windowWidth = width;
        _windowHeight = height;
        WindowResizeEvent?.Invoke(width, height);
    }
}

public class RunSettingsJson
{
    [JsonProperty] public string Category;
}
