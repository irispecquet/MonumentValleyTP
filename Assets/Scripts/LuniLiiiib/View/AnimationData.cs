using UnityEngine;

namespace View
{
    [CreateAssetMenu(fileName = "Animation", menuName = "2D Animation", order = 0)]
    public class AnimationData : ScriptableObject
    {
        [field: SerializeField] public string AnimationName { get; private set; }
        [field: SerializeField] public Sprite[] Sprites { get; private set; }
        [field: SerializeField, Range(0, 120)] public int FramePerSeconds { get; private set; }
        [field: SerializeField] public AnimationType AnimationType { get; private set; }
    }
}