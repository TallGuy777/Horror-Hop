using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagazineManager : MonoBehaviour
{
    public int MagSize { get; private set; } // Số viên đạn hiện tại
    public int MaxMagSize = 4; // Số viên đạn tối đa

    private void Start()
    {
        MagSize = 1; // Khởi tạo số viên đạn ban đầu
    }

    // Thêm một viên đạn vào khẩu súng
    public void AddBullets(int amount)
    {
        if (MagSize < MaxMagSize)
        {
            MagSize += amount;
            if (MagSize > MaxMagSize)
            {
                MagSize = MaxMagSize;
            }
        }
    }

    // Kiểm tra xem còn viên đạn trong khẩu súng hay không
    public bool HasBullets()
    {
        return MagSize > 0;
    }

    // Bắn mất một viên đạn từ khẩu súng
    public void ConsumeBullet()
    {
        if (MagSize > 0)
        {
            MagSize--;
        }
    }

    // Reset số đạn trong khẩu súng
    public void ResetMagazine()
    {
        MagSize = 0;
    }
}
