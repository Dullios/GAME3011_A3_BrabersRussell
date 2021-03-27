using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Material[] dieMaterials;

    [Header("Board Properties")]
    public int width;
    public int height;

    public GameObject[,] board;

    public float xOffset;
    public float yOffset;

    public DiceHandler selectedDie;
    public bool isSwapping = false;

    [Header("Dice Properties")]
    public Vector3 endPosition;

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
        board = new GameObject[width, height];

        isSwapping = true;
        InstantiateBoard();
    }

    private void InstantiateBoard()
    {
        int leftVal = 0;
        int prevVal = 0;

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                GameObject tempDie = DicePool.instance.RetrieveDie();
                tempDie.transform.position = new Vector3(transform.position.x + (x * xOffset), transform.position.y + (y * yOffset) + 10, 0);

                board[x, y] = tempDie;

                DiceHandler diceHandler = tempDie.GetComponent<DiceHandler>();
                diceHandler.gridPosition = new Vector2(x, y);
                diceHandler.fallDelay = (y * 1.5f) + (x * 0.2f);

                if (x > 0)
                    leftVal = board[x - 1, y].GetComponent<DiceHandler>().value;

                do
                    diceHandler.value = Random.Range(1, 7);
                while (diceHandler.value == leftVal || diceHandler.value == prevVal);

                prevVal = diceHandler.value;

                endPosition = new Vector3(transform.position.x + (x * xOffset), transform.position.y + (y * yOffset), 0);
                StartCoroutine(diceHandler.FallingRoutine(endPosition));
            }
        }
    }

    public void SwapDice(GameObject secondDie)
    {
        isSwapping = true;

        Vector2 tempGrid = secondDie.GetComponent<DiceHandler>().gridPosition;
        Vector3 tempPos = secondDie.transform.position;

        secondDie.GetComponent<DiceHandler>().gridPosition = selectedDie.gridPosition;
        secondDie.GetComponent<DiceHandler>().startPosition = secondDie.transform.position;
        selectedDie.gridPosition = tempGrid;
        selectedDie.startPosition = selectedDie.transform.position;

        StartCoroutine(secondDie.GetComponent<DiceHandler>().SwapPosition(selectedDie.transform.position));
        StartCoroutine(selectedDie.SwapPosition(tempPos));

        selectedDie = null;
    }
}
