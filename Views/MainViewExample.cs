using Controllers;
using Kitchen;
using KitchenMods;
using MessagePack;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace KitchenExampleViews.Views
{
    // Use ResponsiveObjectView if you require receiving data from clients. Otherwise, use UpdatableObjectView instead.
    public class MainViewExample : ResponsiveObjectView<MainViewExample.ViewData, MainViewExample.ResponseData>
    {
        // Nesting the ViewSystemBase within the View class like this is not a requirement, but helps simplify referencing ViewData and keeps everything organised within the view.
        /// <summary>
        /// ECS View System
        /// Runs on host and updates views to be broadcasted to clientsYou 
        /// Receives responses from the client to be processed (For Responsive Views)
        /// 
        /// You can use IncrementalViewSystemBase<T> for non-responsive views
        /// Use ResponsiveViewSystemBase<TView, TResp> for responsive views
        /// 
        /// </summary>

        public class UpdateView : ResponsiveViewSystemBase<ViewData, ResponseData>, IModSystem
        {
            EntityQuery Query;

            // Some private fields used for example. Can be ignored
            private bool wasPressed;
            private KeyControl sendUpdateKey = Keyboard.current.tKey;

            protected override void Initialise()
            {
                base.Initialise();

                // Cache Entity Queries
                // This should contain ALL IComponentData that will be used in the class
                Query = GetEntityQuery(typeof(CLinkedView), typeof(CreateViewEntity.SViewExample));
            }

            protected override void OnUpdate()
            {
                using NativeArray<CLinkedView> linkedViews = Query.ToComponentDataArray<CLinkedView>(Allocator.Temp);
                foreach (CLinkedView view in linkedViews)
                {
                    CPlayer cplayer = new CPlayer();

                    cplayer.InputSource = InputSourceIdentifier.Identifier;

                    // For example. If key is pressed send update.
                    // This behavior can be ignored, and a simple SendUpdate can be done instead with the appropriate data for the view.
                    if (sendUpdateKey.isPressed)
                    {
                        if (!wasPressed)
                        {
                            SendUpdate(view.Identifier, new ViewData
                            {
                                Source = InputSourceIdentifier.Identifier
                            });
                        }
                        wasPressed = true;
                    }
                    else wasPressed = false;


                    // You can ignore this if inheriting IncrementalViewSystemBase instead of ResponsiveViewSystemBase
                    // protected bool ApplyUpdates(ViewIdentifier identifier, Action<TResp> act, bool only_final_update = false)
                    // As this is a subview, identifier refers to the main view identifier
                    // act is performed for each ResponseData packet received
                    // only_final_update makes act only performed for the latest packet. The rest are ignored.
                    // Set only_final_update to false if you need something to happen for every packet sent, in the event more than 1 packet is received this frame
                    if (ApplyUpdates(view.Identifier, PerformUpdateWithResponse, only_final_update: true))
                    {
                        // Do something if at least one ResponseData packet was processed this frame for the specified view
                        Main.LogInfo("Received some data!");
                    }
                }
            }

            private void PerformUpdateWithResponse(ResponseData data)
            {
                // Do something for each ResponseData packet received
                // This is ECS only
                Main.LogInfo($"{data.Text} (From {(data.Sender == InputSourceIdentifier.Identifier ? "local" : "remote")})");

            }
        }


        // Definition of Message Packet that will be broadcasted to clients
        // This should contain the minimum amount of data necessary to perform the view's function.
        // You MUST mark your ViewData as MessagePackObject
        // If you don't, the game will run locally but fail in multiplayer
        [MessagePackObject(false)]
        public class ViewData : IViewData, IViewData.ICheckForChanges<ViewData>
        {
            // You MUST also and mark each field with a key
            // All players must be running versions of the game with the same assigned keys.
            // It is recommended not to change keys after releasing your mod
            // The specifc key used does not matter, as long as there is no overlap.
            [Key(0)]
            public int Source;



            // IsChangedFrom is necessary for IncrementalViewSystemBase<T> to determine if a View Update has to be broadcasted.
            /// <summary>
            /// Check if data has changed since last update. This is called by view system to determine if an update should be sent
            /// </summary>
            /// <param name="cached">Cached state from last update</param>
            /// <returns>Returns true if data has changed, false otherwise</returns>
            public bool IsChangedFrom(ViewData check)
            {
                return true;
            }
        }

        // Some private fields used for example. Can be ignored
        private bool wasPressed = false;
        private int counter = 0;
        private KeyControl sendResponseKey = Keyboard.current.yKey;



        // This is called when a ViewData packet is received
        protected override void UpdateData(ViewData data)
        {
            // Perform any view updates here
            // Remember that this is Monobehaviour, not ECS
            // Eg. You can change whether a GameObject is active or not
            if (data.Source == InputSourceIdentifier.Identifier)
                Main.LogInfo("Local client received");
            else
                Main.LogInfo("Remote client received");
        }




        // Definition of Message Packet that will be sent back to host via a callback
        // This should contain the minimum amount of data necessary to perform the view's function.
        // You MUST mark your ViewData as MessagePackObject
        // If you don't, the game will run locally but fail in multiplayer
        [MessagePackObject(false)]
        public class ResponseData : IResponseData
        {
            // You MUST also and mark each field with a key
            // All players must be running versions of the game with the same assigned keys.
            // It is recommended not to change keys after releasing your mod
            // The specifc key used does not matter, as long as there is no overlap.
            [Key(0)] public string Text;
            [Key(1)] public int Sender;
        }


        // HasStateUpdate is from ResponsiveViewObject. You can remove this if you are creating a non-responsive view.
        // This is called every frame on the client
        // An update will be broadcasted back to the host if return is true with the response data
        // It is picked up by `bool ResponsiveViewSystemBase.(ViewIdentifier identifier, Action<TResp> act, bool only_final_update = false)`
        public override bool HasStateUpdate(out IResponseData state)
        {
            // Remember that this is Monobehaviour, not ECS
            // Assign state with the appropriate data and return true if an update is to be sent
            // Otherwise, return false. The value of state does not matter in this case
            state = null;
            if (sendResponseKey.isPressed)
            {
                if (!wasPressed)
                {
                    state = new ResponseData
                    {
                        Text = $"Response {counter++}",
                        Sender = InputSourceIdentifier.Identifier
                    };
                    wasPressed = true;
                    return true;
                }
            }
            else wasPressed = false;

            return false;
        }
    }
}
