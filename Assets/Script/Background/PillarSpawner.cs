using System.Collections.Generic;
using UnityEngine;

public class PillarSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] private ObjectPool pillarPool;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnXPosition = 6f;
    [SerializeField] private float despawnXPosition = -7f;
    [SerializeField] private bool autoStart = false;

    [Header("Pillar Settings")]
    [SerializeField] private float minColliderSize = 2f;
    [SerializeField] private float maxColliderSize = 3.5f;
    [SerializeField] private float minOffset = -2.5f;
    [SerializeField] private float maxOffset = 2.5f;
    [SerializeField] private float scrollSpeed = 2f;

    private readonly List<GameObject> activePillars = new();
    private float spawnTimer;
    private bool running = false;

    void Start()
    {
        spawnTimer = spawnInterval;
        if (autoStart) Begin();   // �������� ���� false ����
    }

    void Update()
    {
        if (!running) return;
        HandleSpawning();
        MovePillars();
        CheckDespawn();
    }

    // ===== GameManager���� ȣ�� =====
    public void Begin()
    {
        running = true;
        spawnTimer = spawnInterval;
        // �ʿ��ϸ� ���� ���� 1�� �̸� ����:
        // SpawnPillarPair();
    }

    public void Stop() => running = false;

    public void Clear()
    {
        for (int i = activePillars.Count - 1; i >= 0; i--)
        {
            var go = activePillars[i];
            if (go) pillarPool.ReturnObject(go);
        }
        activePillars.Clear();
        spawnTimer = spawnInterval;
    }

    public void SetScrollSpeed(float s) => scrollSpeed = Mathf.Max(0f, s);
    public void SetSpawnInterval(float sec) { spawnInterval = Mathf.Max(0.05f, sec); spawnTimer = spawnInterval; }

    // ===== ���� ���� =====
    void HandleSpawning()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnPillarPair();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnPillarPair()
    {
        if (!pillarPool) { Debug.LogWarning("PillarSpawner: pillarPool�� ����ֽ��ϴ�."); return; }

        var pillarPair = pillarPool.GetObject();
        if (!pillarPair) return;

        pillarPair.transform.position = new Vector3(spawnXPosition, 0f, 0f);

        var pair = pillarPair.GetComponent<Pillar>();
        if (pair != null)
            pair.SetupPillars(minColliderSize, maxColliderSize, minOffset, maxOffset);

        if (!activePillars.Contains(pillarPair))
            activePillars.Add(pillarPair);
    }

    void MovePillars()
    {
        float move = scrollSpeed * Time.deltaTime;
        for (int i = activePillars.Count - 1; i >= 0; i--)
        {
            var go = activePillars[i];
            if (go == null || !go.activeInHierarchy)
            {
                activePillars.RemoveAt(i);
                continue;
            }
            go.transform.position += Vector3.left * move;
        }
    }

    void CheckDespawn()
    {
        for (int i = activePillars.Count - 1; i >= 0; i--)
        {
            var go = activePillars[i];
            if (!go) { activePillars.RemoveAt(i); continue; }

            if (go.transform.position.x < despawnXPosition)
            {
                pillarPool.ReturnObject(go);
                activePillars.RemoveAt(i);
            }
        }
    }
}
