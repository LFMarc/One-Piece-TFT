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
        // Delay para recibir dmg y que vaya con la animacion
        yield return new WaitForSeconds(0.5f);

        float finalDamage = Mathf.Max(damage - defense, 0);
        hp -= finalDamage;

        UpdateHealthBar();

        if (hp <= 0)
        {
            StartCoroutine(Die());
        }
    }

    // Método para actualizar la barra de vida
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
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}