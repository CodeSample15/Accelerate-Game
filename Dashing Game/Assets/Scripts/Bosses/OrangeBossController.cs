using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeBossController : MonoBehaviour
{
    [Header("Cosmetic")]
    [SerializeField] private float minBlinkTime;
    [SerializeField] private float maxBlinkTime;

    private bool isAttacking;

    //animation related things
    private Animator anims;
    private float timeSinceLastBlink;
    private float nextBlinkTime;

    void Awake()
    {
        anims = GetComponent<Animator>();

        isAttacking = false;

        timeSinceLastBlink = 0;
        nextBlinkTime = Random.Range(minBlinkTime, maxBlinkTime);
    }

    void Update()
    {
        timeSinceLastBlink += Time.deltaTime;

        if (timeSinceLastBlink >= nextBlinkTime && !isAttacking)
        {
            anims.SetTrigger("Blink");
            timeSinceLastBlink = 0;
            nextBlinkTime = Random.Range(minBlinkTime, maxBlinkTime);
        }
    }
}
