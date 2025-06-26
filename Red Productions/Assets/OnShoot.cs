using UnityEngine;

public class OnShoot : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TomatoLauncher launcher = animator.gameObject.GetComponentInParent<TomatoLauncher>(); // of jouw specifieke script
        Debug.Log(launcher);

        if (launcher != null)
        {
            launcher.DisableShooting();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // Wordt aangeroepen wanneer de animatie stopt
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TomatoLauncher launcher = animator.gameObject.GetComponentInParent<TomatoLauncher>(); // of jouw specifieke script
        Debug.Log(launcher);
        if (launcher != null)
        {
            launcher.EnableShooting();
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
