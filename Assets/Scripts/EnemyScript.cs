using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    [Header("Stats")]
    public float hp;
    public float maxHp;
    public float defense;
    public float damage;
    public float attackSpeed = 1f;

    [Header("References")]
    public Animator charAnim;
    public Image healthBar;
    public Transform attackTarget;

    private bool isDead = false;
    private Coroutine attackRoutine;

    void Start()
    {
        hp = maxHp;
        UpdateHealthBar();

        if (attackTarget == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                attackTarget = playerObj.transform;
        }

        if (attackTarget != null)
        {
            attackRoutine = StartCoroutine(AttackLoop());
        }
    }

    public void ReceiveDamage(float dmg)
    {
        StartCoroutine(ApplyDamageWithDelay(dmg));
    }

    private IEnumerator ApplyDamageWithDelay(float dmg)
    {
        yield return new WaitForSeconds(0.5f);

        if (isDead) yield break;

        float finalDamage = Mathf.Max(dmg - defense, 0);
        hp -= finalDamage;

        UpdateHealthBar();

        if (hp <= 0 && !isDead)
        {
            isDead = true;
            if (attackRoutine != null) StopCoroutine(attackRoutine);
            StartCoroutine(Die());
        }
    }

    private void UpdateHealthBar()
    {
        hp = Mathf.Clamp(hp, 0, maxHp);
        if (healthBar != null)
            healthBar.fillAmount = hp / maxHp;
    }

    private IEnumerator AttackLoop()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(attackSpeed);

            if (attackTarget != null)
            {
                charAnim.SetTrigger("Attack");

                var targetSystem = attackTarget.GetComponent<BattleSystem>();
                if (targetSystem != null)
                {
                    targetSystem.ReceiveDamage(damage);
                }
            }
        }
    }

    IEnumerator Die()
    {
        charAnim.SetTrigger("Die");
        Debug.Log(gameObject.name + " ha muerto.");
        GameManager.Instance.EnemyDefeated();
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
