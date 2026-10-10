using BlockGame.Graphics;
using BlockGame.Utility;

namespace BlockGame;

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

        // SoundManager.Initialize();
        FontManager.Initialize();
        TextureManager.Initialize();
        ModelManager.Initialize();
        ShaderManager.Initialize();

        CameraManager.Initialize();
    }

    public void Dispose() {
        ShaderManager.Terminate();
        ModelManager.Terminate();
        TextureManager.Terminate();
        FontManager.Terminate();
        // SoundManager.Terminate();
    }



    void DoInternals() {
        Delta.CalculateDelta();
        // GUI.Update();
        // FontManager.Update();
    }

    public void MainLoop() {
        DoInternals();
    }
}

internal static class MainThread {
    [STAThread]
    public static void Main() {

        Game game = new();

    }
}