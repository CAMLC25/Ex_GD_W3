using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;     // Tốc độ chạy của nhân vật.
    [SerializeField] private float jumpForce = 15f;    // Lực nhảy (độ cao khi nhảy).
    [SerializeField] private LayerMask groundLayer;    // Xác định layer nào được coi là mặt đất (để tránh lỗi nhảy kép khi đang ở trên không).
    [SerializeField] private Transform groundCheck;    // Vị trí dưới chân nhân vật dùng để vẽ một vòng tròn kiểm tra xem có đang chạm đất không.

    private Animator animator;                         // Biến lưu trữ component Animator để điều khiển hoạt ảnh (chạy, nhảy).
    private bool isGrounded;                           // Biến trạng thái: True nếu đang chạm đất, False nếu đang lơ lửng.
    private Rigidbody2D rb;                            // Biến lưu trữ component vật lý để áp dụng lực và vận tốc.

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleJump();
        UpdateAnimator();
    }

private void HandleMovement()
{
    // Lấy giá trị phím bấm trái/phải (-1 đến 1).
    float moveInput = Input.GetAxis("Horizontal"); 
    
    // Gán vận tốc mới cho động cơ vật lý: Trục X = hướng phím * tốc độ, Trục Y = giữ nguyên vận tốc rớt hiện tại[cite: 34].
    rb.linearVelocity = new Vector2(moveInput*moveSpeed, rb.linearVelocity.y);
    
    // Đảo chiều hình ảnh (Flip) bằng cách đổi Scale của Transform.
    if (moveInput > 0)
    {
        transform.localScale = new Vector3(1, 1, 1);   // Đi sang phải -> Giữ nguyên kích thước 
    }
    else if (moveInput < 0)
    {
        transform.localScale = new Vector3(-1, 1, 1);  // Đi sang trái -> Lật ngược ảnh theo trục X
    }
}

private void HandleJump()
{
    // Nếu bấm nút Jump (Thường là Spacebar) VÀ nhân vật đang đứng trên mặt đất (isGrounded == true)
    if (Input.GetButtonDown("Jump") && isGrounded)
    {
        // Giữ nguyên gia tốc ngang (X) và bơm một lực jumpForce vào trục thẳng đứng (Y) để ném nhân vật lên.
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }
    
    // Liên tục vẽ một vòng tròn tàng hình bán kính 0.2f tại vị trí groundCheck (dưới chân).
    // Nếu vòng tròn này chạm vào bất kỳ vật thể nào thuộc groundLayer, isGrounded sẽ = true. Ngược lại là false.
    isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
}

private void UpdateAnimator()
{
    // isRunning sẽ = true nếu vận tốc di chuyển ngang lớn hơn 0.1 (nghĩa là nhân vật đang nhúc nhích). Dùng giá trị tuyệt đối (Mathf.Abs) để đúng với cả hướng trái và phải.
    bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
    
    // isJumping sẽ = true nếu nhân vật KHÔNG chạm đất.
    bool isJumping = !isGrounded;
    
    // Gửi tín hiệu SetBool (tắt/bật) vào Animator Controller để nó tự động chuyển đổi giữa các trạng thái animation như Idle, Run, Jump.
    animator.SetBool("isRunning", isRunning);
    animator.SetBool("isJumping", isJumping);
}
}
