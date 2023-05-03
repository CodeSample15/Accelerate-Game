using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIndicator : MonoBehaviour
{
    public GameObject boss;
    public float radius = 10f;
    public float maxBossDistance = 300f;

    private Vector3 originalScale;

    void Awake()
    {
        if(FindObjectOfType<BossController>() == null)
            gameObject.SetActive(false);

        originalScale = transform.localScale;
    }

    void LateUpdate()
    {
        transform.localPosition = Vector2.zero;

        Vector2 dir = (boss.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0,0,angle));
        transform.Translate(Vector2.right * radius);

        float distPerc = 1f - (Vector2.Distance(transform.position, boss.transform.position) / maxBossDistance);
        distPerc = Mathf.Max(0f, distPerc);
        transform.localScale = originalScale * distPerc;
    }
}
