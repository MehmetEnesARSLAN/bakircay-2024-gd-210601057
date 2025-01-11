using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TeleportManager : MonoBehaviour
{
    [Header("Teleport Targets")]
    public Transform target1;
    public Transform target2;

    [Header("Teleport Activation")]
    public Button teleportButton; // Işınlama düğmesi
    public float activeDuration = 3f; // Işınlama etkinlik süresi
    public float cooldownDuration = 5f; // Işınlama bekleme süresi
    public bool IsTeleportActive { get; private set; } = false;

    private MatchingArea matchingArea; // MatchingArea referansı

    private void Start()
    {
        // Teleport düğmesine işlev ekle
        if (teleportButton != null)
        {
            teleportButton.onClick.AddListener(ActivateTeleport);
        }

        // MatchingArea referansını bul
        matchingArea = FindObjectOfType<MatchingArea>();
        if (matchingArea == null)
        {
            Debug.LogError("MatchingArea bulunamadı! MatchingArea bileşenini sahnede ekli olduğundan emin olun.");
        }
    }

    private void Update()
    {
        // Işınlama aktifken tıklama kontrolü
        if (IsTeleportActive && Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.CompareTag("Item"))
            {
                TeleportObject(hit.collider.gameObject);
            }
        }
    }

    private void TeleportObject(GameObject obj)
{
    if (matchingArea == null)
    {
        Debug.LogError("MatchingArea referansı eksik!");
        return;
    }

    GameObject prefab = matchingArea.GetPrefabForObject(obj);
    if (prefab == null)
    {
        Debug.LogError("Prefab bulunamadı: " + obj.name);
        return;
    }

    Transform target = null;
    if (target1 != null && target1.childCount == 0)
    {
        target = target1;
    }
    else if (target2 != null && target2.childCount == 0)
    {
        target = target2;
    }

    if (target != null)
    {
        // Eski nesneyi yok et ve prefab'ı spawnla
        Destroy(obj);
        matchingArea.RemovePrefabMapping(obj);

        GameObject newObject = Instantiate(prefab, target.position, target.rotation);
        newObject.transform.SetParent(target);

        // Yeni nesneyi MatchingArea'ya kaydet
        matchingArea.RegisterSpawnedObject(newObject, prefab);
    }
    else
    {
        Debug.Log("Her iki hedef de dolu!");
    }
}




    public void HandleObjectClick(GameObject obj)
{
    if (!IsTeleportActive) return;

    Transform target = null;

    if (target1 != null && target1.childCount == 0)
    {
        target = target1;
    }
    else if (target2 != null && target2.childCount == 0)
    {
        target = target2;
    }

    if (target != null)
    {
        Destroy(obj);
        Instantiate(obj, target.position, target.rotation, target); // Objeyi yeniden spawnla
    }
    else
    {
        Debug.Log("Her iki hedef de dolu!");
    }
}


    private void ActivateTeleport()
    {
        if (!IsTeleportActive)
        {
            IsTeleportActive = true;
            Debug.Log("Işınlama aktif!");
            teleportButton.interactable = false;

            Invoke(nameof(DeactivateTeleport), activeDuration);
        }
    }

    private void DeactivateTeleport()
    {
        IsTeleportActive = false;
        Debug.Log("Işınlama devre dışı!");

        Invoke(nameof(EnableButton), cooldownDuration);
    }

    private void EnableButton()
    {
        teleportButton.interactable = true;
        Debug.Log("Işınlama düğmesi yeniden etkin!");
    }
}
