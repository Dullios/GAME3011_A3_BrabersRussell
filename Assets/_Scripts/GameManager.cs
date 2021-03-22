using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Material[] dieMaterials;

    [Header("Board Properties")]
    public int width;
    public int height;

    public int[,] board;

    public float xOffset;
    public float yOffset;

    public bool isSwapping = false;

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        board = new int[width, height];
        InstantiateTable();
    }

    private void InstantiateTable()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                GameObject tempDie = DicePool.instance.RetrieveDie();
                tempDie.transform.position = new Vector3(transform.position.x + (x * xOffset), transform.position.y + 10, 0);

                DiceHandler diceHandler = tempDie.GetComponent<DiceHandler>();
                diceHandler.fallDelay = (y * 1.5f) + (x * 0.2f);

                Vector3 endPosition = new Vector3(transform.position.x + (x * xOffset), transform.position.y + (y * yOffset), 0);
                StartCoroutine(diceHandler.FallingRoutine(endPosition));
            }
        }
    }
}
