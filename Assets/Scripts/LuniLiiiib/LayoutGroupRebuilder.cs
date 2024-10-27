using UnityEngine;
using UnityEngine.UI;

namespace LuniLib
{
    [DisallowMultipleComponent]
    public class LayoutGroupRebuilder : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rebuiltTransform;
        
        private ContentSizeFitter contentSizeFitter;
        private LayoutGroup layoutGroup;

        private void Awake()
        {
            layoutGroup = GetComponent<LayoutGroup>();
            contentSizeFitter = GetComponent<ContentSizeFitter>();

            if (layoutGroup == null)
                Debug.Log($"Missing layout group on {gameObject.name}.");
            
            if (rebuiltTransform == null)
                rebuiltTransform = transform as RectTransform;
        }

        public void ReloadLayout()
        {
            layoutGroup.enabled = true;

            if (contentSizeFitter != null)
                contentSizeFitter.enabled = true;

            LayoutRebuilder.ForceRebuildLayoutImmediate(rebuiltTransform);

            layoutGroup.enabled = false;

            if (contentSizeFitter != null)
                contentSizeFitter.enabled = false;
        }
    }
}