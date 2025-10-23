using UnityEngine;

[ExecuteAlways]
public class WallResizer : MonoBehaviour
{
    public Camera cam;
    public Transform leftWall;
    public Transform rightWall;
    public Transform topWall;
    public Transform bottomWall;

    public float wallThickness = 1f;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        PositionWalls();
    }

    void Update()
    {
        #if UNITY_EDITOR
                PositionWalls();
        #endif
    }

    void PositionWalls()
    {
        if (cam == null) return;

        float zDist = Mathf.Abs(cam.transform.position.z);

        Vector3 bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, zDist));
        Vector3 topRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, zDist));

        float worldWidth = topRight.x - bottomLeft.x;
        float worldHeight = topRight.y - bottomLeft.y;

        if (leftWall)
        {
            leftWall.position = new Vector3(bottomLeft.x - wallThickness / 2f, 0, 0);
            leftWall.localScale = new Vector3(wallThickness, worldHeight * 2, 1);
        }

        if (rightWall)
        {
            rightWall.position = new Vector3(topRight.x + wallThickness / 2f, 0, 0);
            rightWall.localScale = new Vector3(wallThickness, worldHeight * 2, 1);
        }

        if (topWall)
        {
            topWall.position = new Vector3(0, topRight.y + wallThickness / 2f, 0);
            topWall.localScale = new Vector3(worldWidth * 2, wallThickness, 1);
        }

        if (bottomWall)
        {
            bottomWall.position = new Vector3(0, bottomLeft.y - wallThickness / 2f, 0);
            bottomWall.localScale = new Vector3(worldWidth * 2, wallThickness, 1);
        }
    }
}
