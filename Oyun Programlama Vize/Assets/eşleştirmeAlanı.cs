using UnityEngine;

public class MatchingArea : MonoBehaviour
{
    [SerializeField]
    private GameObject destructionEffectPrefab; // Efekt prefab'ı
    [SerializeField]
    private Vector3 effectPosition = Vector3.zero; // Efektin sabit konumu
    [SerializeField]
    private string targetObjectName = "SpecialObject"; // Belirli obje adı

    private void OnTriggerEnter(Collider other)
    {
        // Eğer belirli bir objeyle çarpışılmışsa (adı kontrol ediliyor)
        if (other.CompareTag("Item") && other.name == targetObjectName)
        {
            StartCoroutine(DestroyWithEffect(other.gameObject, 0.5f));
        }
    }

    private System.Collections.IEnumerator DestroyWithEffect(GameObject obj, float delay)
    {
        // Gecikme süresini bekle
        yield return new WaitForSeconds(delay);

        // Obje yok edilir
        Destroy(obj);

        // Eğer destructionEffectPrefab atanmışsa, efekti sabit pozisyonda tetikle
        if (destructionEffectPrefab != null)
        {
            Instantiate(destructionEffectPrefab, effectPosition, Quaternion.identity);
        }
    }
}





 
