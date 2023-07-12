using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialController : MonoBehaviour
{
    public static TutorialController staticRef;

    [Header("UI Items")]
    [SerializeField] private GameObject HealthBar;

    [Space]

    [SerializeField] private GameObject DashBar;

    [Space]

    [SerializeField] private GameObject Minimap;
    [SerializeField] private GameObject MinimapBorder;

    [SerializeField] private GameObject WaveProgressBar;
    [SerializeField] private GameObject ScoreText;

    [Space]

    [Header("Explanations")]
    [SerializeField] private TextMeshProUGUI ExplanationText;

    [Space]

    [Header("Enemy")]
    [SerializeField] private GameObject Enemy;

    void Awake()
    {
        HealthBar.SetActive(false);
        DashBar.SetActive(false);
        Minimap.SetActive(false);
        MinimapBorder.SetActive(false);
        WaveProgressBar.SetActive(false);
        ScoreText.SetActive(false);

        ExplanationText.SetText("");

        staticRef = this;
    }

    public void Triggered(string name)
    {

    }
}
