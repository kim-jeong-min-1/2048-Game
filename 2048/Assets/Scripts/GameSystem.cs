using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    private const int TILE_SIZE = 4;
    public static GameSystem Inst { get; set; }

    //public List<Block> blocks = new List<Block>();
    public Block[,] blocks = new Block[TILE_SIZE, TILE_SIZE];
    public Color[] blockColors;

    [SerializeField] Block blockPrefab;

    private void Awake()
    {
        Inst = this;
    }

    private void Start()
    {
        GameManager.Inst.TurnOver += SpawnBlock;
        GameManager.Inst.TurnOver.Invoke();
        GameManager.Inst.TurnOver.Invoke();
    }

    public void MoveBlcokToDirection(Vector3 direction)
    {
        if (direction == Vector3.right || direction == Vector3.down)
        {
            for (int y = 0; y < TILE_SIZE; y++)
            {
                for (int x = 3; x >= 0; x--)
                {
                    MoveCheck(blocks[x, y], direction);
                }
            }
        }
        else if (direction == Vector3.left || direction == Vector3.up)
        {
            for (int y = 3; y >= 0; y--)
            {
                for (int x = 0; x < TILE_SIZE; x++)
                {
                    MoveCheck(blocks[x, y], direction);
                }
            }
        }
        MoveBlock(direction);
    }

    public void MoveCheck(Block block, Vector3 dir)
    {
        if (block == null) return;

        Vector3 checkPos = block.transform.position;
        for (int i = 0; i < TILE_SIZE; i++)
        {
            if (OutCheck(checkPos + dir)) break;
            checkPos += dir;

            Ray2D ray = new Ray2D(checkPos, Vector2.zero);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                Block adjBlock = hit.collider.gameObject.GetComponent<Block>();
                if (block.num == adjBlock.num)
                {
                    if(adjBlock.isMerge == false)
                    {
                        block.isMerge = true;
                        block.movePos = adjBlock.movePos;
                    }
                    else
                    {
                        block.movePos = adjBlock.movePos - dir;
                    }                  
                }
                else
                {
                    block.movePos = adjBlock.movePos - dir;
                }
                return;
            }
        }
        block.movePos = checkPos;
    }

    void MoveBlock(Vector3 direction)
    {
        if (direction == Vector3.right || direction == Vector3.down)
        {
            for (int y = 0; y < TILE_SIZE; y++)
            {
                for (int x = 3; x >= 0; x--)
                {
                    if (blocks[x, y] == null) continue;

                    if (blocks[x, y].isMerge == true) StartCoroutine(blocks[x, y].BlockMoving(blocks[x, y].movePos, 0.5f, MergeToBlock));
                    else StartCoroutine(blocks[x, y].BlockMoving(blocks[x, y].movePos, 0.5f));
                }
            }
        }
        else if (direction == Vector3.left || direction == Vector3.up)
        {
            for (int y = 3; y >= 0; y--)
            {
                for (int x = 0; x < TILE_SIZE; x++)
                {
                    if (blocks[x, y] == null) continue;

                    if (blocks[x, y].isMerge == true) StartCoroutine(blocks[x, y].BlockMoving(blocks[x, y].movePos, 0.5f, MergeToBlock));
                    else StartCoroutine(blocks[x, y].BlockMoving(blocks[x, y].movePos, 0.5f));
                }
            }
        }
        GameManager.Inst.curGameState = GameState.Wait;
        GameManager.Inst.Processing();
    }

    public void SpawnBlock()
    {
        Vector2 SpawnPos = new Vector2(Random.Range(0, TILE_SIZE), Random.Range(0, TILE_SIZE));

        if (blocks[(int)SpawnPos.x, (int)SpawnPos.y] != null)
        {
            SpawnBlock();
            return;
        }

        Block block = Instantiate(blockPrefab, SpawnPos, Quaternion.identity);
        blocks[(int)SpawnPos.x, (int)SpawnPos.y] = block;
        
    }

    void SpawnBlock(int number, Color color, Vector2 SpawnPos)
    {
        Block block = Instantiate(blockPrefab, SpawnPos, Quaternion.identity);

        block.num = number;
        block.blockColor = color;
        blocks[(int)SpawnPos.x, (int)SpawnPos.y] = block;
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
        for (int i = 0; i < hit.Length; i++)
        {
            blocks[(int)hit[i].collider.gameObject.transform.position.x, (int)hit[i].collider.gameObject.transform.position.y] = null;
            Destroy(hit[i].collider.gameObject);
        }
        SpawnBlock(number, blockColors[Division(number)], pos);
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
