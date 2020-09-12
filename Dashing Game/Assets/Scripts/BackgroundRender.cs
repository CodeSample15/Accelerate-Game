using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundRender : MonoBehaviour
{
    [SerializeField] GameObject backgroundObject;
    [SerializeField] Vector2 StartPos;
    [SerializeField] Vector2 EndPos;
    [SerializeField] float BackgroundWidth;
    [SerializeField] float BackgroundHeight;

    private float xPos;
    private float yPos;

    // Start is called before the first frame update
    void Awake()
    {
        xPos = StartPos.x;
        yPos = StartPos.y;
        int maxX = (int)Mathf.Ceil(EndPos.x / BackgroundWidth);
        int maxY = (int)Mathf.Ceil(EndPos.y / BackgroundHeight);

        //render background
        for (int y = 0; y < maxY; y++)
        {
            for(int x=0; y < maxX; x++)
            {

            }
        }
    }
}
