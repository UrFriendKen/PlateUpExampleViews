using Kitchen;
using Unity.Entities;

namespace KitchenExampleViews.Views
{
    internal class CreateViewEntity : GameSystemBase
    {
        // Marker component to identify view
        public struct SViewExample : IComponentData { }

        protected override void Initialise()
        {
            base.Initialise();
        }

        protected override void OnUpdate()
        {
            if (Has<SViewExample>())
            {
                return;
            }

            // Create new entity that will be linked to the view, if necessary
            Entity entity = EntityManager.CreateEntity();
            // Add CRequiresView component. The GameObject will be instantiated for each player
            // The Prefab to be used depends on the ViewType
            // A CLinkedView will be automatically attached to the entity with a unique Identifier that marks which GameObject is linked to this entity
            Set(entity, new CRequiresView
            {
                Type = (ViewType)500
            });
            // Add marker so it can be identified and acted on by the correct view system
            Set<SViewExample>(entity);

            // As this is an example, I am marking the entity so that it is temporary and will not be saved
            // You can ignore this if you want the entity to store persistent data that will be tied to the save file.
            Set<CDoNotPersist>(entity);

            // Add component to keep data through scene transitions, if necessary
            Set<CPersistThroughSceneChanges>(entity);
        }
    }
}
