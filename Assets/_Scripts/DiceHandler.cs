using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField]
    private int value;
    public int Value => value;
    
    [SerializeField]
    private DiceColor dieColor;
    public DiceColor DieColor => dieColor;

    [Header("Tumble Values")]
    public float fallDelay;
    public float fallSpeed;
    public bool isTumbling;

    public Vector3 startPosition;
    private float lerpTime = 0.0f;

    private Dictionary<int, Vector3> faceRotation = new Dictionary<int, Vector3>();

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

        value = Random.Range(1, 7);

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
    }

    public void AlignDie()
    {
        Vector3 rot = faceRotation[value];
        transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
    }
}
