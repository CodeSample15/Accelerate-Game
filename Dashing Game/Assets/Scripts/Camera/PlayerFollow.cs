using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    //public
    [SerializeField] public GameObject player;
    [SerializeField] public bool TrackX;
    [SerializeField] public bool TrackY;
    [SerializeField] public float startSize;
    [SerializeField] public float zoomAmount;
    [SerializeField] public float movementSpeed;
    [SerializeField] public float zoomSpeed;
    [SerializeField] public float yOffSet;

    public float DashingZoom;

    //private
    private Player playerScript;
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = startSize;

        playerScript = player.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        float xDistance;
        float yDistance;
        float zDistance;

        if (TrackX)
            xDistance = player.transform.position.x - transform.position.x;
        else
            xDistance = 0;

        if (TrackY)
            yDistance = player.transform.position.y - (transform.position.y - yOffSet);
        else
            yDistance = 0;

        if (playerScript.isDashing)
            zDistance = zoomAmount - cam.orthographicSize;
        else
            zDistance = startSize - cam.orthographicSize;

        Vector2 movement = new Vector2(xDistance, yDistance);
        transform.Translate(movement * movementSpeed * Time.deltaTime);

        cam.orthographicSize += zDistance * zoomSpeed;
    }
}
