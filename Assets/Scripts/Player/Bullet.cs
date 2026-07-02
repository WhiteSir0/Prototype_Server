using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 3f;

    private Vector2 direction;
    private ulong shooterId;

    private void Awake()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer.sprite == null)
        {
            var tex = Texture2D.whiteTexture;
            spriteRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), tex.width / 0.15f);
            spriteRenderer.color = Color.yellow;
        }
    }

    public void Initialize(Vector2 travelDirection, ulong shooterClientId)
    {
        direction = travelDirection.normalized;
        shooterId = shooterClientId;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Invoke(nameof(DespawnSelf), lifetime);
        }
    }

    private void Update()
    {
        if (!IsServer) return;
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        var health = other.GetComponent<PlayerHealth>();
        if (health == null) return;
        if (health.OwnerClientId == shooterId) return;

        health.ApplyDamage(damage);
        DespawnSelf();
    }

    private void DespawnSelf()
    {
        if (NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn();
        }
    }
}
