using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDrag : MonoBehaviour
{
    GameSystem gameSystem => GetComponent<GameSystem>();
    private readonly float dragDistance = 0.8f;

    private Vector2[] dragPoint = new Vector2[2];
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragPoint[0] = Camera.main.ScreenToWorldPoint(Input.mousePosition);          
        }

        if (Input.GetMouseButtonUp(0))
        {
            dragPoint[1] = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float dragX = dragPoint[1].x - dragPoint[0].x;
            float dragY = dragPoint[1].y - dragPoint[0].y;

            if (Mathf.Abs(dragX) < dragDistance && Mathf.Abs(dragY) < dragDistance) return;
            if (Mathf.Abs(dragX) == Mathf.Abs(dragY)) return;

            gameSystem.MoveBlcokToDirection(dragDirection(dragX, dragY));
        }
    }

    Vector2 dragDirection(float x, float y)
    {
        if(Mathf.Abs(x) > Mathf.Abs(y))
        {
            if(x > 0)
            {
                return Vector2.right;
            }
            else if(x < 0)
            {
                return Vector2.left;
            }
        }
        else if(Mathf.Abs(y) > Mathf.Abs(x))
        {
            if (y > 0)
            {
                return Vector2.up;
            }
            else if (y < 0)
            {
                return Vector2.down;
            }
        }
        return Vector2.zero;
    }

}
