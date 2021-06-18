using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCode : MonoBehaviour
{
    [SerializeField] Player player;
    public enum Types { inactive, lazer };
    public Types type;

    //public
    public Vector2 direction;
    public Quaternion rotation;

    public float damage;
    public float speed;
    public float range;

    //private
    private float distanceTraveled;

    // Start is called before the first frame update
    void Awake()
    {
        distanceTraveled = 0;

        type = Types.inactive;
    }

    void Update()
    {
        #region Movement Code
        //Movement --------------------------------------------------------------------------
        transform.rotation = Quaternion.identity;

        if (type == Types.lazer)
        {
            transform.Translate(direction * speed * Time.deltaTime);
            distanceTraveled += speed * Time.deltaTime;
        }

        transform.rotation = rotation;
        #endregion

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
            type = Types.lazer;
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
            player.Health -= damage;
            player.character_animations.SetTrigger("Damage");
            Destroy(gameObject);
        }
    }
}
