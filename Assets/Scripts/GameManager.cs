using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int round = 1;
    public GameObject[] enemyPrefabs;
    public Transform[] enemySpawnPoints;

    public Button startRoundButton;
    private int enemiesRemaining;

    private bool isCombatPhase = false;
    private bool roundInProgress = false;

    private bool hasBoughtCharacter = false; // <- nuevo

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

        // Recompensa de oro por ronda (excepto antes de la primera)
        //if (EconomyManager.Instance != null && round > 1)
        //{
        //    EconomyManager.Instance.AddGold(EconomyManager.Instance.goldPerRound);
        //    Debug.Log($"Ganaste {EconomyManager.Instance.goldPerRound} de oro. Total: {EconomyManager.Instance.currentGold}");
        //}
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

                enemy.UpdateHealthBar(); //update UI
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

            //Recompensa de oro al final de la ronda
            if (EconomyManager.Instance != null)
            {
                EconomyManager.Instance.AddGold(EconomyManager.Instance.goldPerRound);
                Debug.Log($"Ganaste {EconomyManager.Instance.goldPerRound} de oro. Total: {EconomyManager.Instance.currentGold}");
            }

            //Curar a todas las unidades del jugador
            BattleSystem[] playerUnits = FindObjectsOfType<BattleSystem>();
            foreach (BattleSystem unit in playerUnits)
            {
                if (unit != null)
                {
                    unit.HealAmount(500f); //Nueva función que añadimos en BattleSystem
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
