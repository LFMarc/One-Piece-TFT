using System.Collections.Generic;
using UnityEngine;

public class ShopSystem : MonoBehaviour
{
    public static ShopSystem Instance { get; private set; }

    [Header("Configuración de la tienda")]
    public Transform shopPanel;
    public GameObject shopItemPrefab;
    public List<CharacterList> allCharacters;
    private List<CharacterList> availableCharacters;

    public Transform[] spawnPoints; // Puntos de spawn fijos
    private bool[] spawnOccupied;   // Estado persistente de cada punto de spawn

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        availableCharacters = new List<CharacterList>(allCharacters);
        spawnOccupied = new bool[spawnPoints.Length]; // Todos empiezan libres
        GenerateShopItems();
    }

    /// Genera los ítems de la tienda sin modificar el estado de las posiciones ocupadas
    public void GenerateShopItems()
    {
        Debug.Log("Generando tienda");

        availableCharacters = new List<CharacterList>(allCharacters);

        ClearShopItems();

        // Crea hasta 3 ítems mientras haya personajes disponibles
        for (int i = 0; i < 3; i++)
        {
            if (availableCharacters.Count > 0)
            {
                int randomCharacterIndex = Random.Range(0, availableCharacters.Count);
                CharacterList selectedCharacter = availableCharacters[randomCharacterIndex];

                Debug.Log($"Generando ítem para: {selectedCharacter.characterName}");

                GameObject shopItem = Instantiate(shopItemPrefab, shopPanel);

                ShopItemScript itemScript = shopItem.GetComponent<ShopItemScript>();
                if (itemScript != null)
                    itemScript.SetupItem(selectedCharacter);
                else
                    Debug.LogError("No se encontró el script ShopItemScript en el prefab.");

                availableCharacters.RemoveAt(randomCharacterIndex);
            }
        }
    }

    /// Limpia la UI de la tienda
    private void ClearShopItems()
    {
        foreach (Transform child in shopPanel)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("Limpiando la tienda");
    }

    /// Compra un personaje y lo coloca en la primera posición libre
    public void BuyCharacter(CharacterList character, GameObject shopItem)
    {
        Debug.Log($"Comprado: {character.characterName}");
        Destroy(shopItem);

        // Buscar si ya existe en la escena
        BattleSystem existingChar = null;
        foreach (var bs in FindObjectsOfType<BattleSystem>())
        {
            if (bs.character == character)
            {
                existingChar = bs;
                break;
            }
        }

        if (existingChar != null)
        {
            if (existingChar.TryUpgrade())
                Debug.Log($"{character.characterName} mejorado a nivel {existingChar.upgradeLevel}!");
            else
                Debug.Log($"{character.characterName} ya alcanzó el máximo de mejoras (5).");

            if (GameManager.Instance != null)
                GameManager.Instance.OnCharacterBought();

            return;
        }


        // Si no existe, instanciar SOLO si hay hueco
        int freeIndex = GetFirstFreeSpawnIndex();
        if (freeIndex != -1)
        {
            Transform selectedSpawnPoint = spawnPoints[freeIndex];

            GameObject newChar = Instantiate(character.characterPrefab, selectedSpawnPoint.position, Quaternion.identity);
            Debug.Log($"Instanciado {character.characterName} en el spawn point {freeIndex + 1}.");

            spawnOccupied[freeIndex] = true; // Lo marcamos como ocupado

            // Registrar en el medidor de daño
            if (DamageMeterManager.Instance != null)
                DamageMeterManager.Instance.RegisterCharacter(newChar, character);

            // Avisar al GameManager
            if (GameManager.Instance != null)
                GameManager.Instance.OnCharacterBought();
        }
        else
        {
            Debug.Log("No hay spawn points libres. Solo puedes comprar personajes repetidos para mejorarlos.");
        }
    }



    /// Busca el primer spawn libre
    private int GetFirstFreeSpawnIndex()
    {
        for (int i = 0; i < spawnOccupied.Length; i++)
        {
            if (!spawnOccupied[i])
                return i;
        }
        return -1; // No hay libres
    }

    /// Llamar a esta función cuando un personaje muera para liberar su spawn
    public void FreeSpawnPoint(Transform point)
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] == point)
            {
                spawnOccupied[i] = false;
                Debug.Log($"Spawn point {i + 1} liberado.");
                break;
            }
        }
    }
}
