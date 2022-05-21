using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This basically just destroys an object after preset amount of time.
/// Useful if you want a gameobject to only exist for a little bit and then destroy itself.
/// This keeps memory usage lower while making an automated system of getting rid of old gameobjects
/// </summary>
public class TemporaryCreation : MonoBehaviour
{
    [SerializeField] private float lifetime; //how long the object exists

    void Start()
    {
        StartCoroutine(exist());
    }

    private IEnumerator exist()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
