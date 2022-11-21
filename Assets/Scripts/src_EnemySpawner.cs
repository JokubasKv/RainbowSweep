using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawning enemies
/// </summary>
public class src_EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    private GameObject magePrefab;
    [SerializeField]
    private GameObject warriorPrefab;
    [SerializeField]
    private GameObject wraithPrefab;

    [Header("Spawn intervals")]
    [SerializeField]
    private float mageInterval;
    [SerializeField]
    private float warriorInterval;
    [SerializeField]
    private float wraithInterval;

    [Header("Center coordinates")]
    [SerializeField]
    private Transform coords;

    [Header("Spawn offset coordinates")]
    [SerializeField]
    private float offsetX;
    [SerializeField]
    private float offsetZ;

    [SerializeField]
    private bool spawnMage;
    [SerializeField]
    private bool spawnWarrior;
    [SerializeField]
    private bool SpawnWraith;

    [Header("Maximum enenmy count")]
    [SerializeField]
    private int maxCount;

    private int count = 0;


    // Start is called before the first frame update
    void Start()
    {
        if (spawnMage)
        {
            StartCoroutine(spawnEnemy(mageInterval, magePrefab));
        }
        if (spawnWarrior)
        {
            StartCoroutine(spawnEnemy(warriorInterval, warriorPrefab));
        }
        if (SpawnWraith)
        {
            StartCoroutine(spawnEnemy(wraithInterval, wraithPrefab));
        }
    }

    private IEnumerator spawnEnemy(float inverval, GameObject enemy)
    {
        if (count < maxCount)
        {
            yield return new WaitForSeconds(inverval);
            if (count < maxCount)
            {
                GameObject newEnemy = Instantiate(enemy, new Vector3(Random.Range(coords.position.x - offsetX, coords.position.x + offsetX), coords.position.y, Random.Range(coords.position.z - offsetZ, coords.position.z + offsetZ)), Quaternion.identity);
                count++;
                StartCoroutine(spawnEnemy(inverval, enemy));
            }
        }
    }
}
