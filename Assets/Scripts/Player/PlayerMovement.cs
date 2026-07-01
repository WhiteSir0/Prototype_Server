using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 4f;

    private void Update()
    {
        if (!IsOwner) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        Vector2 input = Vector2.zero;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) input.y += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) input.y -= 1f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) input.x -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) input.x += 1f;

        transform.position += (Vector3)(input.normalized * moveSpeed * Time.deltaTime);
    }
}
