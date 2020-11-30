using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    //public
    [SerializeField] public GameObject BulletObject;

    public void shoot(Vector2 startPos, Vector2 direction, Vector2 playerPosition, float damage, float speed, float range, int type)
    {
        GameObject holder = Instantiate(BulletObject, startPos, Quaternion.identity);
        holder.SetActive(true);
        holder.GetComponent<BulletCode>().direction = direction;
        holder.GetComponent<BulletCode>().damage = damage;
        holder.GetComponent<BulletCode>().speed = speed;
        holder.GetComponent<BulletCode>().range = range;
        holder.GetComponent<BulletCode>().setType(type);

        //Calculating where the bullet should point
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        holder.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        holder.GetComponent<BulletCode>().rotation = holder.transform.rotation;
    }
}