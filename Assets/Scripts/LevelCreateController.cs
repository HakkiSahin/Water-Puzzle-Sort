using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Levels")]
public class LevelCreateController : ScriptableObject
{


    public List<LevelProperty> bottles;
   

    [System.Serializable]
    public class LevelProperty
    {
        public List<Color> colors;      
        public int numberBottle;

    }
    public int winBottleCount;




}
