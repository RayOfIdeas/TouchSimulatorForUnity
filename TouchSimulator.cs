using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TouchSimulatorForUnity
{
    public class TouchSimulator : MonoBehaviour
    {
        [Header("One Touch")]
        public Transform touch;

        [Header("Pinch")]
        public float pinchInitialRadius = 100;
        public float pinchInitialAngle = 0;

        [Header("Pinch Components")]
        public Transform pinchPrimary;
        public Transform pinchSecondary;
        public bool isPinchPrimaryTapping;
        public bool isPinchSecondaryTapping;

        Vector2 mouseLastPos, primaryLastPos, secondaryLastPos;
        bool isCursorWarped, isTapping, isTappingSecond;

        public event Action<bool> OnTapInput;
        public event Action<bool> OnTapSecondInput;
        public event Action<Vector2> OnDeltaInput;
        public event Action<Vector2> OnPositionInput;
        public event Action<Vector2, Vector2, Vector2, Vector2> OnPinchInput;
        Vector2 GetMousePosOffset() => Mouse.current.position.value - new Vector2(Screen.width / 2, Screen.height / 2);

        void Awake()
        {
#if !UNITY_EDITOR
            gameObject.SetActive(false);
#endif
            
            pinchPrimary.localPosition = new(0, pinchInitialRadius);
            pinchSecondary.localPosition = new(0, -pinchInitialRadius);
            touch.transform.localEulerAngles = new(0, 0, pinchInitialAngle);
        }

        void Update()
        {
            HandleTapInput();
            HandleTapSecondInput();
            HandleDeltaInput();
            HandlePositionInput();
            HandlePinch();

            void HandleTapInput()
            {
                if (Mouse.current.leftButton.isPressed && !isTapping)
                {
                    isTapping = true;
                    OnTapInput?.Invoke(true);
                }
                else if (!Mouse.current.leftButton.isPressed && isTapping)
                {
                    isTapping = false;
                    OnTapInput?.Invoke(false);
                }
            }

            void HandleTapSecondInput()
            {
                if (Mouse.current.rightButton.isPressed && !isTappingSecond)
                {
                    isTappingSecond = true;
                    OnTapSecondInput?.Invoke(true);
                }
                else if (!Mouse.current.rightButton.isPressed && isTappingSecond)
                {
                    isTappingSecond = false;
                    OnTapSecondInput?.Invoke(false);
                }
            }

            void HandleDeltaInput()
            {
                OnDeltaInput?.Invoke(Mouse.current.delta.value);
            }

            void HandlePositionInput()
            {
                OnPositionInput?.Invoke(Mouse.current.position.value);
            }

            void HandlePinch()
            {
                isPinchPrimaryTapping = Mouse.current.leftButton.isPressed;
                isPinchSecondaryTapping = Mouse.current.rightButton.isPressed;

                if (isPinchPrimaryTapping && isPinchSecondaryTapping)
                {
                    if (!isCursorWarped)
                    {
                        WarpCursor();
                    }
                    else
                    {
                        pinchPrimary.localPosition = GetMousePosOffset() - (Vector2)touch.localPosition;
                        pinchSecondary.localPosition = -pinchPrimary.localPosition;
                        OnPinchInput?.Invoke(
                            pinchPrimary.position, 
                            pinchSecondary.position, 
                            primaryLastPos, 
                            secondaryLastPos);
                    }

                    primaryLastPos = pinchPrimary.position;
                    secondaryLastPos = pinchSecondary.position;
                }
                else
                {
                    touch.localPosition = new(
                        Mouse.current.position.value.x - Screen.width / 2,
                        Mouse.current.position.value.y - Screen.height / 2
                        );

                    ResetUI();
                }

                mouseLastPos = Mouse.current.position.value;

                void ResetUI()
                {
                    pinchPrimary.localPosition = new(0, pinchInitialRadius);
                    pinchSecondary.localPosition = new(0, -pinchInitialRadius);
                    touch.localEulerAngles = Vector3.zero;
                    isCursorWarped = false;
                }

                void WarpCursor()
                {
                    isCursorWarped = true;
                    var warpPos = touch.localPosition + pinchPrimary.localPosition + new Vector3(Screen.width / 2, Screen.height / 2, 0);
                    Mouse.current.WarpCursorPosition(warpPos);
                }
            }
        }
    }
}
