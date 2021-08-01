using UnityEngine;

public class HitWater : MonoBehaviour
{
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            transform.position = new Vector3(-2.68f, 1f, 1.89f);
        }
    }
}
