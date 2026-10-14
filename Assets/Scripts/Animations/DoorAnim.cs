using UnityEngine;

public class DoorAnim : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Open()
    {
        anim.SetBool("IsOpen", true);
    }

    public void Close()
    {
        anim.SetBool("IsOpen", false);
    }
}
