using System;
using UnityEngine;

// Token: 0x0200007D RID: 125
[AddComponentMenu("Dynamic Bone/Dynamic Bone Collider")]
public class DynamicBoneCollider : MonoBehaviour
{
    // Token: 0x040002C4 RID: 708
    public Vector3 m_Center = Vector3.zero;

    // Token: 0x040002C5 RID: 709
    public float m_Radius = 0.5f;

    // Token: 0x040002C6 RID: 710
    public float m_Height;

    // Token: 0x040002C7 RID: 711
    public DynamicBoneCollider.Direction m_Direction;

    // Token: 0x040002C8 RID: 712
    public DynamicBoneCollider.Bound m_Bound;

    // Token: 0x0200007E RID: 126
    public enum Direction
    {
        // Token: 0x040002CA RID: 714
        X,
        // Token: 0x040002CB RID: 715
        Y,
        // Token: 0x040002CC RID: 716
        Z
    }

    // Token: 0x0200007F RID: 127
    public enum Bound
    {
        // Token: 0x040002CE RID: 718
        Outside,
        // Token: 0x040002CF RID: 719
        Inside
    }
}
