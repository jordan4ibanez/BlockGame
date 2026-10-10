using Silk.NET.Windowing;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.EXT;
using Silk.NET.GLFW;
using Silk.NET.Maths;
using System.Text;
using System.Runtime.InteropServices;

namespace BlockGame;

internal static class BlockGame {

    private const string WINDOW_NAME = "BlockGame";
    private static IWindow? window;

    // This is the GLFW library.
    readonly private static Glfw glfw = Glfw.GetApi();

    // This is the instance of Vulkan.
    private static Instance vulkanInstance;

    // This is the Vulkan library DLL. (function pointers)
    private static Vk? vulkan;

    readonly static bool ENABLE_VALIDATION_LAYERS = true;
    readonly static string[] requiredValidationLayers = ["VK_LAYER_KHRONOS_validation"];

    public static unsafe void Main() {

        InitWindow();

        if (window == null) {
            throw new Exception("Window became null");
        }

        window.Run();


        CleanUp();
    }

    static unsafe void InitWindow() {
        WindowOptions options = WindowOptions.DefaultVulkan with {
            Size = new Vector2D<int>(800, 600),
            Title = WINDOW_NAME
        };
        window = Window.Create(options);

        window.Load += Load;
        window.Update += Update;
        window.Render += Render;
    }

    static void Load() {
        InitVulkan();
    }
    static void Update(double delta) {
        if (window == null) {
            throw new Exception("Window went missing");
        }
        window.Close();
    }
    static void Render(double delta) { }



    // static unsafe void Mainloop() {
    //     if (window == null) {
    //         throw new Exception("Window became null");
    //     }

    //     if (window.Handle == 0) {
    //         throw new Exception("Window handle pointer is null");
    //     }

    //     while (!glfw.WindowShouldClose((WindowHandle*)window.Handle)) {
    //         glfw.PollEvents();
    //     }
    // }

    static unsafe void InitVulkan() {
        CreateInstance();
        SetupDebugMessenger();
    }


