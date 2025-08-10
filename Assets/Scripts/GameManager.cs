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

    /// Entra en la fase de preparación: muestra tienda y botón
    public void EnterPreparationPhase()
    {
        Debug.Log($"[Preparación] Ronda {round}");
        isCombatPhase = false;
        roundInProgress = false;

        //resetear valores
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
        }
    }


    /// Entra en la fase de combate: oculta tienda y botón
    public void StartCombatPhase()
    {
        if (roundInProgress) return; // Evita llamadas dobles
        roundInProgress = true;

        Debug.Log($"[Combate] Ronda {round}");
        isCombatPhase = true;

        if (startRoundButton != null)
            startRoundButton.gameObject.SetActive(false);

        // Ocultar la tienda (desactivar panel)
        if (ShopSystem.Instance != null)
        {
            ShopSystem.Instance.shopPanel.gameObject.SetActive(false);
        }

        SpawnEnemies();
    }

    /// Instancia los enemigos
    void SpawnEnemies()
    {
        enemiesRemaining = enemySpawnPoints.Length;

        for (int i = 0; i < enemySpawnPoints.Length; i++)
        {
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Instantiate(prefab, enemySpawnPoints[i].position, Quaternion.identity);
        }
    }

    /// Se llama cuando un enemigo muere
    public void EnemyDefeated()
    {
        if (!isCombatPhase) return;

        enemiesRemaining--;

        if (enemiesRemaining <= 0)
        {
            isCombatPhase = false;
            Debug.Log($"[Ronda Completada] Ronda {round}");
            round++;
            StartCoroutine(NextPreparationPhaseDelay());
        }
    }

    /// Delay entre combate y preparación
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
