using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct OffCode
{
    public int id;
    public string strNotice;
}

public class CodeReponseError : MonoBehaviour
{
    public static CodeReponseError instance;

    public List<OffCode> listCodeBug = new List<OffCode>();
    

    private void Start()
    {
        instance = this;
    }
}
