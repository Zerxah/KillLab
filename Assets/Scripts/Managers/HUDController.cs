using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text powerupText;

    [Header("Targets")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerGunRaycast playerGun;

    void Awake()
    {
        if (!hpText) hpText = GameObject.Find("HPText")?.GetComponent<TMP_Text>();
        if (!ammoText) ammoText = GameObject.Find("AmmoText")?.GetComponent<TMP_Text>();
        if (!powerupText) powerupText = GameObject.Find("PowerupText")?.GetComponent<TMP_Text>();

        if (!playerHealth) playerHealth = FindObjectOfType<PlayerHealth>();
        if (!playerGun) playerGun = FindObjectOfType<PlayerGunRaycast>();

        if (powerupText) powerupText.text = "";
    }

    void Update()
    {

        if (hpText)
        {
            if (playerHealth) hpText.text = $"HP: {playerHealth.CurrentHP}/{playerHealth.MaxHP}";
            else hpText.text = "";
        }

        bool infinite = playerGun && playerGun.InfiniteAmmoActive;

        if (ammoText)
        {
            ammoText.enabled = !infinite;

            if (!infinite && playerGun)
                ammoText.text = $"{playerGun.AmmoInMag}/{playerGun.MagSize} | {playerGun.AmmoReserve}";
            else if (!infinite)
                ammoText.text = "";
        }

        if (powerupText)
        {
            powerupText.enabled = infinite;
            powerupText.text = infinite ? "INFINITE AMMO!" : "";
        }
    }
}



