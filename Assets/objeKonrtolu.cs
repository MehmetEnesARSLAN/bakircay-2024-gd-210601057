 using UnityEngine;

public class ObjectController : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private Rigidbody rb;

    // Hareket ve fiziksel davranış parametreleri
    public float liftAmount = 1.0f; // Obje kaldırma yüksekliği
    public float dragSpeed = 10f; // Obje sürükleme hızı
    public float deceleration = 5f; // Hız azalması (yavaşlama)

    private Vector3 releaseVelocity; // Obje bırakıldığında fare doğrultusundaki hız

    // Oyun alanı sınırları
    public Vector3 minBounds = new Vector3(-10, 0, -10); // Minimum sınırlar
    public Vector3 maxBounds = new Vector3(10, 5, 10);  // Maksimum sınırlar

    // TeleportManager'dan ışınlama durumunu kontrol et
    public TeleportManager teleportManager;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
        }
        teleportManager = FindObjectOfType<TeleportManager>();
    }

    void Update()
    {
        // Eğer ışınlama aktifse, sürükleme işlemini devre dışı bırak
        if (teleportManager != null && teleportManager.IsTeleportActive && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    teleportManager.HandleObjectClick(gameObject);
                }
            }
        }

        HandleDrag(); // Sürükleme işlemini yönet
        CheckBounds(); // Sınır kontrolü
    }

    private void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    StartDragging();
                }
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            DragObject();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            StopDragging();
        }

        if (!isDragging && rb != null)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * deceleration);
        }
    }

    
    private void StartDragging()
    {
        Vector3 newPosition = transform.position + new Vector3(0, liftAmount, 0);
        transform.position = newPosition;

        if (rb != null)
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero; // Önceki hareketi sıfırla
        }

        isDragging = true;
        offset = transform.position - GetMouseWorldPosition();
    }

    private void DragObject()
{
    Vector3 targetPosition = GetMouseWorldPosition() + offset;
    Vector3 moveDirection = targetPosition - transform.position; // Hareket doğrultusunu hesapla

    transform.position = ClampPositionToBounds(new Vector3(
        targetPosition.x,
        transform.position.y,
        targetPosition.z
    ));

    releaseVelocity = moveDirection * dragSpeed; // Hız hesaplamasını burada yap
}


    private void StopDragging()
{
    isDragging = false;

    if (rb != null)
    {
        rb.velocity = releaseVelocity; // Fare doğrultusunda hız uygula
        rb.useGravity = true; // Yerçekimini etkinleştir
        rb.isKinematic = false; // Fiziği yeniden aktif hale getir
    }
}



    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private Vector3 ClampPositionToBounds(Vector3 position)
    {
        return new Vector3(
            Mathf.Clamp(position.x, minBounds.x, maxBounds.x),
            Mathf.Clamp(position.y, minBounds.y, maxBounds.y),
            Mathf.Clamp(position.z, minBounds.z, maxBounds.z)
        );
    }

    private void CheckBounds()
    {
        if (transform.position.x < minBounds.x || transform.position.x > maxBounds.x)
        {
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
                transform.position.y,
                transform.position.z
            );
        }

        if (transform.position.z < minBounds.z || transform.position.z > maxBounds.z)
        {
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                Mathf.Clamp(transform.position.z, minBounds.z, maxBounds.z)
            );
        }

        if (transform.position.y < minBounds.y || transform.position.y > maxBounds.y)
        {
            transform.position = new Vector3(
                transform.position.x,
                Mathf.Clamp(transform.position.y, minBounds.y, maxBounds.y),
                transform.position.z
            );
        }
    }
}
