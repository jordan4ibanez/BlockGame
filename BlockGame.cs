using System.Numerics;
using BlockGame.Utility;
using Raylib_cs;

namespace BlockGame;

public interface IEntity {
    void OnTick();
    void OnHit();
    void OnHPChange();
}

class Frog : IEntity {
    public void OnTick() {
        Console.WriteLine("If the women don't find you handsome, they should at least find you handy");
    }
    public void OnHit() {
        Console.WriteLine("hit");
    }
    public void OnHPChange() {
        Console.WriteLine("HP am have done darn diggity dang changed");
    }
}

class Game : IDisposable {

    readonly string windowTitle = "BlockGame";

    static readonly bool DEBUG_MODE = false;

    public Game() {
        Setup();
    }

    public static bool IsDebugMode() {
        return DEBUG_MODE;
    }

    void Setup() {

        // Reflection to get the package version of raylib-cs.
        // Console.WriteLine($"Raylib-cs: {typeof(Raylib_cs.Raylib).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}");

        Raylib.SetTraceLogLevel(TraceLogLevel.Warning);
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow | ConfigFlags.VSyncHint);

        Raylib.InitWindow(1, 1, windowTitle);
        CenterWindow();

        Raylib.InitAudioDevice();

        Raylib.SetTargetFPS(0);

        // SoundManager.Initialize();
        // FontManager.Initialize();
        // TextureManager.Initialize();
        // ModelManager.Initialize();
        // ShaderManager.Initialize();

        // CameraManager.Initialize();
    }

    public void Dispose() {
        // LevelManager.Unload();
        // ShaderManager.Terminate();
        // ModelManager.Terminate();
        // TextureManager.Terminate();
        // FontManager.Terminate();
        // SoundManager.Terminate();

        Raylib.CloseAudioDevice();
        Raylib.CloseWindow();
    }

    void CenterWindow() {
        int currentMonitor = Raylib.GetCurrentMonitor();
        int monitorWidth = Raylib.GetMonitorWidth(currentMonitor);
        int monitorHeight = Raylib.GetMonitorHeight(currentMonitor);
        int halfMonitorWidth = monitorWidth / 2;
        int halfMonitorHeight = monitorHeight / 2;
        // Console.WriteLine($"Monitor Resolution: {monitorWidth}x{monitorHeight}");
        Raylib.SetWindowSize(halfMonitorWidth, halfMonitorHeight);
        Vector2 monitorPos = Raylib.GetMonitorPosition(currentMonitor);
        int startX = ((monitorWidth - halfMonitorWidth) / 2) + (int)monitorPos.X;
        int startY = (monitorHeight - halfMonitorHeight) / 2 + (int)monitorPos.Y;
        Raylib.SetWindowPosition(startX, startY);
    }

    void DoInternals() {
        Delta.CalculateDelta();
        // GUI.Update();
        // FontManager.Update();
    }

    public void MainLoop() {
        DoInternals();
        Raylib.BeginDrawing();
        {
            Raylib.ClearBackground(Color.Gray);
        }
        Raylib.EndDrawing();
    }
}

internal static class BlockGame {
    [STAThread]
    public static void Main() {

        Game game = new();

        while (!Raylib.WindowShouldClose()) {
            game.MainLoop();
        }
    }
}