using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject shooterPrefab;
    public float shootForce = 10f;
    private Vector3 aimDirection;
    private bool isAiming = false;
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

    void OnMouseDown()
    {
        isAiming = true;
    }

    void OnMouseDrag()
    {
        if (!isAiming) return;
        line.enabled = true;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        aimDirection = (mousePos - shooterOrigin).normalized;
        Debug.DrawLine(shooterOrigin, shooterOrigin + aimDirection * 2, Color.green);
        line.SetPosition(0, shooterOrigin);
        line.SetPosition(1, shooterOrigin + aimDirection * 2f);
    }

    void OnMouseUp()
    {
        if (!isAiming) return;
        line.enabled = false;
        isAiming = false;
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GameObject newBubble = Instantiate(shooterPrefab, shooterOrigin, Quaternion.identity);
        Rigidbody2D rb = newBubble.GetComponent<Rigidbody2D>();
        rb.linearVelocity = aimDirection * shootForce;
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bubble") || collision.gameObject.CompareTag("Shooter"))
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
        //Debug.Log(sr.sprite);
        sr.color = Color.white;

        gridManager.assignToMatrix(b, snappedCell.x, snappedCell.y);

        //gridManager.DestroySimilar(sr, snappedCell.x, snappedCell.y);
        int destroyed = gridManager.DestroySimilar2(sr, snappedCell.x, snappedCell.y);
        if (destroyed > 0)
        {
            int row = snappedCell.x;
            int col = snappedCell.y;
            gridManager.DestroySelf(row, col);
        }
    }

}