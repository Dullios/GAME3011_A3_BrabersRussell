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

        DiceHandler secondHandler = secondDie.GetComponent<DiceHandler>();
        Vector2 tempGrid = secondHandler.gridPosition;
        Vector3 tempPos = secondDie.transform.position;

        secondHandler.gridPosition = selectedDie.gridPosition;
        secondHandler.startPosition = secondDie.transform.position;
        board[(int)selectedDie.gridPosition.x, (int)selectedDie.gridPosition.y] = secondDie;
        
        selectedDie.gridPosition = tempGrid;
        selectedDie.startPosition = selectedDie.transform.position;
        board[(int)tempGrid.x, (int)tempGrid.y] = selectedDie.gameObject;

        StartCoroutine(secondHandler.SwapPosition(selectedDie.transform.position));
        StartCoroutine(selectedDie.SwapPosition(tempPos));

        CheckMatch(secondHandler);
        CheckMatch(selectedDie);

        selectedDie = null;
        isSwapping = false;
    }

    public void CheckMatch(DiceHandler die)
    {
        if (CheckStraightFiveHorizontal(die))
        {
            Debug.Log("Horizontal Straight 5 Made!");
            return;
        }

        if (CheckStraightSixHorizontal(die))
        {
            Debug.Log("Horizontal Straight 6 Made!");
            return;
        }

        if (CheckStraightFiveVertical(die))
        {
            Debug.Log("Vertical Straight 5 Made!");
            return;
        }

        if (CheckStraightSixVertical(die))
        {
            Debug.Log("Vertical Straight 6 Made!");
            return;
        }

        if (CheckThreeHorizontal(die))
        {
            Debug.Log("Match Three Horizontal!");
            return;
        }

        if (CheckThreeVertical(die))
        {
            Debug.Log("Match Three Vertical!");
            return;
        }
    }

    #region Match_Checks
    private bool CheckStraightFiveHorizontal(DiceHandler die)
    {
        DiceHandler otherDie;

        // Check 1-5
        int leftCount = die.value - 1;

        for(int i = leftCount; i > 0; i--)
        {
            if (die.gridPosition.x - i < 0)
                return false;
            else
            {
                otherDie = board[(int)(die.gridPosition.x - i), (int)die.gridPosition.y].GetComponent<DiceHandler>();
                if (otherDie.dieColor != die.dieColor || otherDie.value != die.value - i)
                    return false;
                else
                    continue;
            }
        }

        int rightCount = 5 - die.value;

        for (int i = rightCount; i > 0; i--)
        {
            if (die.gridPosition.x + i >= width)
                return false;
            else
            {
                otherDie = board[(int)(die.gridPosition.x + i), (int)die.gridPosition.y].GetComponent<DiceHandler>();
                if (otherDie.dieColor != die.dieColor || otherDie.value != die.value + i)
                    return false;
                else
                    continue;
            }
        }

        return true;
    }

    private bool CheckStraightSixHorizontal(DiceHandler die)
    {
        DiceHandler otherDie;

        // Check 2-6
        int leftCount = die.value - 2;

        for (int i = leftCount; i > 0; i--)
        {
            if (die.gridPosition.x - i < 0)
                return false;
            else
            {
                otherDie = board[(int)(die.gridPosition.x - i), (int)die.gridPosition.y].GetComponent<DiceHandler>();
                if (otherDie.dieColor != die.dieColor || otherDie.value != die.value - i)
                    return false;
                else
                    continue;
            }
        }

        int rightCount = 6 - die.value;

        for (int i = rightCount; i > 0; i--)
        {
            if (die.gridPosition.x + i >= width)
                return false;
            else
            {
                otherDie = board[(int)(die.gridPosition.x + i), (int)die.gridPosition.y].GetComponent<DiceHandler>();
                if (otherDie.dieColor != die.dieColor || otherDie.value != die.value + i)
                    return false;
                else
                    continue;
            }
        }

        return true;
    }

    private bool CheckStraightFiveVertical(DiceHandler die)
    {
        DiceHandler otherDie;

        // Check 1-5
        int upCount = die.value - 1;

        for (int i = upCount; i > 0; i--)
        {
            if (die.gridPosition.y + i >= height)
                return false;
            else
            {
                otherDie = board[(int)die.gridPosition.x, (int)(die.gridPosition.y + i)].GetComponent<DiceHandler>();
                if (otherDie.dieColor != die.dieColor || otherDie.value != die.value - i)
                    return false;
                else
                    continue;
            }
        }

        int downCount = 5 - die.value;

        for (int i = downCount; i > 0; i--)
        {
            if (die.gridPosition.y - i < 0)
                return false;
            else
            {
                otherDie = board[(int)die.gridPosition.x, (int)(die.gridPosition.y - i)].GetComponent<DiceHandler>();
                if (otherDie.dieColor != die.dieColor || otherDie.value != die.value + i)
                    return false;
                else
                    continue;
            }
        }

        return true;
    }

    private bool CheckStraightSixVertical(DiceHandler die)
    {
        DiceHandler otherDie;

        // Check 2-6
        int upCount = die.value - 2;

        for (int i = upCount; i > 0; i--)
        {
            if (die.gridPosition.y + i >= height)
                return false;
            else
            {
                otherDie = board[(int)die.gridPosition.x, (int)(die.gridPosition.y + i)].GetComponent<DiceHandler>();
                if (otherDie.dieColor != die.dieColor || otherDie.value != die.value - i)
                    return false;
                else
                    continue;
            }
        }

        int downCount = 6 - die.value;

        for (int i = downCount; i > 0; i--)
        {
            if (die.gridPosition.y - i < 0)
                return false;
            else
            {
                otherDie = board[(int)die.gridPosition.x, (int)(die.gridPosition.y - i)].GetComponent<DiceHandler>();
                if (otherDie.dieColor != die.dieColor || otherDie.value != die.value + i)
                    return false;
                else
                    continue;
            }
        }

        return true;
    }

    private bool CheckThreeHorizontal(DiceHandler die)
    {
        if (die.gridPosition.x - 2 >= 0)
        {
            DiceHandler twoLeft = board[(int)(die.gridPosition.x - 2), (int)die.gridPosition.y].GetComponent<DiceHandler>();
            DiceHandler oneLeft = board[(int)(die.gridPosition.x - 1), (int)die.gridPosition.y].GetComponent<DiceHandler>();

            if ((twoLeft.dieColor == die.dieColor && twoLeft.value == die.value) &&
                (oneLeft.dieColor == die.dieColor && oneLeft.value == die.value))
                return true;
        }
        
        if(die.gridPosition.x - 1 >= 0 && die.gridPosition.x + 1 < width)
        {
            DiceHandler oneLeft = board[(int)(die.gridPosition.x - 1), (int)die.gridPosition.y].GetComponent<DiceHandler>();
            DiceHandler oneRight = board[(int)(die.gridPosition.x + 1), (int)die.gridPosition.y].GetComponent<DiceHandler>();

            if ((oneLeft.dieColor == die.dieColor && oneLeft.value == die.value) &&
                (oneRight.dieColor == die.dieColor && oneRight.value == die.value))
                return true;
        }

        if (die.gridPosition.x + 2 < width)
        {
            DiceHandler oneRight = board[(int)(die.gridPosition.x + 1), (int)die.gridPosition.y].GetComponent<DiceHandler>();
            DiceHandler twoRight = board[(int)(die.gridPosition.x + 2), (int)die.gridPosition.y].GetComponent<DiceHandler>();

            if ((oneRight.dieColor == die.dieColor && oneRight.value == die.value) &&
                (twoRight.dieColor == die.dieColor && twoRight.value == die.value))
                return true;
        }

        return false;
    }

    private bool CheckThreeVertical(DiceHandler die)
    {
        if (die.gridPosition.y - 2 >= 0)
        {
            DiceHandler twoDown = board[(int)die.gridPosition.x, (int)(die.gridPosition.y - 2)].GetComponent<DiceHandler>();
            DiceHandler oneDown = board[(int)die.gridPosition.x, (int)(die.gridPosition.y - 1)].GetComponent<DiceHandler>();

            if ((twoDown.dieColor == die.dieColor && twoDown.value == die.value) &&
                (oneDown.dieColor == die.dieColor && oneDown.value == die.value))
                return true;
        }

        if (die.gridPosition.y - 1 >= 0 && die.gridPosition.y + 1 < height)
        {
            DiceHandler oneDown = board[(int)die.gridPosition.x, (int)(die.gridPosition.y - 1)].GetComponent<DiceHandler>();
            DiceHandler oneUp = board[(int)die.gridPosition.x, (int)(die.gridPosition.y + 1)].GetComponent<DiceHandler>();

            if ((oneDown.dieColor == die.dieColor && oneDown.value == die.value) &&
                (oneUp.dieColor == die.dieColor && oneUp.value == die.value))
                return true;
        }

        if (die.gridPosition.y + 2 < height)
        {
            DiceHandler oneUp = board[(int)die.gridPosition.x, (int)(die.gridPosition.y + 1)].GetComponent<DiceHandler>();
            DiceHandler twoUp = board[(int)die.gridPosition.x, (int)(die.gridPosition.y + 2)].GetComponent<DiceHandler>();

            if ((oneUp.dieColor == die.dieColor && oneUp.value == die.value) &&
                (twoUp.dieColor == die.dieColor && twoUp.value == die.value))
                return true;
        }

        return false;
    }

    #endregion
}
