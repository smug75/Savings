using UnityEngine; 
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public class PlayerMovements : MonoBehaviour
{
    public Transform orientation;
    //định hướng

    [Header("Movement")]
    public float MoveSpeed;

    public float GroundDrag;

    public float JumpForce;
    public float JumpCooldown;
    public float AirMultiplier;
    bool ReadyToJump;

    [Header("Keybinds")]
    public KeyCode JumpKey = KeyCode.Space;

    [Header("GroundCheck")]
    public float PlayerHeight;
    public LayerMask WhatIsGround;
    bool grounded;

    float HorizontalInput; //ngang
    float VerticalInput; //đứng
    Vector3 MoveDirections;

    Rigidbody rb;

    Animator animate;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb=GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        //freeRotation dùng để cố định nhân vật khi mà có collision để khỏi bị ngã
        animate=GetComponent<Animator>();
        ReadyToJump = true;
    }

    // Update is called once per frame
    private void Update()
    {
        PlayerInputs();
        UpdateAnimations();
        MovespeedControl();

        grounded = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, WhatIsGround);
        if (grounded)
            rb.linearDamping = GroundDrag;
        else
            rb.linearDamping = 0;
        //sử dụng một cái tia(raycast) hướng xuống(down) để check thử nếu player đang chạm vào đất (grounded), nếu chạm đất thì áp dụng một lực ma sát để giảm tốc độ, tránh cho object nó trượt
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }
    //FixedUpdate được chạy theo khoảng thời gian cố định, không phụ thuộc vào tốc độ khung hình (frame rate)
    //FixedUpdate chạy mỗi 0.02 giây (50 lần mỗi giây)
    //Thường liên quan đến vật lí (physics)
    //ví dụ nếu dùng update thì nếu mà game bị lag thì sẽ khiến cho playermove bị lag

    private void PlayerInputs()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");
        //sự khác biệt giữa GetAxis và GetAxisRaw là giữa độ mượt, GetAxis dần dần chuyển đổi giá trị, còn
        //GetAxisRaw là chuyển giá trị ngay lập tức, giữa -1 và 1 chẳng hạn
        //trong ví dụ này thì lấy input trái phải lên xuống ngay lập tức thay vì để cho input đổi từ từ
        if(Input.GetKey(JumpKey) && ReadyToJump && grounded)
        {
            ReadyToJump = false;

            Jump();

            Invoke("ResetJump", JumpCooldown);
        }
        //kiểm tra nếu input jumpkey (cái nút nhảy) đã chuẩn bị để nhảy (readytojump) và nhân vật đang ở dưới đất (grounded)
        //đặt cái readytojump về false
        //sử dụng invoke để tự động gọi resetjump sau một khoảng thời gian jumpcooldown
        //resetjump chuyển readytojump thành true
    }

    private void PlayerMove()
    {
        MoveDirections = orientation.forward * VerticalInput + orientation.right * HorizontalInput;

        if (grounded)
            rb.AddForce(MoveDirections.normalized * MoveSpeed * 10f, ForceMode.Force);
        else if(!grounded)
            rb.AddForce(MoveDirections.normalized * MoveSpeed * 10f * AirMultiplier, ForceMode.Force);

    //hướng di chuyển (Move Directions) được tính tuỳ theo các input nào mà có thể tác động đến hướng nhìn hiện tại của định hướng (orientation), có tác dụng làm cho chuyển động tương ứng
    //với hướng quay hiện tại của người chơi
    //.normalized dùng để ngăn cản chuyển động nhanh hơn khi di chuyển theo đường chéo (diagonally)
    //nếu player đang ở trên mặt đất (grounded) thì thêm lực di chuyển theo hướng vector (movedirections là vector3) tuỳ theo cái input ở trên
    //nếu player không ở trên mặt đất (!grounded) thì thêm lực di chuyển theo hướng vector nhân thêm cái airMultiplier để kiểm soát khả năng bẻ lái khi người chơi đang ở trên không
    }

    private void UpdateAnimations()
    {
        animate.SetFloat("MoveLeftRight", HorizontalInput);
        animate.SetFloat("MoveUpDown", VerticalInput);
    }

    private void MovespeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        //tạo một vector3 có tên là flatvel chỉ lấy giá trị x và z (horizontal), và chỉnh y=0 (vertical)
        //rb.linearVelocity là tốc độ đang chạy của rigidbody
        if(flatVel.magnitude > MoveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * MoveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
        //.magnitude là một dòng code liên quan đến vector(3) cho biết độ dài (magnitude) của một vector
        //ví dụ như có một mũi tên đang chỉ ra một hướng, thì magnitude là độ dài của mũi tên đó
        //trong dòng code này, thì magnitude đang cho biết một vật đang di chuyển với tốc độ thế nào
        //rb.limitedvelocity là một vector3 biểu thị tốc độ và hướng hiện tại của rigidbody đang di
        //chuyển, được tính theo giây (cái cũ là rb.velocity)
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * JumpForce, ForceMode.Impulse);
    }

    //line này áp dụng một lực nhảy (rb.addforce) hướng lên (transform.up) lên trục Y một cách ngay tức thì (forcemode.impulse)
    //vận tốc trục Y được reset về 0 trước khi áp dụng lực nhảy để tránh cho sức mạnh nhảy của nhân vật khỏi bị tác dụng từ tốc độ đang rơi xuống hoặc bay lên



    private void ResetJump()
    {
        ReadyToJump = true;
    }
}
