using KitchenLib;
using KitchenMods;
using System.Reflection;
using UnityEngine;

// Namespace should have "Kitchen" in the beginning
namespace KitchenExampleViews
{
    public class Main : BaseMod, IModSystem
    {
        // GUID must be unique and is recommended to be in reverse domain name notation
        // Mod Name is displayed to the player and listed in the mods menu
        // Mod Version must follow semver notation e.g. "1.2.3"
        public const string MOD_GUID = "IcedMilo.PlateUp.ExampleViews";
        public const string MOD_NAME = "Example Views";
        public const string MOD_VERSION = "0.1.0";
        public const string MOD_AUTHOR = "IcedMilo";
        public const string MOD_GAMEVERSION = ">=1.1.4";
        // Game version this mod is designed for in semver
        // e.g. ">=1.1.3" current and all future
        // e.g. ">=1.1.3 <=1.2.3" for all from/until

        // Boolean constant whose value depends on whether you built with DEBUG or RELEASE mode, useful for testing
#if DEBUG
        public const bool DEBUG_MODE = true;
#else
        public const bool DEBUG_MODE = false;
#endif

        //internal static new Main instance;

        //internal bool IsViewPrefabsInitialised => AssetDirectory?.ViewPrefabs?.ContainsKey((ViewType)500) ?? false;

        public static AssetBundle Bundle;

        public Main() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
        }

        protected override void OnUpdate()
        {
            //if (AssetDirectory == null || AssetDirectory.ViewPrefabs == null)
            //{
            //    return;
            //}

            //// Creating the view prefab and store in AssetDirectory
            //// This is stopgap, and a Mod version of AssetDirectory is expected as a better alternative in the future (maybe)
            //if (!AssetDirectory.ViewPrefabs.ContainsKey((ViewType)500))
            //{
            //    // Instantiate GameObject (To become Prefab)
            //    GameObject go = new GameObject("ExampleViews - ResponsiveViewExample");
            //    // Add View Component
            //    go.AddComponent<ResponsiveViewExample>();
            //    go.hideFlags = HideFlags.HideAndDontSave;
            //    // Store in AssetDirectory with unique id. Potentially use KitchenLib.Utils.VariousUtils.GetID(string name) and salt with MOD_GUID
            //    AssetDirectory.ViewPrefabs.Add((ViewType)500, go);
            //    Main.LogInfo("Added (ViewType)500 to AssetDirectory.ViewPrefabs");
            //}
        }

        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
            // Perform actions when game data is built
            //Events.BuildGameDataEvent += delegate (object s, BuildGameDataEventArgs args)
            //{
            //    if (args.firstBuild)
            //    {
            //        Appliance blueprintCabinet = args.gamedata.Get<Appliance>(ApplianceReferences.BlueprintCabinet);
            //        GameObject gameObject = new GameObject();
            //        gameObject.AddComponent<ResponsiveSubviewExample>();
            //        gameObject.transform.parent = blueprintCabinet.Prefab.transform;
            //    }
            //};
        }
        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
