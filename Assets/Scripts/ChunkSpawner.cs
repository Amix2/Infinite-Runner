using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkSpawner : MonoBehaviour
{
    public List<GameObject> avaliableChunks;
    public GameObject player;
    public GameObject startingChunk;

    private List<Chunk> orderedChunks;
    private Chunk LastChunk => orderedChunks[orderedChunks.Count - 1];
    private Chunk FirstChunk => orderedChunks[0];
    private Vector3 PlayerPosition => player.transform.position;

    // Start is called before the first frame update
    void Start()
    {
        orderedChunks = new List<Chunk>();
        GameObject firstChunk = Instantiate(startingChunk, Vector3.zero, Quaternion.identity);
        orderedChunks.Add(firstChunk.GetComponent<Chunk>());

        foreach (GameObject chunkGO in avaliableChunks)
        {
            chunkGO.GetComponent<Chunk>().Init();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(LastChunk.DistanceToEnd(PlayerPosition) < Settings.World.minDistanceInFront)
        {
            GameObject chunk = Instantiate(GetNextChunk(), LastChunk.EndPosition, Quaternion.identity);
            orderedChunks.Add(chunk.GetComponent<Chunk>());
        }

        if (FirstChunk.DistanceToEnd(PlayerPosition) < -5)
        {
            Destroy(FirstChunk.gameObject);
            orderedChunks.RemoveAt(0);
        }
    }

    GameObject GetNextChunk()
    {
        List<GameObject> goodNextChunks = new List<GameObject>();
        foreach(GameObject chunkGO in avaliableChunks)
        {
            Chunk chunk = chunkGO.GetComponent<Chunk>();
            if (LastChunk.NumOfOutputLanes == chunk.NumOfInputLanes &&  (LastChunk.OutputMask & chunk.InputMask) > 0)
            {
                goodNextChunks.Add(chunkGO);
            }
        }
        return goodNextChunks[Random.Range(0, goodNextChunks.Count)];
    }
}
