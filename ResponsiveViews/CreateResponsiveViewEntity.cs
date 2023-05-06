using Kitchen;
using Unity.Entities;

namespace KitchenExampleViews.ResponsiveViews
{
    internal class CreateResponsiveViewEntity : GameSystemBase
    {
        // Marker component to identify view in the ResponsiveViewSystemBase. See ResponsiveViewExample.UpdateView
        public struct SResponsiveViewExample : IComponentData { }

        protected override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnUpdate()
        {
            // This is an example. Hence, I'm creating the view in the same update method
            // This should not be taken as a legitamate application of the view
            // You can ignore this check if it does not apply to your application
            if (Has<SResponsiveViewExample>())// || !(Main.instance).IsViewPrefabsInitialised)
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
            // You can ignore this if you want the entity to store persistent data that will be tied to the save file.
            Set<CDoNotPersist>(entity);
        }
    }
}
