using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneBuildScript : MonoBehaviour
{
    [SerializeField] private bool ForPhone;
    [SerializeField] private List<GameObject> ItemsToTurnOff;

    public bool isForPhone
    {
        get { return ForPhone; }
    }

    void Awake()
    {
        foreach(GameObject item in ItemsToTurnOff)
        {
            item.SetActive(ForPhone);
        }
    }
}
