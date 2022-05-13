// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/UI/PlayerControls/ControlMapping.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @ControlMapping : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @ControlMapping()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""ControlMapping"",
    ""maps"": [
        {
            ""name"": ""Actionmap1"",
            ""id"": ""55ba2fb5-e26c-477c-9f57-acc20eb9ac09"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""f1c2c6f3-2542-45df-8f4e-7660f5fe7215"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LookCamera"",
                    ""type"": ""Value"",
                    ""id"": ""c189c0be-0f75-45a9-94b3-d0dd52a2af19"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""9a56b898-3054-40a9-8d2e-0583e12cf6b3"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""ec0e7879-f8bd-4a05-8800-b9072a51cb21"",
                    ""path"": ""<Keyboard>/numpad8"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse;Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""32643cd1-2b6b-4906-b490-acdebf557b8b"",
                    ""path"": ""<Keyboard>/numpad5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse;Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""e127598c-a7df-435e-8bef-a3729e07cd84"",
                    ""path"": ""<Keyboard>/numpad4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse;Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""93c98071-3376-40f4-92c1-3abfa49a2d9c"",
                    ""path"": ""<Keyboard>/numpad6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse;Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector _2"",
                    ""id"": ""3c9ff928-bcd0-4d9e-b369-67fa808e18c5"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""149852c8-e380-4408-a0fb-3ad271a4d960"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse;Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""585d2784-c676-4972-b4d5-c6e01ebcc1b5"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse;Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""cb0f3e9a-7486-467a-ad99-c32b33848c58"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse;Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""a50f6022-a01b-4188-9292-dd6c13a6b65e"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse;Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""d4170212-8606-46f7-94be-9a9ff4a741a8"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LookCamera"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""5d6c453d-e001-480e-a604-953e77642d6c"",
                    ""path"": ""<Keyboard>/numpad3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""LookCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""97c3ed7d-c04a-4f94-b639-0fc28808557d"",
                    ""path"": ""<Keyboard>/numpad1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""LookCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""678190c9-2651-42f0-9645-57e650c66c1e"",
                    ""path"": ""<Keyboard>/numpad7"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""LookCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""d22fd6f5-e74c-4d44-8b78-c2b37fc1fb20"",
                    ""path"": ""<Keyboard>/numpad9"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""LookCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Actnmp_HC_Bttn"",
            ""id"": ""c5d06d07-2701-43c0-b3d3-64f20b18c93e"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""96b5cec0-5b2c-40af-9187-aa9025e85924"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LookCamera"",
                    ""type"": ""Value"",
                    ""id"": ""ba69a09b-82ef-41b4-ada5-68abbd4497eb"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""6a4f2c07-cad2-4a2d-9e91-7903b76475bf"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""ba0d317c-b756-42f0-b3db-052fc72aa17b"",
                    ""path"": ""<Keyboard>/numpad8"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""6b778def-7a6a-4a72-b2c3-0970c201d2f1"",
                    ""path"": ""<Keyboard>/numpad5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8f515e82-ac82-4e37-ab89-1f7cdb8f9ffc"",
                    ""path"": ""<Keyboard>/numpad4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""2fc809bb-8391-46b2-9044-3acc5410fb8c"",
                    ""path"": ""<Keyboard>/numpad6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector _2"",
                    ""id"": ""139f0316-2182-4790-b325-26fdab97a763"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""31fdda8e-7547-42b7-8d76-3519c726a994"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""17673297-b36e-47a5-9759-a21c782fcd46"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""9afd8031-3a04-44d3-9b4b-2f5649ee0a8d"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""a34d795e-b1d2-4a30-acb2-3b66383fca58"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""7a953121-852c-4112-8ced-236bb1d00cf5"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LookCamera"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f84aacf1-8ce8-4377-9611-eadfc42b1093"",
                    ""path"": ""<Keyboard>/numpad3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Android"",
                    ""action"": ""LookCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""d0c62b39-0710-4d47-90e7-53d69f95e3d7"",
                    ""path"": ""<Keyboard>/numpad1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Android"",
                    ""action"": ""LookCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""714dba28-3d94-494c-9e71-c087283bb001"",
                    ""path"": ""<Keyboard>/numpad7"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Android"",
                    ""action"": ""LookCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""04f70bd9-96b8-4988-99e9-b6fe025c0595"",
                    ""path"": ""<Keyboard>/numpad9"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Android"",
                    ""action"": ""LookCamera"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KeyboardMouse"",
            ""bindingGroup"": ""KeyboardMouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""test"",
            ""bindingGroup"": ""test"",
            ""devices"": []
        },
        {
            ""name"": ""Android"",
            ""bindingGroup"": ""Android"",
            ""devices"": [
                {
                    ""devicePath"": ""<Touchscreen>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Actionmap1
        m_Actionmap1 = asset.FindActionMap("Actionmap1", throwIfNotFound: true);
        m_Actionmap1_Movement = m_Actionmap1.FindAction("Movement", throwIfNotFound: true);
        m_Actionmap1_LookCamera = m_Actionmap1.FindAction("LookCamera", throwIfNotFound: true);
        // Actnmp_HC_Bttn
        m_Actnmp_HC_Bttn = asset.FindActionMap("Actnmp_HC_Bttn", throwIfNotFound: true);
        m_Actnmp_HC_Bttn_Movement = m_Actnmp_HC_Bttn.FindAction("Movement", throwIfNotFound: true);
        m_Actnmp_HC_Bttn_LookCamera = m_Actnmp_HC_Bttn.FindAction("LookCamera", throwIfNotFound: true);
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

    // Actionmap1
    private readonly InputActionMap m_Actionmap1;
    private IActionmap1Actions m_Actionmap1ActionsCallbackInterface;
    private readonly InputAction m_Actionmap1_Movement;
    private readonly InputAction m_Actionmap1_LookCamera;
    public struct Actionmap1Actions
    {
        private @ControlMapping m_Wrapper;
        public Actionmap1Actions(@ControlMapping wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Actionmap1_Movement;
        public InputAction @LookCamera => m_Wrapper.m_Actionmap1_LookCamera;
        public InputActionMap Get() { return m_Wrapper.m_Actionmap1; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(Actionmap1Actions set) { return set.Get(); }
        public void SetCallbacks(IActionmap1Actions instance)
        {
            if (m_Wrapper.m_Actionmap1ActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_Actionmap1ActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_Actionmap1ActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_Actionmap1ActionsCallbackInterface.OnMovement;
                @LookCamera.started -= m_Wrapper.m_Actionmap1ActionsCallbackInterface.OnLookCamera;
                @LookCamera.performed -= m_Wrapper.m_Actionmap1ActionsCallbackInterface.OnLookCamera;
                @LookCamera.canceled -= m_Wrapper.m_Actionmap1ActionsCallbackInterface.OnLookCamera;
            }
            m_Wrapper.m_Actionmap1ActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @LookCamera.started += instance.OnLookCamera;
                @LookCamera.performed += instance.OnLookCamera;
                @LookCamera.canceled += instance.OnLookCamera;
            }
        }
    }
    public Actionmap1Actions @Actionmap1 => new Actionmap1Actions(this);

    // Actnmp_HC_Bttn
    private readonly InputActionMap m_Actnmp_HC_Bttn;
    private IActnmp_HC_BttnActions m_Actnmp_HC_BttnActionsCallbackInterface;
    private readonly InputAction m_Actnmp_HC_Bttn_Movement;
    private readonly InputAction m_Actnmp_HC_Bttn_LookCamera;
    public struct Actnmp_HC_BttnActions
    {
        private @ControlMapping m_Wrapper;
        public Actnmp_HC_BttnActions(@ControlMapping wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Actnmp_HC_Bttn_Movement;
        public InputAction @LookCamera => m_Wrapper.m_Actnmp_HC_Bttn_LookCamera;
        public InputActionMap Get() { return m_Wrapper.m_Actnmp_HC_Bttn; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(Actnmp_HC_BttnActions set) { return set.Get(); }
        public void SetCallbacks(IActnmp_HC_BttnActions instance)
        {
            if (m_Wrapper.m_Actnmp_HC_BttnActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_Actnmp_HC_BttnActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_Actnmp_HC_BttnActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_Actnmp_HC_BttnActionsCallbackInterface.OnMovement;
                @LookCamera.started -= m_Wrapper.m_Actnmp_HC_BttnActionsCallbackInterface.OnLookCamera;
                @LookCamera.performed -= m_Wrapper.m_Actnmp_HC_BttnActionsCallbackInterface.OnLookCamera;
                @LookCamera.canceled -= m_Wrapper.m_Actnmp_HC_BttnActionsCallbackInterface.OnLookCamera;
            }
            m_Wrapper.m_Actnmp_HC_BttnActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @LookCamera.started += instance.OnLookCamera;
                @LookCamera.performed += instance.OnLookCamera;
                @LookCamera.canceled += instance.OnLookCamera;
            }
        }
    }
    public Actnmp_HC_BttnActions @Actnmp_HC_Bttn => new Actnmp_HC_BttnActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("KeyboardMouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    private int m_testSchemeIndex = -1;
    public InputControlScheme testScheme
    {
        get
        {
            if (m_testSchemeIndex == -1) m_testSchemeIndex = asset.FindControlSchemeIndex("test");
            return asset.controlSchemes[m_testSchemeIndex];
        }
    }
    private int m_AndroidSchemeIndex = -1;
    public InputControlScheme AndroidScheme
    {
        get
        {
            if (m_AndroidSchemeIndex == -1) m_AndroidSchemeIndex = asset.FindControlSchemeIndex("Android");
            return asset.controlSchemes[m_AndroidSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    public interface IActionmap1Actions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnLookCamera(InputAction.CallbackContext context);
    }
    public interface IActnmp_HC_BttnActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnLookCamera(InputAction.CallbackContext context);
    }
}
