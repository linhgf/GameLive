using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSense : MonoBehaviour
{
    // Start is called before the first frame update
    //单例
    public static AttackSense instance;

    private bool isShake;

    public static AttackSense GetInstance(){
        if(instance == null)
            instance = GameObject.FindObjectOfType<AttackSense>();
        return instance;
    }

    /// <summary>
    /// 攻击命中顿帧
    /// </summary>
    /// <param name="duration">顿帧持续时间</param>
    public void HitPause(float duration){
        StartCoroutine(Pause(duration));
    }

    IEnumerator Pause(float duration){
        float time = duration / 60f;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;
    }

    /// <summary>
    /// 攻击命中震屏
    /// </summary>
    /// <param name="duration">震屏持续时间</param>
    /// <param name="strength">震屏强度</param>
    public void ShakeCamera(float duration, float strength){
        if(!isShake)
            StartCoroutine(Shake(duration, strength));
    }

    IEnumerator Shake(float duration, float strength){

        isShake = true;
        var origin_position = Camera.main.transform.position;

        while(duration > 0){
            duration -= Time.deltaTime;
            Camera.main.transform.position += (Random.insideUnitSphere * strength);
            yield return null;
        }
        Camera.main.transform.position = origin_position;
        isShake = false;

    }

}