    static unsafe void CreateInstance() {

        fixed (char* engineName = "No Engine")
        fixed (char* appName = WINDOW_NAME) {

            //~ VK app info.
            ApplicationInfo appInfo = new();
            appInfo.SType = StructureType.ApplicationInfo;
            appInfo.PApplicationName = (byte*)appName;
            appInfo.ApplicationVersion = Vk.MakeVersion(1, 0, 0);
            appInfo.PEngineName = (byte*)engineName;
            appInfo.EngineVersion = Vk.MakeVersion(1, 0, 0);
            appInfo.ApiVersion = Vk.Version10;

            //~ VK create info.
            InstanceCreateInfo createInfo = new();
            createInfo.SType = StructureType.InstanceCreateInfo;
            createInfo.PApplicationInfo = &appInfo;

            List<byte[]> validationLayerNameBytes = [.. requiredValidationLayers.Select(Encoding.UTF8.GetBytes)];
            GCHandle[] validationLayerNameHandles = new GCHandle[validationLayerNameBytes.Count];
            byte** validationLayerNamePointers = null;

            if (ENABLE_VALIDATION_LAYERS) {
                Console.WriteLine("Enabling validation layers.");

                createInfo.EnabledLayerCount = (uint)requiredValidationLayers.Length;

                //! Manual memory management. (Malloc) [validationLayerNameHandles, validationLayerNamePointers]
                validationLayerNamePointers = (byte**)NativeMemory.Alloc((nuint)validationLayerNameBytes.Count, (nuint)sizeof(byte*));
                for (int i = 0; i < validationLayerNameBytes.Count; i++) {
                    validationLayerNameHandles[i] = GCHandle.Alloc(validationLayerNameBytes[i], GCHandleType.Pinned);
                    validationLayerNamePointers[i] = (byte*)validationLayerNameHandles[i].AddrOfPinnedObject();
                }
                //! End manual memory management. (Malloc) [validationLayerNameHandles, validationLayerNamePointers]

                createInfo.PpEnabledLayerNames = validationLayerNamePointers;
            } else {
                createInfo.EnabledLayerCount = 0;
            }

            // VK required extensions.
            string[] requiredExtensionNames = GetRequiredExtensions();

            List<byte[]> requiredExtensionNamesBytes = [.. requiredExtensionNames.Select(Encoding.UTF8.GetBytes)];
            GCHandle[] requiredExtensionNamesHandles = new GCHandle[requiredExtensionNamesBytes.Count];
            byte** requiredExtensionNamePointers = null;

            {
                //! Manual memory management. (Malloc) [requiredExtensionNamesHandles, requiredExtensionNamePointers]
                requiredExtensionNamePointers = (byte**)NativeMemory.Alloc((nuint)requiredExtensionNamesBytes.Count, (nuint)sizeof(byte*));
                for (int i = 0; i < requiredExtensionNamesBytes.Count; i++) {
                    requiredExtensionNamesHandles[i] = GCHandle.Alloc(requiredExtensionNamesBytes[i], GCHandleType.Pinned);
                    requiredExtensionNamePointers[i] = (byte*)requiredExtensionNamesHandles[i].AddrOfPinnedObject();
                }
                //! End manual memory management. (Malloc) [requiredExtensionNamesHandles, requiredExtensionNamePointers]

                createInfo.EnabledExtensionCount = (uint)requiredExtensionNames.Length;
                createInfo.PpEnabledExtensionNames = requiredExtensionNamePointers;
            }

            vulkan = Vk.GetApi(createInfo, out vulkanInstance);



            //! Manual memory management. (Free) [requiredExtensionNamesHandles, requiredExtensionNamePointers]
            for (int i = 0; i < requiredExtensionNamesHandles.Length; i++) {
                if (requiredExtensionNamesHandles[i].IsAllocated) requiredExtensionNamesHandles[i].Free();
            }
            if (requiredExtensionNamePointers != null) NativeMemory.Free(requiredExtensionNamePointers);
            //! End manual memory management. (Free) [requiredExtensionNamesHandles, requiredExtensionNamePointers]

            //! Manual memory management. (Free) [validationLayerNameHandles, validationLayerNamePointers]
            for (int i = 0; i < validationLayerNameHandles.Length; i++) {
                if (validationLayerNameHandles[i].IsAllocated) validationLayerNameHandles[i].Free();
            }
            if (validationLayerNamePointers != null) NativeMemory.Free(validationLayerNamePointers);
            //! End manual memory management. (Free) [validationLayerNameHandles, validationLayerNamePointers]

            // VK extension support check.
            DebugPrintExtensionSupport(vulkan);

            // VK validation layer support check.
            if (ENABLE_VALIDATION_LAYERS && !CheckValidationLayerSupport()) {
                throw new Exception("Missing validation layer!");
            }
        }
    }

    static unsafe void SetupDebugMessenger() {
        if (!ENABLE_VALIDATION_LAYERS) {
            return;
        }

        DebugUtilsMessengerCreateInfoEXT createInfo = new();
        createInfo.SType = StructureType.DebugUtilsMessengerCreateInfoExt;
        createInfo.MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt | DebugUtilsMessageSeverityFlagsEXT.WarningBitExt | DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt;
        createInfo.MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt | DebugUtilsMessageTypeFlagsEXT.ValidationBitExt | DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt;
        createInfo.PfnUserCallback = new PfnDebugUtilsMessengerCallbackEXT(DebugCallback);
        createInfo.PUserData = null; // Optional

        Console.WriteLine("Enabled vulkan debug messenger.");
    }

    static unsafe uint DebugCallback(DebugUtilsMessageSeverityFlagsEXT messageSeverity,
        DebugUtilsMessageTypeFlagsEXT messageType,
        DebugUtilsMessengerCallbackDataEXT* pCallbackData,
        void* pUserData) {

        if (pCallbackData == null) {
            throw new Exception("null data");
        }

        Console.WriteLine(Marshal.PtrToStringUTF8((nint)pCallbackData->PMessage));

        return Vk.False;
    }

