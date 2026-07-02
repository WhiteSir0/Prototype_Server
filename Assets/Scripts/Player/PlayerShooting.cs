using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : NetworkBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float spawnOffset = 0.6f;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 direction = (mouseWorldPos - (Vector2)transform.position).normalized;
            ShootServerRpc(direction);
        }
    }

    [ServerRpc]
    private void ShootServerRpc(Vector2 direction, ServerRpcParams rpcParams = default)
    {
        Vector3 spawnPosition = transform.position + (Vector3)(direction * spawnOffset);
        GameObject bulletInstance = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        bulletInstance.GetComponent<Bullet>().Initialize(direction, OwnerClientId);
        bulletInstance.GetComponent<NetworkObject>().Spawn();
    }
}
