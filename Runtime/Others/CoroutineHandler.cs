using System.Collections;
using UnityEngine;

namespace Acedia
{
    // TODO CoroutineHandler: Documentations
    public class CoroutineHandler : IEnumerator
    {
        /// <summary> The currently running Coroutine handled by this object. </summary>
        public Coroutine HandledCoroutine { get; private set; }
        /// <summary> Which <see cref="MonoBehaviour"/> is currently running the Coroutine. </summary>
        public MonoBehaviour Runner { get; private set; }

        /// <summary> Tells if the Coroutine has finished running without interruptions. </summary>
        public bool Done { get; private set; }
        /// <summary> Tells if the Coroutine has been stopped before it's finishied running. </summary>
        public bool Interrupted { get; private set; }
        /// <summary> Tells if the Coroutine has stopped running, with or without interruptions. </summary>
        public bool Ended => Done || Interrupted;
        public object Current { get; private set; }

        public bool MoveNext() => !Done;
        public void Reset() => throw new System.NotImplementedException();

        public CoroutineHandler(MonoBehaviour runner, IEnumerator coroutine)
        {
            Current = runner.StartCoroutine(Run(coroutine));
            HandledCoroutine = (Coroutine)Current;
            Runner = runner;
        }

        private IEnumerator Run(IEnumerator coroutine)
        {
            Done = false;
            yield return coroutine;
            Done = true;
        }

        public bool StopCoroutine()
        {
            if (Interrupted) return false;
            if (Done) return false;

            Runner.StopCoroutine(HandledCoroutine);
            Interrupted = true;
            Current = null;

            return true;
        }
    }
}