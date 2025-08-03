using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemScript : MonoBehaviour
{
    public CharacterList characterData; //Asignado en el script de la tienda
    public Button button;
    private Image itemImage;
    private TextMeshProUGUI itemNameText;

    void Awake()
    {
        itemImage = transform.Find("Image").GetComponent<Image>();
        itemNameText = transform.Find("Text").GetComponent<TextMeshProUGUI>();

        if (button == null)
        {
            button = GetComponent<Button>();
        }
    }

    void Start()
    {
        // Asigna el método al evento onClick del botón
        button.onClick.AddListener(OnButtonClick);

        // Actualiza la visualización del item
        UpdateItemDisplay();
    }

    public void SetupItem(CharacterList character)
    {
        characterData = character; // Guarda la referencia al personaje
        UpdateItemDisplay();
    }

    private void UpdateItemDisplay()
    {
        if (characterData != null)
        {
            // Configura la imagen y el texto del personaje
            itemImage.sprite = characterData.characterIcon;
            itemNameText.text = characterData.characterName;
        }
        else
        {
            Debug.LogError("characterData no está configurado en ShopItem.");
        }
    }

    void OnButtonClick()
    {
        // Llama al método de la tienda para manejar la compra
        ShopSystem.Instance.BuyCharacter(characterData, gameObject);
    }
}
