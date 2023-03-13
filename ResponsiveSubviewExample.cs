using Controllers;
using Kitchen;
using KitchenMods;
using MessagePack;
using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace KitchenExampleViews
{
    public class ResponsiveSubviewExample : UpdatableObjectView<ResponsiveSubviewExample.ViewData>, ISpecificViewResponse
    {
        // Nesting the ViewSystemBase within the View class like this is not a requirement, but helps simplify referencing ViewData and keeps everything organised within the view.
        /// <summary>
        /// ECS Reponsive View System
        /// Runs on host and updates views to be broadcasted to clients
        /// Receives responses from the client to be processed
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
                Query = GetEntityQuery(typeof(CLinkedView), typeof(CBlueprintStore));
            }

            protected override void OnUpdate()
            {
                using NativeArray<CLinkedView> linkedViews = Query.ToComponentDataArray<CLinkedView>(Allocator.Temp);
                foreach (CLinkedView view in linkedViews)
                {
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
                Main.LogInfo(data.Text);
            }
        }



        // Definition of Message Packet that will be broadcasted to clients
        // This should contain the minimum amount of data necessary to perform the view's function.
        // You MUST mark your ViewData as MessagePackObject
        // If you don't, the game will run locally but fail in multiplayer
        [MessagePackObject(false)]
        public class ViewData : ISpecificViewData, IViewData.ICheckForChanges<ViewData>
        {
            // You MUST also and mark each field with a key
            // All players must be running versions of the game with the same assigned keys.
            // It is recommended not to change keys after releasing your mod
            // The specifc key used does not matter, as long as there is no overlap.
            [Key(0)]
            public int Source;



            /// <summary>
            /// Find cached subview instance within a prefab from its main view
            /// </summary>
            /// <param name="view">Main view (eg. ApplianceView/ItemView/etc.)</param>
            /// <returns>Subview instance of type T</returns>
            public IUpdatableObject GetRelevantSubview(IObjectView view)
            {
                return view.GetSubView<ResponsiveSubviewExample>();
            }



            // This is a feature of IncrementalViewSystemBase<T>, which ResponsiveViewSystemBase<TView, TResp> inherits
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




        // Definition of Message Packet that will be sent back to host via a callback
        // This should contain the minimum amount of data necessary to perform the view's function.
        // You MUST mark your ViewData as MessagePackObject
        // If you don't, the game will run locally but fail in multiplayer
        [MessagePackObject(false)]
        public class ResponseData : IResponseData, IViewResponseData
        {
            // You MUST also and mark each field with a key
            // All players must be running versions of the game with the same assigned keys.
            // It is recommended not to change keys after releasing your mod
            // The specifc key used does not matter, as long as there is no overlap.
            [Key(0)]
            public string Text;
        }


        // Cached callback to send data back to host.
        // First parameter is the ResponseData instance
        // Second parameter is typeof(ResponseData). This is used to identify the view system that will handle the response
        // Callback is initialized after the first ViewData is received
        private Action<IResponseData, Type> Callback;


        // Some private fields used for example. Can be ignored
        private bool wasPressed = false;
        private int counter = 0;
        private KeyControl incrementCounterKey = Keyboard.current.yKey;


        // This runs locally for each client every frame
        public void Update()
        {
            // Remember that this is Monobehaviour, not ECS
            // Use this to prepare the response data to be sent
            // You can use Callback here as well. But must perform a null check, since Callback may not be initialized (I'm not sure I recommend this XD)

            if (incrementCounterKey.isPressed)
            {
                if (!wasPressed)
                {
                    Main.LogInfo($"Incremented counter to {++counter}");
                }
                wasPressed = true;
            }
            else wasPressed = false;
        }

        protected override void UpdateData(ViewData data)
        {
            // Perform any view updates here
            // Remember that this is Monobehaviour, not ECS
            // Eg. You can change whether a GameObject is active or not
            if (data.Source == InputSourceIdentifier.Identifier)
                Main.LogInfo("Local client received");
            else
                Main.LogInfo("Remote client received");


            // Callback will be null on first UpdateData
            // Expect a NullReferenceException to be thrown
            Callback.Invoke(new ResponseData
            {
                Text = $"{counter} (Sent by {(data.Source == InputSourceIdentifier.Identifier ? "local" : "remote")})"
            }, typeof(ResponseData));
            counter = 0;
        }



        // This is automatically called after each UpdateData call
        // Hence, this is when Callback is initialized
        public void SetCallback(Action<IResponseData, Type> callback)
        {
            // Cache callback to send data back to host.
            Callback = callback;
        }
    }
}
