using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//This script is in charge of the animations and behaviours of all crystals. This script will be used in a prefab to make the creation of crystals easy
public class Crystals : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private Material lightning_mat;
    [SerializeField] private int maxHits;
    [SerializeField] private float playerBounceBack;
    [SerializeField] private float colliderSize;
    [SerializeField] private float bobbleAmount;
    [SerializeField] private float bobbleSpeed;
    [SerializeField] private float particleEmissionStrength;
    [SerializeField] private float lightningEmissionStrength;
    [SerializeField] private float scale;

    public enum Type { blue, green, orange, pink, red, tutorial };
    public Type crystalType;

    private ParticleSystem lightning_effect; //plays in bursts
    private ParticleSystem lightning_constant_effect; //plays constantly when crystal is activated

    private Animator whiteScreenFade;

    private Vector3 spin;
    private Vector3 targetSpin;
    private Vector3 spinVelocity; //for the smoothdamp to work with
    private Vector3 targetSpinVelocity; //for slowing down the target spin

    private Vector3 startLocation;

    private bool locked;

    private float speedIncrease;

    private float lightningAmount;
    private float lastHitTime;

    private float elapsedTime; //for the bobble

    private float hitCooldown; //minimum amount of time before crystal can be hit again

    private bool transitionStarted;

    void Awake()
    {
        assignSprite();
        lightning_effect = transform.GetChild(1).GetComponent<ParticleSystem>();
        lightning_constant_effect = transform.GetChild(2).GetComponent<ParticleSystem>();

        //check to see if the crystal is unlocked
        if (!isUnlocked())
        {
            locked = true;
            Color spriteCol = GetComponent<SpriteRenderer>().color;
            spriteCol.a = 0.5f;
            GetComponent<SpriteRenderer>().color = spriteCol;

            //remove all particle systems
            Destroy(lightning_effect.gameObject);
            Destroy(lightning_constant_effect.gameObject);
            Destroy(transform.GetChild(0).gameObject);
        }
        else
        {
            locked = false;

            whiteScreenFade = FindObjectOfType<Player>().white_fade;

            var emission = lightning_constant_effect.emission;
            emission.rateOverTime = 0;
            lightningAmount = 0;

            elapsedTime = 0;

            hitCooldown = 0.001f;

            lastHitTime = Time.time;

            transitionStarted = false;

            ParticleSystemRenderer sparkle = transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
            ParticleSystemRenderer lightning = lightning_effect.gameObject.GetComponent<ParticleSystemRenderer>();
            ParticleSystemRenderer lightning_constant = lightning_constant_effect.gameObject.GetComponent<ParticleSystemRenderer>();

            //create new instances of the lightning shader for the two effects
            lightning.trailMaterial = Instantiate(lightning_mat);
            lightning_constant.trailMaterial = Instantiate(lightning_mat);

            transform.position = new Vector3(transform.position.x, transform.position.y, 5);
            transform.localScale = new Vector3(scale, scale, scale);
            startLocation = transform.position;

            sparkle.gameObject.transform.localScale = transform.localScale;
            lightning.gameObject.transform.localScale = transform.localScale;
            lightning_constant.gameObject.transform.localScale = transform.localScale;

            speedIncrease = 10f;

            switch (crystalType)
            {
                case Type.blue:
                    sparkle.material.SetVector("_EmissionColor", new Vector3(28, 108, 191) * particleEmissionStrength);
                    lightning.trailMaterial.SetVector("_Color", new Vector4(28, 108, 191, 1) * lightningEmissionStrength);
                    lightning_constant.trailMaterial.SetVector("_Color", new Vector4(28, 108, 191, 1) * lightningEmissionStrength);
                    break;

                case Type.green:
                    sparkle.material.SetVector("_EmissionColor", new Vector3(0, 191, 4) * particleEmissionStrength);
                    lightning.trailMaterial.SetVector("_Color", new Vector4(0, 191, 4, 1) * lightningEmissionStrength);
                    lightning_constant.trailMaterial.SetVector("_Color", new Vector4(0, 191, 4, 1) * lightningEmissionStrength);
                    break;

                case Type.orange:
                    sparkle.material.SetVector("_EmissionColor", new Vector3(210, 54, 0) * particleEmissionStrength);
                    lightning.trailMaterial.SetVector("_Color", new Vector4(210, 54, 0, 1) * lightningEmissionStrength);
                    lightning_constant.trailMaterial.SetVector("_Color", new Vector4(210, 54, 0, 1) * lightningEmissionStrength);
                    break;

                case Type.pink:
                    sparkle.material.SetVector("_EmissionColor", new Vector3(191, 34, 125) * particleEmissionStrength);
                    lightning.trailMaterial.SetVector("_Color", new Vector4(191, 34, 125, 1) * lightningEmissionStrength);
                    lightning_constant.trailMaterial.SetVector("_Color", new Vector4(191, 34, 125, 1) * lightningEmissionStrength);
                    break;

                case Type.red:
                    sparkle.material.SetVector("_EmissionColor", new Vector3(255, 0, 0) * particleEmissionStrength);
                    lightning.trailMaterial.SetVector("_Color", new Vector4(255, 0, 0, 1) * lightningEmissionStrength);
                    lightning_constant.trailMaterial.SetVector("_Color", new Vector4(255, 0, 0, 1) * lightningEmissionStrength);
                    break;

                case Type.tutorial:
                    sparkle.material.SetVector("_EmissionColor", new Vector3(50, 50, 50) * particleEmissionStrength);
                    lightning.trailMaterial.SetVector("_Color", new Vector4(50, 50, 50, 1) * lightningEmissionStrength);
                    lightning_constant.trailMaterial.SetVector("_Color", new Vector4(50, 50, 50, 1) * lightningEmissionStrength);
                    break;
            }

            //set the size of the crystal's box collider to the size of the crystal itself
            BoxCollider2D col = GetComponent<BoxCollider2D>();
            col.size = GetComponent<SpriteRenderer>().sprite.bounds.size * colliderSize;
        }
    }

    void FixedUpdate()
    {
        if(!locked && lightningAmount >= maxHits)
        {
            //play lightning particles when transitioning to a boss level (makes the amount of lightning played not tied to frame rate, might change in the future)
            lightning_effect.Play();
        }
    }

    void Update()
    {
        if (!locked)
        {
            if (lightningAmount == maxHits) //crystal has been hit enough times
            {
                //start transition coroutine
                if (!transitionStarted)
                {
                    StartCoroutine(transition());
                    transitionStarted = true;
                }
            }
            else //crystal is still noraml (not hit enough)
            {
                spin = Vector3.SmoothDamp(spin, targetSpin, ref spinVelocity, 0.5f); //update the current speed
                transform.Rotate(spin);

                targetSpin = Vector3.SmoothDamp(targetSpin, Vector3.zero, ref targetSpinVelocity, 0.7f); //update the target speed to slowly reset it to zero

                if (Mathf.Abs(targetSpin.z) < 1.5f)
                {
                    float difference = transform.rotation.z;
                    targetSpin = new Vector3(0, 0, difference);

                    if (Mathf.Abs(difference) < 0.02)
                    {
                        //bobble the crystal up and down
                        transform.Translate(Vector2.up * Mathf.Sin(elapsedTime * bobbleSpeed) * bobbleAmount * Time.deltaTime);
                        elapsedTime += Time.deltaTime;
                    }
                }
                else
                {
                    transform.position = startLocation;
                }

                if (lightningAmount > 0 && Time.time - lastHitTime > 2.5f) //decrease the amount of lightning every 2.5 seconds when left alone
                {
                    lightningAmount--;
                    lastHitTime = Time.time;
                }

                var emission = lightning_constant_effect.emission;
                emission.rateOverTime = lightningAmount;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(!locked && other.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<Player>().isDashing)
            {
                other.gameObject.transform.position = (Vector2)transform.position;

                //calculate random knockback
                other.gameObject.GetComponent<Player>().KnockBack = -other.gameObject.GetComponent<Rigidbody2D>().velocity * playerBounceBack;

                lightning_effect.Play();

                targetSpin = new Vector3(0, 0, spin.z + speedIncrease); //increase with respect to the current speed, not the target speed
                if (lightningAmount < maxHits && Time.time - lastHitTime > hitCooldown)
                {
                    lightningAmount++;
                    lastHitTime = Time.time;
                }
            }
        }
    }

    IEnumerator transition()
    {
        //transition to appropriate boss level
        if(crystalType != Type.tutorial)
            whiteScreenFade.SetTrigger("FadeIn");

        //save player data to temporary object
        if(crystalType != Type.tutorial)
            FindObjectOfType<Player>().saveTempState();
   
        yield return new WaitForSeconds(0.5f);

        switch (crystalType)
        {
            case Type.blue:
                SceneManager.LoadSceneAsync(3);
                break;

            case Type.green:
                SceneManager.LoadSceneAsync(4);
                break;

            case Type.orange:
                SceneManager.LoadSceneAsync(5);
                break;

            case Type.pink:
                SceneManager.LoadSceneAsync(6);
                break;

            case Type.red:
                SceneManager.LoadSceneAsync(7);
                break;

            case Type.tutorial:
                Player.staticReference.transform.position = new Vector2(-151.15f, -6.86f); //HARDCODE THIS IN
                gameObject.SetActive(false);
                break;
        }
    }

    private void assignSprite()
    {
        switch (crystalType)
        {
            case Type.blue:
                GetComponent<SpriteRenderer>().sprite = sprites[0];
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

            case Type.tutorial:
                GetComponent<SpriteRenderer>().sprite = sprites[5];
                break;
        }
    }

    private bool isUnlocked()
    {
        if (crystalType == Type.tutorial) return true;

        //1, 2, 3, 4, 5
        //b, g, o, p, r
        int unlockedCrystals = Saver.loadData().CrystalsUnlocked;

        //quick check
        if (unlockedCrystals == 0) return false;
        if (unlockedCrystals == 5) return true;

        //more thorough check
        int crystalLevel = 0;
        switch(crystalType)
        {
            case Type.blue:
                crystalLevel = 1;
                break;

            case Type.green:
                crystalLevel = 2;
                break;

            case Type.orange:
                crystalLevel = 3;
                break;

            case Type.pink:
                crystalLevel = 4;
                break;

            case Type.red:
                crystalLevel = 5;
                break;
        }

        return crystalLevel <= unlockedCrystals;
    }
}
