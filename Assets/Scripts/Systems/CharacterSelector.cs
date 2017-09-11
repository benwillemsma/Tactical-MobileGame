using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    static int selectionOptions;
    static int selection = 0;

    private List<Transform> choices = new List<Transform>();
    public Text Name;

    private void Awake()
    {
        foreach (Transform child in transform)
            choices.Add(child);
        selectionOptions = choices.Count;
    }

    void Start()
    {
        if (selectionOptions > 0)
        { 
            for (int i = 0; i < selectionOptions; i++)
            {
                choices[i].localPosition = transform.rotation * Quaternion.AngleAxis((360 / selectionOptions * i + selection), Vector3.up) * (transform.forward * selectionOptions);
                choices[i].rotation = Quaternion.LookRotation(-choices[i].localPosition, Vector3.up);
            }
        }
        Name.text = choices[selection].name;
    }

    public void NextCharacter(int Delta)
    {
        selection = (selection + Delta) % selectionOptions;
        if (selection < 0)
            selection += selectionOptions;

        if (selectionOptions > 0)
            transform.Rotate(Vector3.up * 360 / selectionOptions * -Delta);
        Name.text = choices[selection].name;
    }
}
