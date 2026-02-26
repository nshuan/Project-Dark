using Sirenix.OdinInspector;
using UnityEngine;

namespace Dark.Scripts.OutGame.Upgrade
{
    public class UIUpgradeTreeNodeDecorCopier : MonoBehaviour
    {
        public Transform objectA;
        public Transform objectB;

        [Button("Copy Decor_High")]
        public void CopyDecorHigh()
        {
            if (objectA.childCount != objectB.childCount)
            {
                Debug.LogWarning("Child count mismatch!");
            }

            int count = Mathf.Min(objectA.childCount, objectB.childCount);

            for (int i = 0; i < count; i++)
            {
                Transform childA = objectA.GetChild(i);
                Transform childB = objectB.GetChild(i);

                Transform decorA = childA.Find("Decor_High");
                Transform decorB = childB.Find("Decor_High");

                if (decorA == null || decorB == null)
                    continue;

                for (int j = decorB.childCount - 1; j >= 0; j--)
                {
                    DestroyImmediate(decorB.GetChild(j).gameObject);
                }
                
                if (decorA.childCount > 0)
                {
                    foreach (Transform decoChild in decorA)
                    {
                        GameObject copy = Instantiate(decoChild.gameObject, decorB);
                        copy.transform.localPosition = decoChild.localPosition;
                        copy.transform.localRotation = decoChild.localRotation;
                        copy.transform.localScale = decoChild.localScale;
                    }
                }
            }

            Debug.Log("Copy Done");
        }
    }
}