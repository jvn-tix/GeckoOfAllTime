using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTargetController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float maxOffset = 3f;

    private void Update()
    {
        if (player == null) return;

        Vector2 offset = Vector2.zero;

        if (Mouse.current != null)
        {
            Vector2 halfScreen = new Vector2(Screen.width, Screen.height) * 0.5f;
            Vector2 fromCenter = Mouse.current.position.ReadValue() - halfScreen;

            Vector2 normalized = new Vector2(fromCenter.x / halfScreen.x, fromCenter.y / halfScreen.y);
            normalized = Vector2.ClampMagnitude(normalized, 1f);

            offset = normalized * maxOffset;
        }

        transform.position = new Vector3(
            player.position.x + offset.x,
            player.position.y + offset.y,
            transform.position.z);
    }
}