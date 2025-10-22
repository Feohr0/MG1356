using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    // Düþmanýn baþlangýç caný
    public int health = 2;

    // Dýþarýdan çaðrýlacak hasar alma fonksiyonu
    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        Debug.Log(gameObject.name + " hasar aldý! Kalan can: " + health);

        // Caný sýfýr veya altýna düþerse
        if (health <= 0)
        {
            Die();
        }
    }

    // Düþman yok olma fonksiyonu
    private void Die()
    {
        Debug.Log(gameObject.name + " yok edildi!");
        // Burada patlama efekti, ses vs. eklenebilir.
        Destroy(gameObject);
    }
}