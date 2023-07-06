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

    [Tooltip("How far the spikes start from the center")]
    [SerializeField] private float offset;

    [Space]
    [SerializeField] private float timePerRotation;
    [SerializeField] private int numRotations;
    [SerializeField] private float attackTime;

    private GameObject[] objectPool;
    private float timeElapsed;

    private int activeSpikes;

    private bool isActive;

    void Awake()
    {
        timeElapsed = 0f;
        activeSpikes = 0;
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
            timeElapsed += Time.deltaTime;
            if(timeElapsed > attackTime)
            {
                foreach (GameObject obj in objectPool)
                    obj.SetActive(false);

                isActive = false;
            }
        }
    }

    public void BeginSpawning()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);

        isActive = true;
        activeSpikes = 0;
        timeElapsed = 0;

        StartCoroutine(spawn());
    }

    private IEnumerator spawn()
    {
        while(activeSpikes < numRotations)
        {
            if(!PauseButton.IsPaused)
            {
                //spawn a new spike
                activeSpikes++;

                transform.Rotate(new Vector3(0, 0, 180f / numRotations), Space.World);
                objectPool[activeSpikes - 1].SetActive(true);
                objectPool[activeSpikes - 1].transform.rotation = transform.rotation;
                objectPool[activeSpikes - 1].transform.position = transform.position;
                objectPool[activeSpikes - 1].transform.Translate(objectPool[activeSpikes - 1].transform.right * offset);
            }

            yield return new WaitForSeconds(timePerRotation);

            if (!isActive)
                break;
        }

        yield return new WaitForEndOfFrame();
    }

    void OnDestroy()
    {
        foreach(GameObject obj in objectPool)
        {
            Destroy(obj);
        }
    }
}
