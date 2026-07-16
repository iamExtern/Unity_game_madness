using UnityEngine;

public class DebugController : MonoBehaviour
{
    public Transform cam;
    public Transform camCont;

    public float sens;
    public float speed;

    private float xRotation;
    private float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        MouseController();
        MoveController();
    }

    private void MouseController()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sens;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        camCont.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void MoveController()
    {
        float front = Input.GetKey(KeyCode.W) ? 1f : 0f;
        float back = Input.GetKey(KeyCode.S) ? -1f : 0f;
        float right = Input.GetKey(KeyCode.D) ? 1f : 0f;
        float left = Input.GetKey(KeyCode.A) ? -1f : 0f;
        float up = Input.GetKey(KeyCode.Space) ? 1f : 0f;
        float down = Input.GetKey(KeyCode.LeftShift) ? -1f : 0f;

        float topDownInput = up + down;
        float vertInput = front + back;
        float horInput = right + left;

        Vector3 moveDirection = camCont.forward * vertInput + camCont.right * horInput;
        moveDirection.y += topDownInput;

        camCont.transform.position += moveDirection.normalized * speed * Time.deltaTime;
    }
}
