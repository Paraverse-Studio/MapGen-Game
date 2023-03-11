using Paraverse.Mob.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FloatFloatEvent : UnityEvent<float, float>
{
}

[System.Serializable]
public class IntIntEvent : UnityEvent<int, int>
{
}

[System.Serializable]
public class StringEvent : UnityEvent<string>
{
}

[System.Serializable]
public class BoolEvent : UnityEvent<bool>
{
}

[System.Serializable]
public class GameObjectEvent : UnityEvent<GameObject>
{
}

[System.Serializable]
public class TransformEvent : UnityEvent<Transform>
{
}

[System.Serializable]
public class SelectableEvent : UnityEvent<Selectable>
{
}

[System.Serializable]
public class MobControllersListEvent : UnityEvent<List<MobController>>
{
}

[System.Serializable]
public class ItemCardEvent : UnityEvent<ItemCard>
{
}


public class CustomUnityEvents : MonoBehaviour
{

}
