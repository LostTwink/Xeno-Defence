using UnityEngine;
using UnityEngine.InputSystem;

public class DevController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float sprintMultiplier = 2f;
    public float mouseSensitivity = 0.18f;
    public bool lockCursorOnStart = true;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        if (lockCursorOnStart)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        // Toggle cursor lock with Escape
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return; // don't process look/move while unlocked
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        // Mouse look using new Input System
        Vector2 mouseDelta = Vector2.zero;
        if (Mouse.current != null)
            mouseDelta = Mouse.current.delta.ReadValue();

        rotationY += mouseDelta.x * mouseSensitivity;
        rotationX -= mouseDelta.y * mouseSensitivity;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.localEulerAngles = new Vector3(rotationX, rotationY, 0f);

        // Movement using Keyboard (WASD + QE) with optional sprint
        Vector3 move = Vector3.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) move += Vector3.forward;
            if (Keyboard.current.sKey.isPressed) move += Vector3.back;
            if (Keyboard.current.aKey.isPressed) move += Vector3.left;
            if (Keyboard.current.dKey.isPressed) move += Vector3.right;
            if (Keyboard.current.eKey.isPressed) move += Vector3.up;
            if (Keyboard.current.qKey.isPressed) move += Vector3.down;
        }

        float speed = moveSpeed;
        if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed)
            speed *= sprintMultiplier;

        if (move.sqrMagnitude > 0f)
        {
            Vector3 worldDelta = transform.TransformDirection(move.normalized) * speed * Time.deltaTime;
            transform.position += worldDelta;
        }
    }
}
