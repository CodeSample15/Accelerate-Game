using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Use this script for animating text fields that show numbers being increased
[RequireComponent(typeof(TextMeshProUGUI))]
public class ScoreAnimation : MonoBehaviour
{
    [SerializeField] private float animationSpeed;

    private TextMeshProUGUI display;
    private int value;

    void Awake()
    {
        display = GetComponent<TextMeshProUGUI>();
    }

    public void runAnimation(int startNumber, int endNumber)
    {
        //TODO: create method to support counting down from a start to end (right now it only works with counting up)
        if(startNumber <= endNumber)
            StartCoroutine(animate(startNumber, endNumber));
    }

    public void runAnimation(float delay, int startNumber, int endNumber)
    {
        //TODO: create method to support counting down from a start to end (right now it only works with counting up)
        if (startNumber <= endNumber)
            StartCoroutine(animate(delay, startNumber, endNumber));
    }

    IEnumerator animate(int start, int end)
    {
        value = start;

        while(value < end)
        {
            display.SetText("+" + value.ToString());
            value++;
            yield return new WaitForSeconds(animationSpeed);
        }

        display.SetText("+" + end.ToString());
    }

    IEnumerator animate(float delay, int start, int end)
    {
        display.SetText("+0");
        yield return new WaitForSeconds(delay);

        value = start;

        while (value < end)
        {
            display.SetText("+" + value.ToString());
            value++;
            yield return new WaitForSeconds(animationSpeed);
        }

        display.SetText("+" + end.ToString());
    }
}
