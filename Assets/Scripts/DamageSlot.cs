using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageSlot : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI damageText;

    private float totalDamage;

    public void SetupSlot(Sprite icon)
    {
        iconImage.sprite = icon;
        damageText.text = "0";
        totalDamage = 0;
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
}
