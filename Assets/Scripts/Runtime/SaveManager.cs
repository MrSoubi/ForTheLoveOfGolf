using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class PlayerData
{
    public int coins;
    public int holes;
    public Vector3 positionSpawn;
    public List<bool> coinsObject;
    public List<bool> holesObject;
    public List<bool> doors;
    public List<bool> pannels;
    public List<bool> challenges;
}

[System.Serializable]
public class SaveManager
{
    private static readonly string EncryptionKey = "ajekoBnPxI9jGbnYCOyvE9alNy9mM/Kw";

    public static int coins = 0;
    public static int holes = 0;
    public static Vector3 positionSpawn = new Vector3(0,0,0);
    public static List<bool> coinsObject = new List<bool>();
    public static List<bool> holesObject = new List<bool>();
    public static List<bool> doors = new List<bool>();
    public static List<bool> pannels = new List<bool>();
    public static List<bool> challenges = new List<bool>();

    private static string Encrypt(string plainText, string key)
    {
        byte[] iv;
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.GenerateIV();
            iv = aes.IV;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                    {
                        streamWriter.Write(plainText);
                    }

                    array = memoryStream.ToArray();
                }
            }
        }

        byte[] combinedArray = new byte[iv.Length + array.Length];
        Array.Copy(iv, 0, combinedArray, 0, iv.Length);
        Array.Copy(array, 0, combinedArray, iv.Length, array.Length);

        return Convert.ToBase64String(combinedArray);
    }

    private static string Decrypt(string cipherText, string key)
    {
        byte[] buffer = Convert.FromBase64String(cipherText);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);

            byte[] iv = new byte[aes.BlockSize / 8];
            byte[] cipherArray = new byte[buffer.Length - iv.Length];

            Array.Copy(buffer, iv, iv.Length);
            Array.Copy(buffer, iv.Length, cipherArray, 0, cipherArray.Length);

            aes.IV = iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream(cipherArray))
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
    }

    public static void SaveToFile()
    {
        PlayerData data = new PlayerData
        {
            coins = coins,
            holes = holes,
            positionSpawn = positionSpawn,
            coinsObject = coinsObject,
            holesObject = holesObject,
            doors = doors,
            pannels = pannels,
            challenges = challenges,
        };

        string playerDataJson = JsonUtility.ToJson(data);
        string encryptedJson = Encrypt(playerDataJson, EncryptionKey);

        string filePath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Save", "Save.json");

        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(filePath, playerDataJson);
    }

    private static bool ValidateData(PlayerData data)
    {
        if (data.coins < 0) return false;
        if (data.holes < 0) return false;
        if (data.positionSpawn == null) return false;
        if (data.coinsObject == null) return false;
        if (data.holesObject == null) return false;
        if (data.doors == null) return false;
        if (data.pannels == null) return false;
        if (data.challenges == null) return false;

        return true;
    }

    public static bool LoadFile()
    {
        try
        {
            string filePath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Save", "Save.json");

            if (File.Exists(filePath))
            {
                string encryptedJson = File.ReadAllText(filePath);
                string decryptedJson = Decrypt(encryptedJson, EncryptionKey);
                PlayerData data = JsonUtility.FromJson<PlayerData>(decryptedJson);

                if (ValidateData(data))
                {
                    coins = data.coins;
                    holes = data.holes;
                    positionSpawn = data.positionSpawn;
                    coinsObject = data.coinsObject;
                    holesObject = data.holesObject;
                    doors = data.doors;
                    pannels = data.pannels;
                    challenges = data.challenges;

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        catch (System.Exception)
        {
            return false;
        }
    }
}
