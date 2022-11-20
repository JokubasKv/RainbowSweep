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

    [Header("Maximum enenmy count")]
    [SerializeField]
    private int maxCount;

    private int count = 0;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spawnEnemy(mageInterval, magePrefab));
        StartCoroutine(spawnEnemy(warriorInterval, warriorPrefab));
        StartCoroutine(spawnEnemy(wraithInterval, wraithPrefab));
    }

    private IEnumerator spawnEnemy(float inverval, GameObject enemy)
    {
        if (count < maxCount)
        {
            yield return new WaitForSeconds(inverval);
            if (count < maxCount)
            {
                GameObject newEnemy = Instantiate(enemy, new Vector3(Random.Range(coords.position.x - 10f, coords.position.x + 10), 0, Random.Range(coords.position.z - 10f, coords.position.z + 10f)), Quaternion.identity);
                count++;
                StartCoroutine(spawnEnemy(inverval, enemy));
            }
        }
    }
}
