using Mirror;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
public class PlayerController : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private float turnSmoothVelocity;
    private Transform cam;

    [Header("UI")]
    [SerializeField] private GameObject nameCanvasPrefab;
    private TMP_Text nameText;

    [Header("Animation")]
    private Animator animator;

    // --- Network variables --- 
    [SyncVar(hook = nameof(OnNameChanged))] private string playerName = "Player";
    [SyncVar] private bool isRunning;

    #region Unity Lifecycle
    public override void OnStartLocalPlayer()
    {
        // Installing a nickname
        string inputName = PlayerPrefs.GetString("PlayerName", "");
        if (string.IsNullOrEmpty(inputName))
            inputName = "Player_" + Random.Range(1000, 9999);

        CmdSetName(inputName);

        // Setting up the camera
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            ThirdPersonCamera camScript = mainCam.GetComponent<ThirdPersonCamera>();
            if (camScript == null)
                camScript = mainCam.gameObject.AddComponent<ThirdPersonCamera>();

            camScript.target = transform;
        }

        cam = Camera.main.transform;
    }

    private void Start()
    {
        // UI overhead
        if (nameCanvasPrefab != null)
        {
            GameObject nameCanvas = Instantiate(nameCanvasPrefab, transform);
            nameCanvas.transform.localPosition = new Vector3(0, 2f, 0);
            nameText = nameCanvas.GetComponentInChildren<TMP_Text>();
            nameText.text = playerName;
        }

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            // Update animations for other players
            if (animator != null)
                animator.SetBool("Run", isRunning);
            return;
        }

        HandleMovement();
        HandleMessage();
        HandleSpawn();
    }
    #endregion

    #region Movement
    private void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            Vector3 moveDir = GetCameraRelativeDirection(h, v);

            // Player's Turn
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref turnSmoothVelocity,
                0.1f
            );
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Movement
            transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
        }

        bool moving = inputDir.magnitude >= 0.1f;
        if (isRunning != moving)
            CmdSetRunning(moving);

        animator.SetBool("Run", moving);
    }

    private Vector3 GetCameraRelativeDirection(float h, float v)
    {
        if (cam == null) cam = Camera.main.transform;

        Vector3 camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
        return (camForward * v + cam.right * h).normalized;
    }
    #endregion

    #region Input Handlers
    private void HandleMessage()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            CmdSendMessage();
    }

    private void HandleSpawn()
    {
        if (Input.GetKeyDown(KeyCode.F))
            GetComponent<ObjectSpawner>()?.TrySpawnCube();
    }
    #endregion

    #region SyncVar Hooks
    private void OnNameChanged(string oldName, string newName)
    {
        if (nameText != null)
            nameText.text = newName;
    }
    #endregion

    #region Commands & RPC
    [Command] private void CmdSetRunning(bool value) => isRunning = value;
    [Command] private void CmdSetName(string newName) => playerName = newName;
    [Command] private void CmdSendMessage() => RpcReceiveMessage($"Привет от {playerName}");
    [ClientRpc] private void RpcReceiveMessage(string msg) => Debug.Log(msg);
    #endregion
}
