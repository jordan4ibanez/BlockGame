using BlockGame.Graphics;
using BlockGame.Utility;

namespace BlockGame;

class Game : IDisposable {

    public static readonly string WINDOW_TITLE = "BlockGame";

    public static readonly bool DEBUG_MODE = false;

    public Game() {
        Setup();
    }

    void Setup() {

    }

    public void Dispose() {

    }

    void DoInternals() {
        Delta.CalculateDelta();
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