using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningCreditsScript : MonoBehaviour
{
    [Header("Background particles")]
    public ParticleSystem BackgroundParticles;
    public List<Color[]> BackgroundColors;

    [Space]

    public float Duration;

    private float elapsed;

    void InitColors()
    {
        BackgroundColors = new List<Color[]>();

        Color[] one = new Color[2];
        one[0] = new Color(1, 0.5372f, 0);
        one[1] = new Color(0.498f, 0, 0);
        BackgroundColors.Add(one);

        Color[] two = new Color[2];
        two[0] = new Color(0, 0.9529f, 1);
        two[1] = new Color(0, 0.298f, 0.49f);
        BackgroundColors.Add(two);

        Color[] three = new Color[2];
        three[0] = new Color(0.76078f, 0, 1);
        three[1] = new Color(0, 0.337f, 0.55686f);
        BackgroundColors.Add(three);

        Color[] four = new Color[2];
        four[0] = new Color(0, 1, 0.17647f);
        four[1] = new Color(0, 0.3f, 0);
        BackgroundColors.Add(four);
    }

    void Awake()
    {
        InitColors();

        int randColor = Random.Range(0, BackgroundColors.Count);

        ParticleSystem.MainModule settings = BackgroundParticles.main;
        settings.startColor = new ParticleSystem.MinMaxGradient(BackgroundColors[randColor][0], BackgroundColors[randColor][1]);
    }

    void Start()
    {
        elapsed = 0f;
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        if (elapsed >= Duration)
            SceneManager.LoadScene(2); //load the main game
    }
}
