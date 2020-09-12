using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    //public
    public GameObject player;
    public float movementSpeed;

    //private
    private float yOffSet;

    // Start is called before the first frame update
    void Start()
    {
        movementSpeed = 7f;
        yOffSet = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        float xDistance = player.transform.position.x - transform.position.x;
        float yDistance = player.transform.position.y - (transform.position.y - yOffSet);
        Vector2 movement = new Vector2(xDistance, yDistance);

        transform.Translate(movement * movementSpeed * Time.deltaTime);
    }
}
