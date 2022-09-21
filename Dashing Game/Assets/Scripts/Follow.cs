using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] private GameObject target;

    void FixedUpdate()
    {
        transform.position = target.transform.position;
    }
}
