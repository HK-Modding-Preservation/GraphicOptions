using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Modding;
using Modding.Menu;
using Modding.Menu.Config;
using SFCore.Generics;
using SFCore.Utils;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using UObject = UnityEngine.Object;
using UScenes = UnityEngine.SceneManagement;
using Satchel.BetterMenus;

namespace GraphicOptions;

public class GraphicOptionsSettings
{
    #region Camera Settings

    public bool CameraBloomEnabled = true;
    public bool CameraBlurEnabled = true;
    public bool CameraDynamicResolutionAllowed = false;
    public bool CameraFilmGrainEnabled = false;
    public bool CameraHdrEnabled = false;
    public bool CameraMsaaAllowed = true;
    public bool CameraUseOcclusionCulling = true;
    public bool CameraUsePhysicalProperties = false;

    #endregion

    #region Quality Settings

    public AnisotropicFiltering QualityAnisotropicFiltering = AnisotropicFiltering.Disable;
    public int QualityAntiAliasing = 0;
    public int QualityAsyncUploadBufferSize = 4;
    public bool QualityAsyncUploadPersistentBuffer = true;
    public int QualityAsyncUploadTimeSlice = 2;
    public bool QualityBillboardsFaceCameraPosition = false;
    public float QualityLodBias = 0.7f;
    public int QualityMaximumLODLevel = 0;
    public int QualityParticleRaycastBudget = 96;
    public int QualityPixelLightCount = 1;
    public bool QualityRealtimeReflectionProbes = false;
    public float QualityResolutionScalingFixedDPIFactor = 1;
    public float QualityShadowCascade2Split = 0.33333334f;
    public Vector3 QualityShadowCascade4Split = new Vector3(0.06666667f, 0.2f, 0.46666667f);
    public int QualityShadowCascades = 1;
    public float QualityShadowDistance = 20f;
    public ShadowmaskMode QualityShadowmaskMode = ShadowmaskMode.Shadowmask;
    public float QualityShadowNearPlaneOffset = 2f;
    public ShadowProjection QualityShadowProjection = ShadowProjection.StableFit;
    public ShadowResolution QualityShadowResolution = ShadowResolution.Low;
    public ShadowQuality QualityShadows = ShadowQuality.Disable;
    public SkinWeights QualitySkinWeights = SkinWeights.TwoBones;
    public bool QualitySoftParticles = false;
    public bool QualitySoftVegetation = false;
    public bool QualityStreamingMipmapsActive = false;
    public bool QualityStreamingMipmapsAddAllCameras = true;
    public int QualityStreamingMipmapsMaxFileIORequests = 1024;
    public int QualityStreamingMipmapsMaxLevelReduction = 2;
    public float QualityStreamingMipmapsMemoryBudget = 512;
    public int QualityStreamingMipmapsRenderersPerFrame = 512;
    public int QualityVSyncCount = 1;

    #endregion
}

internal struct OptionStruct
{
    public Type ParamType;
    public string Name;
    public string[] Options;
    public float[] FromToOption;
    public Action<object> ApplySetting;
    public Func<object> RefreshSetting;
    public Action<MenuSelectable> CancelAction;
    public string Description;
    public HorizontalOptionStyle Style;
}

public class GraphicOptions : GlobalSettingsMod<GraphicOptionsSettings>, ICustomMenuMod
{
    bool ICustomMenuMod.ToggleButtonInsideMenu => true;
    public Menu MenuRef;

