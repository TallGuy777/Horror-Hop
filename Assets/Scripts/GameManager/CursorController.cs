using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorController : MonoBehaviour
{
    private bool isCursorVisible = false;

    void Start()
    {
        // Ẩn con trỏ chuột khi bắt đầu game
        SetCursorVisibility(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene("GamePlay");
        }
        // Kiểm tra nếu phím 'I' được nhấn
        if (Input.GetKeyDown(KeyCode.I))
        {
            // Đổi trạng thái hiển thị của con trỏ chuột
            isCursorVisible = !isCursorVisible;
            SetCursorVisibility(isCursorVisible);
        }
    }

    void SetCursorVisibility(bool isVisible)
    {
        Cursor.visible = isVisible;
        if (isVisible)
        {
            Cursor.lockState = CursorLockMode.None; // Hiển thị con trỏ chuột và cho phép di chuyển
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // Ẩn con trỏ chuột và khóa vị trí của nó ở giữa màn hình
        }
    }
}
