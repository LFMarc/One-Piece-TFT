using System.Collections.Generic;
using UnityEngine;

public class DamageMeterManager : MonoBehaviour
{
    public static DamageMeterManager Instance;

    public DamageSlot[] damageSlots;
    private Dictionary<GameObject, DamageSlot> assignedSlots = new Dictionary<GameObject, DamageSlot>();

    public Sprite[] upgradeSprites; // array con sprites del 0 al 5

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterCharacter(GameObject character, CharacterList characterData)
    {
        // Evita registrar dos veces el mismo personaje
        if (assignedSlots.ContainsKey(character))
            return;

        foreach (DamageSlot slot in damageSlots)
        {
            if (!assignedSlots.ContainsValue(slot))
            {
                assignedSlots[character] = slot;
                slot.SetupSlot(characterData.damageGraphIcon);

                //Nivel 1
                if (upgradeSprites.Length > 0)
                    slot.UpdateUpgradeSprite(upgradeSprites[0]);

                return;
            }
        }

        Debug.LogWarning("No hay slots libres para el medidor de daño.");
    }


    public void ReportDamage(GameObject character, float damage)
    {
        // Asegurar que usamos siempre el GameObject raíz
        if (character.transform.root != character)
            character = character.transform.root.gameObject;

        if (assignedSlots.ContainsKey(character))
        {
            assignedSlots[character].AddDamage(damage);
        }
        else
        {
            Debug.LogWarning($"No se encontró slot asignado para {character.name}");
        }
    }

    // Resetea solo el texto, mantiene asignaciones
    public void ResetDamageValues()
    {
        foreach (var slot in damageSlots)
        {
            slot.ResetDamage();
        }
    }

    // Borra todo (si quieres reiniciar slots)
    public void ClearAll()
    {
        foreach (var slot in damageSlots)
        {
            slot.ResetDamage();
        }
        assignedSlots.Clear();
    }

    public void UpdateUpgradeIcon(GameObject character, int upgradeLevel)
    {
        if (assignedSlots.TryGetValue(character, out DamageSlot slot))
        {
            // Validamos que no se pase del array
            if (upgradeLevel >= 0 && upgradeLevel < upgradeSprites.Length)
            {
                slot.UpdateUpgradeSprite(upgradeSprites[upgradeLevel]);
            }
        }
        else
        {
            Debug.LogWarning($"No se encontró slot asignado para {character.name}");
        }
    }

}
