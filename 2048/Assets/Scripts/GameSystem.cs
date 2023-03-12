using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    private const int TILE_SIZE = 4;
    private const int FINALLY_BLOCK_NUM = 2048;
    public static GameSystem Inst { get; set; }

    //public List<Block> blocks = new List<Block>();
    public Block[,] blocks = new Block[TILE_SIZE, TILE_SIZE];
    public Color[] blockColors;
    Vector3[] dir = { Vector3.up, Vector3.down, Vector3.right, Vector3.left };

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

    public void MoveBlcokToDirection(Vector3 direction)
    {
        if (direction == dir[2] || direction == dir[1])
        {
            for (int y = 0; y < TILE_SIZE; y++)
            {
                for (int x = 3; x >= 0; x--)
                {
                    MoveCheck(blocks[x, y], direction);
                }
            }
        }
        else if (direction == dir[3] || direction == dir[0])
        {
            for (int y = 3; y >= 0; y--)
            {
                for (int x = 0; x < TILE_SIZE; x++)
                {
                    MoveCheck(blocks[x, y], direction);
                }
            }
        }

        if (NotMoveCheck(direction))
        {
            GameManager.Inst.curGameState = GameState.Wait;
        }
        else
        {
            MoveBlock(direction);
        }
        Invoke("AfterProcess", 0.1f);
    }

    public void MoveCheck(Block block, Vector3 dir)
    {
        if (block == null) return;

        Vector3 checkPos = block.transform.position;
        for (int i = 0; i < TILE_SIZE; i++)
        {
            if (OutCheck(checkPos + dir)) break;
            checkPos += dir;

            var checkBlock = blocks[(int)checkPos.x, (int)checkPos.y];
            if (checkBlock != null)
            {
                if (block.num == checkBlock.num)
                {
                    if (checkBlock.isMerge == false)
                    {
                        block.isMerge = true;
                        block.movePos = checkBlock.movePos;
                        block.mergeBlock = checkBlock;
                    }
                    else
                    {
                        block.movePos = checkBlock.movePos - dir;
                    }
                    return;
                }
                block.movePos = checkBlock.movePos - dir;
                return;
            }
        }
        block.movePos = checkPos;
    }

    void MoveBlock(Vector3 direction)
    {
        if (direction == dir[2] || direction == dir[1])
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
        else if (direction == dir[3] || direction == dir[0])
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
    }

    void AfterProcess()
    {
        if (ClearCheck()) GameManager.Inst.curGameState = GameState.Clear;
        if (FailCheck() == 0) GameManager.Inst.curGameState = GameState.GameOver;
        GameManager.Inst.Processing();
    }

    bool NotMoveCheck(Vector3 dir)
    {
        for (int x = 0; x < TILE_SIZE; x++)
        {
            for (int y = 0; y < TILE_SIZE; y++)
            {
                if (blocks[x, y] == null) continue;

                var checkPos = blocks[x, y].transform.position + dir;
                Ray2D ray = new Ray2D(checkPos, Vector2.zero);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                if (hit.collider != null)
                {
                    if (blocks[x, y].num == hit.collider.GetComponent<Block>().num)
                    {
                        return false;
                    }
                }
                else
                {
                    if (!OutCheck(checkPos)) return false;
                }
            }
        }
        return true;
    }

    int FailCheck()
    {
        int check = 0;
        for (int i = 0; i < dir.Length; i++)
        {
            if (!NotMoveCheck(dir[i]))
            {
                check++;
            }
        }
        return check;
    }

    bool ClearCheck()
    {
        for (int x = 0; x < TILE_SIZE; x++)
        {
            for (int y = 0; y < TILE_SIZE; y++)
            {
                if (blocks[x, y] == null) continue;

                if (blocks[x, y].num == FINALLY_BLOCK_NUM)
                {
                    return true;
                }
            }
        }
        return false;
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
        var block = blocks[(int)pos.x, (int)pos.y];
        if (block.mergeBlock == null)  return;

        var mergeBlock = block.mergeBlock;
        blocks[(int)block.transform.position.x, (int)block.transform.position.y] = null;
        blocks[(int)mergeBlock.transform.position.x, (int)mergeBlock.transform.position.y] = null;

        var number = block.num * 2;

        Destroy(mergeBlock.gameObject);
        Destroy(block.gameObject);

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
