using System.Collections;
using UnityEngine;

namespace LuniLib.UnityUtils
{
    public class WaitForParallelCoroutines : CustomYieldInstruction
    {
        public WaitForParallelCoroutines(MonoBehaviour runner, params IEnumerator[] coroutines)
        {
            foreach (IEnumerator coroutine in coroutines)
                runner.StartCoroutine(this.StartCoroutineWithCounter(runner, coroutine));
        }
        
        private int counter;

        public override bool keepWaiting => this.counter > 0;

        private IEnumerator StartCoroutineWithCounter(MonoBehaviour runner, IEnumerator coroutine)
        {
            this.counter++;
            yield return runner.StartCoroutine(coroutine);
            this.counter--;
        }
    }
}
