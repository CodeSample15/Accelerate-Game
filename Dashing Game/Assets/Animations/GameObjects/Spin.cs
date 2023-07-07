using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] private float speed;

    void Update()
    {
        if(!PauseButton.IsPaused)
            transform.Rotate(new Vector3(0,0,1) * speed * Time.deltaTime);
    }
}
