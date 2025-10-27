using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject shooterPrefab;
    public float shootForce = 10f;
    private Vector3 aimDirection;
    private bool isAiming = true;
    LineRenderer line;
    public GameObject bubblePrefab;
    private Vector3 shooterOrigin = new Vector3(0, -4, 0);
    public SpriteRenderer spriteRenderer;
    public ShooterManager sManager;
    private float bubbleRadius = 0.30f;
    public BubbleSpawnManager BSM;
    public Bubble bub;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        line = GetComponent<LineRenderer>();
        sManager = FindFirstObjectByType<ShooterManager>();
        BSM = FindFirstObjectByType<BubbleSpawnManager>();
    }

    private void Start()
    {
        line.enabled = true;
    }

    private void Update()
    {
        if (!isAiming) return;
        line.enabled = true;


        Vector3 mousePos2 = Input.mousePosition;

        bool isMouseOutsideScreen = mousePos2.x < 0 || mousePos2.x > Screen.width ||
                                    mousePos2.y < 0 || mousePos2.y > Screen.height;

        if (isMouseOutsideScreen)
        {
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        aimDirection = (mousePos - shooterOrigin).normalized;

        if (aimDirection.y < 0f)
        {
            aimDirection.y = 0f;
            aimDirection = aimDirection.normalized;
        }

        float maxAngle = 60f;
        float angleFromUp = Vector3.Angle(Vector3.up, aimDirection);
        if (angleFromUp > maxAngle)
        {
            float sign = Mathf.Sign(aimDirection.x);
            float rad = maxAngle * Mathf.Deg2Rad;
            aimDirection = new Vector3(Mathf.Sin(rad) * sign, Mathf.Cos(rad), 0f).normalized;
        }

        Debug.DrawLine(shooterOrigin, shooterOrigin + aimDirection * 2, Color.green);
        line.SetPosition(0, shooterOrigin);
        line.SetPosition(1, shooterOrigin + aimDirection * 2f);


        if (Input.GetMouseButtonDown(0))
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("ogshooter");
            Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = aimDirection * shootForce;
            
        }
    }

    void OnMouseDown()
    {
        isAiming = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (sManager.hasShot) return;
        if (collision.gameObject.CompareTag("Bubble"))
        {
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            ConvertToBubble();
            Destroy(gameObject);
            sManager.hasShot = true;
        }
    }

    void ConvertToBubble()
    {
        Vector3 hitPos = transform.position;
        var gridManager = FindFirstObjectByType<BubbleSpawnManager>();

        Vector2Int snappedCell = gridManager.SnapToGrid(hitPos);
        Vector3 snappedPos = gridManager.GridToWorld(snappedCell.x, snappedCell.y);

        GameObject b = Instantiate(bubblePrefab, snappedPos, Quaternion.identity);
        b.transform.localScale = Vector3.one * (bubbleRadius * 0.80f);

        SpriteRenderer sr = b.GetComponent<SpriteRenderer>();
        sr.sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
        sr.color = Color.white;

        gridManager.assignToMatrix(b, snappedCell.x, snappedCell.y);

        int destroyed = gridManager.DestroySimilar2(b.GetComponent<SpriteRenderer>(), snappedCell.x, snappedCell.y);
        if (destroyed > 0)
        {
            int row = snappedCell.x;
            int col = snappedCell.y;
        }
        gridManager.ClearHanging();
    }

}