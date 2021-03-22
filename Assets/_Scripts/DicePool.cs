using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DicePool : MonoBehaviour
{
    public GameObject diePrefab;

    private Queue<GameObject> dieQueue = new Queue<GameObject>();

    public static DicePool instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        PopulateQueue(GameManager.instance.width * GameManager.instance.height);
    }

    private void PopulateQueue(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject tempDie = Instantiate(diePrefab, Vector3.zero, Quaternion.identity, transform);
            dieQueue.Enqueue(tempDie);
        }
    }

    public GameObject RetrieveDie()
    {
        return dieQueue.Dequeue();
    }
}
