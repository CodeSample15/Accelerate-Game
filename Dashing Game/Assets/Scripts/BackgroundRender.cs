using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundRender : MonoBehaviour
{
    [SerializeField] GameObject backgroundObject;
    [SerializeField] Vector2 StartPos;
    [SerializeField] float BackgroundWidth;
    [SerializeField] float BackgroundHeight;
    [SerializeField] int Size;

    private float xPos;
    private float yPos;

    // Start is called before the first frame update
    void Awake()
    {
        xPos = StartPos.x;
        yPos = StartPos.y;

        //render background
        for (int y = 0; y < Size; y++)
        {
            GameObject temp = Instantiate(backgroundObject, new Vector3(0, 0, 0), Quaternion.identity);
            temp.GetComponent<Background>().x = 0;
            temp.GetComponent<Background>().y = yPos;
            temp.GetComponent<Background>().layer = y;

            float color = y * 0.1f;
            temp.GetComponent<Renderer>().material.SetColor("_BaseColor", new Color(color, color, color));
            yPos += BackgroundHeight;
        }

        backgroundObject.SetActive(false);
    }
}
