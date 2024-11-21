using UnityEngine;

public class ObjectController : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private Rigidbody rb;

    // Yükseklik değişkeni
    public float liftAmount = 1.0f;

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
            transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        }

        if (Input.GetMouseButtonUp(0))
        {
            // Fare bırakıldığında obje serbest kalır ve yer çekimine tabi olur
            isDragging = false;
            if (rb != null)
            {
                rb.useGravity = true;
            }
        }

        // Dokunmatik ekran kontrolü
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));

            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (touch.phase == TouchPhase.Began)
            {
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
                        }

                        isDragging = true;
                        offset = transform.position - touchPosition;
                    }
                }
            }

            if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isDragging)
            {
                // Dokunmatik ile objeyi zemine paralel hareket ettirme
                Vector3 targetPosition = touchPosition + offset;
                transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                // Dokunma bırakıldığında obje serbest kalır ve yer çekimine tabi olur
                isDragging = false;
                if (rb != null)
                {
                    rb.useGravity = true;
                }
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
