using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class FileHandler {

    public static void SaveToJSON<T> (List<T> toSave, string filepath, string filename) {
        Debug.Log (GetPath (filepath, filename));
        string content = JsonHelper.ToJson<T> (toSave.ToArray ());
        WriteFile (GetPath (filepath, filename), content);
    }

    public static void SaveToJSON<T> (T toSave,string filepath, string filename) {
        string content = JsonUtility.ToJson (toSave);
        WriteFile (GetPath (filepath, filename), content);
    }

    public static List<T> ReadListFromJSON<T> (string filepath, string filename) {
        string content = ReadFile (GetPath (filepath, filename));

        if (string.IsNullOrEmpty (content) || content == "{}") {
            return new List<T> ();
        }

        List<T> res = JsonHelper.FromJson<T> (content).ToList ();

        return res;

    }

    public static T ReadFromJSON<T> (string filepath, string filename) {
        string content = ReadFile (GetPath (filepath, filename));

        if (string.IsNullOrEmpty (content) || content == "{}") {
            return default (T);
        }

        T res = JsonUtility.FromJson<T> (content);

        return res;

    }

    public static string GetPath (string filepath, string filename) {
        filepath = Path.Combine(Application.persistentDataPath, filepath);
        if(!Directory.Exists(filepath)){
            Directory.CreateDirectory(filepath);
        }
        return Path.Combine(filepath, filename);
    }

    private static void WriteFile (string path, string content) {
        FileStream fileStream = new FileStream (path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter (fileStream)) {
            writer.Write (content);

            writer.Close();
        }
        fileStream.Close();
    }

    private static string ReadFile (string path) {
        if (File.Exists (path)) {
            using (StreamReader reader = new StreamReader (path)) {
                string content = reader.ReadToEnd ();
                return content;
            }
        }
        return "";
    }
}

public static class JsonHelper {
    public static T[] FromJson<T> (string json) {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>> (json);
        return wrapper.Items;
    }

    public static string ToJson<T> (T[] array) {
        Wrapper<T> wrapper = new Wrapper<T> ();
        wrapper.Items = array;
        return JsonUtility.ToJson (wrapper);
    }

    public static string ToJson<T> (T[] array, bool prettyPrint) {
        Wrapper<T> wrapper = new Wrapper<T> ();
        wrapper.Items = array;
        return JsonUtility.ToJson (wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T> {
        public T[] Items;
    }
}