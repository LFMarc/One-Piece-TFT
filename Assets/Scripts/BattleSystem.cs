using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public CharacterList character;
    public List<EnemyScript> enemies;

    public Animator charAnim;
    public float movSpeed;
    private Vector3 startingPosition;

    private Dictionary<EnemyScript, float> attackTimers;
    private float attackCd;

    private int enemyIndexNow = 0;
    private AudioSource audioSource;  // Referencia al AudioSource del personaje
    private int attackSoundIndex = 0; // Índice para alternar entre los sonidos de ataque

    void Start()
    {
        charAnim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        startingPosition = transform.position;

        attackTimers = new Dictionary<EnemyScript, float>();
        enemies = new List<EnemyScript>();
        attackCd = Time.time;
    }

    void Update()
    {
        if (character.maxHp <= 0)
        {
            Destroy(gameObject);
            return;
        }

        if (!GameManager.Instance || !GameManager.Instance.IsCombatPhase())
        {
            return; // Solo atacar en fase de combate
        }

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

                enemyTarget.RecieveDamage(character.dmg);

                if (DamageMeterManager.Instance != null)
                    DamageMeterManager.Instance.ReportDamage(transform.root.gameObject, character.dmg);

                attackCd = Time.time;
            }
            else
            {
                BuscarSiguienteEnemigoVivo();
            }
        }
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
        {
            enemyIndexNow = 0;
        }
    }

    IEnumerator MoverHaciaEnemigo(Vector3 enemyPosition)
    {
        // Si la velocidad es 0, dispara desde lejos sin moverse
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
}
