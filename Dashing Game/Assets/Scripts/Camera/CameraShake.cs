using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float Intensity;
    [SerializeField] private float Duration;
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
