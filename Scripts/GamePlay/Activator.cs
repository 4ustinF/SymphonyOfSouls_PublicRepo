using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour
{
    [SerializeField] private List<GameObject> _objects = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < _objects.Count; i++)
        {
            _objects[i].SetActive(false);
        }
    }

    public void Activate()
    {
        for (int i = 0; i < _objects.Count; i++)
        {
            _objects[i].SetActive(true);
        }
    }
}
