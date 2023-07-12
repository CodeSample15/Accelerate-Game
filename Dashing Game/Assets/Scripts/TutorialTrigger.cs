using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private string TriggerName;
    [SerializeField] private bool DisableOnTrigger = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player") && TutorialController.staticRef != null)
        {
            TutorialController.staticRef.Triggered(TriggerName);

            if (DisableOnTrigger)
                gameObject.SetActive(false);
        }
    }
}
