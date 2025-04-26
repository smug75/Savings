using UnityEngine; 
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public class PlayerMovements : MonoBehaviour
{
    public Transform orientation;

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

    float HorizontalInput;
    float VerticalInput;
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
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }
    //FixedUpdate được chạy theo khoảng thời gian cố định, không phụ thuộc vào tốc độ khung hình (frame rate)
    //Thường liên quan đến vật lí (physics)

    private void PlayerInputs()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        VerticalInput = Input.GetAxisRaw("Vertical");
        //sự khác biệt giữa GetAxis và GetAxisRaw là giữa độ mượt, GetAxis dần dần chuyển đổi giá trị, còn
        //GetAxisRaw là chuyển giá trị ngay lập tức, giữa -1 và 1 chẳng hạn
        if(Input.GetKey(JumpKey) && ReadyToJump && grounded)
        {
            ReadyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), JumpCooldown);
        }
    }

    private void PlayerMove()
    {
        MoveDirections = orientation.forward * VerticalInput + orientation.right * HorizontalInput;

        if (grounded)
            rb.AddForce(MoveDirections.normalized * MoveSpeed * 10f, ForceMode.Force);
        else if(!grounded)
            rb.AddForce(MoveDirections.normalized * MoveSpeed * 10f * AirMultiplier, ForceMode.Force);

    //.normalized là một dòng code liên quan đến vector(3) mà trong đó hướng vector không thay đổi,
    //nhưng độ dài (magnitude) của nó luôn tổng cộng chính xác về bằng 1
    //.normalized dùng để ngăn cản chuyển động nhanh hơn khi di chuyển theo đường chéo (diagonally)
    //người chơi di chuyển theo hướng tương đối với vật thể định hướng (orientation object)
    //nếu định hướng (orientation) bị quay, chuyển động sẽ được tự động điều chỉnh 
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

    private void ResetJump()
    {
        ReadyToJump = true;
    }
}
