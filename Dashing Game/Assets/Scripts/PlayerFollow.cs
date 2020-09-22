using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    //public
    [SerializeField]public GameObject player;
    [SerializeField] public bool TrackX;
    [SerializeField] public bool TrackY;
    [SerializeField]public float movementSpeed;
    [SerializeField] public float yOffSet;

    // Update is called once per frame
    void Update()
    {
        float xDistance;
        float yDistance;

        if (TrackX)
            xDistance = player.transform.position.x - transform.position.x;
        else
            xDistance = 0;

        if (TrackY)
            yDistance = player.transform.position.y - (transform.position.y - yOffSet);
        else
            yDistance = 0;

        Vector2 movement = new Vector2(xDistance, yDistance);
        transform.Translate(movement * movementSpeed * Time.deltaTime);
    }
}
