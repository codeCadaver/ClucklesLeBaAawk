using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Suicide : MonoBehaviour
{
    [SerializeField] private float _delay = 1f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, _delay);
    }
}
