using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerCamera : MonoBehaviour
{
    public float sensitivityX;
    public float sensitivityY;

    public Transform orientation;
    public Transform MainPlayer;

    float xRotation;
    float yRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // dùng để khóa con trỏ chuột vào giữa màn hình và làm cho nó vô hình
        
    }

    // Update is called once per frame
    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivityX;
        //trục X trái phải
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivityY;
        //trục Y lên xuống

        yRotation += mouseX; //đẩy camera trái/phải
        xRotation -= mouseY; //xoay camera lên/xuống
        //là một loại toán tử
        //dùng để tích luỹ chuyển động của con chuột vào biến xoay xRotation và yRotation

        xRotation = Mathf.Clamp(xRotation, -90f, 80f);
        //giới hạn góc quay camera chiều dọc (xRotation) giữa giá trị -90° và 90° để camera không quay quá lên trên hoặc xuống

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        //sử dụng trục quay con chuột trái phải (xRotation) lên xuống (yRotation) rồi chuyển các góc đó thành một Quaternion, để quay cho mượt
        //cái 0 để tránh camera không nghiêng sang một bên
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        MainPlayer.rotation = Quaternion.Euler(0, yRotation, 0);

    }
    //time.deltatime có tác dụng làm cho di chuyển không phụ thuộc vào tốc độ khung hình
    //ví dụ như nếu không có time.deltatime, thì hệ thống chạy 120 fps sẽ có chuyển động nhanh gấp đôi hệ thống chạy 60 fps
}
