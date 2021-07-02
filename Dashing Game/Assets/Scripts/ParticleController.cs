using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class ParticleController : MonoBehaviour
{
    private List<ParticleSystem> particles;

    public List<ParticleSystem> Particles
    {
        get { return Particles; }
    }

    void Awake()
    {
        Thread checker = new Thread(new ThreadStart(ParticleChecking));
    }

    public void StartSystem(ParticleSystem system)
    {
        system.Play();
        particles.Add(system);
    }

    private void ParticleChecking()
    {
        while (true)
        {
            //loop through the particle systems and remove any that are finished playing
            for (int i = 0; i < particles.Count; i++)
            {
                if (!particles[i].IsAlive())
                {
                    //Destroy the inactive particle system
                    Destroy(particles[i].gameObject);
                    particles.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
