using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// RadarObject class is used to store information about each object that will appear on the radar.
public class RadarObject
{
    // Image for the icon representing the radar object.
    public Image icon { get; set; }

    // The actual game object represented by the radar icon.
    public GameObject owner { get; set; }
}


public class Radar : MonoBehaviour
{
    // The player's position in the game world.
    public Transform playerPos;

    // Scaling factor for the radar display to control how much of the game world is shown.
    public float mapScale = 2.0f;

    // List of all radar objects in the game.
    public static List<RadarObject> radObjects = new List<RadarObject>();

    // Register a new radar object by providing the game object and its icon.
    public static void RegisterRadarObject(GameObject o, Image i)
    {
        // Instantiate the icon.
        Image image = Instantiate(i);

        // Add the radar object to the list of radar objects.
        radObjects.Add(new RadarObject() { owner = o, icon = image });
    }

    // Remove a radar object by providing the game object.
    public static void RemoveRadarObject(GameObject o)
    {
        // Create a new list to store radar objects that will not be removed.
        List<RadarObject> newList = new List<RadarObject>();
        for (int i = 0; i < radObjects.Count; i++)
        {
             // If the radar object's owner is the object to be removed, destroy its icon and skip to the next object.
            if (radObjects[i].owner == o)
            {
                Destroy(radObjects[i].icon);
                continue;
            }
            else
                newList.Add(radObjects[i]);
        }

        // Clear the original list and add the remaining objects.
        radObjects.RemoveRange(0, radObjects.Count);
        radObjects.AddRange(newList);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // If the player's position is not set, return.
        if (playerPos == null) return;

        // Iterate through all radar objects.
        foreach (RadarObject ro in radObjects)
        {
            // Calculate the position of the radar object relative to the player.
            Vector3 radarPos = ro.owner.transform.position - playerPos.position;
            // Calculate the distance from the player to the radar object, scaled by the mapScale.
            float distToObject = Vector3.Distance(playerPos.position, ro.owner.transform.position) * mapScale;

            // Calculate the angle between the player's forward direction and the radar object.
            float deltay = Mathf.Atan2(radarPos.x, radarPos.z) * Mathf.Rad2Deg - 270 - playerPos.eulerAngles.y;
            // Calculate the x and z positions of the radar icon.
            radarPos.x = distToObject * Mathf.Cos(deltay * Mathf.Deg2Rad) * -1;
            radarPos.z = distToObject * Mathf.Sin(deltay * Mathf.Deg2Rad);

            // Set the radar icon's parent transform to the radar's transform.
            ro.icon.transform.SetParent(this.transform);
            // Get the RectTransform component of the radar.
            RectTransform rt = this.GetComponent<RectTransform>();
            // Set the position of the radar icon based on the calculated radar position and the RectTransform's pivot.
            ro.icon.transform.position = new Vector3(radarPos.x + rt.pivot.x, radarPos.z + rt.pivot.y, 0)
                    + this.transform.position;
        }
    }
}
