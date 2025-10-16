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
    [SerializeField] private float minColliderSize = 2f; // BoxCollider2D �ּ� ũ��
    [SerializeField] private float maxColliderSize = 3.5f; // BoxCollider2D �ִ� ũ��
    [SerializeField] private float minOffset = -2.5f; // BoxCollider2D �ּ� ������
    [SerializeField] private float maxOffset = 2.5f; // BoxCollider2D �ִ� ������
    [SerializeField] private float scrollSpeed = 2f; // ��� �̵� �ӵ�

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

    // ���� Ÿ�̸� ���� �� ��� �� ����
    void HandleSpawning()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnPillarPair();
            spawnTimer = spawnInterval;
        }
    }


    // ���� ��ġ�� ��� �� ����
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

    // ��� Ȱ��ȭ�� ����� �������� �̵�
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

    // ȭ�� ������ ���� ����� ��Ȱ��ȭ�ϰ� Ǯ�� ��ȯ
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
