using UnityEngine;

namespace Gameplay
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Transform _playerTransform;

        public int Index { get; private set; }

        public void Init(Transform camera, Transform player)
        {
            camera.transform.position = _cameraTransform.position;
            player.transform.position = _playerTransform.position;
        }

        public void SetIndex(int index)
        {
            Index = index;
        }
    }
}