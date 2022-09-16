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
    public List<Block> blocks = new List<Block>();
    public int[,] Tile = new int[4,4];

    [SerializeField] Block blockPrefab;

    private void Start()
    {
        Invoke("SpawnBlock", 2f);
        Invoke("SpawnBlock", 2f);
    }

    void SpawnBlock()
    {
        Vector2 SpawnPos = new Vector2(Random.Range(0, 4), Random.Range(0, 4));

        if(Tile[(int)SpawnPos.x, (int)SpawnPos.y] == (int)TileType.fill)
        {
            SpawnBlock();
            return;
        }

        Tile[(int)SpawnPos.x, (int)SpawnPos.y] = (int)TileType.fill;
        Block block = Instantiate(blockPrefab, SpawnPos, Quaternion.identity);
        blocks.Add(block);

        StartCoroutine(BlockSizeUp(block.gameObject, 0.5f));
    }

    IEnumerator BlockSizeUp(GameObject block, float time)
    {
        float percent = 0;
        float current = 0;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            block.transform.localScale = Vector2.Lerp(block.transform.localScale, Vector2.one * 0.88f, percent);

            yield return null;
        }
    }
}
