using System.Collections.Generic;
using UnityEngine;

public class ChunkSpawner : MonoBehaviour
{
    public List<GameObject> avaliableChunks;
    public GameObject player;
    public GameObject startingChunk;
    public GameObject flatChunk;

    private int emptyChunksAfterDeath = 3;
    private List<Chunk> orderedChunks;
    public Chunk LastChunk => orderedChunks.Count > 0 ? orderedChunks[orderedChunks.Count - 1] : null;
    public Chunk FirstChunk => orderedChunks[0];
    private Vector3 PlayerPosition => player.transform.position;

    // Start is called before the first frame update
    private void Start()
    {
        foreach (GameObject chunkGO in avaliableChunks)
        {
            chunkGO.GetComponent<Chunk>().Init();
        }
        flatChunk.GetComponent<Chunk>().Init();

        if(orderedChunks == null)
        {
            orderedChunks = new List<Chunk>();
            GameObject firstChunk = Instantiate(startingChunk, Vector3.zero, Quaternion.identity);
        
            orderedChunks.Add(firstChunk.GetComponent<Chunk>());
        }


        GameObject.Find("Player").GetComponent<PlayerController>().OnDeath += PutEmptyLevelOnDeath;
    }

    // Update is called once per frame
    private void Update()
    {
        if (LastChunk.DistanceToEnd(PlayerPosition) < Settings.World.minDistanceInFront)
        {
            GameObject nextChunk = GetNextChunk();
            InsertNextChunk(nextChunk);
        }

        if (FirstChunk.DistanceToEnd(PlayerPosition) < 0)
        {
            Destroy(FirstChunk.gameObject);
            orderedChunks.RemoveAt(0);
        }
    }

    private void InsertNextChunk(GameObject nextChunk)
    {
        Vector3 spawnPos = LastChunk != null ? LastChunk.EndPosition : Vector3.zero;
        GameObject chunk = Instantiate(nextChunk, spawnPos, Quaternion.identity);
        orderedChunks.Add(chunk.GetComponent<Chunk>());
    }

    public void SetNextChunk(GameObject nextChunk)
    {
        for(int i = orderedChunks.Count-1; i>=1; i--)
        {
            Destroy(orderedChunks[i].gameObject);
            orderedChunks.RemoveAt(i);
        }
        InsertNextChunk(nextChunk);
    }

    public void ClearChunks()
    {
        foreach(var chunk in orderedChunks)
        {
            Destroy(chunk.gameObject);
        }
        orderedChunks = new List<Chunk>();
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