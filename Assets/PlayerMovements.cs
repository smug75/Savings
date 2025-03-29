using UnityEngine; 
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerMovements : MonoBehaviour
{
    public Transform orientation;
    public float MoveSpeed;
    //Có thể được chỉnh sửa trong Unity Inspector.
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
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInputs();
        UpdateAnimations();
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
    }
    private void PlayerMove()
    {
        MoveDirections = orientation.forward * VerticalInput + orientation.right * HorizontalInput;
        rb.AddForce(MoveDirections.normalized * MoveSpeed * 10f, ForceMode.Force);
    }
    private void UpdateAnimations()
    {
        animate.SetFloat("MoveLeftRight", HorizontalInput);
        animate.SetFloat("MoveUpDown", VerticalInput);
    }
    // .normalized dùng để ngăn cản chuyển động nhanh hơn khi di chuyển theo đường chéo (diagonally)
    // người chơi di chuyển theo hướng tương đối với vật thể định hướng (orientation object)
    // nếu định hướng (orientation) bị quay, chuyển động sẽ được tự động điều chỉnh 
}
