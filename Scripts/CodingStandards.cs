using System.Collections;
using UnityEngine;

// When fully finished with a script remove any unnecessary libraries
// This can be done by removing the grayed/greyed out using cases
// or use ctrl + r + g to remove any libraies that arent being use in the script
using System.Collections.Generic;

public class CodingStandards : MonoBehaviour
{
    // Use headers to keep info clear and concise 
    [Header("References")]
    [SerializeField] private GameObject _tempGameObject = null; // Fully explain varible like this

    [Header("Varibles")]
    private bool _tempBool1 = false; // Varible names will start with a underscore followed by a lowercase. Any word after that will be capitlized
    public bool tempBool2 = false; // This applies to all varibles public, protect and private

    // After the varibles we will have getters/setters
    public bool GetTempBool() { return _tempBool1; }
    public bool TempBool { get => _tempBool1; private set { } }
    public GameObject GetTempGameObject() { return _tempGameObject; }

    // After the getters/setters we will have the MonoBehaviour methods like start, update, awake,....
    private void Start()
    {
        bool check = false;
        if (_tempBool1 == check)
        {
            // When Starting a Coroutine dont use a "string" but rather the function name 
            // In case you change the name of the IEnumerator it will throw an error / allows for fast access to the IEnumerator
            StartCoroutine(TempEnumerator()); 
        }
    }

    // After the MonoBehaviour methods then we will have our methods
    private IEnumerator TempEnumerator() // Even though everything is private initially make sure to state it anyway in your functions
    {
        yield return new WaitForSeconds(1.0f);
    }

    // GitHub Push
    // Summary example - [Will - Branch] created player controller // We start with branch we are pushing into then giving a summary title
    // Description example - Explain specfically what you did in further detail in bullet points
    /*
        - Added PlayerController Script
        - Created Player prefab
    */
}
