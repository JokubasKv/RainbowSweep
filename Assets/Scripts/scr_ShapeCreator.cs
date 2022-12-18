using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_ShapeCreator : MonoBehaviour
{
    [System.Serializable]
    public class Spawnables 
    {
        public Spawnables(string shape, GameObject spawnbles, Color color) 
        {
            spawnables = spawnbles;
            onWhatShape = shape;
            spawnColor = color;
        }

        public GameObject spawnables;
        public string onWhatShape;
        public Color spawnColor;
    }

    public List<Spawnables> spawnables=new List<Spawnables>();

    public void SpawnObject(string objectName, List<LineRenderer> lines)
    {
        bool found=false;
        foreach(var spawn in spawnables)
        {
            Debug.Log("|"+spawn.onWhatShape + "| |" + objectName+"|");
            if (spawn.onWhatShape == objectName)
            {

                List<Vector3> allPoints = new List<Vector3>();
                foreach (var line in lines)
                {
                    Vector3[] positions = new Vector3[line.positionCount];
                    line.GetPositions(positions);
                    allPoints.AddRange(positions);
                }
                Vector3 middlePoint = GetMiddlePoint(allPoints);


                Instantiate(spawn.spawnables, middlePoint, Quaternion.identity);

                found = true;

                foreach (LineRenderer line in lines)
                {
                    line.startColor = spawn.spawnColor;
                    line.endColor = spawn.spawnColor;
                    StartCoroutine(LineDisapear(line, 3f));
                }
                return;
            }
        }
        if (!found)
        {
            Debug.Log(objectName + " | Failed to find corespondig shape name");

            foreach (LineRenderer line in lines)
            {
                //StartCoroutine(LineDisapear(line, 3f));
            }
        }
    }

    private IEnumerator LineDisapear(LineRenderer line, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float width = Mathf.Lerp(line.startWidth, 0, time / duration);
            line.startWidth = width;
            line.endWidth = width;
            yield return null;
        }
        Destroy(line.gameObject);
    }

    Vector3 GetMiddlePoint(List<Vector3> points)
    {
        var totalX = 0f;
        var totalY = 0f;
        var totalZ = 0f;
        foreach (var point in points)
        {
            totalX += point.x;
            totalY += point.y;
            totalZ += point.z;
        }
        var centerX = totalX / points.Count;
        var centerY = totalY / points.Count;
        var centerZ = totalZ / points.Count;

        return new Vector3(centerX,centerY,centerZ);
    }
}
