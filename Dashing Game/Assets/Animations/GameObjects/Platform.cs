using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private bool MoveX;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    [Space]

    [SerializeField] private bool MoveY;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    [Space]

    [SerializeField] private float SpeedX;
    [SerializeField] private float SpeedY;

    [Space]

    [Tooltip("When true platform will start moving to the right when the game starts")]
    [SerializeField] private bool StartWithPositiveX;
    [Tooltip("When true platform will start moving up when the game starts")]
    [SerializeField] private bool StartWithPositiveY;

    private bool movingUpX;
    private bool movingUpY;

    void Start()
    {
        Vector2 startPos = transform.localPosition;
        if (MoveX) {
            if (startPos.x > maxX)
                startPos.x = maxX;
            if (startPos.x < minX)
                startPos.x = minX;
        }

        if(MoveY)
        {
            if (startPos.y > maxY)
                startPos.y = maxY;
            if (startPos.y < minY)
                startPos.y = minY;
        }

        transform.localPosition = startPos;
        movingUpX = StartWithPositiveX;
    }

    void Update()
    {
        if (!PauseButton.IsPaused)
        {
            if (MoveX)
            {
                if (movingUpX)
                    transform.Translate(SpeedX * transform.right * Time.deltaTime);
                else
                    transform.Translate(SpeedX * -transform.right * Time.deltaTime);

                if (transform.localPosition.x > maxX || transform.localPosition.x < minX)
                    movingUpX = !movingUpX;
            }

            if (MoveY)
            {
                if (movingUpY)
                    transform.Translate(SpeedY * transform.up * Time.deltaTime);
                else
                    transform.Translate(SpeedY * -transform.up * Time.deltaTime);
                
                if (transform.localPosition.y > maxY || transform.localPosition.y < minY)
                    movingUpY = !movingUpY;
            }
        }
    }
}
