using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Visual Gembok / Kunci")]
    public GameObject lockIcon; // Jika kosong, otomatis mengambil GameObject ini sendiri jika dipasang langsung di kunci_0

    [Header("Status Pintu")]
    public bool isUnlocked = false;

    void Awake()
    {
        EnsureLockIconAssigned();
        LockDoor();
    }

    public void EnsureLockIconAssigned()
    {
        if (lockIcon == null)
        {
            // Jika script ini dipasang langsung di objek "kunci_0", jadikan dirinya sendiri sebagai visual gembok
            if (gameObject.name.ToLower().Contains("kunci") || gameObject.name.ToLower().Contains("gembok") || gameObject.name.ToLower().Contains("lock"))
            {
                lockIcon = gameObject;
            }
            else
            {
                Transform found = transform.Find("kunci_0");
                if (found == null && GameObject.Find("kunci_0") != null) found = GameObject.Find("kunci_0").transform;
                if (found != null) lockIcon = found.gameObject;
            }
        }
    }

    // Mengunci pintu dan memunculkan visual gambar gembok/kunci
    public void LockDoor()
    {
        isUnlocked = false;
        EnsureLockIconAssigned();
        if (lockIcon != null) lockIcon.SetActive(true);
    }

    // Membuka pintu dan menghilangkan visual gambar gembok/kunci
    public void UnlockDoor()
    {
        isUnlocked = true;
        EnsureLockIconAssigned();
        if (lockIcon != null) lockIcon.SetActive(false);
        Debug.Log("Kode berhasil dipecahkan! Gambar gembok pada pintu telah menghilang.");
    }
}
