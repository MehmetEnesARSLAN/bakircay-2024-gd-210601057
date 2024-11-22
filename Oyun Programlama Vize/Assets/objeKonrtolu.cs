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
    public Vector3 minBounds = new Vector3(-10, 0, -10);
    public Vector3 maxBounds = new Vector3(10, 5, 10);

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
        }
    }

    void Update()
    {
        // Fare ile hareket kontrolü
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    // Obje yerden kaldırılırken mevcut rotasyonu korunur
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
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            // Obje, zemine paralel olacak şekilde taşınır
            Vector3 targetPosition = GetMouseWorldPosition() + offset;
            Vector3 moveDirection = targetPosition - transform.position;

            // Obje hareketini sınırlar arasında tut
            transform.position = ClampPositionToBounds(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));

            // Fare bırakıldığında doğrultuyu belirlemek için hızı kaydet
            releaseVelocity = moveDirection * dragSpeed;
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            // Fare bırakıldığında objeye son hızını ver
            isDragging = false;
            if (rb != null)
            {
                rb.useGravity = true;
                rb.velocity = releaseVelocity; // Fare doğrultusunda hareket
            }
        }

        // Yavaşlama efekti
        if (!isDragging && rb != null)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * deceleration);
        }

        // Oyun alanı sınır kontrolü
        CheckBounds();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private Vector3 ClampPositionToBounds(Vector3 position)
    {
        // Pozisyonu belirtilen sınırlar arasında tut
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
            Vector3 clampedPosition = new Vector3(Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x), transform.position.y, transform.position.z);
            transform.position = clampedPosition;
            if (rb != null) rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
        }

        if (transform.position.z < minBounds.z || transform.position.z > maxBounds.z)
        {
            Vector3 clampedPosition = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z, minBounds.z, maxBounds.z));
            transform.position = clampedPosition;
            if (rb != null) rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
        }

        if (transform.position.y < minBounds.y)
        {
            transform.position = new Vector3(transform.position.x, minBounds.y, transform.position.z);
            if (rb != null) rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Obje kenarlara çarptığında yukarı zıplamasını engelle
        if (rb != null)
        {
            Vector3 velocity = rb.velocity;
            velocity.y = 0; // Y eksenindeki hareketi sıfırla
            rb.velocity = velocity;
        }
    }
}
