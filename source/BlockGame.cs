using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.Vulkan;

namespace BlockGame;

class Game : IDisposable {
    public Game() { }
    public void Dispose() { }
}

internal static class MainThread {
    public static void Main() {
        WindowOptions options = WindowOptions.DefaultVulkan with {
            Size = new Vector2D<int>(800, 600),
            Title = "test"
        };

    }
}