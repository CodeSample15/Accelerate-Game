using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeAttack : MonoBehaviour
{
    [SerializeField] private GameObject spike;
    [SerializeField] private Color spikeColor;
    [SerializeField] private float spikeSpeed;
    [SerializeField] private float spikeDamage;
    [SerializeField] private float spikeSize;

    [Space]
    [SerializeField] private float timePerRotation;
    [SerializeField] private int numRotations;
    [SerializeField] private float attackTime;

    private GameObject[] objectPool;
    private float timeElapsed;

    private int activeSpikes;

    private bool spawningSpike;
    private bool isActive;

    void Awake()
    {
        timeElapsed = 0f;
        activeSpikes = 0;
        spawningSpike = false;
        isActive = false;

        objectPool = new GameObject[numRotations];
        for(int i=0; i<numRotations; i++)
        {
            objectPool[i] = Instantiate(spike, transform.position, Quaternion.identity);
            objectPool[i].SetActive(false);
            objectPool[i].GetComponent<SpriteRenderer>().color = spikeColor;
            objectPool[i].transform.localScale = Vector3.one * spikeSize;
            objectPool[i].GetComponent<SpikeCode>().spikeSpeed = spikeSpeed;
            objectPool[i].GetComponent<SpikeCode>().spikeDamage = spikeDamage;
        }
    }

    void Update()
    {
        if (!PauseButton.IsPaused && isActive)
        {
            if (!spawningSpike)
            {
                spawningSpike = true;
                StartCoroutine(spawnSpike());
            }

            timeElapsed += Time.deltaTime;
            if(timeElapsed > attackTime)
            {
                foreach (GameObject obj in objectPool)
                {
                    obj.SetActive(false);
                    obj.transform.position = transform.position;
                }

                activeSpikes = 0;
                spawningSpike = false;
                timeElapsed = 0;
                isActive = false;
            }
        }
    }

    public void BeginSpawning()
    {
        isActive = true;
    }

    IEnumerator spawnSpike()
    {
        if (activeSpikes < numRotations)
        {
            activeSpikes++;

            transform.Rotate(new Vector3(0, 0, 180f / numRotations), Space.World);
            objectPool[activeSpikes - 1].SetActive(true);
            objectPool[activeSpikes - 1].transform.rotation = transform.rotation;
        }

        yield return new WaitForSeconds(timePerRotation);

        spawningSpike = false;
    }

    void OnDestroy()
    {
        foreach(GameObject obj in objectPool)
        {
            Destroy(obj);
        }
    }
}
