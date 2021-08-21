using GestureRecognizer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinerObject_MountainBikeMinigame1 : MonoBehaviour
{
    public List<GesturePatternDraw> line = new List<GesturePatternDraw>();

    public void Sleep()
    {
        int j = 0;
        while (!line[j].IsActive())
        {
            j++;
        }
        line[j].gameObject.SetActive(false);
        line.RemoveAt(0);
    }

    public void Open()
    {
        int j = 0;
        while (line[j].IsActive())
        {
            j++;
        }
        line[j].gameObject.SetActive(true);
    }
}
