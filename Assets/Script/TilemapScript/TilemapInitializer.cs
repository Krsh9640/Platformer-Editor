using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapInitializer : MonoBehaviour
{
    [SerializeField] private List<TileCategory> categoriesToCreateTilemapFor;
    [SerializeField] private Transform grid;

    private GameObject manager;
    private GameState gameState;

    private void Awake()
    {
        manager = GameObject.Find("Manager");
        gameState = manager.GetComponent<GameState>();
    }

    private void Start()
    {
        foreach (TileCategory category in categoriesToCreateTilemapFor)
        {
            GameObject obj = new GameObject(category.name);
            obj.layer = 7;

            Tilemap map = obj.AddComponent<Tilemap>();
            TilemapRenderer tr = obj.AddComponent<TilemapRenderer>();
            TilemapCollider2D tc = obj.AddComponent<TilemapCollider2D>();
            CompositeCollider2D cmp = obj.AddComponent<CompositeCollider2D>();

            Rigidbody2D rigidbody2D = obj.GetComponent<Rigidbody2D>();
            rigidbody2D.bodyType = RigidbodyType2D.Static;

            tc.usedByComposite = true;

            obj.transform.SetParent(grid);

            tr.sortingOrder = category.SortingOrder;

            category.Tilemap = map;
        }
    }
}