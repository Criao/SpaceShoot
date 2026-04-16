using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogBase : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    public virtual void Open()//virtual，子类可以重写这个方法
    {
        this.gameObject.SetActive(true);
    }
    public void Close()
    {
        if (animator != null)
        {
            animator.SetTrigger("Close");
        }
        StartCoroutine(HideDialog());

    }

    private IEnumerator HideDialog()
    {
        yield return new WaitForSecondsRealtime(0.4f);
        this.gameObject.SetActive(false);
    }
}
