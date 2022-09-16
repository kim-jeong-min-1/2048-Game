using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public delegate void Block_Delegate();

public class Block : MonoBehaviour
{
    [HideInInspector]
    public TextMeshPro numText;

    public int num;
    Block_Delegate block_Delegate;

    void Start()
    {
        numText = transform.GetChild(0).GetComponent<TextMeshPro>();
        block_Delegate += SetNumber;
        block_Delegate += SetColor;
    }

    public void SetNumber()
    {
        numText.text = $"{num}";

        if(numText.text.Length == 3)
        {
            numText.fontSize = 3.5f;
        }
        else if(numText.text.Length == 4)
        {
            numText.fontSize = 2.5f;
        }
    }

    public void SetColor()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
