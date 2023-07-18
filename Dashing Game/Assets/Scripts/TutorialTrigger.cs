using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private string TriggerName;
    [SerializeField] private bool DisableOnTrigger = true;

    [Space]

    [Tooltip("If any walls need to be set to active when triggered, add them to this list")]
    [SerializeField] private GameObject[] walls;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player") && TutorialController.staticRef != null)
        {
            TutorialController.staticRef.Triggered(TriggerName);
            TutorialController.staticRef.checkpoint = transform.position;

            foreach (GameObject wall in walls)
                wall.SetActive(true);

            if (DisableOnTrigger)
                gameObject.SetActive(false);
        }
    }
}
