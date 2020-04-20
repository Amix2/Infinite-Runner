using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    public int length;
    public bool[] inputArray;
    public bool[] outputArray;

    public int NumOfInputLanes { get; private set; }
    public int NumOfOutputLanes { get; private set; }
    public int InputMask { get; private set; }
    public int OutputMask { get; private set; }
    public Vector3 BeginPosition => transform.position;
    public Vector3 EndPosition => BeginPosition + new Vector3(0,0,length);

    public float DistanceToBegin(Vector3 fromPosition)
    {
        return BeginPosition.z - fromPosition.z;
    }

    public float DistanceToEnd(Vector3 fromPosition)
    {
        return EndPosition.z - fromPosition.z;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupInputData();
        SetupOutputData();

    }

    private void SetupOutputData()
    {
        NumOfOutputLanes = outputArray.Length;
        OutputMask = 0;
        foreach (bool output in outputArray)
        {
            OutputMask <<= 1;
            if (output)
            {
                OutputMask ^= 1;
            }
        }
    }

    private void SetupInputData()
    {
        NumOfInputLanes = inputArray.Length;
        InputMask = 0;
        foreach (bool input in inputArray)
        {
            InputMask <<= 1;
            if (input)
            {
                InputMask ^= 1;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + new Vector3(0,0,length / 2), new Vector3(Mathf.Max(NumOfInputLanes, NumOfOutputLanes), 4f, length));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
