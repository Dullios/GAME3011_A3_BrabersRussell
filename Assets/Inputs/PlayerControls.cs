// GENERATED AUTOMATICALLY FROM 'Assets/Inputs/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""PlayerActionMap"",
            ""id"": ""ac8774cd-43aa-4613-807b-359c6fdb1e0c"",
            ""actions"": [
                {
                    ""name"": ""Select"",
                    ""type"": ""Button"",
                    ""id"": ""a8d50b48-e5ca-4a1d-911d-217a32409588"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""359b44b6-9ff7-4750-851b-324ac0645483"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""6a3900c1-af70-41fc-979f-1d03cb0c9054"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""be579c13-f64c-47b2-be52-0fd048577700"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // PlayerActionMap
        m_PlayerActionMap = asset.FindActionMap("PlayerActionMap", throwIfNotFound: true);
        m_PlayerActionMap_Select = m_PlayerActionMap.FindAction("Select", throwIfNotFound: true);
        m_PlayerActionMap_MousePosition = m_PlayerActionMap.FindAction("MousePosition", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // PlayerActionMap
    private readonly InputActionMap m_PlayerActionMap;
    private IPlayerActionMapActions m_PlayerActionMapActionsCallbackInterface;
    private readonly InputAction m_PlayerActionMap_Select;
    private readonly InputAction m_PlayerActionMap_MousePosition;
    public struct PlayerActionMapActions
    {
        private @PlayerControls m_Wrapper;
        public PlayerActionMapActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Select => m_Wrapper.m_PlayerActionMap_Select;
        public InputAction @MousePosition => m_Wrapper.m_PlayerActionMap_MousePosition;
        public InputActionMap Get() { return m_Wrapper.m_PlayerActionMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActionMapActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActionMapActions instance)
        {
            if (m_Wrapper.m_PlayerActionMapActionsCallbackInterface != null)
            {
                @Select.started -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnSelect;
                @Select.performed -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnSelect;
                @Select.canceled -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnSelect;
                @MousePosition.started -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnMousePosition;
                @MousePosition.performed -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnMousePosition;
                @MousePosition.canceled -= m_Wrapper.m_PlayerActionMapActionsCallbackInterface.OnMousePosition;
            }
            m_Wrapper.m_PlayerActionMapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Select.started += instance.OnSelect;
                @Select.performed += instance.OnSelect;
                @Select.canceled += instance.OnSelect;
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
            }
        }
    }
    public PlayerActionMapActions @PlayerActionMap => new PlayerActionMapActions(this);
    public interface IPlayerActionMapActions
    {
        void OnSelect(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
    }
}
