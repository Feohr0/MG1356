using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    // D��man�n ba�lang�� can�
    public int health = 2;

    // D��ar�dan �a�r�lacak hasar alma fonksiyonu
    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        Debug.Log(gameObject.name + " hasar ald�! Kalan can: " + health);

        // Can� s�f�r veya alt�na d��erse
        if (health <= 0)
        {
            Die();
        }
    }

    // D��man yok olma fonksiyonu
    private void Die()
    {
        Debug.Log(gameObject.name + " yok edildi!");
        // Burada patlama efekti, ses vs. eklenebilir.
        Destroy(gameObject);
    }
}