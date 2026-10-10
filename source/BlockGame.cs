using Silk.NET.Windowing;
using Silk.NET.Vulkan;
using Silk.NET.GLFW;
using Silk.NET.Maths;
using System.Text;
using System.Runtime.InteropServices;

namespace BlockGame;

internal static class BlockGame {

    private const string WINDOW_NAME = "BlockGame";
    private static IWindow? window;

    public static unsafe void Main() {

        WindowOptions options = WindowOptions.DefaultVulkan with {
            Size = new Vector2D<int>(800, 600),
            Title = WINDOW_NAME
        };

        window = Window.Create(options);

        Glfw glfw = Glfw.GetApi();

        fixed (char* engineName = "No Engine")
        fixed (char* appName = WINDOW_NAME) {
            // VK setup.
            ApplicationInfo appInfo = new();
            appInfo.SType = StructureType.ApplicationInfo;
            appInfo.PApplicationName = (byte*)appName;
            appInfo.ApplicationVersion = Vk.MakeVersion(1, 0, 0);
            appInfo.PEngineName = (byte*)engineName;
            appInfo.EngineVersion = Vk.MakeVersion(1, 0, 0);
            appInfo.ApiVersion = Vk.Version10;

            //  VK init.
            InstanceCreateInfo createInfo = new();
            createInfo.SType = StructureType.InstanceCreateInfo;
            createInfo.PApplicationInfo = &appInfo;

            uint glfwExtensionCount = 0;
            byte** glfwExtensions;

            glfwExtensions = glfw.GetRequiredInstanceExtensions(out glfwExtensionCount);

            createInfo.EnabledExtensionCount = glfwExtensionCount;
            createInfo.PpEnabledExtensionNames = glfwExtensions;

            // Debug print out available extensions.
            for (int i = 0; i < glfwExtensionCount; i++) {
                byte* currentExtension = glfwExtensions[i];
                int length = 0;
                for (int l = 0; l < 255; l++) {
                    if (currentExtension[l] == 0) {
                        length = l;
                        break;
                    }
                    Console.WriteLine(Marshal.PtrToStringUTF8((nint)currentExtension));
                }
            }
        }
    }
}