    static unsafe string[] GetRequiredExtensions() {
        uint glfwExtensionCount = 0;
        char** glfwExtensions = (char**)glfw.GetRequiredInstanceExtensions(out glfwExtensionCount);

        List<string> requiredExtensions = [];
        for (int i = 0; i < glfwExtensionCount; i++) {
            string? requiredExtensionString = Marshal.PtrToStringUTF8((nint)glfwExtensions[i]);
            if (requiredExtensionString != null) {
                requiredExtensions.Add(requiredExtensionString);
            }
        }

        if (ENABLE_VALIDATION_LAYERS) {
            requiredExtensions.Add(ExtDebugUtils.ExtensionName);
        }

        Console.WriteLine($"Vulkan required extensions: [ {string.Join(", ", requiredExtensions)} ]");

        return [.. requiredExtensions];
    }

    static unsafe bool CheckValidationLayerSupport() {
        if (vulkan == null) {
            throw new Exception("Vulkan exploded");
        }

        uint layerCount = 0;
        vulkan.EnumerateInstanceLayerProperties(ref layerCount, null);

        LayerProperties[] availableLayers = new LayerProperties[layerCount];

        vulkan.EnumerateInstanceLayerProperties(ref layerCount, ref availableLayers[0]);

        List<string> foundValidationLayers = [];
        foreach (var layer in availableLayers) {
            string? foundLayerName = Marshal.PtrToStringUTF8((nint)layer.LayerName);
            if (foundLayerName != null) {
                foundValidationLayers.Add(foundLayerName);
            }
        }
        foreach (var requiredLayer in requiredValidationLayers) {
            Console.Write($"Looking for validation layer {requiredLayer}...");
            if (!foundValidationLayers.Contains(requiredLayer)) {
                Console.WriteLine($"MISSING!");
                return false;
            }
            Console.WriteLine("Found!");
        }
        return true;
    }

    static unsafe void CleanUp() {

        if (vulkan == null) {
            throw new Exception("Vulkan DLL became null somehow");
        }


        if (window == null) {
            throw new Exception("Window became null somehow");
        }

        vulkan.DestroyInstance(vulkanInstance, null);
        window.Dispose();
        glfw.Terminate();

    }

    static unsafe void DebugPrintExtensionSupport(Vk vulkan) {
        StringBuilder builder = new();
        builder.Append("Vulkan Supported Extensions: [ ");
        uint extensionCount = 0;
        vulkan.EnumerateInstanceExtensionProperties((byte*)null, ref extensionCount, null);
        ExtensionProperties[] extensions = new ExtensionProperties[extensionCount];
        vulkan.EnumerateInstanceExtensionProperties((byte*)null, &extensionCount, ref extensions[0]);

        for (int i = 0; i < extensionCount; i++) {
            fixed (byte* extensionName = extensions[i].ExtensionName) {
                builder.Append(Marshal.PtrToStringUTF8((nint)extensionName));
                if (i < extensionCount - 1) {
                    builder.Append(", ");
                }
            }
        }
        builder.Append(" ]");
        Console.WriteLine(builder.ToString());
    }


    //! Code left here as a refresher.
    // static unsafe void DebugPrintRequiredExtensions(uint glfwExtensionCount, byte** extensions) {
    //     StringBuilder builder = new();
    //     builder.Append("Vulkan Required Extensions: [ ");
    //     for (int i = 0; i < glfwExtensionCount; i++) {
    //         byte* currentExtension = extensions[i];
    //         builder.Append(Marshal.PtrToStringUTF8((nint)currentExtension));
    //         if (i < glfwExtensionCount - 1) {
    //             builder.Append(", ");
    //         }
    //     }
    //     builder.Append(" ]");
    //     Console.WriteLine(builder.ToString());
    // }
}