using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataItemHandler : MonoBehaviour {

  public GameObject triggerPrefab;
  private Animator animator;

	void Awake () {
    Debug.Log(gameObject.name + " is waking up ヽ( ´O｀)ゞ");

    try {
      DataItem[] data = GameObject.Find("Data Loader").GetComponent<DataLoader>().dataClass.items;
      DataItem item = System.Array.Find(data, o => gameObject.name.Equals(o.name));

      transform.localPosition = new Vector3(item.x, item.y, item.z);
      transform.localScale = new Vector3(item.scale, item.scale, 1);

      foreach (TriggerClass triggers in item.animData.triggers) {
        GameObject triggerObject = Instantiate(triggerPrefab, transform.parent);
        BoxCollider2D boxCollider2D = triggerObject.AddComponent<BoxCollider2D>();

        triggerObject.transform.parent = transform;
        triggerObject.gameObject.name = triggers.name;

        boxCollider2D.size = new Vector2(triggers.scaleX, triggers.scaleY);
        boxCollider2D.offset = new Vector2(triggers.offsetX, triggers.offsetY);

        triggerObject.GetComponent<TransitionClickHandler>().triggerName = triggers.name;
      }

      animator = gameObject.AddComponent<Animator>();
      gameObject.AddComponent<SpriteRenderer>();

      Debug.Log(gameObject.name + " is wide awake O.O");
    } catch (System.Exception ex) {
      Debug.LogWarning("Unable to find create animation object " + gameObject.name + " from data!");
      Debug.Log(ex);
    }
  }

  void Start() {
    animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animation Controllers/" + gameObject.name + "/" + gameObject.name);
    Debug.Log(gameObject.name + " animation controller loaded");
  }

}