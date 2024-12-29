using System;
using CMF;
using UnityEngine;

// Todo: Make camera bob while moving
public class ControllerBounce : MonoBehaviour
{
    [SerializeField] private Controller controller;
    public float offset;
    public Vector2 range;
    public int frequency;
    public bool direction = true;
    public Vector3 offsetVector;

    private void Awake() {
        frequency = (int)range.x;
    }

    private void Update() {
        
        offset = Mathf.Sin((frequency * Time.deltaTime));
        direction = frequency <= range.x ? false : frequency >= range.y ? true : direction;
        frequency += direction ? 1 : -1;
        offsetVector = transform.localPosition;
        offsetVector.y = offset;
        
    }

    private void FixedUpdate() {
        transform.position = Vector3.Lerp(transform.position, transform.position+offsetVector, 5 * Time.deltaTime);
    }
}
