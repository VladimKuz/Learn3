using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    void Update()
    {
        // Тест 1: старый Input Manager
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("OLD INPUT: Space работает");
        }
        
        // Тест 2: новый Input System напрямую через клавиатуру
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("NEW INPUT (Keyboard.current): Space работает");
        }
    }
}