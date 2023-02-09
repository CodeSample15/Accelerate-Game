using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is in charge of the animations and behaviours of all crystals. This script will be used in a prefab to make the creation of crystals easy
public class Crystals : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private float scale;

    private ParticleSystem sparkle;

    public enum Type { blue, green, orange, pink, red };
    public Type crystalType;

    void Awake()
    {
        sparkle = GetComponentInChildren<ParticleSystem>();
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        transform.localScale = new Vector3(scale, scale, scale);

        switch(crystalType)
        {
            case Type.blue:
                GetComponent<SpriteRenderer>().sprite = sprites[0];
                sparkle.
                break;

            case Type.green:
                GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;

            case Type.orange:
                GetComponent<SpriteRenderer>().sprite = sprites[2];
                break;

            case Type.pink:
                GetComponent<SpriteRenderer>().sprite = sprites[3];
                break;

            case Type.red:
                GetComponent<SpriteRenderer>().sprite = sprites[4];
                break;
        }
    }
}
