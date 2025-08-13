using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Character List/Character")]
public class CharacterList : ScriptableObject
{
    [Header("Character info")]
    public string characterName;
    public Sprite characterIcon;
    public float maxHp;
    public float defense;
    public float dmg;
    public float attackSpeed;
    public float abilityCharge;
    public float abilityThreshold;

    [Header("Damage Graph")]
    public Sprite damageGraphIcon;

    [Header("Prefabs and SE")]
    public GameObject characterPrefab;

    public GameObject spellPrefab;
    public AudioClip spellSound;    // Sonido del spell
    public AudioClip[] attackSounds; // array para tener múltiples sonidos de ataque

    [Header("Synergy")]
    public Synergy[] synergies;         // Sinergias de este personaje
}