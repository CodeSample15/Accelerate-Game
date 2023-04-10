using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningLaser : MonoBehaviour
{
    [SerializeField] private float DamagePerSecond;
    [SerializeField] private float TurnSpeed;

    private Player player;
    private LineRenderer[] lasers;
    private float angle;

    void Awake()
    {
        player = FindObjectOfType<Player>();

        lasers = new LineRenderer[4];
        for (int i = 0; i < 4; i++)
            lasers[i] = GetComponentsInChildren<LineRenderer>()[i];

        angle = 0;
    }

    void Update()
    {
        if (!PauseButton.IsPaused)
        {
            angle += TurnSpeed * Time.deltaTime;

            //update each laser's angle
            float offset = 0;
            foreach (LineRenderer r in lasers)
            {
                Vector2 raycastDir = Quaternion.AngleAxis(angle + offset, Vector3.forward) * Vector2.up;
                int mask = 1 << 8; //only hit the ground layer
                RaycastHit2D hit = Physics2D.Raycast(transform.position, raycastDir, Mathf.Infinity, mask);

                Vector2 endLoc = transform.position;
                if (hit.collider != null)
                {
                    endLoc = (Vector2)transform.position + (raycastDir * hit.distance);

                    //check for collisions with player with another raycast
                    mask = 1 << 8 | 1 << 12;
                    hit = Physics2D.Raycast(transform.position, raycastDir, Mathf.Infinity, mask);
                    if (hit.collider != null && hit.collider.CompareTag("Player"))
                        player.Health -= DamagePerSecond * Time.deltaTime;
                }

                r.SetPosition(0, transform.position);
                r.SetPosition(1, endLoc);

                offset += 90;
            }
        }
    }
}
