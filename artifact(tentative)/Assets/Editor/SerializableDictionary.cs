
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(LevelUpDictionary))]
[CustomPropertyDrawer(typeof(ItemDictionary))]
[CustomPropertyDrawer(typeof(SEDictionary))]

public class SerializableDictionary : SerializableDictionaryPropertyDrawer
{

}