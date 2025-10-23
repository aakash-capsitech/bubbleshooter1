using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

[ExecuteInEditMode]
public class BubbleSpawnManager : MonoBehaviour
{
    public GameObject BubblePrefab;
    public Sprite[] bubbleSprites;
    public ShooterManager smng;
    private int rows = 50, cols = 8;
    private float bubbleRadius = 0.30f;
    float horizontalSpacing;
    float verticalSpacing;
    public GameObject particles;
    public int score;
    public TextMeshProUGUI sc;
    public TextMeshProUGUI game;
    public Button restart;
    public Image GameO;
    //public GameObject game;

    Bubble[,] grid;

    void Awake()
    {
        //GenerateGrid();
    }

    private void Start()
    {
        score = 0;
        sc.text = "Score: " + score.ToString();


        Camera cam = Camera.main;
        Vector3 left = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 right = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, cam.nearClipPlane));

        float worldWidth = right.x - left.x;

        cols = Mathf.FloorToInt(worldWidth / (bubbleRadius*2));

        GenerateGrid();

        Debug.Log("Screen Width in World Units: " + worldWidth);
        Debug.Log("Columns that fit: " + cols);
    }

    public void DestroySelf(int i, int j)
    {
        if (grid[i, j] != null)
        {
            Destroy(grid[i, j].gameObject);

            grid[i, j] = null;
        }
    }

    void GenerateGrid()
    {
        ClearGrid();
        if (BubblePrefab == null) return;

        horizontalSpacing = bubbleRadius * 2f;
        verticalSpacing = bubbleRadius * 2f;
        grid = new Bubble[rows, cols];

        for (int r = 0; r < rows-10; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                //float rn = UnityEngine.Random.Range(0f, 9f);
                float rn = 4f;
                if (rn <= 5f)
                {
                    Vector3 pos = GridToWorld(r, c);
                    GameObject b = Instantiate(BubblePrefab, pos, Quaternion.identity, transform);
                    b.transform.localScale = Vector3.one * (bubbleRadius * 0.80f);

                    SpriteRenderer sr = b.GetComponent<SpriteRenderer>();
                    sr.sprite = bubbleSprites[UnityEngine.Random.Range(0, bubbleSprites.Length)];
                    sr.color = Color.white;
                    grid[r, c] = b.GetComponent<Bubble>();
                }
            }
        }
        for(int i=0; i<cols;i++)
        {
            if (grid[0,i] != null)
            {
                Bubble b = grid[0,i];
                SpriteRenderer ss = b.GetComponentInParent<SpriteRenderer>();
                SpriteRenderer ss2 = b.GetComponent<SpriteRenderer>();
            }
        }
    }

    void ClearGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (Application.isEditor)
                DestroyImmediate(transform.GetChild(i).gameObject);
            else
                Destroy(transform.GetChild(i).gameObject);
        }
    }

    public Vector3 GridToWorld(int row, int col)
    {
        float x = (col -(cols/2)) * horizontalSpacing;
        if (((row - 40) & 1) == 0) x += horizontalSpacing / 2f;
        float y = -(row - 40) * verticalSpacing;
        return new Vector3(x, y, 0);
    }

    public void rearrangeGrid()
    {             
        for(int i=rows-1; i>0; i--)
        {
            for(int j=0; j<cols; j++)
            {
                Vector3 pos = GridToWorld(i, j);
                if (grid[i-1, j] == null) continue;
                Bubble bubble = grid[i-1, j];
                bubble.transform.position = pos;
                grid[i, j] = bubble;
                grid[i-1, j] = null;
            }
        }
    }

    public Vector2Int SnapToGrid(Vector3 worldPos)
    {
        int closestRow = 0, closestCol = 0;
        float minDist = Mathf.Infinity;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Vector3 gridPos = GridToWorld(r, c);
                float dist = Vector3.Distance(worldPos, gridPos);
                if (dist < minDist && grid[r,c] == null)
                {
                    minDist = dist;
                    closestRow = r;
                    closestCol = c;
                }
            }
        }

        Vector3 snapPos = GridToWorld(closestRow, closestCol);
        return new Vector2Int(closestRow, closestCol);
    }

    public void assignToMatrix(GameObject b, int r, int c)
    {
        grid[r, c] = b.GetComponent<Bubble>();
        SpriteRenderer sr = b.GetComponent<SpriteRenderer>();
        var gridManager = FindFirstObjectByType<BubbleSpawnManager>();
        //gridManager.DestroySimilar(sr, r, c);
        gridManager.GameOver();
    }

    void GameOver()
    {
        bool dest = false;
        for(int i = rows-1; i >= 0; i--)
        {
            for(int  j = 0; j< cols; j++)
            {
                if (grid[i,j] != null && i>=43)
                {
                    dest = true;
                    break;
                }
            }
        }
        if (!dest)
        {
            return;
        }
        for(int i = rows-1;i >= 0;i--)
        {
            for(int j = 0;j< cols; j++)
            {
                if(grid[i,j] != null)
                {
                    Destroy(grid[i, j].gameObject);

                    grid[i, j] = null;
                }
            }
        }     
        //Debug.Log("Game Over");
        game.gameObject.SetActive(true);
        restart.gameObject.SetActive(true);
        GameO.gameObject.SetActive(true);

    }

    List<(int, int)> FindChildren(int row, int col)
    {
        List<(int, int)> neighbors = new List<(int, int)>();

        int[,] evenDirs = {
        {-1, 0}, {-1, -1},
        {0, -1}, {0, 1},
        {1, 0}, {1, -1}
    };

        int[,] oddDirs = {
        {-1, 0}, {-1, 1},
        {0, -1}, {0, 1},
        {1, 0}, {1, 1}
    };

        int[,] dirs = (row % 2 == 0) ? oddDirs : evenDirs;

        for (int i = 0; i < 6; i++)
        {
            int nr = row + dirs[i, 0];
            int nc = col + dirs[i, 1];

            if (nr >= 0 && nr < rows && nc >= 0 && nc < cols)
                neighbors.Add((nr, nc));
        }

        return neighbors;
    }

    public void DestroySimilar(SpriteRenderer s, int r, int c)
    {
        List<(int, int)> connected = BFSSameBubble(s, r, c);

        int minMatch = 3;
        if (connected.Count >= minMatch)
        {
            //Debug.Log($"Destroying {connected.Count} connected bubbles at ({r},{c})");

            foreach (var (row, col) in connected)
            {
                if (grid[row, col] != null)
                {
                    Destroy(grid[row, col].gameObject);
                    IncrementScore(3);

                    Instantiate(particles, GridToWorld(row, col), particles.transform.rotation);
                    grid[row, col] = null;
                }
            }
        }
        else
        {
            //Debug.Log($"Only {connected.Count} connected — no destruction.");
        }
        ClearHanging();
    }

    public int DestroySimilar2(SpriteRenderer s, int r, int c)
    {
        int destroyed = 0;
        List<(int, int)> connected = BFSSameBubble(s, r, c);

        int minMatch = 3;
        if (connected.Count >= minMatch)
        {
            //Debug.Log($"Destroying {connected.Count} connected bubbles at ({r},{c})");

            foreach (var (row, col) in connected)
            {
                if (grid[row, col] != null)
                {
                    Destroy(grid[row, col].gameObject);
                    destroyed = 1;
                    IncrementScore(3);

                    Instantiate(particles, GridToWorld(row, col), particles.transform.rotation);
                    grid[row, col] = null;
                }
            }
        }
        else
        {
            //Debug.Log($"Only {connected.Count} connected — no destruction.");
        }
        ClearHanging();
        return destroyed;
    }

    List<(int, int)> BFSSameBubble(SpriteRenderer s, int startRow, int startCol)
    {
        var gridManager = FindFirstObjectByType<BubbleSpawnManager>();
        var grid = gridManager.grid;
        int n = grid.GetLength(0);
        int m = grid.GetLength(1);

        Sprite targetSprite = s.sprite;

        bool[,] visited = new bool[n, m];
        List<(int, int)> result = new List<(int, int)>();
        Queue<(int, int)> queue = new Queue<(int, int)>();

        queue.Enqueue((startRow, startCol));
        visited[startRow, startCol] = true;

        while (queue.Count > 0)
        {
            var (r, c) = queue.Dequeue();
            result.Add((r, c));

            foreach (var (nr, nc) in FindChildren(r, c))
            {
                if (visited[nr, nc]) continue;

                Bubble neighbor = grid[nr, nc];
                if (neighbor == null) continue;

                var neighborRenderer = neighbor.GetComponent<SpriteRenderer>();
                if (neighborRenderer != null && neighborRenderer.sprite == targetSprite)
                {
                    visited[nr, nc] = true;
                    queue.Enqueue((nr, nc));
                }
            }
        }

        return result;
    }

    public void ClearHanging()
    {
        if (grid == null) return;

        //int n = grid.GetLength(0);
        //int m = grid.GetLength(1);
        int n = rows;
        int m = cols;

        int fr = 0;

        for(int i = 0;  i < n; i++)
        {
            bool found = false;
            for(int j = 0; j < m; j++)
            {
                if(grid[i, j] != null)
                {
                    found = true;
                    break;
                }
            }
            if(found == true)
            {
                fr = i;
                break;
            }
        }

        bool[,] visited = new bool[n, m];

        Queue<(int, int)> queue = new Queue<(int, int)>();

        for (int c = 0; c < m; c++)
        {
            if (grid[fr, c] != null)
            {
                queue.Enqueue((fr, c));
                visited[fr, c] = true;
            }
        }

        while (queue.Count > 0)
        {
            var (r, c) = queue.Dequeue();

            foreach (var (nr, nc) in FindChildren(r, c))
            {
                if (nr < 0 || nr >= n || nc < 0 || nc >= m) continue;
                if (visited[nr, nc]) continue;

                if (grid[nr, nc] != null)
                {
                    visited[nr, nc] = true;
                    queue.Enqueue((nr, nc));
                }
            }
        }

        for (int r = 0; r < n; r++)
        {
            for (int c = 0; c < m; c++)
            {
                if (grid[r, c] != null && !visited[r, c])
                {
                    Bubble b = grid[r, c];
                    if (b == null) continue;
                    Rigidbody2D rb = b.GetComponent<Rigidbody2D>();
                    rb.gravityScale = 9.8f;
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    //if(rb.transform.position.y < -6)
                    //{
                    //    Destroy(b.gameObject);
                    //}
                    Destroy(grid[r, c].gameObject);
                    grid[r, c] = null;
                    IncrementScore(3);
                }
            }
        }

        //Debug.Log("Cleared hanging bubbles!");
    }

    public void IncrementScore(int scr)
    {
        score += scr;
        sc.text = "Score: " + score.ToString();
    }

    public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}