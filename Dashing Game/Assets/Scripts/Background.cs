using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public GameObject player;
    public float x = 0;
    public float y = 0;
    public float xOffSet;
    public int layer = 0;

    private float parallax;

    void Start()
    {
        parallax = 5;
        xOffSet = 10;
    }

    // Update is called once per frame
    void Update()
    {
        float offSet;
        if (layer % 2 == 0)
            offSet = xOffSet;
        else
            offSet = 0;

        float xPos = player.transform.position.x / (layer * parallax);
        xPos += offSet;

        transform.position = new Vector3(xPos, y, (layer * 10) + 20);
    }
}
