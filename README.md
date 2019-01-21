# Cont-animation
### Externalize GUI to follow data patterns from a JSON file, allowing non-devs to make content changes

## The Scripts
**SceneLoader.cs**

This script serializes the JSON into an object that other objects can grab values from. Each key:value pair corresponds with fields in a class. Nested key:value objects are then serialized into their own class. For example a JSON like this:

```json
"stringKey":"hello",
"intKey": 5,
"objectKey":{
  "floatKey" : 0.2,
  "boolKey":false
}
```
will have the following class structure:

```c#
[System.Serializable]
public class JsonContainer{
  public string stringKey;
  public int intKey;
  public NestedObject objectKey;
}

[System.Serializable]
public class Nested{
  public float floatKey;
  public bool boolKey;
}
```

PLEASE NOTE THAT THE KEYS AND VARIABLE NAMES ARE THE SAME!!!!! Additionally item key/var must match corresponding folder name. This is also true for animation states key/var.

**AnimationImporter.cs** *editor only*

*Unity Animation Setup*
```json
.
└── GameObject
    ├── Animator Component
    │   └── Controller
    │       └── Layer
    │           └── StateMachine
    │               ├── AnimationStates
    │               │   ├── AnimatorClip
    │               │   │   ├── AnimationEvents
    │               │   │   ├── AnimationCurveBindings //we target SpriteRender here
    │               │   │   │   └── ObjectReferenceKeyframes //(could be child of something else)
    │               │   │   └── AnimationClipSettings
    │               │   └── Transitions
    │               └── Parameters
    └── Sprite Renderer
```
This script loops through the array of items from Scene Loader and creates & applies the proper animation structure. For every item a controller is created at the item's root folder. Then it creates an animator state at the 1st layer with a clip. each clip has a curve binding with every sprite as a keyframe. Every clip is created at the root of each animation state folder.  It also applies settings and animation events(with the spawn function) to the clip. 

Finally it creates parameters to the controller, as well as transitions between states with appropriate parameter requirements.

All of these elements are written to the disk. If there any changes the script must be enabled to apply updates. And re-disabled for builds.

**DataHandler.cs**

This script is attached to the generic DataItem. It finds which object it is from the `JSON/DataClass` from its name and applies the correct transform, animator, sprite renderer to the object. Additionally, it creates children `TriggerObjects` and set its name to a corresponding parameter in the `.controller`

It also applies Animator Controller on `Start()`

### The Resources Folder

All items must have thier own root folder under `Resources`. For every animation sequence there must be folder with that states name under the item folder. In the animation state folder there should be the sequence of PNGs for that state:
```json
.
└── Resources
    ├── item_1
    │   ├── anim_1
    │   │   └── {seq. of pngs}
    │   ├── anim_2
    │   │   └── {seq. of pngs}
    │   └── anim_3
    │   │   └── {seq. of pngs}
    └── item_2
        ├── anim_1
        │   └── {seq. of pngs}
        ├── anim_2
        │   └── {seq. of pngs}
        └── anim_3
            └── {seq. of pngs}
```

While activated, the `AnimationImporter.cs` will then create the Animator Clips(`.anim`) inside each animation state folder, and the Animator Contoller(`.controller`) at the item root folder at runtime.

Additionally there is the `prefab` folder. Inside there is the generic item object, `DataItem.prefab` with `DataHandler.cs` atttached, as well as the generic trigger/mouse hit detection object, `TriggerObject.prefab` with `ClickScript.cs` atttached. Lastly there is `BallSprite.prefab`, a stand-in object to trigger instantiation via the attached `Spawn.cs`

