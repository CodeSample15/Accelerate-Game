using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCode : MonoBehaviour
{
    [SerializeField] Player player;
    public enum Types { inactive, spike };
    public Types type;

    //public
    public Vector2 direction;
    public Quaternion rotation;

    public float damage;
    public float speed;
    public float range;

    //private
    private float distanceTraveled;

    //spike bullet
    private float rotationSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        distanceTraveled = 0;

        //spike bullet
        rotationSpeed = 1f;

        type = Types.inactive;
    }

    void Update()
    {
        if (!PauseButton.IsPaused)
        {
            #region Movement Code
            //Movement --------------------------------------------------------------------------
            transform.rotation = Quaternion.identity;

            if (type == Types.spike)
            {
                transform.Translate(direction * speed * Time.deltaTime);
                transform.Rotate(Vector2.right * rotationSpeed * Time.deltaTime);
                distanceTraveled += speed * Time.deltaTime;
            }

            transform.rotation = rotation;
            #endregion
        }

        if (distanceTraveled > range)
        {
            Destroy(gameObject);
        }
    }

    public void setType(int bulletType)
    {
        if(bulletType == 0)
        {
            type = Types.inactive;
        }
        else if(bulletType == 1)
        {
            type = Types.spike;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            if (!player.GetComponent<Player>().isDashing)
            {
                player.Health -= damage;
                player.character_animations.SetTrigger("Damage");
            }
            Destroy(gameObject);
        }
    }
}
