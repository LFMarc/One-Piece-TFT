using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    public float hp;
    public float maxHp;
    public float defense;

    public Animator charAnim;
    public Image healthBar;

    private bool isDead = false;  //Flag para evitar muerte doble

    void Start()
    {
        hp = maxHp;
        UpdateHealthBar();
    }

    public void RecieveDamage(float damage)
    {
        StartCoroutine(ApplyDamageWithDelay(damage));
    }

    private IEnumerator ApplyDamageWithDelay(float damage)
    {
        yield return new WaitForSeconds(0.5f);

        if (isDead) yield break;

        float finalDamage = Mathf.Max(damage - defense, 0);
        hp -= finalDamage;

        UpdateHealthBar();

        if (hp <= 0 && !isDead)
        {
            isDead = true;
            StartCoroutine(Die());
        }
    }

    private void UpdateHealthBar()
    {
        hp = Mathf.Clamp(hp, 0, maxHp);

        if (healthBar != null)
        {
            healthBar.fillAmount = hp / maxHp;
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