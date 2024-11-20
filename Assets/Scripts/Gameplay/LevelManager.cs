using UnityEngine;

namespace Gameplay
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private Level[] _levels;
        
        private PlayerController _player;
        private Level _currentLevel;

        public void Init(int startingLevelIndex, PlayerController playerController)
        {
            _player = playerController;
            Level startingLevel = _levels[startingLevelIndex];
            
            if(startingLevel != null)
                SwitchLevel(startingLevel);
            else
                Debug.LogError("this level has not be assigned to LevelManager");

            for (int index = 0; index < _levels.Length; index++)
                _levels[index].SetIndex(index);
        }

        private void SwitchLevel(Level newLevel)
        {
            newLevel.Init(Camera.main.transform, _player.transform);
            _currentLevel = newLevel;
        }

        public bool TryLaunchNextLevel()
        {
            int newIndex = _currentLevel.Index + 1;

            if (newIndex <= _levels.Length - 1)
            {
                SwitchLevel(_levels[newIndex]);
                return true;
            }

            return false;
        }
    }
}