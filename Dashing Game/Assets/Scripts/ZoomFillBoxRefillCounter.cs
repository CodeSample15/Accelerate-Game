using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ZoomFillBoxRefillCounter : MonoBehaviour
{
    //[SerializeField] public ZoomFillBox fillbox;
    private ZoomFillBox fillbox;
    private TextMeshProUGUI text;

    void Start()
    {
        fillbox = GetComponentInParent<ZoomFillBox>();
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(fillbox.TimeSinceLastRefill == fillbox.refillTime || !ZoomFillBox.ActivatedBoxes[fillbox.ID])
        {
            text.SetText("");
        }
        else
        {
            int timeLeft = (int)fillbox.refillTime - fillbox.TimeSinceLastRefill;
            text.SetText(timeLeft.ToString());
        }
    }
}
