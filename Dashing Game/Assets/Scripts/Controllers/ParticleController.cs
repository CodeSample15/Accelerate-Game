using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public List<ParticleSystem> Particles;

    void Awake()
    {
        Particles = new List<ParticleSystem>();
    }

    public void AddParticles(ParticleSystem system)
    {
        system.Play();
        Particles.Add(system);
    }

    void Update()
    {
        //loop through the particle systems and remove any that are finished playing
        for (int i = 0; i < Particles.Count; i++)
        {
            if (!Particles[i].IsAlive())
            {
                //Destroy the inactive particle system
                Destroy(Particles[i].gameObject);;
                Particles.RemoveAt(i);
                i--;
            }
        }
    }
}
