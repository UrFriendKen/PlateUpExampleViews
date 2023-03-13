using Kitchen;
using Unity.Entities;
using UnityEngine;

namespace KitchenExampleViews
{
    internal class CreateResponsiveViewEntity : FranchiseSystem
    {
        // Marker component to identify view in the ResponsiveViewSystemBase. See ResponsiveViewExample.UpdateView
        public struct SResponsiveViewExample : IComponentData { }

        private static bool IsViewPrefabsInitialised = false;

        protected override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnUpdate()
        {
            // Creating the view prefab and store in AssetDirectory
            // This is stopgap, and a Mod version of AssetDirectory is expected as a better alternative in the future
            if (Has<SAssetDirectory>() && !IsViewPrefabsInitialised)
            {
                // Instantiate GameObject (To become Prefab)
                GameObject go = new GameObject("ExampleViews - ResponsiveViewExample");
                // Add View Component
                go.AddComponent<ResponsiveViewExample>();
                go.hideFlags = HideFlags.HideAndDontSave;
                // Store in AssetDirectory with unique id. Potentially use KitchenLib.Utils.VariousUtils.GetID(string name) and salt with MOD_GUID
                base.AssetDirectory.ViewPrefabs.Add((ViewType)500, go);
                IsViewPrefabsInitialised = true;
            }


            // This is an example. Hence, I'm creating the view in the same update method
            // This should not be taken as a legitamate application of the view
            // You can ignore this check if it does not apply to your application
            if (Has<SResponsiveViewExample>() || !IsViewPrefabsInitialised)
            {
                return;
            }


            // Create new entity that will be linked to the view (when required)
            Entity entity = EntityManager.CreateEntity();
            // Add CRequiresView component. The GameObject will be instantiated for each player
            // The Prefab to be used depends on the ViewType
            // A CLinkedView will be automatically attached to the entity with a unique Identifier that marks which GameObject is linked to this entity
            // See ResponsiveViewExample.UpdateView.OnUpdate()
            Set(entity, new CRequiresView
            {
                Type = (ViewType)500
            });
            // Add marker so it can be identified and acted on by the correct view system
            Set<SResponsiveViewExample>(entity);
            
            // As this is an example, I am marking the entity so that it is temporary and will not be saved
            // You can ignore this
            Set<CDoNotPersist>(entity);
        }
    }
}
