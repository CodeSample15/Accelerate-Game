using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhoneBuildScript : MonoBehaviour
{
    [SerializeField] private bool ForPhone;
    [SerializeField] private List<GameObject> ItemsToTurnOff;
    [SerializeField] private List<GameObject> ItemsToTurnOn;
    [SerializeField] private Button UIJumpButton;

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

        foreach(GameObject item in ItemsToTurnOn)
        {
            item.SetActive(!ForPhone);
        }
    }

    void Start()
    {
        //assign jump button for touch screen
        if (ForPhone && Player.staticReference != null)
            UIJumpButton.onClick.AddListener(Player.staticReference.jump);
    }
}
