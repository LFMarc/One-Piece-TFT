using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageSlot : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI damageText;

    [Header("Upgrade UI")]
    public Image upgradeIcon; // Nueva imagen para mostrar el nivel de mejora

    private float totalDamage;

    public void SetupSlot(Sprite icon)
    {
        iconImage.sprite = icon;
        damageText.text = "0";
        totalDamage = 0;

        if (upgradeIcon != null)
            upgradeIcon.enabled = false; // Oculto al inicio
    }

    public void AddDamage(float amount)
    {
        totalDamage += amount;
        damageText.text = Mathf.RoundToInt(totalDamage).ToString();
    }

    public void ResetDamage()
    {
        totalDamage = 0;
        damageText.text = "0";
    }

    public void UpdateUpgradeSprite(Sprite newSprite)
    {
        if (upgradeIcon != null)
        {
            upgradeIcon.sprite = newSprite;
            upgradeIcon.enabled = true;
        }
    }

    public void SetDead()
    {
        if (iconImage != null)
        {
            iconImage.color = Color.gray; // Aplica tinte gris
        }

        if (upgradeIcon != null)
        {
            upgradeIcon.color = Color.gray; // También el icono de upgrade
        }

        damageText.color = Color.gray; // El texto del daño también en gris
    }

}
