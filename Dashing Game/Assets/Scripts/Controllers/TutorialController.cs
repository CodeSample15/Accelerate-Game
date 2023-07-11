using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialController : MonoBehaviour
{
    [Header("UI Items")]
    [SerializeField] private GameObject HealthBar;
    [SerializeField] private GameObject HealthBarIcon;

    [Space]

    [SerializeField] private GameObject DashBar;
    [SerializeField] private GameObject DashBarIcon;

    [Space]

    [SerializeField] private GameObject Minimap;
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
        HealthBarIcon.SetActive(false);
        DashBar.SetActive(false);
        DashBarIcon.SetActive(false);
        Minimap.SetActive(false);
        WaveProgressBar.SetActive(false);
        ScoreText.SetActive(false);

        ExplanationText.SetText("");
    }
}
