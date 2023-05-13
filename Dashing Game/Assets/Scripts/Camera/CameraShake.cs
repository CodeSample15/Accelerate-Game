using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Tooltip("The maximum amount the camera can move in any direction")]
    [SerializeField] private float Intensity;

    [Tooltip("How many times the camera changes its location")]
    [SerializeField] private float Duration;

    [Tooltip("Seconds between each shake")]
    [SerializeField] private float Frequency;

    public void Shake()
    {
        StartCoroutine(shake()); //starting this processes as a coroutine so that other processes can run while the camera shakes
    }

    private IEnumerator shake()
    {
        Vector3 startPos = transform.position; //keeping track of the camera's original 2d position

        for (int i = 0; i < Duration; i++)
        {
            //creating a random location based off of the intensity and then moving the camera to that position
            float x = startPos.x + Random.Range(-Intensity, Intensity);
            float y = startPos.y + Random.Range(-Intensity, Intensity);

            transform.position = new Vector3(x, y, startPos.z);

            yield return new WaitForSeconds(Frequency);

            transform.position = startPos;
        }
    }
}
