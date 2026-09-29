using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000079 RID: 121
[AddComponentMenu("Dynamic Bone/Dynamic Bone")]
public class DynamicBone : MonoBehaviour
{
    // Token: 0x0400028D RID: 653
    public Transform m_Root;

    // Token: 0x0400028E RID: 654
    public float m_UpdateRate = 60f;

    // Token: 0x0400028F RID: 655
    public DynamicBone.UpdateMode m_UpdateMode;

    // Token: 0x04000290 RID: 656
    [Range(0f, 1f)]
    public float m_Damping = 0.1f;

    // Token: 0x04000291 RID: 657
    public AnimationCurve m_DampingDistrib;

    // Token: 0x04000292 RID: 658
    [Range(0f, 1f)]
    public float m_Elasticity = 0.1f;

    // Token: 0x04000293 RID: 659
    public AnimationCurve m_ElasticityDistrib;

    // Token: 0x04000294 RID: 660
    [Range(0f, 1f)]
    public float m_Stiffness = 0.1f;

    // Token: 0x04000295 RID: 661
    public AnimationCurve m_StiffnessDistrib;

    // Token: 0x04000296 RID: 662
    [Range(0f, 1f)]
    public float m_Inert;

    // Token: 0x04000297 RID: 663
    public AnimationCurve m_InertDistrib;

    // Token: 0x04000298 RID: 664
    public float m_Radius;

    // Token: 0x04000299 RID: 665
    public AnimationCurve m_RadiusDistrib;

    // Token: 0x0400029A RID: 666
    public float m_EndLength;

    // Token: 0x0400029B RID: 667
    public Vector3 m_EndOffset = Vector3.zero;

    // Token: 0x0400029C RID: 668
    public Vector3 m_Gravity = Vector3.zero;

    // Token: 0x0400029D RID: 669
    public Vector3 m_Force = Vector3.zero;

    // Token: 0x0400029E RID: 670
    public List<DynamicBoneCollider> m_Colliders;

    // Token: 0x0400029F RID: 671
    public List<Transform> m_Exclusions;

    // Token: 0x040002A0 RID: 672
    public DynamicBone.FreezeAxis m_FreezeAxis;

    // Token: 0x040002A1 RID: 673
    public bool m_DistantDisable;

    // Token: 0x040002A2 RID: 674
    public Transform m_ReferenceObject;

    // Token: 0x040002A3 RID: 675
    public float m_DistanceToObject = 20f;

    // Token: 0x040002A4 RID: 676
    [Tooltip("Check this if you want the bone to be dynamic even on low performance HW")]
    public bool neverOptimize;

    // Token: 0x0200007A RID: 122
    public enum UpdateMode
    {
        // Token: 0x040002AF RID: 687
        Normal,
        // Token: 0x040002B0 RID: 688
        AnimatePhysics,
        // Token: 0x040002B1 RID: 689
        UnscaledTime
    }

    // Token: 0x0200007B RID: 123
    public enum FreezeAxis
    {
        // Token: 0x040002B3 RID: 691
        None,
        // Token: 0x040002B4 RID: 692
        X,
        // Token: 0x040002B5 RID: 693
        Y,
        // Token: 0x040002B6 RID: 694
        Z
    }

    // Token: 0x0200007C RID: 124
    private class Particle
    {
        // Token: 0x040002B7 RID: 695
        public Transform m_Transform;

        // Token: 0x040002B8 RID: 696
        public int m_ParentIndex = -1;

        // Token: 0x040002B9 RID: 697
        public float m_Damping;

        // Token: 0x040002BA RID: 698
        public float m_Elasticity;

        // Token: 0x040002BB RID: 699
        public float m_Stiffness;

        // Token: 0x040002BC RID: 700
        public float m_Inert;

        // Token: 0x040002BD RID: 701
        public float m_Radius;

        // Token: 0x040002BE RID: 702
        public float m_BoneLength;

        // Token: 0x040002BF RID: 703
        public Vector3 m_Position = Vector3.zero;

        // Token: 0x040002C0 RID: 704
        public Vector3 m_PrevPosition = Vector3.zero;

        // Token: 0x040002C1 RID: 705
        public Vector3 m_EndOffset = Vector3.zero;

        // Token: 0x040002C2 RID: 706
        public Vector3 m_InitLocalPosition = Vector3.zero;

        // Token: 0x040002C3 RID: 707
        public Quaternion m_InitLocalRotation = Quaternion.identity;
    }
}