using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotate : MonoBehaviour
{
    [SerializeField] private Vector2 _rotateSpeed;
    private Vector2 RotateSpeed => _rotateSpeed;
    
    [SerializeField] private float _rotateXAngle;
    private float RotateXAngle => _rotateXAngle;

    // Update is called once per frame
    private void Update()
    {
        transform.rotation = Quaternion.Euler(Mathf.Sin(Time.realtimeSinceStartup * _rotateSpeed.x) * RotateXAngle, Time.realtimeSinceStartup * _rotateSpeed.y, 0f);
    }
}
