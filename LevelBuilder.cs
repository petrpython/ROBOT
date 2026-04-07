using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public GameObject wallPrefab;
    public float arenaSize = 20f;
    public float wallThickness = 1f;

    void Start()
    {
        CreateArena();
    }

    void CreateArena()
    {
        // Верхняя стена
        CreateWall(new Vector3(0, arenaSize / 2 + wallThickness / 2, 0),
                   new Vector3(arenaSize + wallThickness * 2, wallThickness, 1));

        // Нижняя стена
        CreateWall(new Vector3(0, -arenaSize / 2 - wallThickness / 2, 0),
                   new Vector3(arenaSize + wallThickness * 2, wallThickness, 1));

        // Левая стена
        CreateWall(new Vector3(-arenaSize / 2 - wallThickness / 2, 0, 0),
                   new Vector3(wallThickness, arenaSize, 1));

        // Правая стена
        CreateWall(new Vector3(arenaSize / 2 + wallThickness / 2, 0, 0),
                   new Vector3(wallThickness, arenaSize, 1));
    }

    void CreateWall(Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Quad);
        wall.name = "Wall";
        wall.transform.position = position;
        wall.transform.localScale = scale;

        // Материал стены
        SpriteRenderer sr = wall.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = new Color(100f / 255, 100f / 255, 100f / 255);

        // Настройка коллайдера
        BoxCollider2D collider = wall.GetComponent<BoxCollider2D>();
        if (collider != null)
            collider.size = new Vector2(scale.x, scale.y);

        // Слой Walls
        wall.layer = LayerMask.NameToLayer("Walls");
    }
}
