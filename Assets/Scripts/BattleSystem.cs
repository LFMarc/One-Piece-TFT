using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleSystem : MonoBehaviour
{
    public CharacterList character;
    public List<EnemyScript> enemies;

    [Header("UI")]
    public Image healthBar;

    public Animator charAnim;
    public float movSpeed;
    private Vector3 startingPosition;

    private Dictionary<EnemyScript, float> attackTimers;
    private float attackCd;
    private float currentHp;

    private int enemyIndexNow = 0;
    private AudioSource audioSource;
    private int attackSoundIndex = 0;

    public int upgradeLevel = 0;

    void Start()
    {
        charAnim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        startingPosition = transform.position;

        currentHp = character.maxHp;
        UpdateHealthBar();

        attackTimers = new Dictionary<EnemyScript, float>();
        enemies = new List<EnemyScript>();
        attackCd = Time.time;
    }

    void Update()
    {
        if (currentHp <= 0)
        {
            Die();
            return;
        }

        if (!GameManager.Instance || !GameManager.Instance.IsCombatPhase())
            return;

        CleanEnemyList();
        if (enemies.Count == 0)
        {
            TryFindEnemies();
            return;
        }

        if (Time.time - attackCd >= character.attackSpeed)
        {
            EnemyScript enemyTarget = enemies[enemyIndexNow];

            if (enemyTarget != null && enemyTarget.hp > 0)
            {
                charAnim.SetTrigger("Attack");
                StartCoroutine(MoverHaciaEnemigo(enemyTarget.transform.position));

                enemyTarget.ReceiveDamage(character.dmg);

                // Solo reporta daño si es Player
                if (DamageMeterManager.Instance != null && CompareTag("Player"))
                {
                    DamageMeterManager.Instance.ReportDamage(transform.root.gameObject, character.dmg);
                }

                attackCd = Time.time;
            }
            else
            {
                BuscarSiguienteEnemigoVivo();
            }
        }
    }

    public void ReceiveDamage(float dmg)
    {
        float finalDamage = Mathf.Max(dmg - character.defense, 0);
        currentHp -= finalDamage;
        UpdateHealthBar();

        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
    }

    public void HealToFull()
    {
        currentHp = character.maxHp;
        UpdateHealthBar();
    }


    private void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.fillAmount = currentHp / character.maxHp;
    }

    private void Die()
    {
        charAnim.SetTrigger("Die");
        Destroy(gameObject, 1f);
    }

    void PlayAttackSound()
    {
        if (character.attackSounds != null && character.attackSounds.Length > 0 && audioSource != null)
        {
            audioSource.PlayOneShot(character.attackSounds[attackSoundIndex]);
            attackSoundIndex = (attackSoundIndex + 1) % character.attackSounds.Length;
        }
    }

    void BuscarSiguienteEnemigoVivo()
    {
        enemyIndexNow++;
        if (enemyIndexNow >= enemies.Count)
            enemyIndexNow = 0;
    }

    IEnumerator MoverHaciaEnemigo(Vector3 enemyPosition)
    {
        if (movSpeed <= 0f)
        {
            PlayAttackSound();
            yield break;
        }

        Vector3 direccion = (enemyPosition - transform.position).normalized;
        float distancia = Vector3.Distance(transform.position, enemyPosition);

        while (distancia > 0.1f)
        {
            transform.position += direccion * movSpeed * Time.deltaTime;
            distancia = Vector3.Distance(transform.position, enemyPosition);
            yield return null;
        }

        PlayAttackSound();
        StartCoroutine(TpBack());
    }

    IEnumerator TpBack()
    {
        yield return new WaitForSeconds(0.3f);
        transform.position = startingPosition;
    }

    void TryFindEnemies()
    {
        EnemyScript[] targetsFound = FindObjectsOfType<EnemyScript>();
        enemies.Clear();
        enemies.AddRange(targetsFound);

        // Randomizar objetivo
        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyScript temp = enemies[i];
            int randomIndex = Random.Range(i, enemies.Count);
            enemies[i] = enemies[randomIndex];
            enemies[randomIndex] = temp;
        }

        ResetAttackTimers();
        enemyIndexNow = 0;
    }

    void CleanEnemyList()
    {
        enemies.RemoveAll(e => e == null || e.hp <= 0);
    }

    void ResetAttackTimers()
    {
        attackTimers.Clear();
        foreach (EnemyScript enemy in enemies)
        {
            attackTimers[enemy] = Time.time;
        }
        attackCd = Time.time;
    }

    public bool TryUpgrade()
    {
        if (upgradeLevel >= 5)
            return false;

        upgradeLevel++;

        character.dmg *= 2f;
        character.maxHp *= 2f;
        character.attackSpeed *= 0.8f;
        HealToFull();

        // Actualizar DamageMeterSlot
        if (DamageMeterManager.Instance != null)
        {
            DamageMeterManager.Instance.UpdateUpgradeIcon(transform.root.gameObject, upgradeLevel);
        }

        return true;
    }

}
