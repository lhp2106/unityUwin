using UnityEngine;

public class VKDonotDestroy : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.transform);
    }
}