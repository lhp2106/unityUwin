using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinWheel : MonoBehaviour
{
    public SpinWheelController controller;
    private float anglePerItem;
    //    public int anim;

    [Space(20)]
    [Header("Config")]
    public int around; 
    public bool isClock; 
    public float angleTo; 
    public float time; 
    public int pieces;
    public List<AnimationCurve> animationCurves;
    void Start()
    {
        anglePerItem = 360 / pieces;

        if (!isClock)
            around = -around;
    }

    public void SpinStart(int itemNumber)
    {
        float maxAngle = 360 * around - (itemNumber * anglePerItem);
        StartCoroutine(SpinLuckyWheel(time, maxAngle + angleTo));
    }

    IEnumerator SpinLuckyWheel(float time, float maxAngle)
    {
        float timer = 0f;
        float startAngle = transform.eulerAngles.z;
        maxAngle = -maxAngle + startAngle;

        while (timer < time)
        {
            float angle = maxAngle * animationCurves[0].Evaluate(timer / time);
            transform.eulerAngles = new Vector3(0.0f, 0.0f, (angle - startAngle));
            timer += Time.unscaledDeltaTime;
            yield return 0;
        }

        controller.UpdateComplete();
    }
}