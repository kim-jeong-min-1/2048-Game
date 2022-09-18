using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
    public int num;
    public Color blockColor;

    public TextMeshPro numText => transform.GetChild(0).GetComponent<TextMeshPro>();
    private SpriteRenderer sR => GetComponent<SpriteRenderer>();

    void Start()
    {
        SetBlock();
    }
    
    public void SetBlock()
    {
        sR.color = blockColor;
        numText.text = $"{this.num}";
        if(num >= 8)
        {
            numText.color = Color.white;
        }

        if (numText.text.Length == 3)
        {
            numText.fontSize = 3.5f;
        }
        else if (numText.text.Length == 4)
        {
            numText.fontSize = 2.5f;
        }
        StartCoroutine(BlockSizeUp(0.5f));
    }

    public IEnumerator BlockMoving(Vector3 pos, float time, System.Action<Vector2> merge = null)
    {
        GameSystem.Inst.Tile[(int)transform.position.x, (int)transform.position.y] = (int)TileType.empty;
        GameSystem.Inst.Tile[(int)pos.x, (int)pos.y] = (int)TileType.fill;

        while (transform.position != pos)
        {
            transform.position = Vector2.MoveTowards(transform.position, pos, time);
            yield return new WaitForFixedUpdate();
        }
        merge?.Invoke(pos);
    }

    IEnumerator BlockSizeUp(float time)
    {
        float percent = 0;
        float current = 0;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            transform.localScale = Vector2.Lerp(transform.localScale, Vector2.one * 0.88f, percent);

            yield return null;
        }
    }
}
