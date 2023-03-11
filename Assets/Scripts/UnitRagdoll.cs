using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollRootBone;



    public void Setup(Transform originalRootBone){
        float explosionForce = Random.Range(300f, 700f);
        float explosionRange = Random.Range(5f, 20f);

        MatchAllChildTransforms(originalRootBone, ragdollRootBone);
        ApplyExplosionToRagdoll(ragdollRootBone, explosionForce, transform.position, explosionRange);
    }

    private void MatchAllChildTransforms(Transform root, Transform clone){
        foreach (Transform child in root){
            Transform cloneChild = clone.Find(child.name);
            
            if (cloneChild != null){
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;

                MatchAllChildTransforms(child, cloneChild);
            }

        }
    }

    private void ApplyExplosionToRagdoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange){
        foreach (Transform child in root){
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody)){
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToRagdoll(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}
