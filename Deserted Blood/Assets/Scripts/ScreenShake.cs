using System.Xml;
using UnityEngine;

struct pos
{
    public Vector3 position;
    public Vector3 rotation;
}

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake instance;

    float duration;
    float timer;
    float amplitude;

    [SerializeField] float scale;
    float iterations = 0;

    [SerializeField] float moveSpeed = 1.0f;
    [SerializeField] AnimationCurve moveCurveX;
    [SerializeField] AnimationCurve moveCurveY;
    float t = 0;

    pos curPos;
    pos startPos;
    pos targetPos;

    Vector3 prevDir;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (duration <= 0)
            return;

        if (timer >= duration)
        {
            timer = 0;
            duration = 0;
            curPos.position = Vector3.zero;
            //curPos.rotation = Vector3.zero;
            transform.localPosition = Vector3.zero;
            //transform.rotation = Quaternion.identity;
        }
        else
        {
            timer += Time.deltaTime;
            if (curPos.position != targetPos.position)
            {
                MoveCam();
            }
            else
            {
                ++iterations;
                startPos = curPos;
                t = 0;
                SetTarget();
            }
        }
    }

    void MoveCam()
    {
        curPos.position.x = Mathf.Lerp(startPos.position.x, targetPos.position.x, moveCurveX.Evaluate(t));
        curPos.position.y = Mathf.Lerp(startPos.position.y, targetPos.position.y, moveCurveY.Evaluate(t));
        t += Time.deltaTime * moveSpeed;
        //curPos.rotation += Vector3.Lerp(curPos.rotation, targetPos.rotation, lerpSpeed * Time.deltaTime);
        transform.localPosition = curPos.position;
        //transform.eulerAngles = curPos.rotation;
    }

    void SetTarget()
    {
        if (iterations == 0)
        {
            prevDir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
            prevDir.Normalize();
            targetPos.position = prevDir * amplitude;
            return;
        }


        Vector3 newDir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0);
        newDir = -prevDir + newDir;
        newDir.Normalize();
        amplitude = Mathf.Lerp(amplitude, 0, timer / duration);
        targetPos.position = newDir * (amplitude);
    }

    public void ShakeScreen(float _duration, float _amplitude = 0.5f)
    {
        iterations = 0;
        amplitude = _amplitude;
        duration = _duration;
        startPos.position = Vector3.zero;
        startPos.rotation = Vector3.zero;
        SetTarget();
    }
}
