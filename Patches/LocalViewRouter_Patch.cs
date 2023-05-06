using HarmonyLib;
using Kitchen;
using UnityEngine;

namespace KitchenExampleViews.Patches
{
    [HarmonyPatch]
    static class LocalViewRouter_Patch
    {
        static GameObject _container;
        static GameObject _responsiveViewPrefab;

        [HarmonyPatch(typeof(LocalViewRouter), "GetPrefab")]
        [HarmonyPrefix]
        static bool GetPrefab_Prefix(ViewType view_type, ref GameObject __result)
        {
            if (view_type == (ViewType)500)
            {
                if (_container == null)
                {
                    _container = new GameObject("Example Prefab Hider");
                    _container.SetActive(false);
                }

                if (_responsiveViewPrefab == null)
                {
                    // Instead of creating a new GameObject, you can elect to create a prefab in Unity (which can include other child GameObjects and components that serve your needs)
                    _responsiveViewPrefab = new GameObject("ExampleViews - ResponsiveViewExample");
                    _responsiveViewPrefab.transform.SetParent(_container.transform);

                    // Add Main View Component
                    _responsiveViewPrefab.AddComponent<ResponsiveViewExample>();
                    Main.LogInfo("Created (ViewType)500");

                    // Subviews can be added onto any prefab as well. Subviews receive an update when its ViewData is sent to the the main view; In this case (ViewType)500
                    _responsiveViewPrefab.AddComponent<ResponsiveSubviewExample>();
                }
                __result = _responsiveViewPrefab;
                return false;
            }
            return true;
        }
    }
}
