using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Component Inputs")]
    public GoatControls goatControls;
    public Transform camPivot, orientation, model;
    public Rigidbody rb;

    [Header("Variables")]
    public int herbCount = 0;
    public Vector2 sensitivity = new Vector2(15f, 12f); // x is Left/Right, y is Up/Down. THIS DOES NOT CORRELATE TO CAM ROTATION THOUGH
    public float lookLimitUp = -40f, lookLimitDown = 10f;
    public float rotX, rotY, rotLerpX, rotLerpY;
    public Vector3 moveDir, modelRotLerp;
    private Vector2 kbInputs;
    public float speedDefault = 1f, speedMax = 4f;

    void OnEnable()
    {
        if (goatControls == null) goatControls = new GoatControls();
        goatControls.Enable();
    }

    void OnDisable()
    {
        goatControls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        orientation = GameObject.Find("Orientation").transform;
        model = GameObject.Find("PlayerModel").transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Mouse Inputs to code
        float mouseLR = goatControls.Defaults.CaMovement.ReadValue<Vector2>().x * Time.deltaTime * sensitivity.x;
        float mouseUD = goatControls.Defaults.CaMovement.ReadValue<Vector2>().y * Time.deltaTime * sensitivity.y;

        rotY += mouseLR;
        rotX -= mouseUD;
        rotX = Mathf.Clamp(rotX, lookLimitUp, lookLimitDown);
        MoveCamBasedOnInputs();

        // Keyboard Inputs to Code
        kbInputs = goatControls.Defaults.Movement.ReadValue<Vector2>();
    }

	private void FixedUpdate()
    {
        MovePlayerBasedOnInputs();
    }

    void MovePlayerBasedOnInputs()
    {
        // Calculate Movement Direction
        moveDir = (orientation.forward * kbInputs.y) + (orientation.right * kbInputs.x);
        moveDir.y = 0f;
        rb.AddForce(moveDir.normalized * speedDefault * 10f, ForceMode.Force);

        // Rotate model based on movement direction
        if (moveDir != Vector3.zero)
        {

            modelRotLerp = Vector3.Lerp(modelRotLerp, moveDir.normalized, Time.deltaTime * 10f);
            model.transform.forward = modelRotLerp;
        }
    }

	void MoveCamBasedOnInputs()
	{
        // Lerps for smoother camera movement
        if (Mathf.Abs(rotLerpX - rotX) <= 0.08f)
            rotLerpX = rotX;
        else
            rotLerpX = Mathf.Lerp(rotLerpX, rotX, Time.deltaTime * 5f);

        if (Mathf.Abs(rotLerpY - rotY) <= 0.08f)
            rotLerpY = rotY;
        else
            rotLerpY = Mathf.Lerp(rotLerpY, rotY, Time.deltaTime * 5f);

        // The actual camera rotating parts
        camPivot.transform.localRotation = Quaternion.Euler(rotLerpX, rotLerpY, 0f);

        // Set orientation based on camera's current position
        orientation.rotation = Quaternion.Euler(0, camPivot.rotation.eulerAngles.y, 0);
    }
}
