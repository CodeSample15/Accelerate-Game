using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPadLogic : MonoBehaviour
{
    [SerializeField] private float boostStrength;


    public float Boost
    {
        get { return boostStrength; }
    }
}
