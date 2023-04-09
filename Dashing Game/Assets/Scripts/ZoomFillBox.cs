using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ZoomFillBox : MonoBehaviour
{
    [SerializeField] public float refillTime;

    private ParticleSystem particles;
    private Light2D lights;

    private TextMeshProUGUI display;

    private bool refilled;
    private float timeSinceLastRefill;

    //to keep track of the boxes in the scene so that one can deactivate once the player uses it (to force the player to move through the level rather than camping)
    private static int zoomFillBoxCount;
    private static List<bool> activatedFillBoxes;
    private int boxID;

    private float tintAmount; //how much the box tints when it's inactive

    public int ID
    {
        get { return boxID; }
    }

    public static List<bool> ActivatedBoxes
    {
        get { return activatedFillBoxes; }
    }

    public int TimeSinceLastRefill
    {
        get { return Mathf.RoundToInt(timeSinceLastRefill); }
    }

    void Awake()
    {
        particles = GetComponentInChildren<ParticleSystem>();
        lights = GetComponentInChildren<Light2D>();

        display = GetComponentInChildren<Canvas>().gameObject.GetComponentInChildren<TextMeshProUGUI>();

        //start out with the boxes being filled
        refilled = true;
        timeSinceLastRefill = refillTime;

        zoomFillBoxCount = 0;
        activatedFillBoxes = new List<bool>();
    }

    void Start()
    {
        boxID = zoomFillBoxCount; //set the current id of the box to how many boxes there are already
        zoomFillBoxCount++;
        activatedFillBoxes.Add(true);

        tintAmount = 0.5f;
    }

    void Update()
    {
        if (!PauseButton.IsPaused && activatedFillBoxes[boxID])
        {
            //make sure the box is untinted
            Color temp = GetComponentInChildren<SpriteRenderer>().color;
            temp.a = 1f;
            GetComponentInChildren<SpriteRenderer>().color = temp;

            //refilling and playing particles when ready
            if (timeSinceLastRefill >= refillTime)
            {
                refilled = true;
                if(!particles.isPlaying)
                    particles.Play();
                lights.enabled = true;
            }
            else
            {
                timeSinceLastRefill += Time.deltaTime;
                particles.Stop();
                lights.enabled = false;
            }
        }
        else if(!activatedFillBoxes[boxID])
        {
            particles.Stop();

            //tint the box to make it seem inactive
            Color temp = GetComponentInChildren<SpriteRenderer>().color;
            temp.a = tintAmount;
            GetComponentInChildren<SpriteRenderer>().color = temp;
        }

        //update text display
        if (TimeSinceLastRefill == refillTime || !ActivatedBoxes[boxID])
        {
            display.SetText("");
        }
        else
        {
            int timeLeft = (int)refillTime - TimeSinceLastRefill;
            display.SetText(timeLeft.ToString());
        }
    }

    private void resetAllBoxes()
    {
        //reset all of the boxes to be active
        for(int i=0; i<activatedFillBoxes.Count; i++)
        {
            activatedFillBoxes[i] = true;
        }
    }
         
    private void OnTriggerEnter2D(Collider2D other)
    {
        //check if the box is active first
        if (activatedFillBoxes[boxID])
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (refilled && other.GetComponent<Player>().DashPower < other.GetComponent<Player>().MaxDash)
                {
                    timeSinceLastRefill = 0f;

                    other.GetComponent<Player>().DashPower = other.GetComponent<Player>().MaxDash;
                    refilled = false;

                    if (activatedFillBoxes.Count > 1)
                    {
                        resetAllBoxes(); //setting all previously turned off boxes to on
                        activatedFillBoxes[boxID] = false; //turn off the box
                    }
                }
            }
        }
    }
}