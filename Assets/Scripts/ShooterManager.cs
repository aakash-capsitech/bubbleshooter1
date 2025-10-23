using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ShooterManager : MonoBehaviour
{
    //public GameObject actualShooter;
    public GameObject shooterPrefab;
    public float shootForce = 10f;
    public GameObject bubblePrefab;
    public bool hasShot = false;
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
        shooters = new GameObject[3];
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
            GameObject ns = Instantiate(shooterPrefab, new Vector3((shooterOrigin.x - i)/1.2f, shooterOrigin.y, shooterOrigin.z), shooterPrefab.transform.rotation);
            shooters[i] = ns;
            rb2d = ns.GetComponent<Rigidbody2D>();
            rb2d.bodyType = RigidbodyType2D.Static;
            ns.transform.localScale = Vector3.one * (bubbleRadius * 0.80f);
            SpriteRenderer sr = ns.GetComponent<SpriteRenderer>();
            sr.sprite = shooterSprites[Random.Range(0, shooterSprites.Length)];
            sr.color = Color.white;
            //toggleInputs(ns, false);
        }
        GameObject nso = Instantiate(shooterPrefab,transform.position, shooterPrefab.transform.rotation);
        rb2d = nso.GetComponent<Rigidbody2D>();
        rb2d.bodyType = RigidbodyType2D.Static;
        nso.transform.localScale = Vector3.one * (bubbleRadius * 0.80f);
        SpriteRenderer sro = nso.GetComponent<SpriteRenderer>();
        sro.sprite = shooterSprites[Random.Range(0, shooterSprites.Length)];
        sro.color = Color.white;
        //nso.GetComponent<Shooter>().enabled = true;
        //toggleInputs(nso, true);
    }

    private void GenerateShooter()
    {
        for (int i = 0; i < 2; i++)
        {
            shooters[i] = shooters[i+1];
            GameObject og = shooters[i];

            og.transform.position = new Vector3((shooterOrigin.x - i - 1) / 1.2f, shooterOrigin.y, shooterOrigin.z);
            //og.GetComponent<Shooter>().enabled = true;
            //toggleInputs(og, true);
        }
        GameObject ns = Instantiate(shooterPrefab, new Vector3((shooterOrigin.x - 2 - 1) / 1.2f, shooterOrigin.y, shooterOrigin.z), shooterPrefab.transform.rotation);
        shooters[2] = ns;
        rb2d = ns.GetComponent<Rigidbody2D>();
        rb2d.bodyType = RigidbodyType2D.Static;
        ns.transform.localScale = Vector3.one * (bubbleRadius * 0.80f);
        SpriteRenderer sr = ns.GetComponent<SpriteRenderer>();
        sr.sprite = shooterSprites[Random.Range(0, shooterSprites.Length)];
        sr.color = Color.white;
        //toggleInputs(ns, false);
    }

    private void toggleInputs(GameObject shooter, bool isActive)
    {
        Shooter s = shooter.GetComponent<Shooter>();
        if (s != null)
        {
            s.enabled = false;
        }
        Collider2D col = shooter.GetComponent<Collider2D>();
        if(col != null)
        {
            col.enabled = isActive;
        }
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

            if(og != null)
            {
                og.transform.position = shooterOrigin;
                og.GetComponent<Shooter>().enabled = true;
                GenerateShooter();
            }

        }
    }
}