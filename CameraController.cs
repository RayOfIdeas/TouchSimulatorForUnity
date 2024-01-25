using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchSimulatorForUnity
{
    public class CameraController : MonoBehaviour
    {
        [Header("Zoom")]
        [SerializeField]
        float orthographicSize = 12;

        [SerializeField]
        float orthographicSizeMin = 6;

        [SerializeField]
        float orthographicSizeMax = 20;

        [Header("Move")]
        [SerializeField]
        Transform borderRight;

        [SerializeField]
        Transform borderLeft;        
        
        [SerializeField]
        Transform borderTop;

        [SerializeField]
        Transform borderBottom;

        Vector2 currentPointerPos, lastPointerPos;
        bool isTapping, isTappingSecond;

        void Awake()
        {
            Camera.main.orthographicSize = orthographicSize;

#if UNITY_EDITOR
            var touchSimulator = FindFirstObjectByType<TouchSimulator>();
            touchSimulator.OnPositionInput += OnPositionInput;
            touchSimulator.OnTapInput += OnTapInput;
            touchSimulator.OnTapSecondInput += OnTapSecondInput;
            touchSimulator.OnPinchInput += OnPinchInput;
#endif

            void OnPositionInput(Vector2 pos)
            {
                lastPointerPos = currentPointerPos;
                currentPointerPos = pos;
                
                if (isTapping && !isTappingSecond)
                {
                    var currentPointerPosWorld = Camera.main.ScreenToWorldPoint(currentPointerPos);
                    var lastPointerPosWorld = Camera.main.ScreenToWorldPoint(lastPointerPos);
                    var direction = currentPointerPosWorld - lastPointerPosWorld;
                    MoveBy(-direction);
                }
            }

            void OnTapInput(bool isTapping)
            {
                this.isTapping = isTapping;
                if (isTapping)
                {
                    
                }
                else
                {
                    lastPointerPos = Vector2.zero;
                    currentPointerPos = Vector2.zero;
                }
            }

            void OnTapSecondInput(bool isTapping)
            {
                this.isTappingSecond = isTapping;
            }

            void OnPinchInput(Vector2 primaryCurrent, Vector2 secondaryCurrent, Vector2 primaryLast, Vector2 secondaryLast)
            {
                if (!isTapping || !isTappingSecond)
                    return;

                var lastPinchPos = new Vector2(
                    (primaryLast.x + secondaryLast.x) / 2,
                    (primaryLast.y + secondaryLast.y) / 2);
                var lastPosWorld = Camera.main.ScreenToWorldPoint(lastPinchPos);

                var currentPinchDistance = Vector2.Distance(primaryCurrent, secondaryCurrent);
                var lastPinchDistance = Vector2.Distance(primaryLast, secondaryLast);
                var newOrthographicSize = lastPinchDistance * Camera.main.orthographicSize / currentPinchDistance;
                var lastOrthographicSize = Camera.main.orthographicSize;
                SetOrthographicSize(newOrthographicSize);

                var currentPinchPos = new Vector2(
                    (primaryCurrent.x + secondaryCurrent.x) / 2,
                    (primaryCurrent.y + secondaryCurrent.y) / 2);
                var currentPosWorld = Camera.main.ScreenToWorldPoint(currentPinchPos);

                var scaleMarginDirection = lastPosWorld - currentPosWorld;
                MoveBy(scaleMarginDirection * newOrthographicSize / lastOrthographicSize);
            }
        }

        public void SetOrthographicSize(float newOrthographicSize)
        {
            Camera.main.orthographicSize = newOrthographicSize;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, orthographicSizeMin, orthographicSizeMax);
            ConstrainPosition();
        }

        public void MoveBy(Vector2 direction)
        {
            Camera.main.transform.position = new(
                Camera.main.transform.position.x + direction.x, 
                Camera.main.transform.position.y + direction.y,
                Camera.main.transform.position.z);

            ConstrainPosition();
        }

        void ConstrainPosition()
        {
            var aspectRatio = (float)Screen.width / Screen.height;

            var maxPos = new Vector2(
                borderRight.position.x - Camera.main.orthographicSize * aspectRatio,
                borderTop.position.y - Camera.main.orthographicSize
                );

            var minPos = new Vector2(
                borderLeft.position.x + Camera.main.orthographicSize * aspectRatio,
                borderBottom.position.y + Camera.main.orthographicSize
                );

            Camera.main.transform.position = new(
                Mathf.Clamp(Camera.main.transform.position.x, minPos.x, maxPos.x),
                Mathf.Clamp(Camera.main.transform.position.y, minPos.y, maxPos.y),
                Camera.main.transform.position.z
                );
        }
    }
}
