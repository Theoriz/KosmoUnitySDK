#if ENABLE_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM_PACKAGE
#define USE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

using System.Collections.Generic;
using UnityEngine;

namespace Kosmo
{
    public class KosmoCameraController : MonoBehaviour
    {
        const float k_MouseSensitivityMultiplier = 0.01f;

        /// <summary>
        /// Rotation speed when using the mouse.
        /// </summary>
        public float m_LookSpeedMouse = 4.0f;

        public KeyCode m_ResetShortcut = KeyCode.R;

#if !USE_INPUT_SYSTEM
        private static string kMouseX = "Mouse X";
        private static string kMouseY = "Mouse Y";
#endif

#if USE_INPUT_SYSTEM
        InputAction lookAction;
#endif

        void OnEnable()
        {
            RegisterInputs();
        }

        void RegisterInputs()
        {
#if USE_INPUT_SYSTEM
            var map = new InputActionMap("Free Camera");

            lookAction = map.AddAction("look", binding: "<Mouse>/delta");

            lookAction.AddBinding("<Gamepad>/rightStick").WithProcessor("scaleVector2(x=15, y=15)");

            lookAction.Enable();
#endif
        }

        float inputRotateAxisX, inputRotateAxisY;

        void UpdateInputs()
        {
            inputRotateAxisX = 0.0f;
            inputRotateAxisY = 0.0f;

#if USE_INPUT_SYSTEM
            var lookDelta = lookAction.ReadValue<Vector2>();
            inputRotateAxisX = lookDelta.x * m_LookSpeedMouse * k_MouseSensitivityMultiplier;
            inputRotateAxisY = lookDelta.y * m_LookSpeedMouse * k_MouseSensitivityMultiplier;
#else
            if (Input.GetMouseButton(0))
            {
                inputRotateAxisX = Input.GetAxis(kMouseX) * m_LookSpeedMouse;
                inputRotateAxisY = Input.GetAxis(kMouseY) * m_LookSpeedMouse;
            }
#endif
        }

        void Update()
        {

            UpdateInputs();

            bool moved = inputRotateAxisX != 0.0f || inputRotateAxisY != 0.0f;
            if (moved)
            {
                float rotationX = transform.localEulerAngles.x;
                float newRotationY = transform.localEulerAngles.y + inputRotateAxisX;

                // Weird clamping code due to weird Euler angle mapping...
                float newRotationX = (rotationX - inputRotateAxisY);
                if (rotationX <= 90.0f && newRotationX >= 0.0f)
                    newRotationX = Mathf.Clamp(newRotationX, 0.0f, 90.0f);
                if (rotationX >= 270.0f)
                    newRotationX = Mathf.Clamp(newRotationX, 270.0f, 360.0f);

                transform.localRotation = Quaternion.Euler(newRotationX, newRotationY, transform.localEulerAngles.z);
            }

            if (Input.GetKeyDown(m_ResetShortcut))
            {
                transform.localRotation = Quaternion.identity;
            }
        }
    }
}
