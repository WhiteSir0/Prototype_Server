using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 4f;

    private InputSystem_Actions actions;

    private void Awake()
    {
        actions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        actions.Player.Enable();
    }

    private void OnDisable()
    {
        actions.Player.Disable();
    }

    private void Update()
    {
        if (!IsOwner) return;

        Vector2 input = actions.Player.Move.ReadValue<Vector2>();
        transform.position += (Vector3)(input.normalized * moveSpeed * Time.deltaTime);
    }
}
