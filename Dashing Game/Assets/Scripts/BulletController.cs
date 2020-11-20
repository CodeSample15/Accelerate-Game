using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    //public
    [SerializeField] public GameObject BulletObject;

    public void shoot(Vector2 startPos, Vector2 direction, float damage, float speed, float range)
    {
        GameObject holder = Instantiate(BulletObject, startPos, Quaternion.identity);
        holder.GetComponent<BulletCode>().direction = direction;
        holder.GetComponent<BulletCode>().damage = damage;
        holder.GetComponent<BulletCode>().speed = speed;
        holder.GetComponent<BulletCode>().range = range;
    }
}