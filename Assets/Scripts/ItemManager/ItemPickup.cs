using System.Collections.Generic;
using UnityEngine;

public class PickupThrow : MonoBehaviour
{
    // Private fields
    private GameObject heldItem;            // Vật phẩm đang được nhặt
    private Rigidbody heldItemRb;           // Rigidbody của vật phẩm đang được nhặt
    private bool originalCCDState;          // Trạng thái ban đầu của Collision Detection Mode (CCD)
    private TagPrefabMapping currentTagPrefabMapping; // Lưu trữ thông tin tag và prefab hiện tại

    // Public fields
    public float throwForce = 10f;          // Lực ném
    public KeyCode pickUpKey = KeyCode.E;   // Phím để nhặt vật phẩm
    public KeyCode throwKey = KeyCode.F;    // Phím để ném vật phẩm
    public Transform handPosition;          // Vị trí để giữ vật phẩm (HandPlayer)
    public List<TagPrefabMapping> tagPrefabMappings;  // Danh sách các tag và prefab tương ứng
    [System.Serializable]
    public struct TagPrefabMapping
    {
        public string tag;           // Tag của vật phẩm
        public GameObject prefab;    // Prefab tương ứng khi nhặt
        public GameObject prefabModel; // Prefab tương ứng khi ném
    }

    // Triggered when entering a collider
    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu có va chạm với vật phẩm có tag trong danh sách và chưa có vật phẩm nào đang được nhặt
        if (heldItem == null && IsTagInList(other.gameObject.tag))
        {
            Debug.Log("Nhấn " + pickUpKey.ToString() + " để nhặt vật phẩm.");
        }
    }

    // Triggered while staying in a collider
    private void OnTriggerStay(Collider other)
    {
        // Nhặt vật phẩm khi nhấn phím E và chưa có vật phẩm trong tay
        if (Input.GetKeyDown(pickUpKey) && heldItem == null && IsTagInList(other.gameObject.tag))
        {
            PickUpItem(other.gameObject);
        }
    }

    // Called every frame
    private void Update()
    {
        // Ném vật phẩm khi nhấn phím F và có vật phẩm trong tay
        if (Input.GetKeyDown(throwKey) && heldItem != null)
        {
            ThrowItem();
        }
    }

    // Function to pick up an item
    private void PickUpItem(GameObject item)
    {
        // Lấy Rigidbody của đối tượng được nhặt
        heldItemRb = item.GetComponent<Rigidbody>();
        if (heldItemRb == null)
        {
            Debug.LogWarning("Không tìm thấy Rigidbody gắn với vật phẩm: " + item.name);
            return; // Không nhặt đối tượng nếu không có Rigidbody
        }

        // Lưu trạng thái ban đầu của CCD
        originalCCDState = heldItemRb.collisionDetectionMode == CollisionDetectionMode.Continuous;

        // Tắt CCD
        heldItemRb.collisionDetectionMode = CollisionDetectionMode.Discrete;

        // Nhặt vật phẩm
        heldItem = item;

        // Biến vật phẩm thành kinematic để ném
        heldItemRb.isKinematic = true;

        // Đặt vật phẩm vào vị trí của tay (handPosition)
        heldItem.transform.position = handPosition.position;
        heldItem.transform.rotation = handPosition.rotation;
        heldItem.transform.parent = handPosition; // Đặt vật phẩm là con của handPosition

        // Kích hoạt prefab tương ứng nếu có
        if (TryGetTagPrefabMapping(heldItem.tag, out currentTagPrefabMapping))
        {
            if (currentTagPrefabMapping.prefab != null)
            {
                GameObject newObject = Instantiate(currentTagPrefabMapping.prefab, handPosition.position, handPosition.rotation);
                newObject.transform.parent = handPosition;
                Destroy(heldItem); // Hủy vật phẩm gốc sau khi nhặt
                heldItem = newObject;
                heldItemRb = heldItem.GetComponent<Rigidbody>();
                heldItemRb.isKinematic = true;

                Debug.Log("Đã nhặt vật phẩm: " + heldItem.name);
            }
            else
            {
                Debug.LogError("Prefab không tồn tại cho tag: " + heldItem.tag);
            }
        }
        else
        {
            Debug.LogError("Không tìm thấy prefab cho tag: " + heldItem.tag);
        }
    }

    // Function to throw the held item
    private void ThrowItem()
    {
        // Tạo vật phẩm mới từ prefabModel nếu có
        if (currentTagPrefabMapping.prefabModel != null)
        {
            GameObject thrownItem = Instantiate(currentTagPrefabMapping.prefabModel, heldItem.transform.position, heldItem.transform.rotation);
            Rigidbody thrownItemRb = thrownItem.GetComponent<Rigidbody>();
            if (thrownItemRb != null)
            {
                thrownItemRb.isKinematic = false;
                thrownItemRb.collisionDetectionMode = originalCCDState ? CollisionDetectionMode.Continuous : CollisionDetectionMode.Discrete;
                thrownItemRb.AddForce(transform.forward * throwForce, ForceMode.Impulse);

                Debug.Log("Đã ném vật phẩm: " + thrownItem.name);
            }
            else
            {
                Debug.LogError("Không tìm thấy Rigidbody gắn với prefabModel: " + thrownItem.name);
            }
        }
        else
        {
            Debug.LogError("PrefabModel không tồn tại cho tag: " + heldItem.tag);
        }

        // Hủy vật phẩm đang giữ
        Destroy(heldItem);

        // Xóa trạng thái nhặt vật phẩm
        heldItem = null;
        heldItemRb = null;
    }

    // Function to check if a tag exists in the list of tagPrefabMappings
    private bool IsTagInList(string tag)
    {
        foreach (var mapping in tagPrefabMappings)
        {
            if (mapping.tag == tag)
                return true;
        }
        return false;
    }

    // Function to get prefab associated with a tag
    private bool TryGetTagPrefabMapping(string tag, out TagPrefabMapping mapping)
    {
        foreach (var map in tagPrefabMappings)
        {
            if (map.tag == tag)
            {
                mapping = map;
                return true;
            }
        }
        mapping = new TagPrefabMapping();
        return false;
    }
}
