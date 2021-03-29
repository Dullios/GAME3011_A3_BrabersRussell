using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum DiceColor
{
    RED,
    BLUE,
    GREEN,
    YELLOW,
    BLACK,
    WHITE
}

public class DiceHandler : MonoBehaviour
{
    [Header("Die Values")]
    public int value;
    public DiceColor dieColor;

    public bool isSelected;

    [Header("Grid Position")]
    public Vector2 gridPosition;

    [Header("Tumble Values")]
    public float fallDelay;
    public float fallSpeed;
    public bool isTumbling;

    public float swapSpeed;

    public Vector3 startPosition;
    private float lerpTime = 0.0f;

    private Dictionary<int, Vector3> faceRotation = new Dictionary<int, Vector3>();

    private Vector2 mousePosition;

    // Start is called before the first frame update
    void Start()
    {
        // Set Rotation Vector3 per side
        faceRotation[1] = new Vector3(0, 180, 0);
        faceRotation[2] = new Vector3(0, 90, 90);
        faceRotation[3] = new Vector3(90, 0, 90);
        faceRotation[4] = new Vector3(90, 0, -90);
        faceRotation[5] = new Vector3(-90, 0, 0);
        faceRotation[6] = new Vector3(0, 0, -90);
        
        startPosition = transform.position;
    }

    public IEnumerator FallingRoutine(Vector3 endPos)
    {
        yield return new WaitForSeconds(fallDelay);

        while (lerpTime < 1)
        {
            transform.Rotate(Random.Range(-5.0f, -15.0f), Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f));

            lerpTime += fallSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, endPos, lerpTime);

            if (lerpTime >= 1.0f)
            {
                isTumbling = false;
                AlignDie();
            }

            yield return null;
        }

        lerpTime = 0.0f;

        if (gridPosition.x == GameManager.instance.width - 1 && gridPosition.y == GameManager.instance.height - 1)
            GameManager.instance.isSwapping = false;
    }

    public void AlignDie()
    {
        Vector3 rot = faceRotation[value];
        transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
    }

    public void OnMousePosition(InputValue value)
    {
        mousePosition = value.Get<Vector2>();
    }

    public void OnSelect(InputValue value)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        Physics.Raycast(ray, out hit, 20.0f);
        Debug.DrawRay(ray.origin, ray.direction, Color.blue, 1.0f);

        if (hit.transform != null && hit.transform.CompareTag("Die") && hit.transform.gameObject == gameObject)
        {
            if (GameManager.instance.isSwapping)
                return;

            if (isSelected)
            {
                isSelected = false;
                GameManager.instance.selectedDie = null;
            }
            else
            {
                if (GameManager.instance.selectedDie == null)
                {
                    isSelected = true;
                    GameManager.instance.selectedDie = this;
                }
                else
                {
                    DiceHandler selected = GameManager.instance.selectedDie;

                    if((gridPosition.x >= selected.gridPosition.x - 1 && gridPosition.x <= selected.gridPosition.x + 1 && gridPosition.y == selected.gridPosition.y) ||
                        (gridPosition.y >= selected.gridPosition.y - 1 && gridPosition.y <= selected.gridPosition.y + 1 && gridPosition.x == selected.gridPosition.x))
                        GameManager.instance.SwapDice(gameObject);
                }
            }
        }
    }

    public IEnumerator SwapPosition(Vector3 endPos)
    {
        while (lerpTime < 1)
        {
            lerpTime += swapSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, endPos, lerpTime);

            yield return null;
        }

        lerpTime = 0.0f;
        isSelected = false;
        //GameManager.instance.isSwapping = false;
    }
}
