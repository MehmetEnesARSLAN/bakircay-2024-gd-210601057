using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchingArea : MonoBehaviour
{
    [Header("Matching and Snap Points")]
    [SerializeField] private Transform[] snapPoints; // Sabit eşleşme noktaları
    [SerializeField] private float snapDuration = 0.5f; // Yerleştirme animasyon süresi

    [Header("Effects and Animation")]
    [SerializeField] private GameObject destructionEffectPrefab; // Efekt prefab'ı
    [SerializeField] private Transform gameAreaCenter; // Oyun alanı merkezi (hatalı eşleşme için)
    [SerializeField] private Transform matchAreaCenter; // Eşleşme alanı merkezi (doğru eşleşme için)
    [SerializeField] private float pushForce = 5f; // İttirme gücü
    [SerializeField] private float delay = 1.5f; // Gecikme süresi

    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] pairPrefabs; // Çift nesnelerin prefab'ları
    [SerializeField] private Transform[] spawnPoints; // Özel spawn noktaları

    private Dictionary<Transform, GameObject> placedObjects = new Dictionary<Transform, GameObject>();
    private Dictionary<GameObject, GameObject> prefabMapping = new Dictionary<GameObject, GameObject>();
    private int activeObjectCount = 0; // Sahnedeki aktif nesne sayısı

    private void Start()
    {
        SpawnPairs(); // Oyunu başlatırken çiftleri spawn et
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            Transform nearestSnapPoint = GetNearestSnapPoint(other.transform);

            if (nearestSnapPoint != null && !placedObjects.ContainsKey(nearestSnapPoint))
            {
                StartCoroutine(SnapToPosition(other.gameObject, nearestSnapPoint));
                placedObjects[nearestSnapPoint] = other.gameObject;

                CheckForMatch();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            foreach (var pair in placedObjects)
            {
                if (pair.Value == other.gameObject)
                {
                    placedObjects.Remove(pair.Key);
                    break;
                }
            }
        }
    }

    private Transform GetNearestSnapPoint(Transform obj)
    {
        Transform nearestPoint = null;
        float minDistance = float.MaxValue;

        foreach (Transform snapPoint in snapPoints)
        {
            if (!placedObjects.ContainsKey(snapPoint))
            {
                float distance = Vector3.Distance(obj.position, snapPoint.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestPoint = snapPoint;
                }
            }
        }

        return nearestPoint;
    }

    private IEnumerator SnapToPosition(GameObject obj, Transform targetPosition)
    {
        Vector3 startPosition = obj.transform.position;
        Vector3 endPosition = targetPosition.position;

        float elapsedTime = 0f;

        while (elapsedTime < snapDuration)
        {
            obj.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / snapDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = endPosition;
    }

    private void CheckForMatch()
{
    if (placedObjects.Count == snapPoints.Length)
    {
        GameObject obj1 = placedObjects[snapPoints[0]];
        GameObject obj2 = placedObjects[snapPoints[1]];

        // Prefab eşleşmesini kontrol et
        if (prefabMapping.ContainsKey(obj1) && prefabMapping.ContainsKey(obj2))
        {
            GameObject prefab1 = prefabMapping[obj1];
            GameObject prefab2 = prefabMapping[obj2];

            if (prefab1 == prefab2)
            {
                StartCoroutine(DestroyWithDelay(obj1, obj2));
            }
            else
            {
                StartCoroutine(DelayBeforePush(obj1, obj2));
            }
        }
        else
        {
            Debug.LogWarning("Eşleşme kontrolünde prefab bulunamadı!");
        }
    }
}



    private IEnumerator DestroyWithDelay(GameObject obj1, GameObject obj2)
    {
        yield return new WaitForSeconds(delay);

        StartCoroutine(DestroyWithShrinkAnimation(obj1, obj2));
        UIManager.Instance.UpdateScore(10); // Eşleşme başına 10 puan ekle
    }

    private IEnumerator DestroyWithShrinkAnimation(GameObject obj1, GameObject obj2)
    {
        float elapsedTime = 0f;
        Vector3 initialScale1 = obj1.transform.localScale;
        Vector3 initialScale2 = obj2.transform.localScale;
        Vector3 startPosition1 = obj1.transform.position;
        Vector3 startPosition2 = obj2.transform.position;

        Vector3 endPosition = matchAreaCenter.position;

        while (elapsedTime < delay)
        {
            float t = elapsedTime / delay;

            obj1.transform.position = Vector3.Lerp(startPosition1, endPosition, t);
            obj2.transform.position = Vector3.Lerp(startPosition2, endPosition, t);

            obj1.transform.localScale = Vector3.Lerp(initialScale1, Vector3.zero, t);
            obj2.transform.localScale = Vector3.Lerp(initialScale2, Vector3.zero, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        PlayDestructionEffect(endPosition);

        Destroy(obj1);
        Destroy(obj2);

        placedObjects.Clear();

        activeObjectCount -= 2;

        CheckAndSpawnNewObjects();
    }

    private IEnumerator DelayBeforePush(GameObject obj1, GameObject obj2)
    {
        yield return new WaitForSeconds(delay);

        PushTowardsCenter(obj1);
        PushTowardsCenter(obj2);
    }

    private void PushTowardsCenter(GameObject obj)
    {
        if (gameAreaCenter != null)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (gameAreaCenter.position - obj.transform.position).normalized;
                rb.AddForce(direction * pushForce, ForceMode.Impulse);
            }
        }
    }

    private void PlayDestructionEffect(Vector3 position)
    {
        if (destructionEffectPrefab != null)
        {
            Instantiate(destructionEffectPrefab, position, Quaternion.identity);
        }
    }

    private void CheckAndSpawnNewObjects()
    {
        if (activeObjectCount <= 0)
        {
            SpawnPairs();
        }
    }

    private void SpawnPairs()
{
    activeObjectCount = 0;
    prefabMapping.Clear(); // Eski kayıtları temizle
    List<int> usedSpawnPoints = new List<int>();

    foreach (GameObject prefab in pairPrefabs)
    {
        for (int i = 0; i < 2; i++) // Her prefab'dan 2 tane spawn et
        {
            int spawnIndex;

            // Kullanılmayan bir spawn noktası bul
            do
            {
                spawnIndex = Random.Range(0, spawnPoints.Length);
            }
            while (usedSpawnPoints.Contains(spawnIndex));

            usedSpawnPoints.Add(spawnIndex);

            Vector3 spawnPosition = spawnPoints[spawnIndex].position;
            GameObject spawnedObject = Instantiate(prefab, spawnPosition, Quaternion.identity);

            // Prefab ile nesne eşleştir
            prefabMapping[spawnedObject] = prefab;

            activeObjectCount++;
        }
    }
}


    public void ResetGame()
    {
    // Sahnedeki mevcut nesneleri temizle
    foreach (GameObject obj in FindObjectsOfType<GameObject>())
    {
        if (obj.CompareTag("Item"))
        {
            Destroy(obj);
        }
              
    }
    prefabMapping.Clear();
    // Eşleşme alanını ve spawn noktalarını sıfırla
    placedObjects.Clear();
    activeObjectCount = 0;

    // Yeni çiftleri spawn et
    SpawnPairs();
    }

    public GameObject GetPrefabForObject(GameObject obj)
{
    if (prefabMapping.ContainsKey(obj))
    {
        return prefabMapping[obj];
    }
    return null;
}

public void RemovePrefabMapping(GameObject obj)
    {
        if (prefabMapping.ContainsKey(obj))
        {
            prefabMapping.Remove(obj);
        }
    }

    public void RegisterSpawnedObject(GameObject spawnedObject, GameObject prefab)
{
    if (!prefabMapping.ContainsKey(spawnedObject))
    {
        prefabMapping[spawnedObject] = prefab;
    }
}


}
