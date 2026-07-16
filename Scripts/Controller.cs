using UnityEngine;

public class Controller : MonoBehaviour
{
    public float sens;
    public float speed;
    public Transform playerMain;
    public Transform cam;
    public Animator camAnim;
    public Animator empContAnim;

    private Rigidbody rb;
    private float xRotation;
    private float yRotation;

    private ControllerMode controllerMode = ControllerMode.Moving;
    private bool moving = true;
    private bool mouseControll = true;
    private bool mobileCam = false;

    private void Start()
    {
        rb = transform.GetComponent<Rigidbody>();

        if (Options.pc)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        MouseController();
    }

    private void FixedUpdate()
    {
        MoveController();
    }

    public void TouchDown()
    {
        mobileCam = true;
    }

    public void TouchUp()
    {
        mobileCam = false;
    }

    private void MouseController()
    {
        if (!mouseControll)
            return;

        if (Options.pc)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sens;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sens;

            SetRotation(mouseX, mouseY);
        }
        else if (mobileCam)
        {
            Vector2 touchDelta = CameraControllerPanel.instance.GetTouchDeltaPosition() * Time.deltaTime * (-sens * 0.02f);

            SetRotation(touchDelta.x, touchDelta.y);
        }

        void SetRotation(float mouse_x, float mouse_y)
        {
            yRotation += mouse_x;
            xRotation -= mouse_y;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cam.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            playerMain.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }

    private void MoveController()
    {
        if (!moving)
            return;

        float horInput, vertInput;
        if (Options.pc)
        {
            horInput = Input.GetAxisRaw("Horizontal");
            vertInput = Input.GetAxisRaw("Vertical");
        }
        else
        {
            horInput = GuiManager.instance.joystick.Horizontal;
            vertInput = GuiManager.instance.joystick.Vertical;
        }

        if (horInput == 0 && vertInput == 0)
        {
            camAnim.SetBool("Walk", false);
            empContAnim.SetBool("Walk", false);
        }
        else
        {
            camAnim.SetBool("Walk", true);
            empContAnim.SetBool("Walk", true);
        }

        Vector3 moveDirection = playerMain.forward * vertInput + playerMain.right * horInput;
        rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);

        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    public void SetControllerMode(ControllerMode _controllerMode)
    {
        controllerMode = _controllerMode;
        if (_controllerMode == ControllerMode.Moving)
        {
            if (Options.pc)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            moving = true;
            mouseControll = true;
        }
        else if (_controllerMode == ControllerMode.CursorControll)
        {
            camAnim.SetBool("Walk", false);
            empContAnim.SetBool("Walk", false);
            if (Options.pc)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            moving = false;
            mouseControll = false;
        }
        else
        {
            camAnim.SetBool("Walk", false);
            empContAnim.SetBool("Walk", false);
            if (Options.pc)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            moving = false;
            mouseControll = false;
        }
    }

    public void CancelForces()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public enum ControllerMode
    {
        Moving,
        CursorControll,
        Freeze
    }
}
