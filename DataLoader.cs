using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataLoader : MonoBehaviour {

  public string DATA_FILE = "data.json";
  public DataClass dataClass = new DataClass();

  void Awake() {
    string filePath = Path.Combine(Application.streamingAssetsPath, "Animation Data/" + DATA_FILE);
    if (File.Exists(filePath)) {
      string dataAsJson = File.ReadAllText(filePath);
      dataClass = JsonUtility.FromJson<DataClass>(dataAsJson);
      Debug.Log("Found JSON (♥_♥)");
    } else {
      Debug.LogError("beep beep no json!");
    }
  }
}

[System.Serializable]
public class DataClass {
  public DataItem[] items;
}

[System.Serializable]
public class DataItem {
  public string name;
  public float x;
  public float y;
  public float z;
  public float scale;
  public AnimData animData;
}

[System.Serializable]
public class AnimData {
  public AnimClass[] animations;
  public TriggerClass[] triggers;
  public TransitionClass[] transitions;
}

[System.Serializable]
public class AnimClass {
  public string name;
  public float fps;
  public bool isLoop;
  public AnimEventClass[] children;
}

[System.Serializable]
public class AnimEventClass {
  public string name;
  public float time;
}

[System.Serializable]
public class TriggerClass {
  public float offsetX;
  public float offsetY;
  //change to sizeX + Y
  public float scaleX;
  public float scaleY;
  public string name;
}

[System.Serializable]
public class TransitionClass {
  public string source;
  public string target;
  public bool hasExitTime;
  public float duration = 0;
  public float exitTime = 0;
  public string[] conditions;
}