    public override string GetVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }

    public GraphicOptions() : base("Graphic Options")
    {
        SetCallbacks();
    }

    public override void Initialize()
    {
        Log("Initializing");

        SetCameraStuff();
        SetQualityStuff();

        Log("Initialized");
    }

    private void SetCallbacks()
    {
        UScenes.SceneManager.activeSceneChanged += (_, toScene) => { SetBlur(toScene); };
    }

    private void SetBlur(UScenes.Scene scene)
    {
        GameObject blurPlane = scene.Find("BlurPlane");
        if (blurPlane == null) return;
        blurPlane.SetActive(GlobalSettings.CameraBlurEnabled);
    }

    private void SetCameraStuff()
    {
        GameCameras.instance.mainCamera.gameObject.GetComponent<BloomOptimized>().enabled =
            GlobalSettings.CameraBloomEnabled;
        GameCameras.instance.mainCamera.gameObject.GetComponent<FastNoise>().enabled =
            GlobalSettings.CameraFilmGrainEnabled;
        GameCameras.instance.mainCamera.allowDynamicResolution = GlobalSettings.CameraDynamicResolutionAllowed;
        GameCameras.instance.mainCamera.allowHDR = GlobalSettings.CameraHdrEnabled;
        GameCameras.instance.mainCamera.allowMSAA = GlobalSettings.CameraMsaaAllowed;
        GameCameras.instance.mainCamera.useOcclusionCulling = GlobalSettings.CameraUseOcclusionCulling;
        GameCameras.instance.mainCamera.usePhysicalProperties = GlobalSettings.CameraUsePhysicalProperties;
    }

    private void SetQualityStuff()
    {
        QualitySettings.anisotropicFiltering = GlobalSettings.QualityAnisotropicFiltering;
        QualitySettings.antiAliasing = GlobalSettings.QualityAntiAliasing;
        QualitySettings.asyncUploadBufferSize = GlobalSettings.QualityAsyncUploadBufferSize;
        QualitySettings.asyncUploadPersistentBuffer = GlobalSettings.QualityAsyncUploadPersistentBuffer;
        QualitySettings.asyncUploadTimeSlice = GlobalSettings.QualityAsyncUploadTimeSlice;
        QualitySettings.billboardsFaceCameraPosition = GlobalSettings.QualityBillboardsFaceCameraPosition;
        QualitySettings.lodBias = GlobalSettings.QualityLodBias;
        QualitySettings.maximumLODLevel = GlobalSettings.QualityMaximumLODLevel;
        QualitySettings.particleRaycastBudget = GlobalSettings.QualityParticleRaycastBudget;
        QualitySettings.pixelLightCount = GlobalSettings.QualityPixelLightCount;
        QualitySettings.realtimeReflectionProbes = GlobalSettings.QualityRealtimeReflectionProbes;
        QualitySettings.resolutionScalingFixedDPIFactor = GlobalSettings.QualityResolutionScalingFixedDPIFactor;
        QualitySettings.shadowCascade2Split = GlobalSettings.QualityShadowCascade2Split;
        QualitySettings.shadowCascade4Split = GlobalSettings.QualityShadowCascade4Split;
        QualitySettings.shadowCascades = GlobalSettings.QualityShadowCascades;
        QualitySettings.shadowDistance = GlobalSettings.QualityShadowDistance;
        QualitySettings.shadowmaskMode = GlobalSettings.QualityShadowmaskMode;
        QualitySettings.shadowNearPlaneOffset = GlobalSettings.QualityShadowNearPlaneOffset;
        QualitySettings.shadowProjection = GlobalSettings.QualityShadowProjection;
        QualitySettings.shadowResolution = GlobalSettings.QualityShadowResolution;
        QualitySettings.shadows = GlobalSettings.QualityShadows;
        QualitySettings.skinWeights = GlobalSettings.QualitySkinWeights;
        QualitySettings.softParticles = GlobalSettings.QualitySoftParticles;
        QualitySettings.softVegetation = GlobalSettings.QualitySoftVegetation;
        QualitySettings.streamingMipmapsActive = GlobalSettings.QualityStreamingMipmapsActive;
        QualitySettings.streamingMipmapsAddAllCameras = GlobalSettings.QualityStreamingMipmapsAddAllCameras;
        QualitySettings.streamingMipmapsMaxFileIORequests = GlobalSettings.QualityStreamingMipmapsMaxFileIORequests;
        QualitySettings.streamingMipmapsMaxLevelReduction = GlobalSettings.QualityStreamingMipmapsMaxLevelReduction;
        QualitySettings.streamingMipmapsMemoryBudget = GlobalSettings.QualityStreamingMipmapsMemoryBudget;
        QualitySettings.streamingMipmapsRenderersPerFrame = GlobalSettings.QualityStreamingMipmapsRenderersPerFrame;
        QualitySettings.vSyncCount = GlobalSettings.QualityVSyncCount;
    }

    public Menu PrepareMenu()
    {
        return new Menu("Graphic Options", new Element[]
        {
            new TextPanel("Camera Options"),
            new HorizontalOption("Bloom",
                "Enables or disables bloom.",
                new[] { "Off", "On" },
                SetCameraBloomEnabled,
                GetCameraBloomEnabled),
            new HorizontalOption("Blur",
                "Enables or disables the blur effect of the background.",
                new[] { "Off", "On" },
                SetCameraBlurEnabled,
                GetCameraBlurEnabled),
            new HorizontalOption("Dynamic Resolution",
                "Enables or disables dynamic resolution scaling.",
                new[] { "Off", "On" },
                SetCameraDynamicResolutionAllowed,
                GetCameraDynamicResolutionAllowed),
            new HorizontalOption("Film Grain",
                "Enables or disables the film grain effect.",
                new[] { "Off", "On" },
                SetCameraFilmGrainEnabled,
                GetCameraFilmGrainEnabled),
            new HorizontalOption("Allow HDR",
                "Enables or disables high dynamic range rendering.",
                new[] { "Off", "On" },
                SetCameraHdrEnabled,
                GetCameraHdrEnabled),
            new HorizontalOption("Allow MSAA",
                "Enables or disables MSAA rendering.",
                new[] { "Off", "On" },
                SetCameraMsaaAllowed,
                GetCameraMsaaAllowed),
            new HorizontalOption("Occlusion Culling",
                "Whether or not the Camera will use occlusion culling during rendering.",
                new[] { "Off", "On" },
                SetCameraUseOcclusionCulling,
                GetCameraUseOcclusionCulling),
            new HorizontalOption("Physical Properties",
                "Enable to use physical camera properties to compute the field of view and the frustum.",
                new[] { "Off", "On" },
                SetCameraUsePhysicalProperties,
                GetCameraUsePhysicalProperties),

            new TextPanel("Quality Options"),
            new HorizontalOption("Anisotropic Filtering ",
                "Global anisotropic filtering mode.",
                new[] { "Off", "On", "Force On" },
                SetQualityAnisotropicFiltering,
                GetQualityAnisotropicFiltering),
            new HorizontalOption("Anti Aliasing ",
                "Choose the level of Multi-Sample Anti-aliasing (MSAA) that the GPU performs.",
                new[] { "Off", "2x MSAA", "4x MSAA", "8x MSAA" },
                SetQualityAntiAliasing,
                GetQualityAntiAliasing),
            new CustomSlider("Async Upload Buffer Size",
                (option) =>
                {
                    GlobalSettings.QualityAsyncUploadBufferSize = (int)option;
                    SetQualityStuff();
                },
                () => GlobalSettings.QualityAsyncUploadBufferSize, 2f, 512f, true),
            new HorizontalOption("Async Upload Persist",
                "Make ring buffer allocation persist after all upload operations have completed. Increases memory usage.",
                new[] { "Off", "On" },
                SetQualityAsyncUploadPersistentBuffer,
                GetQualityAsyncUploadPersistentBuffer),
            new CustomSlider("Async Upload Time Slice",
                (option) =>
                {
                    GlobalSettings.QualityAsyncUploadTimeSlice = (int)option;
                    SetQualityStuff();
                },
                () => GlobalSettings.QualityAsyncUploadTimeSlice, 1f, 33f, true),
            new HorizontalOption("Billboards Face Camera",
                "If enabled, billboards will face towards camera position rather than camera orientation.",
                new[] { "Off", "On" },
                SetQualityBillboardsFaceCameraPosition,
                GetQualityBillboardsFaceCameraPosition),
            new CustomSlider("LOD Bias",
                (option) =>
                {
                    GlobalSettings.QualityLodBias = option;
                    SetQualityStuff();
                },
                () => GlobalSettings.QualityLodBias, 0f, 10f, false),
            new CustomSlider("Maximum LOD Level",
                (option) =>
                {
                    GlobalSettings.QualityMaximumLODLevel = (int)option;
                    SetQualityStuff();
                },
                () => GlobalSettings.QualityMaximumLODLevel, 0f, 10f, true),
            new CustomSlider("Particle Raycast Budget",
                (option) =>
                {
                    GlobalSettings.QualityParticleRaycastBudget = (int)option;
                    SetQualityStuff();
                },
                () => GlobalSettings.QualityParticleRaycastBudget, 0f, 1000f, true),
            new CustomSlider("Pixel Light Count",
                (option) =>
                {
                    GlobalSettings.QualityPixelLightCount = (int)option;
                    SetQualityStuff();
                },
                () => GlobalSettings.QualityPixelLightCount, 0f, 100f, true),
            new HorizontalOption("Realtime Reflection Probes",
                "Enables real-time reflection probes.",
                new[] { "Off", "On" },
                SetQualityRealtimeReflectionProbes,
                GetQualityRealtimeReflectionProbes),
            new CustomSlider("Resolution Scaling",
                (option) =>
                {
                    GlobalSettings.QualityResolutionScalingFixedDPIFactor = option;
                    SetQualityStuff();
                },
                () => GlobalSettings.QualityResolutionScalingFixedDPIFactor, 0f, 10f, false),
            new CustomSlider("Shadow Cascade 2-Split",
                (option) =>
                {
                    GlobalSettings.QualityShadowCascade2Split = option;
                    SetQualityStuff();
                },
                () => GlobalSettings.QualityShadowCascade2Split, 0f, 1f, false),
            new CustomSlider("Shadow Cascade 4-Split - X",
                (option) =>
                {
                    GlobalSettings.QualityShadowCascade4Split = new Vector3(option,
                        GlobalSettings.QualityShadowCascade4Split.y, GlobalSettings.QualityShadowCascade4Split.z);
                    SetQualityStuff();
                },
                () => GlobalSettings.QualityShadowCascade2Split, 0f, 1f, false),
            new CustomSlider("Shadow Cascade 4-Split - Y",
                (option) =>
                {
                    GlobalSettings.QualityShadowCascade4Split = new Vector3(GlobalSettings.QualityShadowCascade4Split.x,
                        option, GlobalSettings.QualityShadowCascade4Split.z);
                    SetQualityStuff();
                },
                () => GlobalSettings.QualityShadowCascade2Split, 0f, 1f, false),
            new CustomSlider("Shadow Cascade 4-Split - Z",
                (option) =>
                {
                    GlobalSettings.QualityShadowCascade4Split = new Vector3(GlobalSettings.QualityShadowCascade4Split.x,
                        GlobalSettings.QualityShadowCascade4Split.y, option);
                    SetQualityStuff();
                },
                () => GlobalSettings.QualityShadowCascade2Split, 0f, 1f, false),
            new CustomSlider("Shadow Cascades",
                (option) =>
                {
                    GlobalSettings.QualityShadowCascades = (int)option;
                    SetQualityStuff();
                },
                () => GlobalSettings.QualityShadowCascades, 0f, 10f, true),
            new CustomSlider("Shadow Distance",
                (option) =>
                {
                    GlobalSettings.QualityShadowDistance = option;
                    SetQualityStuff();
                },
                () => GlobalSettings.QualityShadowDistance, 0f, 100f, false),
            new HorizontalOption("Shadowmask Mode",
                "The rendering mode of Shadowmask.",
                new[] { "Normal", "Distance" },
                SetQualityShadowmaskMode,
                GetQualityShadowmaskMode),
            new CustomSlider("Shadow Near Limit",
                (option) =>
                {
                    GlobalSettings.QualityShadowNearPlaneOffset = option;
                    SetQualityStuff();
                },
                () => GlobalSettings.QualityShadowNearPlaneOffset, 0f, 10f, false),
            new HorizontalOption("Shadow Projection",
                "Directional light shadow projection.",
                new[] { "Close", "Stable" },
                SetQualityShadowProjection,
                GetQualityShadowProjection),
            new HorizontalOption("Shadow Resolution",
                "The default resolution of the shadow maps.",
                new[] { "Low", "Medium", "High", "Very High" },
                SetQualityShadowResolution,
                GetQualityShadowResolution),
            new HorizontalOption("Shadows",
                "Real-time Shadows type to be used.",
                new[] { "Off", "Hard", "All" },
                SetQualityShadows,
                GetQualityShadows),
            new HorizontalOption("Skin Weights",
                "The maximum number of bones per vertex that are taken into account during skinning.",
                new[] { "One", "Two", "Four", "Unlimited" },
                SetQualitySkinWeights,
                GetQualitySkinWeights),
            new HorizontalOption("Soft Particles",
                "Should soft blending be used for particles?",
                new[] { "Off", "On" },
                SetQualitySoftParticles,
                GetQualitySoftParticles),
            new HorizontalOption("Soft Vegetation",
                "Use a two-pass shader for the vegetation in the terrain engine.",
                new[] { "Off", "On" },
                SetQualitySoftVegetation,
                GetQualitySoftVegetation),
            new HorizontalOption("Mipmaps Active",
                "Enable automatic streaming of texture mipmap levels based on their distance from all active cameras.",
                new[] { "Off", "On" },
                SetQualityStreamingMipmapsActive,
                GetQualityStreamingMipmapsActive),
            new HorizontalOption("Mipmaps All Cameras",
                "Process all enabled Cameras for texture streaming.",
                new[] { "Off", "On" },
                SetQualityStreamingMipmapsAddAllCameras,
                GetQualityStreamingMipmapsAddAllCameras),
            new CustomSlider("Mipmaps Max IO",
                (option) =>
                {
                    GlobalSettings.QualityStreamingMipmapsMaxFileIORequests = (int)option;
                    SetQualityStuff();
                },
                () => GlobalSettings.QualityStreamingMipmapsMaxFileIORequests, 0f, 5120f, true),
            new CustomSlider("Mipmaps Max Reduction",
                (option) =>
                {
                    GlobalSettings.QualityStreamingMipmapsMaxLevelReduction = (int)option;
                    SetQualityStuff();
                },
                () => GlobalSettings.QualityStreamingMipmapsMaxLevelReduction, 0f, 10f, true),
            new CustomSlider("Mipmaps Memory",
                (option) =>
                {
                    GlobalSettings.QualityStreamingMipmapsMemoryBudget = option;
                    SetQualityStuff();
                },
                () => GlobalSettings.QualityStreamingMipmapsMemoryBudget, 0f, 4096f, false),
            new CustomSlider("Mipmaps Renderers",
                (option) =>
                {
                    GlobalSettings.QualityStreamingMipmapsRenderersPerFrame = (int)option;
                    SetQualityStuff();
                },
                () => GlobalSettings.QualityStreamingMipmapsRenderersPerFrame, 0f, 4096f, true),
            new HorizontalOption("VSync",
                "The number of vertical syncs that should pass between each frame.",
                new[] { "Off", "Refresh Rate", "Half Refresh Rate", "Third Refresh Rate", "Quarter Refresh Rate" },
                SetQualityVSyncCount,
                GetQualityVSyncCount),
        });
    }

    public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
    {
        MenuRef ??= PrepareMenu();

        return MenuRef.GetMenuScreen(modListMenu);
    }

    #region Setters and Getters

    private void SetCameraBloomEnabled(int option)
    {
        GlobalSettings.CameraBloomEnabled = IntToBool(option);
        SetCameraStuff();
    }

    private int GetCameraBloomEnabled()
    {
        return BoolToInt(GlobalSettings.CameraBloomEnabled);
    }

    private void SetCameraBlurEnabled(int option)
    {
        GlobalSettings.CameraBlurEnabled = IntToBool(option);
        SetBlur(UScenes.SceneManager.GetActiveScene());
    }

    private int GetCameraBlurEnabled()
    {
        return BoolToInt(GlobalSettings.CameraBlurEnabled);
    }

    private void SetCameraDynamicResolutionAllowed(int option)
    {
        GlobalSettings.CameraDynamicResolutionAllowed = IntToBool(option);
        SetCameraStuff();
    }

    private int GetCameraDynamicResolutionAllowed()
    {
        return BoolToInt(GlobalSettings.CameraDynamicResolutionAllowed);
    }

    private void SetCameraFilmGrainEnabled(int option)
    {
        GlobalSettings.CameraFilmGrainEnabled = IntToBool(option);
        SetCameraStuff();
    }

    private int GetCameraFilmGrainEnabled()
    {
        return BoolToInt(GlobalSettings.CameraFilmGrainEnabled);
    }

    private void SetCameraHdrEnabled(int option)
    {
        GlobalSettings.CameraHdrEnabled = IntToBool(option);
        SetCameraStuff();
    }

    private int GetCameraHdrEnabled()
    {
        return BoolToInt(GlobalSettings.CameraHdrEnabled);
    }

    private void SetCameraMsaaAllowed(int option)
    {
        GlobalSettings.CameraMsaaAllowed = IntToBool(option);
        SetCameraStuff();
    }

    private int GetCameraMsaaAllowed()
    {
        return BoolToInt(GlobalSettings.CameraMsaaAllowed);
    }

    private void SetCameraUseOcclusionCulling(int option)
    {
        GlobalSettings.CameraUseOcclusionCulling = IntToBool(option);
        SetCameraStuff();
    }

    private int GetCameraUseOcclusionCulling()
    {
        return BoolToInt(GlobalSettings.CameraUseOcclusionCulling);
    }

    private void SetCameraUsePhysicalProperties(int option)
    {
        GlobalSettings.CameraUsePhysicalProperties = IntToBool(option);
        SetCameraStuff();
    }

    private int GetCameraUsePhysicalProperties()
    {
        return BoolToInt(GlobalSettings.CameraUsePhysicalProperties);
    }

    private void SetQualityAnisotropicFiltering(int option)
    {
        GlobalSettings.QualityAnisotropicFiltering = option switch
        {
            0 => AnisotropicFiltering.Disable,
            1 => AnisotropicFiltering.Enable,
            2 => AnisotropicFiltering.ForceEnable,
            _ => AnisotropicFiltering.Disable
        };
        SetQualityStuff();
    }

    private int GetQualityAnisotropicFiltering()
    {
        return GlobalSettings.QualityAnisotropicFiltering switch
        {
            AnisotropicFiltering.Disable => 0,
            AnisotropicFiltering.Enable => 1,
            AnisotropicFiltering.ForceEnable => 2,
            _ => 0
        };
    }

    private void SetQualityAntiAliasing(int option)
    {
        GlobalSettings.QualityAntiAliasing = option switch
        {
            0 => 0,
            1 => 2,
            2 => 4,
            3 => 8,
            _ => 0
        };
        SetQualityStuff();
    }

    private int GetQualityAntiAliasing()
    {
        return GlobalSettings.QualityAntiAliasing switch
        {
            0 => 0,
            2 => 1,
            4 => 2,
            8 => 3,
            _ => 0
        };
    }

    private void SetQualityAsyncUploadPersistentBuffer(int option)
    {
        GlobalSettings.QualityAsyncUploadPersistentBuffer = IntToBool(option);
        SetQualityStuff();
    }

    private int GetQualityAsyncUploadPersistentBuffer()
    {
        return BoolToInt(GlobalSettings.QualityAsyncUploadPersistentBuffer);
    }

    private void SetQualityBillboardsFaceCameraPosition(int option)
    {
        GlobalSettings.QualityBillboardsFaceCameraPosition = IntToBool(option);
        SetQualityStuff();
    }

    private int GetQualityBillboardsFaceCameraPosition()
    {
        return BoolToInt(GlobalSettings.QualityBillboardsFaceCameraPosition);
    }

    private void SetQualityRealtimeReflectionProbes(int option)
    {
        GlobalSettings.QualityRealtimeReflectionProbes = IntToBool(option);
        SetQualityStuff();
    }

    private int GetQualityRealtimeReflectionProbes()
    {
        return BoolToInt(GlobalSettings.QualityRealtimeReflectionProbes);
    }

    private void SetQualityShadowmaskMode(int option)
    {
        GlobalSettings.QualityShadowmaskMode = option switch
        {
            0 => ShadowmaskMode.Shadowmask,
            1 => ShadowmaskMode.DistanceShadowmask,
            _ => ShadowmaskMode.Shadowmask
        };
        SetQualityStuff();
    }

    private int GetQualityShadowmaskMode()
    {
        return GlobalSettings.QualityShadowmaskMode switch
        {
            ShadowmaskMode.Shadowmask => 0,
            ShadowmaskMode.DistanceShadowmask => 1,
            _ => 0
        };
    }

    private void SetQualityShadowProjection(int option)
    {
        GlobalSettings.QualityShadowProjection = option switch
        {
            0 => ShadowProjection.CloseFit,
            1 => ShadowProjection.StableFit,
            _ => ShadowProjection.CloseFit
        };
        SetQualityStuff();
    }

    private int GetQualityShadowProjection()
    {
        return GlobalSettings.QualityShadowProjection switch
        {
            ShadowProjection.CloseFit => 0,
            ShadowProjection.StableFit => 1,
            _ => 0
        };
    }

    private void SetQualityShadowResolution(int option)
    {
        GlobalSettings.QualityShadowResolution = option switch
        {
            0 => ShadowResolution.Low,
            1 => ShadowResolution.Medium,
            2 => ShadowResolution.High,
            3 => ShadowResolution.VeryHigh,
            _ => ShadowResolution.Low
        };
        SetQualityStuff();
    }

    private int GetQualityShadowResolution()
    {
        return GlobalSettings.QualityShadowResolution switch
        {
            ShadowResolution.Low => 0,
            ShadowResolution.Medium => 1,
            ShadowResolution.High => 2,
            ShadowResolution.VeryHigh => 3,
            _ => 0
        };
    }

    private void SetQualityShadows(int option)
    {
        GlobalSettings.QualityShadows = option switch
        {
            0 => ShadowQuality.Disable,
            1 => ShadowQuality.HardOnly,
            2 => ShadowQuality.All,
            _ => ShadowQuality.Disable
        };
        SetQualityStuff();
    }

    private int GetQualityShadows()
    {
        return GlobalSettings.QualityShadows switch
        {
            ShadowQuality.Disable => 0,
            ShadowQuality.HardOnly => 1,
            ShadowQuality.All => 2,
            _ => 0
        };
    }

    private void SetQualitySkinWeights(int option)
    {
        GlobalSettings.QualitySkinWeights = option switch
        {
            0 => SkinWeights.OneBone,
            1 => SkinWeights.TwoBones,
            2 => SkinWeights.FourBones,
            3 => SkinWeights.Unlimited,
            _ => SkinWeights.OneBone
        };
        SetQualityStuff();
    }

    private int GetQualitySkinWeights()
    {
        return GlobalSettings.QualitySkinWeights switch
        {
            SkinWeights.OneBone => 0,
            SkinWeights.TwoBones => 1,
            SkinWeights.FourBones => 2,
            SkinWeights.Unlimited => 3,
            _ => 0
        };
    }

    private void SetQualitySoftParticles(int option)
    {
        GlobalSettings.QualitySoftParticles = IntToBool(option);
        SetQualityStuff();
    }

    private int GetQualitySoftParticles()
    {
        return BoolToInt(GlobalSettings.QualitySoftParticles);
    }

    private void SetQualitySoftVegetation(int option)
    {
        GlobalSettings.QualitySoftVegetation = IntToBool(option);
        SetQualityStuff();
    }

    private int GetQualitySoftVegetation()
    {
        return BoolToInt(GlobalSettings.QualitySoftVegetation);
    }

    private void SetQualityStreamingMipmapsActive(int option)
    {
        GlobalSettings.QualityStreamingMipmapsActive = IntToBool(option);
        SetQualityStuff();
    }

    private int GetQualityStreamingMipmapsActive()
    {
        return BoolToInt(GlobalSettings.QualityStreamingMipmapsActive);
    }

    private void SetQualityStreamingMipmapsAddAllCameras(int option)
    {
        GlobalSettings.QualityStreamingMipmapsAddAllCameras = IntToBool(option);
        SetQualityStuff();
    }

    private int GetQualityStreamingMipmapsAddAllCameras()
    {
        return BoolToInt(GlobalSettings.QualityStreamingMipmapsAddAllCameras);
    }

    private void SetQualityVSyncCount(int option)
    {
        GlobalSettings.QualityVSyncCount = option switch
        {
            0 => 0,
            1 => 1,
            2 => 2,
            3 => 3,
            4 => 4,
            _ => 0
        };
        SetQualityStuff();
    }

    private int GetQualityVSyncCount()
    {
        return GlobalSettings.QualityVSyncCount switch
        {
            0 => 0,
            1 => 1,
            2 => 2,
            3 => 3,
            4 => 4,
            _ => 0
        };
    }

    private static bool IntToBool(int option)
    {
        return option switch
        {
            0 => false,
            1 => true,
            _ => false
        };
    }

    private static int BoolToInt(bool option)
    {
        return option switch
        {
            false => 0,
            true => 1
        };
    }

    #endregion
}