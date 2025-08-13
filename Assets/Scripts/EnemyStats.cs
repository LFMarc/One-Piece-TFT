using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float maxHp;
    public float currentHp;
    public float defense;
    public float damage;
    public float attackSpeed;
    //public int goldReward;
    //public int xpReward;

    // Ejemplo de recibir daño
    public void TakeDamage(float amount)
    {
        currentHp -= amount;
        if (currentHp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto.");
        GameManager.Instance.EnemyDefeated();
        Destroy(gameObject);
    }
}
