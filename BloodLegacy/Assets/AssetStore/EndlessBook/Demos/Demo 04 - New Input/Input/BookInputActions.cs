//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/EndlessBook/Demos/Demo 04 - New Input/Input/BookInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @BookInputActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @BookInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""BookInputActions"",
    ""maps"": [
        {
            ""name"": ""TouchPad"",
            ""id"": ""514e45fc-854a-4e94-91b1-5968d304fa20"",
            ""actions"": [
                {
                    ""name"": ""Press"",
                    ""type"": ""Button"",
                    ""id"": ""782490ec-7888-4be1-8bfe-6eba57b7f04d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""544739c9-731b-4d1f-98a3-007fa0933d3a"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Press"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // TouchPad
        m_TouchPad = asset.FindActionMap("TouchPad", throwIfNotFound: true);
        m_TouchPad_Press = m_TouchPad.FindAction("Press", throwIfNotFound: true);
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
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // TouchPad
    private readonly InputActionMap m_TouchPad;
    private ITouchPadActions m_TouchPadActionsCallbackInterface;
    private readonly InputAction m_TouchPad_Press;
    public struct TouchPadActions
    {
        private @BookInputActions m_Wrapper;
        public TouchPadActions(@BookInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Press => m_Wrapper.m_TouchPad_Press;
        public InputActionMap Get() { return m_Wrapper.m_TouchPad; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(TouchPadActions set) { return set.Get(); }
        public void SetCallbacks(ITouchPadActions instance)
        {
            if (m_Wrapper.m_TouchPadActionsCallbackInterface != null)
            {
                @Press.started -= m_Wrapper.m_TouchPadActionsCallbackInterface.OnPress;
                @Press.performed -= m_Wrapper.m_TouchPadActionsCallbackInterface.OnPress;
                @Press.canceled -= m_Wrapper.m_TouchPadActionsCallbackInterface.OnPress;
            }
            m_Wrapper.m_TouchPadActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Press.started += instance.OnPress;
                @Press.performed += instance.OnPress;
                @Press.canceled += instance.OnPress;
            }
        }
    }
    public TouchPadActions @TouchPad => new TouchPadActions(this);
    public interface ITouchPadActions
    {
        void OnPress(InputAction.CallbackContext context);
    }
}