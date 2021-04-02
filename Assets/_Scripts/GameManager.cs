using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum Difficulty
{
    EASY,
    MEDIUM,
    HARD
}

public class GameManager : MonoBehaviour
{
    public Material[] dieMaterials;

    [Header("Board Properties")]
    public int width;
    public int height;

    public GameObject[,] board;

    public float xOffset;
    public float yOffset;

    public float resetHeight = 6.1f; // Increase by 1.1 vertically

    [Header("Dice Properties")]
    public DiceHandler selectedDie;
    public Vector3 endPosition;

    [Header("Game States")]
    public bool gameStarted = false;
    [SerializeField] private bool isSwapping = false;
    public bool IsSwapping => isSwapping;

    [Header("Settings and UI")]
    
    public Difficulty difficulty;
    public float timer;
    public float maxTime;
    public int reqScore;
    public int score;

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
        //board = new GameObject[width, height];

        //isSwapping = true;
        //InstantiateBoard();
    }

    private void Update()
    {
        if (gameStarted)
        {
            timer -= Time.deltaTime;
            UIManager.instance.SetTimer(timer);

            if (timer <= 0)
            {
                UIManager.instance.SetGameOverText(false);
                gameStarted = false;
            }

            if(score >= reqScore)
            {
                UIManager.instance.SetGameOverText(true);
                gameStarted = false;
            }
        }
    }

    public void SetIsSwapping()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (board[x, y].GetComponent<DiceHandler>().isTumbling)
                {
                    isSwapping = true;
                    return;
                }
            }
        }

        isSwapping = false;
        gameStarted = true;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CheckMatch(board[x, y].GetComponent<DiceHandler>());
            }
        }
    }

    public void InstantiateBoard()
    {
        board = new GameObject[width, height];
        isSwapping = true;

        timer = maxTime;

        int leftVal = 0;
        int prevVal = 0;

        // Set material by difficulty
        int maxCount = (int)difficulty;
        int count = 0;

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

                diceHandler.GetComponent<MeshRenderer>().material = dieMaterials[count];
                diceHandler.dieColor = (DiceColor)count;
                
                count++;
                if (count > maxCount)
                    count = 0;

                if (x > 0)
                    leftVal = board[x - 1, y].GetComponent<DiceHandler>().value;

                do
                    diceHandler.value = Random.Range(1, 7);
                while (diceHandler.value == leftVal || diceHandler.value == prevVal);

                prevVal = diceHandler.value;

                Vector3 startPosition = diceHandler.transform.position;
                endPosition = new Vector3(transform.position.x + (x * xOffset), transform.position.y + (y * yOffset), 0);
                StartCoroutine(diceHandler.FallingRoutine(startPosition, endPosition));
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
        board[(int)selectedDie.gridPosition.x, (int)selectedDie.gridPosition.y] = secondDie;
        
        selectedDie.gridPosition = tempGrid;
        board[(int)tempGrid.x, (int)tempGrid.y] = selectedDie.gameObject;

        StartCoroutine(secondHandler.SwapPosition(secondDie.transform.position, selectedDie.transform.position));
        StartCoroutine(selectedDie.SwapPosition(selectedDie.transform.position, tempPos));

        selectedDie = null;
    }

    public void CheckMatch(DiceHandler die)
    {
        DiceHandler[] diceList;

        if (CheckStraightFiveHorizontal(die))
        {
            score += 250;
            UIManager.instance.SetScore(score);

            int left = die.value - 1;
            DiceHandler leftMostDie = board[(int)die.gridPosition.x - left, (int)die.gridPosition.y].GetComponent<DiceHandler>();

            diceList = new DiceHandler[5];
            for (int i = 0; i < 5; i++)
                diceList[i] = board[(int)leftMostDie.gridPosition.x + i, (int)leftMostDie.gridPosition.y].GetComponent<DiceHandler>();

            ResetHorizontal(diceList);

            Debug.Log("Horizontal Straight 5 Made!");
            return;
        }

        if (CheckStraightSixHorizontal(die))
        {
            score += 250;
            UIManager.instance.SetScore(score);

            int left = die.value - 2;
            DiceHandler leftMostDie = board[(int)die.gridPosition.x - left, (int)die.gridPosition.y].GetComponent<DiceHandler>();

            diceList = new DiceHandler[5];
            for (int i = 0; i < 5; i++)
                diceList[i] = board[(int)leftMostDie.gridPosition.x + i, (int)leftMostDie.gridPosition.y].GetComponent<DiceHandler>();

            ResetHorizontal(diceList);

            Debug.Log("Horizontal Straight 6 Made!");
            return;
        }

        if (CheckStraightFiveVertical(die))
        {
            score += 250;
            UIManager.instance.SetScore(score);

            int up = die.value - 1;
            DiceHandler upMostDie = board[(int)die.gridPosition.x, (int)die.gridPosition.y + up].GetComponent<DiceHandler>();

            diceList = new DiceHandler[5];
            for (int i = 0; i < 5; i++)
                diceList[i] = board[(int)upMostDie.gridPosition.x, (int)upMostDie.gridPosition.y - i].GetComponent<DiceHandler>();

            ResetVertical(diceList);

            Debug.Log("Vertical Straight 5 Made!");
            return;
        }

        if (CheckStraightSixVertical(die))
        {
            score += 250;
            UIManager.instance.SetScore(score);

            int up = die.value - 2;
            DiceHandler upMostDie = board[(int)die.gridPosition.x, (int)die.gridPosition.y + up].GetComponent<DiceHandler>();

            diceList = new DiceHandler[5];
            for (int i = 0; i < 5; i++)
                diceList[i] = board[(int)upMostDie.gridPosition.x, (int)upMostDie.gridPosition.y - i].GetComponent<DiceHandler>();

            ResetVertical(diceList);

            Debug.Log("Vertical Straight 6 Made!");
            return;
        }

        if (CheckThreeHorizontal(die, out diceList))
        {
            score += 100;
            UIManager.instance.SetScore(score);

            ResetHorizontal(diceList);

            Debug.Log("Match Three Horizontal!");
            return;
        }

        if (CheckThreeVertical(die, out diceList))
        {
            score += 100;
            UIManager.instance.SetScore(score);

            ResetVertical(diceList);

            Debug.Log("Match Three Vertical!");
            return;
        }
    }

    private void ResetHorizontal(DiceHandler[] diceList)
    {
        isSwapping = true;
        float delay = 0.0f;

        foreach (DiceHandler dieHandler in diceList)
        {
            dieHandler.isTumbling = true;

            int xPos = (int)dieHandler.gridPosition.x;
            int yPos = (int)dieHandler.gridPosition.y;

            Vector3 pos = dieHandler.transform.position;
            pos.y = resetHeight;
            dieHandler.transform.position = pos;

            for(int y = yPos + 1; y < height; y++)
            {
                Vector3 endPos = board[xPos, y].transform.position;
                endPos.y -= 1.1f;

                StartCoroutine(board[xPos, y].GetComponent<DiceHandler>().SwapPosition(board[xPos, y].transform.position, endPos));

                board[xPos, y - 1] = board[xPos, y];
                board[xPos, y - 1].GetComponent<DiceHandler>().gridPosition.y = y - 1;
            }

            dieHandler.value = Random.Range(1, 7);
            dieHandler.fallDelay = delay;
            
            pos.y = 3.8f;
            StartCoroutine(dieHandler.FallingRoutine(dieHandler.transform.position, pos));
            board[xPos, height - 1] = dieHandler.gameObject;
            board[xPos, height - 1].GetComponent<DiceHandler>().gridPosition = new Vector2(xPos, height - 1);

            delay += 0.5f;
        }
    }

    private void ResetVertical(DiceHandler[] diceList)
    {
        isSwapping = true;
        float delay = 0.5f;

        // Move cleared dice to the top
        for(int i = 0; i < diceList.Length; i++)
        {
            diceList[i].isTumbling = true;

            int xPos = (int)diceList[i].gridPosition.x;
            int yPos = (int)diceList[i].gridPosition.y;

            Vector3 pos = diceList[i].transform.position;
            pos.y = resetHeight + (yOffset * i);
            diceList[i].transform.position = pos;

            diceList[i].value = Random.Range(1, 7);
            diceList[i].fallDelay = delay * i;
        }

        // Slide other dice down
        DiceHandler topDie = diceList[0];

        if (topDie.gridPosition.y != height - 1)
        {
            for (int y = (int)(topDie.gridPosition.y + 1); y < height; y++)
            {
                DiceHandler die = board[(int)topDie.gridPosition.x, y].GetComponent<DiceHandler>();

                Vector3 endPos = die.transform.position;
                endPos.y -= 1.1f * diceList.Length;
                StartCoroutine(die.SwapPosition(die.transform.position, endPos));

                board[(int)die.gridPosition.x, y - diceList.Length] = die.gameObject;
                die.gridPosition = new Vector2(die.gridPosition.x, y - diceList.Length);
            }
        }

        // Drop cleared dice
        for (int i = 0; i < diceList.Length; i++)
        {
            Vector3 endPos = diceList[i].transform.position;
            endPos.y = 3.8f - (yOffset * (diceList.Length - i - 1));

            StartCoroutine(diceList[i].FallingRoutine(diceList[i].transform.position, endPos));

            int gridY = height - (diceList.Length - i);
            board[(int)diceList[i].gridPosition.x, gridY] = diceList[i].gameObject;
            diceList[i].gridPosition = new Vector2(diceList[i].gridPosition.x, gridY);
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

    private bool CheckThreeHorizontal(DiceHandler die, out DiceHandler[] diceList)
    {
        diceList = new DiceHandler[3];

        if (die.gridPosition.x - 2 >= 0)
        {
            DiceHandler twoLeft = board[(int)(die.gridPosition.x - 2), (int)die.gridPosition.y].GetComponent<DiceHandler>();
            DiceHandler oneLeft = board[(int)(die.gridPosition.x - 1), (int)die.gridPosition.y].GetComponent<DiceHandler>();

            if ((twoLeft.dieColor == die.dieColor && twoLeft.value == die.value) &&
                (oneLeft.dieColor == die.dieColor && oneLeft.value == die.value))
            {
                diceList[0] = twoLeft;
                diceList[1] = oneLeft;
                diceList[2] = die;
                
                return true;
            }
        }
        
        if(die.gridPosition.x - 1 >= 0 && die.gridPosition.x + 1 < width)
        {
            DiceHandler oneLeft = board[(int)(die.gridPosition.x - 1), (int)die.gridPosition.y].GetComponent<DiceHandler>();
            DiceHandler oneRight = board[(int)(die.gridPosition.x + 1), (int)die.gridPosition.y].GetComponent<DiceHandler>();

            if ((oneLeft.dieColor == die.dieColor && oneLeft.value == die.value) &&
                (oneRight.dieColor == die.dieColor && oneRight.value == die.value))
            {
                diceList[0] = oneLeft;
                diceList[1] = die;
                diceList[2] = oneRight;

                return true;
            }
        }

        if (die.gridPosition.x + 2 < width)
        {
            DiceHandler oneRight = board[(int)(die.gridPosition.x + 1), (int)die.gridPosition.y].GetComponent<DiceHandler>();
            DiceHandler twoRight = board[(int)(die.gridPosition.x + 2), (int)die.gridPosition.y].GetComponent<DiceHandler>();

            if ((oneRight.dieColor == die.dieColor && oneRight.value == die.value) &&
                (twoRight.dieColor == die.dieColor && twoRight.value == die.value))
            {
                diceList[0] = die;
                diceList[1] = oneRight;
                diceList[2] = twoRight;

                return true;
            }
        }

        return false;
    }

    private bool CheckThreeVertical(DiceHandler die, out DiceHandler[] diceList)
    {
        diceList = new DiceHandler[3];

        if (die.gridPosition.y - 2 >= 0)
        {
            DiceHandler twoDown = board[(int)die.gridPosition.x, (int)(die.gridPosition.y - 2)].GetComponent<DiceHandler>();
            DiceHandler oneDown = board[(int)die.gridPosition.x, (int)(die.gridPosition.y - 1)].GetComponent<DiceHandler>();

            if ((twoDown.dieColor == die.dieColor && twoDown.value == die.value) &&
                (oneDown.dieColor == die.dieColor && oneDown.value == die.value))
            {
                diceList[0] = die;
                diceList[1] = oneDown;
                diceList[2] = twoDown;

                return true;
            }
        }

        if (die.gridPosition.y - 1 >= 0 && die.gridPosition.y + 1 < height)
        {
            DiceHandler oneDown = board[(int)die.gridPosition.x, (int)(die.gridPosition.y - 1)].GetComponent<DiceHandler>();
            DiceHandler oneUp = board[(int)die.gridPosition.x, (int)(die.gridPosition.y + 1)].GetComponent<DiceHandler>();

            if ((oneDown.dieColor == die.dieColor && oneDown.value == die.value) &&
                (oneUp.dieColor == die.dieColor && oneUp.value == die.value))
            {
                diceList[0] = oneUp;
                diceList[1] = die;
                diceList[2] = oneDown;

                return true;
            }
        }

        if (die.gridPosition.y + 2 < height)
        {
            DiceHandler oneUp = board[(int)die.gridPosition.x, (int)(die.gridPosition.y + 1)].GetComponent<DiceHandler>();
            DiceHandler twoUp = board[(int)die.gridPosition.x, (int)(die.gridPosition.y + 2)].GetComponent<DiceHandler>();

            if ((oneUp.dieColor == die.dieColor && oneUp.value == die.value) &&
                (twoUp.dieColor == die.dieColor && twoUp.value == die.value))
            {
                diceList[0] = twoUp;
                diceList[1] = oneUp;
                diceList[2] = die;

                return true;
            }
        }

        return false;
    }

    #endregion
}
