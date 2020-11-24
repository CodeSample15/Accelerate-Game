using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI display;

    private float secondCounter;
    private int frames;
    private int lastUpdate;

    // Start is called before the first frame update
    void Start()
    {
        secondCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(secondCounter >= 1)
        {
            lastUpdate = frames;
            frames = 0;
            secondCounter = 0;
        }

        frames++;
        secondCounter += Time.deltaTime;

        display.SetText("FPS: " + lastUpdate.ToString());
    }
}
