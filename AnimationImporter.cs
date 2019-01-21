using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class AnimationImporter : MonoBehaviour {

  public DataLoader dataLoader;

  private void Start() {

    Debug.Log("Importing Animations   ⊂( ･∀･) 彡　=͟͟͞͞ ⌧");

    EditorCurveBinding spriteBinding = new EditorCurveBinding {
      type = typeof(SpriteRenderer),
      path = "",
      propertyName = "m_Sprite"
    };

    DataItem[] jsonItems = dataLoader.dataClass.items;

    if (!AssetDatabase.IsValidFolder("Assets/Resources/Animation Controllers")) {
      AssetDatabase.CreateFolder("Assets/Resources", "Animation Controllers");
    }

    foreach (DataItem item in jsonItems) {
      if (!AssetDatabase.IsValidFolder("Assets/Resources/Animation Controllers/" + item.name)) {
        AssetDatabase.CreateFolder("Assets/Resources/Animation Controllers", item.name);
      }

      AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Resources/Animation Controllers/" + item.name + "/" + item.name + ".controller");
      AnimatorStateMachine root = controller.layers[0].stateMachine;
      TransitionClass[] transitions = item.animData.transitions;
      AnimClass[] animations = item.animData.animations;
      TriggerClass[] triggers = item.animData.triggers;

      foreach (AnimClass anim in animations) {

        Sprite[] sprites = Resources.LoadAll<Sprite>("Animation Assets/" + item.name + "/" + anim.name);
        ObjectReferenceKeyframe[] spriteKeyFrames = sprites.Select( (sprite,index) => ( 
          new ObjectReferenceKeyframe {
            time = index / anim.fps,
            value = sprite
          }
        )).ToArray();

        AnimationClip clip = new AnimationClip() {
          name = anim.name
        };

        if (anim.isLoop) {
          AnimationClip tempClip = new AnimationClip();
          AnimationClipSettings newSettings = AnimationUtility.GetAnimationClipSettings(tempClip);
          newSettings.loopTime = true;
          AnimationUtility.SetAnimationClipSettings(clip, newSettings);
        }

        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyFrames);
        if (anim.children!=null) {
          AnimationEvent[] events = anim.children.Select( (e,index) => (
            new AnimationEvent() {
              stringParameter = e.name,
              time =  e.time,
              functionName = "spawn"
            }
          )).ToArray();

          AnimationUtility.SetAnimationEvents(clip, events);
        }

        //allows animations to build properly
        AssetDatabase.CreateAsset(clip, "Assets/Resources/Animation Controllers/" + item.name + "/" + clip.name + ".anim");
        controller.AddMotion(clip);
      }

      foreach (TriggerClass trig in triggers) {
        controller.AddParameter(trig.name,AnimatorControllerParameterType.Trigger);
      }

      foreach (TransitionClass t in transitions) {
        AnimatorCondition[] conditionsArray = t.conditions.Select( (con,index) => (
          new AnimatorCondition() {
            mode=AnimatorConditionMode.If,
            parameter = con,
            threshold = 0
          }
        )).ToArray();

        AnimatorState origin = System.Array.Find(root.states, element => t.source.Equals(element.state.name)).state;   
        AnimatorState target = System.Array.Find(root.states, element => t.target.Equals(element.state.name)).state;  

        AnimatorStateTransition transition = new AnimatorStateTransition() {
          hasExitTime = t.hasExitTime,
          destinationState = target,
          conditions = conditionsArray
        };
        origin.AddTransition(transition);
      }
    }
    Debug.Log("Animations Imported! ♪┏(・o･)┛♪┗ ( ･o･) ┓");

  }
  
}
