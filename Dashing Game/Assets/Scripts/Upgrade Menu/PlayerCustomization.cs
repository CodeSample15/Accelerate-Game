using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCustomization : MonoBehaviour
{
    [Header("Preview Animation")]
    [SerializeField] private float TurnSpeedNormal;
    [SerializeField] private float TurnSpeedMax;
    [SerializeField] private float TurnSpeedDecay;

    //for the animation
    private GameObject playerPrev;

    private float curSpeed;
    private float targetSpeed;

    private float curVel;
    private float targetVel;

    void Awake()
    {
        playerPrev = transform.Find("").gameObject;

        curSpeed = 0;
        targetSpeed = TurnSpeedNormal;

        curVel = 0;
        targetVel = 0;
    }

    void Update()
    {
        //updating the player preview with a spin animation
        curSpeed = Mathf.SmoothDamp(curSpeed, targetSpeed, ref curVel, 0.1f);
        targetSpeed = Mathf.SmoothDamp(targetSpeed, TurnSpeedNormal, ref targetVel, TurnSpeedDecay);

        transform.Rotate(new Vector3(0, 0, curSpeed));
    }

    public void Spin()
    {
        curSpeed = TurnSpeedMax;
    }
}
