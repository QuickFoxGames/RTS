using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempScript : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) animator.SetBool("HasWeapon", !animator.GetBool("HasWeapon"));
        animator.SetFloat("X", Input.GetAxis("Horizontal") * (Input.GetKey(KeyCode.LeftShift) ? 2f : 1f));
        animator.SetFloat("Z", Input.GetAxis("Vertical") * (Input.GetKey(KeyCode.LeftShift) ? 2f : 1f));
    }
}