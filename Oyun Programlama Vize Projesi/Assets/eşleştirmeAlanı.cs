using UnityEngine;

public class MatchingArea : MonoBehaviour
{
    // Alanda bulunan objeler
    private GameObject firstObject = null;
    private GameObject secondObject = null;

    public float throwForce = 5f; // Fırlatma kuvveti
    public Transform gameAreaCenter; // Oyun alanının merkez noktası

    private void OnTriggerEnter(Collider other)
    {
        // Alanın ilk objesi yoksa bu objeyi ekle
        if (firstObject == null)
        {
            firstObject = other.gameObject;
        }
        else if (secondObject == null && other.gameObject != firstObject)
        {
            // İkinci obje yoksa ve farklı bir obje ise ikinci olarak ekle
            secondObject = other.gameObject;

            // İki obje aynı mı kontrol et
            if (firstObject.tag == secondObject.tag)
            {
                // Aynı tag'e sahiplerse 1 saniye gecikme ile kaybolsunlar
                StartCoroutine(DestroyWithDelay(firstObject, secondObject, 1f));

                // Alanı boşalt
                firstObject = null;
                secondObject = null;
            }
            else
            {
                // Farklı tag'e sahiplerse oyun alanının merkezine doğru fırlat
                ThrowObjectTowardsGameAreaCenter(firstObject);
                ThrowObjectTowardsGameAreaCenter(secondObject);

                // Alanı boşalt
                firstObject = null;
                secondObject = null;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Obje alandan çıkarsa slotunu boşalt
        if (other.gameObject == firstObject)
        {
            firstObject = null;
        }
        else if (other.gameObject == secondObject)
        {
            secondObject = null;
        }
    }

    private System.Collections.IEnumerator DestroyWithDelay(GameObject obj1, GameObject obj2, float delay)
    {
        // Gecikme süresi
        yield return new WaitForSeconds(delay);

        // Objeleri yok et
        Destroy(obj1);
        Destroy(obj2);
    }

    private void ThrowObjectTowardsGameAreaCenter(GameObject obj)
    {
        // Objede Rigidbody kontrol et
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = obj.AddComponent<Rigidbody>(); // Rigidbody yoksa ekle
        }

        // Oyun alanının merkezine doğru yön
        Vector3 centerPosition = gameAreaCenter.position; // Oyun alanının merkez noktası
        Vector3 throwDirection = (centerPosition - obj.transform.position).normalized; // Yön vektörü
        rb.AddForce(throwDirection * throwForce, ForceMode.Impulse); // Kuvvet uygula
    }
}
