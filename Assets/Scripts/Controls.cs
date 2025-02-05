using UnityEngine;
using UnityEngine.InputSystem;

public class Controls : MonoBehaviour
{
    [SerializeField] private InputAction controls;

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();


    private void Update()
    {
       
    }
}

