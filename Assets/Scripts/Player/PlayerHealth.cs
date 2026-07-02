using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    private NetworkVariable<int> health = new NetworkVariable<int>(
        100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private Text hpText;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            health.Value = maxHealth;
        }

        if (IsOwner)
        {
            var hpTextObject = GameObject.Find("HPText");
            if (hpTextObject != null)
            {
                hpText = hpTextObject.GetComponent<Text>();
                UpdateHpText(health.Value);
            }
            health.OnValueChanged += OnHealthChanged;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            health.OnValueChanged -= OnHealthChanged;
        }
    }

    private void OnHealthChanged(int previousValue, int newValue)
    {
        UpdateHpText(newValue);
    }

    private void UpdateHpText(int value)
    {
        if (hpText != null)
        {
            hpText.text = $"HP: {value}";
        }
    }

    public void ApplyDamage(int amount)
    {
        if (!IsServer) return;
        health.Value = Mathf.Max(0, health.Value - amount);
    }
}
