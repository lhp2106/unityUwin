using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VKFileHelper
{
    public static string LoadTextFromFile(string fileName)
    {
        VKDebug.Log("Load Text File From : " + fileName);
        try
        {
            FileStream file = File.OpenRead(GetPath(fileName));
            StreamReader rd = new StreamReader(file);
            string data = rd.ReadToEnd();
            rd.Close();
            file.Close();
            return data;
        }
        catch (Exception ex)
        {
            VKDebug.Log("Error when read text file from " + GetPath(fileName) + " with exception : " + ex.Message);
            return null;
        }
    }

    public static void WriteBinaryToFile(string fileName, byte[] newData)
    {
        VKDebug.Log("Write Binary File To : " + fileName);
        FileStream file = File.Create(GetPath(fileName));
        BinaryWriter bw = new BinaryWriter(file);
        bw.Write(newData);
        bw.Close();
        file.Close();
    }

    public static void WriteBinaryToPath(string path, byte[] newData)
    {
        //Create the Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }

        try
        {
            File.WriteAllBytes(path, newData);
            Debug.Log("Saved Data to: " + path.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To Save Data to: " + path.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }
    }

    public static void WriteTextToFile(string fileName, string newData)
    {
        FileStream file = File.Create(GetPath(fileName));
        StreamWriter bw = new StreamWriter(file);
        bw.Write(newData);
        bw.Close();
        file.Close();
    }

    public static void DeleteFile(string fileName)
    {
        VKDebug.Log("Delete File : " + fileName);
        File.Delete(GetPath(fileName));
    }

    public static bool CheckPathExists(string path)
    {
        return Directory.Exists(Path.GetDirectoryName(path));
    }

    public static bool CheckFileExists(string fileName)
    {
        return File.Exists(GetPath(fileName));
    }

    public static string GetPath(string fileName)
    {
        VKDebug.LogColorRed(Path.Combine(Application.persistentDataPath, fileName));
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    public static void SaveTextureToFile(Texture2D texture, string filename)
    {
        string path = GetPath(filename);

        try
        {
            if (filename.Contains(".png"))
                File.WriteAllBytes(path, texture.EncodeToPNG());
            else if (filename.Contains(".jpg"))
                File.WriteAllBytes(path, texture.EncodeToJPG());

            VKDebug.Log("Saved Data to: " + path.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            VKDebug.LogWarning("Failed To Save Data to: " + path.Replace("/", "\\"));
            VKDebug.LogWarning("Error: " + e.Message);
        }
    }


    public static Texture2D LoadTexture(string filename)
    {
        Texture2D tex = null;
        byte[] fileData;

        string path = GetPath(filename);
        if (File.Exists(path))
        {
            fileData = File.ReadAllBytes(path);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
        }
        return tex;
    }
}
