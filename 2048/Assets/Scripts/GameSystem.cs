using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    empty,
    fill
}

public class GameSystem : MonoBehaviour
{
    private const int TILE_SIZE = 4;
    public static GameSystem Inst { get; set; }

    public List<Block> blocks = new List<Block>();
    public int[,] Tile = new int[TILE_SIZE, TILE_SIZE];
    public Color[] blockColors;

    [SerializeField] Block blockPrefab;

    private void Awake()
    {
        Inst = this;
    }

    private void Start()
    {
        SpawnBlock();
        SpawnBlock();
    }

    public void MoveBlcok(Vector3 direction)
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            MoveCheck(blocks[i], direction);
        }
        SpawnBlock();
    }

    public void MoveCheck(Block block, Vector3 dir)
    {
        Vector3 checkPos = block.transform.position;
        for (int i = 0; i < TILE_SIZE; i++)
        {
            if (OutCheck(checkPos + dir)) break;
            checkPos += dir;

            Ray2D ray = new Ray2D(checkPos, Vector2.zero);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                if (Tile[(int)checkPos.x, (int)checkPos.y] == (int)TileType.fill)
                {
                    if (block.num == hit.collider.gameObject.GetComponent<Block>().num)
                    {
                        StartCoroutine(block.BlockMoving(checkPos, 0.5f, MergeToBlock));
                        return;
                    }
                    else
                    {
                        StartCoroutine(block.BlockMoving(checkPos - dir, 0.5f));
                        return;
                    }
                }
            }
            else
            {
                if (Tile[(int)checkPos.x, (int)checkPos.y] == (int)TileType.fill)
                {
                    StartCoroutine(block.BlockMoving(checkPos - dir, 0.5f));
                    return;
                }
            }
        }
        StartCoroutine(block.BlockMoving(checkPos, 0.5f));
    }

    void SpawnBlock()
    {
        Vector2 SpawnPos = new Vector2(Random.Range(0, TILE_SIZE), Random.Range(0, TILE_SIZE));

        if (Tile[(int)SpawnPos.x, (int)SpawnPos.y] == (int)TileType.fill)
        {
            SpawnBlock();
            return;
        }

        Tile[(int)SpawnPos.x, (int)SpawnPos.y] = (int)TileType.fill;
        Block block = Instantiate(blockPrefab, SpawnPos, Quaternion.identity);
        blocks.Add(block);
    }

    void SpawnBlock(int number, Color color, Vector2 SpawnPos)
    {
        Tile[(int)SpawnPos.x, (int)SpawnPos.y] = (int)TileType.fill;
        Block block = Instantiate(blockPrefab, SpawnPos, Quaternion.identity);

        block.num = number;
        block.blockColor = color;
        blocks.Add(block);
    }

    void MergeToBlock(Vector2 pos)
    {
        Ray2D ray = new Ray2D(pos, Vector2.zero);
        RaycastHit2D[] hit = Physics2D.RaycastAll(ray.origin, ray.direction);

        if (hit.Length <= 1)
        {
            return;
        }

        int number = hit[0].collider.gameObject.GetComponent<Block>().num * 2;
        SpawnBlock(number, blockColors[Division(number)], pos);
        for (int i = 0; i < hit.Length; i++)
        {
            blocks.Remove(hit[i].collider.GetComponent<Block>());
            Destroy(hit[i].collider.gameObject);
        }
    }

    int Division(int num)
    {
        int count = 0;
        while (num != 1)
        {
            num /= 2;
            count++;
        }

        return count;
    }

    bool OutCheck(Vector2 pos)
    {
        if (pos.x < 0 || pos.x > TILE_SIZE - 1 ||
            pos.y < 0 || pos.y > TILE_SIZE - 1)
        {
            return true;
        }
        return false;
    }
}
