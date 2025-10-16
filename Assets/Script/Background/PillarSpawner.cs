using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] private ObjectPool pillarPool;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnXPosition;
    [SerializeField] private float despawnXPosition;

    [Header("Pillar Settings")]
    [SerializeField] private float minColliderSize = 2f; // BoxCollider2D 최소 크기
    [SerializeField] private float maxColliderSize = 3.5f; // BoxCollider2D 최대 크기
    [SerializeField] private float minOffset = -2.5f; // BoxCollider2D 최소 오프셋
    [SerializeField] private float maxOffset = 2.5f; // BoxCollider2D 최대 오프셋
    [SerializeField] private float scrollSpeed = 2f; // 기둥 이동 속도

    private List<GameObject> activePillars = new List<GameObject>();
    private float spawnTimer;

    void Start()
    {
        SpawnPillarPair();
        spawnTimer = spawnInterval;
    }

    void Update()
    {
        HandleSpawning();
        MovePillars();
        CheckDespawn();
    }

    // 스폰 타이머 관리 및 기둥 쌍 스폰
    void HandleSpawning()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnPillarPair();
            spawnTimer = spawnInterval;
        }
    }


    // 스폰 위치에 기둥 쌍 생성
    void SpawnPillarPair()
    {
        GameObject pillarPair = pillarPool.GetObject();
        pillarPair.transform.position = new Vector3(spawnXPosition, 0, 0);

        Pillar pairScript = pillarPair.GetComponent<Pillar>();
        if (pairScript != null)
        {
            pairScript.SetupPillars(minColliderSize, maxColliderSize, minOffset, maxOffset);
        }

        activePillars.Add(pillarPair);
    }

    // 모든 활성화된 기둥을 왼쪽으로 이동
    void MovePillars()
    {
        foreach (GameObject pillar in activePillars)
        {
            if (pillar.activeInHierarchy)
            {
                pillar.transform.position += Vector3.left * scrollSpeed * Time.deltaTime;
            }
        }
    }

    // 화면 밖으로 나간 기둥을 비활성화하고 풀에 반환
    void CheckDespawn()
    {
        for (int i = activePillars.Count - 1; i >= 0; i--)
        {
            if (activePillars[i].transform.position.x < despawnXPosition)
            {
                pillarPool.ReturnObject(activePillars[i]);
                activePillars.RemoveAt(i);
            }
        }
    }
}
