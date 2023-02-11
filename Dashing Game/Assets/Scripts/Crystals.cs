using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is in charge of the animations and behaviours of all crystals. This script will be used in a prefab to make the creation of crystals easy
public class Crystals : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private float colliderSize;
    [SerializeField] private float bobbleAmount;
    [SerializeField] private float bobbleSpeed;
    [SerializeField] private float particleEmissionStrength;
    [SerializeField] private float lightningEmissionStrength;
    [SerializeField] private float scale;

    public enum Type { blue, green, orange, pink, red };
    public Type crystalType;

    private ParticleSystem lightning_effect; //plays in bursts
    private ParticleSystem lightning_constant_effect; //plays constantly when crystal is activated

    private Vector3 spin;
    private Vector3 targetSpin;
    private Vector3 spinVelocity; //for the smoothdamp to work with
    private Vector3 targetSpinVelocity; //for slowing down the target spin

    private float speedIncrease;

    private float lightningAmount;
    private float lastHitTime;

    void Awake()
    {
        lightning_effect = transform.GetChild(1).GetComponent<ParticleSystem>();
        lightning_constant_effect = transform.GetChild(2).GetComponent<ParticleSystem>();

        var emission = lightning_constant_effect.emission;
        emission.rateOverTime = 0;
        lightningAmount = 0;

        lastHitTime = Time.time;

        ParticleSystemRenderer sparkle = transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
        ParticleSystemRenderer lightning = lightning_effect.gameObject.GetComponent<ParticleSystemRenderer>();
        ParticleSystemRenderer lightning_constant = lightning_constant_effect.gameObject.GetComponent<ParticleSystemRenderer>();

        transform.position = new Vector3(transform.position.x, transform.position.y, 5);
        transform.localScale = new Vector3(scale, scale, scale);
        sparkle.gameObject.transform.localScale = transform.localScale;
        lightning.gameObject.transform.localScale = transform.localScale;
        lightning_constant.gameObject.transform.localScale = transform.localScale;

        speedIncrease = 5f;

        switch (crystalType)
        {
            case Type.blue:
                GetComponent<SpriteRenderer>().sprite = sprites[0];
                sparkle.material.SetVector("_EmissionColor", new Vector3(28, 108, 191) * particleEmissionStrength);
                lightning.trailMaterial.SetVector("_Color", new Vector4(28, 108, 191, 1) * lightningEmissionStrength);
                lightning_constant.trailMaterial.SetVector("_Color", new Vector4(28, 108, 191, 1) * lightningEmissionStrength);
                break;

            case Type.green:
                GetComponent<SpriteRenderer>().sprite = sprites[1];
                sparkle.material.SetVector("_EmissionColor", new Vector3(0, 191, 4) * particleEmissionStrength);
                lightning.trailMaterial.SetVector("_Color", new Vector4(0, 191, 4, 1) * lightningEmissionStrength);
                lightning_constant.trailMaterial.SetVector("_Color", new Vector4(0, 191, 4, 1) * lightningEmissionStrength);
                break;

            case Type.orange:
                GetComponent<SpriteRenderer>().sprite = sprites[2];
                sparkle.material.SetVector("_EmissionColor", new Vector3(210, 54, 0) * particleEmissionStrength);
                lightning.trailMaterial.SetVector("_Color", new Vector4(210, 54, 0, 1) * lightningEmissionStrength);
                lightning_constant.trailMaterial.SetVector("_Color", new Vector4(210, 54, 0, 1) * lightningEmissionStrength);
                break;

            case Type.pink:
                GetComponent<SpriteRenderer>().sprite = sprites[3];
                sparkle.material.SetVector("_EmissionColor", new Vector3(191, 34, 125) * particleEmissionStrength);
                lightning.trailMaterial.SetVector("_Color", new Vector4(191, 34, 125, 1) * lightningEmissionStrength);
                lightning_constant.trailMaterial.SetVector("_Color", new Vector4(191, 34, 125, 1) * lightningEmissionStrength);
                break;

            case Type.red:
                GetComponent<SpriteRenderer>().sprite = sprites[4];
                sparkle.material.SetVector("_EmissionColor", new Vector3(255, 0, 0) * particleEmissionStrength);
                lightning.trailMaterial.SetVector("_Color", new Vector4(255, 0, 0, 1) * lightningEmissionStrength);
                lightning_constant.trailMaterial.SetVector("_Color", new Vector4(255, 0, 0, 1) * lightningEmissionStrength);
                break;
        }

        //set the size of the crystal's box collider to the size of the crystal itself
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.size = GetComponent<SpriteRenderer>().sprite.bounds.size * colliderSize;
    }

    void Update()
    {
        transform.Translate(Vector2.up * Mathf.Sin(Time.time * bobbleSpeed) * bobbleAmount * Time.deltaTime);

        spin = Vector3.SmoothDamp(spin, targetSpin, ref spinVelocity, 0.5f); //update the current speed
        transform.Rotate(spin);

        targetSpin = Vector3.SmoothDamp(targetSpin, Vector3.zero, ref targetSpinVelocity, 0.6f); //update the target speed to slowly reset it to zero

        if(Mathf.Abs(targetSpin.z) < 1.5f)
        {
            float difference = transform.rotation.z;
            targetSpin = new Vector3(0, 0, difference);
        }

        if (lastHitTime > 0 && Time.time - lastHitTime > 2)
        {
            lightningAmount--;
            lastHitTime = Time.time;
        }

        var emission = lightning_constant_effect.emission;
        emission.rateOverTime = lightningAmount;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<Player>().isDashing)
            {
                lightning_effect.Play();

                targetSpin = new Vector3(0, 0, spin.z + speedIncrease); //increase with respect to the current speed, not the target speed
                lightningAmount++;

                lastHitTime = Time.time;
            }
        }
    }
}
