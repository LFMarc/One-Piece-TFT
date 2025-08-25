using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Para usar TMP_Text

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Rondas")]
    public int round = 1;
    public TMP_Text roundCounterText; // Asigna el texto en el inspector (su padre tiene el BG)

    [Header("Enemigos")]
    public GameObject[] enemyPrefabs;
    public Transform[] enemySpawnPoints;

    [Header("UI")]
    public Button startRoundButton;

    private int enemiesRemaining;
    private bool isCombatPhase = false;
    private bool roundInProgress = false;
    private bool hasBoughtCharacter = false;

    public GameObject menuPanel;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        EnterPreparationPhase();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuPanel != null)
            {
                menuPanel.SetActive(!menuPanel.activeSelf);
            }
        }
    }

    public void EnterPreparationPhase()
    {
        Debug.Log($"[Preparación] Ronda {round}");
        isCombatPhase = false;
        roundInProgress = false;
        hasBoughtCharacter = false;

        if (DamageMeterManager.Instance != null)
            DamageMeterManager.Instance.ResetDamageValues();

        if (ShopSystem.Instance != null)
        {
            ShopSystem.Instance.shopPanel.gameObject.SetActive(true);
            ShopSystem.Instance.GenerateShopItems();
        }

        if (startRoundButton != null)
        {
            startRoundButton.gameObject.SetActive(true);
            startRoundButton.onClick.RemoveAllListeners();
            startRoundButton.onClick.AddListener(StartCombatPhase);

            startRoundButton.interactable = round > 1;
        }

        // Ocultar el contador completo (texto + BG) en fase de compra
        if (roundCounterText != null)
            roundCounterText.transform.parent.gameObject.SetActive(false);
    }

    // Este método lo llamará ShopSystem cuando se compre un personaje
    public void OnCharacterBought()
    {
        hasBoughtCharacter = true;
        if (startRoundButton != null)
            startRoundButton.interactable = true;
    }

    public void StartCombatPhase()
    {
        if (roundInProgress) return;
        roundInProgress = true;

        Debug.Log($"[Combate] Ronda {round}");
        isCombatPhase = true;

        if (startRoundButton != null)
            startRoundButton.gameObject.SetActive(false);

        if (ShopSystem.Instance != null)
            ShopSystem.Instance.shopPanel.gameObject.SetActive(false);

        // Mostrar y actualizar el contador completo (texto + BG) en combate
        if (roundCounterText != null)
        {
            roundCounterText.text = $"Round {round}";
            roundCounterText.transform.parent.gameObject.SetActive(true);
        }

        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        enemiesRemaining = enemySpawnPoints.Length;

        for (int i = 0; i < enemySpawnPoints.Length; i++)
        {
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            GameObject enemyObj = Instantiate(prefab, enemySpawnPoints[i].position, Quaternion.identity);

            // Aplicar escalado por ronda
            EnemyScript enemy = enemyObj.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                float hpMultiplier = Mathf.Pow(1.5f, round - 1);   // Vida se multiplica 1.5x cada ronda
                float dmgMultiplier = Mathf.Pow(1.25f, round - 1); // Daño se multiplica 1.25x cada ronda

                enemy.maxHp *= hpMultiplier;
                enemy.hp = enemy.maxHp; // curar al full al inicio
                enemy.damage *= dmgMultiplier;

                enemy.UpdateHealthBar(); // actualizar UI
            }
        }
    }

    public void EnemyDefeated()
    {
        if (!isCombatPhase) return;

        enemiesRemaining--;
        if (enemiesRemaining <= 0)
        {
            isCombatPhase = false;
            Debug.Log($"[Ronda Completada] Ronda {round}");

            // Recompensa de oro al final de la ronda
            if (EconomyManager.Instance != null)
            {
                EconomyManager.Instance.AddGold(EconomyManager.Instance.goldPerRound);
                Debug.Log($"Ganaste {EconomyManager.Instance.goldPerRound} de oro. Total: {EconomyManager.Instance.currentGold}");
            }

            // Curar a todas las unidades del jugador
            BattleSystem[] playerUnits = FindObjectsOfType<BattleSystem>();
            foreach (BattleSystem unit in playerUnits)
            {
                if (unit != null)
                {
                    unit.HealAmount(500f); // Nueva función que añadimos en BattleSystem
                }
            }

            round++;
            StartCoroutine(NextPreparationPhaseDelay());
        }
    }

    IEnumerator NextPreparationPhaseDelay()
    {
        yield return new WaitForSeconds(2f);
        EnterPreparationPhase();
    }

    public bool IsCombatPhase()
    {
        return isCombatPhase;
    }
}
