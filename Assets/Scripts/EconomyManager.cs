using TMPro;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;

    [Header("Economía")]
    public int currentGold = 1000; // Oro inicial
    public int characterCost = 1000;
    public int goldPerRound = 1000;

    [Header("UI")]
    public TextMeshProUGUI goldText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateGoldUI();
    }

    public bool CanAffordCharacter()
    {
        return currentGold >= characterCost;
    }

    public bool SpendGoldOnCharacter()
    {
        if (CanAffordCharacter())
        {
            currentGold -= characterCost;
            UpdateGoldUI();
            return true;
        }
        return false;
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateGoldUI();
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
            goldText.text = $"{currentGold}";
    }
}
