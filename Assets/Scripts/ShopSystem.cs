using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopSystem : MonoBehaviour
{
    public static ShopSystem Instance { get; private set; } // Singleton instance

    [Header("Configuración de la tienda")]
    public Transform shopPanel;
    public GameObject shopItemPrefab;
    public List<CharacterList> allCharacters; // Lista pública de personajes disponibles en el pool
    private List<CharacterList> availableCharacters; // Lista privada de personajes disponibles para la tienda

    // Cambiar el spawnPoint a un array para manejar múltiples puntos de aparición
    public Transform[] spawnPoints; // Array de spawn points
    private List<Transform> availableSpawnPoints; // Lista para controlar spawn points disponibles

    void Awake()
    {
        // Asegúrate de que solo haya una instancia del ShopSystem
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Destruye los duplicados
        }
    }

    void Start()
    {
        availableCharacters = new List<CharacterList>(allCharacters);

        // Inicializar la lista de spawn points disponibles
        availableSpawnPoints = new List<Transform>(spawnPoints);

        GenerateShopItems();
    }

    private void GenerateShopItems()
    {
        Debug.Log("Generando tienda");

        // Limpiar la tienda antes de agregar nuevos elementos
        ClearShopItems();

        // Reiniciar la lista de spawn points disponibles
        availableSpawnPoints = new List<Transform>(spawnPoints);

        // Generar 3 items aleatorios para la tienda
        for (int i = 0; i < 3; i++)
        {
            if (availableCharacters.Count > 0 && availableSpawnPoints.Count > 0)
            {
                // Seleccionar un personaje aleatorio de la lista
                int randomCharacterIndex = Random.Range(0, availableCharacters.Count);
                CharacterList selectedCharacter = availableCharacters[randomCharacterIndex];
                Debug.Log($"Generando Item para: {selectedCharacter.characterName}");

                // Instanciar el prefab de la tienda
                GameObject shopItem = Instantiate(shopItemPrefab, shopPanel);

                // Obtener el script ShopItemScript y configurar el item
                ShopItemScript itemScript = shopItem.GetComponent<ShopItemScript>();
                if (itemScript != null)
                {
                    itemScript.SetupItem(selectedCharacter);
                }
                else
                {
                    Debug.LogError("No se encontró el script ShopItemScript en el prefab.");
                }

                // Eliminar el personaje seleccionado de la lista de disponibles
                availableCharacters.RemoveAt(randomCharacterIndex);
            }
        }
    }

    private void ClearShopItems()
    {
        // Limpiar los elementos existentes en el panel de la tienda
        foreach (Transform child in shopPanel)
        {
            Destroy(child.gameObject);
        }
        Debug.Log("Limpiando la tienda");
    }

    public void BuyCharacter(CharacterList character, GameObject shopItem)
    {
        // Aquí puedes implementar la lógica para comprar el personaje
        Debug.Log($"Comprado: {character.characterName}");

        // Destruir el ítem de la tienda
        Destroy(shopItem);

        // Instanciar el prefab del personaje en la escena
        if (character.characterPrefab != null)
        {
            // Verificar si hay spawn points disponibles
            if (availableSpawnPoints.Count > 0)
            {
                // Seleccionar un spawn point aleatorio de los disponibles
                int spawnIndex = Random.Range(0, availableSpawnPoints.Count);
                Transform selectedSpawnPoint = availableSpawnPoints[spawnIndex];

                // Instanciar el personaje en el spawn point seleccionado
                Instantiate(character.characterPrefab, selectedSpawnPoint.position, Quaternion.identity);
                Debug.Log($"Instanciado {character.characterName} en la escena en el spawn point {spawnIndex + 1}.");

                // Eliminar ese spawn point de la lista de disponibles
                availableSpawnPoints.RemoveAt(spawnIndex);
            }
            else
            {
                Debug.LogError("No hay spawn points disponibles.");
            }
        }
        else
        {
            Debug.LogError("El prefab del personaje no está asignado.");
        }
    }
}