using UnityEngine;

public class BeardGuyScript : MonoBehaviour
{
    Animator animator;
    bool walking = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) {
            walking = !walking;
            animator.SetBool("walking", walking);
        }
    }
}
