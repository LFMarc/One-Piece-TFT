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
        audioSource = GetComponent<AudioSource>();  // Obtener el AudioSource del personaje
        startingPosition = transform.position;
        attackTimers = new Dictionary<EnemyScript, float>();
        enemies = new List<EnemyScript>();
        EnemyScript[] targetsFound = FindObjectsOfType<EnemyScript>();
        enemies.AddRange(targetsFound);
        ResetAttackTimers();
    }

    void Update()
    {
        if (character.maxHp <= 0)
        {
            Destroy(gameObject);
            return;
        }

        // Mira si hay enemigos
        if (enemies.Count > 0)
        {
            // Comprueba el temporizador de ataque
            if (Time.time - attackCd >= character.attackSpeed)
            {
                // Obtener el enemigo actual
                EnemyScript enemyTarget = enemies[enemyIndexNow];

                // Verificar si el enemigo actual está vivo
                if (enemyTarget.hp > 0)
                {
                    // Ataque
                    charAnim.SetTrigger("Attack");

                    StartCoroutine(MoverHaciaEnemigo(enemyTarget.transform.position));
                    enemyTarget.RecieveDamage(character.dmg);

                    // Reiniciar el temporizador de ataque solo si el enemigo actual está vivo
                    attackCd = Time.time;
                }
                else
                {
                    // Si el enemigo actual está muerto, buscar el próximo enemigo vivo como objetivo
                    BuscarSiguienteEnemigoVivo();
                }
            }
        }
    }

    // Función para alternar y reproducir los sonidos de ataque
    void PlayAttackSound()
    {
        if (character.attackSounds != null && character.attackSounds.Length > 0 && audioSource != null)
        {
            // Alterna entre los sonidos de ataque
            audioSource.PlayOneShot(character.attackSounds[attackSoundIndex]);

            // Cambiar al siguiente sonido de ataque (alternar entre ellos)
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
        Vector3 direccion = (enemyPosition - transform.position).normalized;
        float distancia = Vector3.Distance(transform.position, enemyPosition);

        while (distancia > 0.1f)
        {
            transform.position += direccion * movSpeed * Time.deltaTime;
            distancia = Vector3.Distance(transform.position, enemyPosition);
            yield return null;
        }

        PlayAttackSound();  // Reproducir el sonido de ataque aquí

        StartCoroutine(TpBack());
    }

    IEnumerator TpBack()
    {
        yield return new WaitForSeconds(0.3f);
        transform.position = startingPosition;
    }

    void ResetAttackTimers()
    {
        // Reiniciar el temporizador de ataque para cada enemigo
        foreach (EnemyScript enemy in enemies)
        {
            attackTimers[enemy] = Time.time;
        }

        // Establecer el temporizador de ataque global al inicio del juego
        attackCd = Time.time;
    }
}
