using UnityEngine;

public class Bubble : MonoBehaviour
{
    public int row;
    public int col;
    public SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}