using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    private void Awake()
    {
        Camera camera = GetComponent<Camera>();
        Rect rect = camera.rect;

        //(가로 / 세로) 카메라 비율 구하는 공식 
        //같은 비율의 해상도라면 해당 공식을 적용했을 때 같은 값이 나오게 됨

        //만약 스크린사이즈가 16:9의 비율이라면 cameraHeight에는 1값이 들어가게 됨
        float cameraHeight = ((float)Screen.width / Screen.height) / ((float)9 / 16);
        float cameraWidth = 1f / cameraHeight;

        //값이 1보다 작다면 세로가 짤린 것이고
        //1보다 크다면 가로가 짤린 것이다.
        if (cameraHeight < 1)
        {
            rect.height = cameraHeight;
            rect.y = (1f - cameraHeight) / 2f;
        }
        else
        {
            rect.width = cameraWidth;
            rect.x = (1f - cameraWidth) / 2f;
        }
        camera.rect = rect;
    }
}
