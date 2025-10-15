using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ShooterManager : MonoBehaviour
{
    public GameObject shooterPrefab;
    public float shootForce = 10f;
    public GameObject bubblePrefab;
    public bool hasShot = true;
    private Vector3 shooterOrigin = new Vector3(0, -4, 0);
    private float bubbleRadius = 0.30f;
    public BubbleSpawnManager bubbleSpawnManager;
    private Rigidbody2D rb2d;
    public static int shoots = 0;
    public Sprite[] shooterSprites;
    private GameObject[] shooters;

    private void Awake()
    {
        bubbleSpawnManager = FindFirstObjectByType<BubbleSpawnManager>();
    }

    private void Start()
    {
        shooters = new GameObject[6];
        GenerateShooters();
    }

    private void Update()
    {
        helpShot2();
    }

    private void GenerateShooters()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject ns = Instantiate(shooterPrefab, new Vector3((shooterOrigin.x - i -1)/1.5f, shooterOrigin.y, shooterOrigin.z), shooterPrefab.transform.rotation);
            shooters[i] = ns;
            rb2d = ns.GetComponent<Rigidbody2D>();
            rb2d.bodyType = RigidbodyType2D.Static;
            ns.transform.localScale = Vector3.one * (bubbleRadius * 0.80f);
            SpriteRenderer sr = ns.GetComponent<SpriteRenderer>();
            sr.sprite = shooterSprites[Random.Range(0, shooterSprites.Length)];
            sr.color = Color.white;
        }
    }

    private void GenerateShooter()
    {
        for (int i = 1; i < 3; i++)
        {
            shooters[i - 1] = shooters[i];
            GameObject og = shooters[i-1];

            og.transform.position = new Vector3((shooterOrigin.x - i-1 - 1) / 1.5f, shooterOrigin.y, shooterOrigin.z);
        }
        GameObject ns = Instantiate(shooterPrefab, new Vector3((shooterOrigin.x - 2 - 1) / 1.5f, shooterOrigin.y, shooterOrigin.z), shooterPrefab.transform.rotation);
        shooters[2] = ns;
        rb2d = ns.GetComponent<Rigidbody2D>();
        rb2d.bodyType = RigidbodyType2D.Static;
        ns.transform.localScale = Vector3.one * (bubbleRadius * 0.80f);
        SpriteRenderer sr = ns.GetComponent<SpriteRenderer>();
        sr.sprite = shooterSprites[Random.Range(0, shooterSprites.Length)];
        sr.color = Color.white;
    }

    public void helpShot2()
    {
        if (hasShot)
        {
            
            shoots++;
            int rs = Random.Range(0, 20);
            if (rs >= 10 && shoots >= 7)
            {
                shoots = 0;
                bubbleSpawnManager.rearrangeGrid();
            }
            hasShot = false;

            GameObject og = shooters[0];

            og.transform.position = shooterOrigin;
            Debug.Log(shooterOrigin);
            GenerateShooter();
        }
    }

    public void helpShot()
    {
        if (hasShot)
        {
            //GenerateShooter();
            shoots++;
            int rs = Random.Range(0, 20);
            if (rs >= 10 && shoots >= 7)
            {
                shoots = 0;
                bubbleSpawnManager.rearrangeGrid();
            }
            hasShot = false;
            GameObject ns = Instantiate(shooterPrefab, shooterOrigin, shooterPrefab.transform.rotation);
            rb2d = ns.GetComponent<Rigidbody2D>();
            rb2d.bodyType = RigidbodyType2D.Static;
            ns.transform.localScale = Vector3.one * (bubbleRadius * 0.80f);
            SpriteRenderer sr = ns.GetComponent<SpriteRenderer>();
            sr.sprite = shooterSprites[Random.Range(0, shooterSprites.Length)];
            sr.color = Color.white;
        }
    }
}