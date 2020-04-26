using System.Collections.Generic;
using UnityEngine;

public class ChunkSpawner : MonoBehaviour
{
    public List<GameObject> avaliableChunks;
    public GameObject player;
    public GameObject startingChunk;
    public GameObject flatChunk;
    public bool skipIntro = true;

    private int emptyChunksAfterDeath = 3;
    private List<Chunk> orderedChunks;
    public Chunk LastChunk => orderedChunks[orderedChunks.Count - 1];
    public Chunk FirstChunk => orderedChunks[0];
    private Vector3 PlayerPosition => player.transform.position;

    // Start is called before the first frame update
    private void Start()
    {
        orderedChunks = new List<Chunk>();
        GameObject firstChunk;
        if (skipIntro)
        {
            firstChunk = Instantiate(flatChunk, Vector3.zero, Quaternion.identity);
        }
        else
        {
            firstChunk = Instantiate(startingChunk, Vector3.zero, Quaternion.identity);
        }
        orderedChunks.Add(firstChunk.GetComponent<Chunk>());

        foreach (GameObject chunkGO in avaliableChunks)
        {
            chunkGO.GetComponent<Chunk>().Init();
        }
        flatChunk.GetComponent<Chunk>().Init();

        GameObject.Find("Player").GetComponent<PlayerController>().OnDeath += PutEmptyLevelOnDeath;
    }

    // Update is called once per frame
    private void Update()
    {
        if (LastChunk.DistanceToEnd(PlayerPosition) < Settings.World.minDistanceInFront)
        {
            GameObject chunk = Instantiate(GetNextChunk(), LastChunk.EndPosition, Quaternion.identity);
            orderedChunks.Add(chunk.GetComponent<Chunk>());
        }

        if (FirstChunk.DistanceToEnd(PlayerPosition) < 0)
        {
            Destroy(FirstChunk.gameObject);
            orderedChunks.RemoveAt(0);
        }
    }

    private void PutEmptyLevelOnDeath()
    {
        for (int i = 0; i < emptyChunksAfterDeath; i++)
        {
            GameObject chunk = Instantiate(flatChunk, FirstChunk.EndPosition + new Vector3(0, 0, i * flatChunk.GetComponent<Chunk>().length), Quaternion.identity);
            chunk.name = "AfterDeathChunk_" + i;
            orderedChunks.Insert(1, chunk.GetComponent<Chunk>());
        }
        for (int i = emptyChunksAfterDeath + 1; i < orderedChunks.Count; i++)
        {
            orderedChunks[i].transform.position += new Vector3(0, 0, emptyChunksAfterDeath * flatChunk.GetComponent<Chunk>().length);
        }
    }

    private GameObject GetNextChunk()
    {
        List<GameObject> goodNextChunks = new List<GameObject>();
        foreach (GameObject chunkGO in avaliableChunks)
        {
            Chunk chunk = chunkGO.GetComponent<Chunk>();
            if (LastChunk.NumOfOutputLanes == chunk.NumOfInputLanes && (LastChunk.OutputMask & chunk.InputMask) > 0)
            {
                goodNextChunks.Add(chunkGO);
            }
        }
        return goodNextChunks[Random.Range(0, goodNextChunks.Count)];
    }
}