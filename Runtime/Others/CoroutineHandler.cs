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

        public bool HasStarted => HandledCoroutine != null;
        /// <summary> Tells if the Coroutine has finished running without interruptions. </summary>
        public bool Done { get; private set; }
        /// <summary> Tells if the Coroutine has been stopped before it's finishied running. </summary>
        public bool Interrupted { get; private set; }
        /// <summary> Tells if the Coroutine has stopped running, with or without interruptions. </summary>
        public bool Ended => Done || Interrupted;
        public object Current { get; private set; }

        public bool MoveNext() => !Done;
        public void Reset() => throw new System.NotImplementedException();

        private IEnumerator Coroutine { get; set; }

        public CoroutineHandler(MonoBehaviour runner) : this(runner, null, true) { }

        public CoroutineHandler(MonoBehaviour runner, IEnumerator coroutine, bool startManually = false)
        {
            Runner = runner;
            Coroutine = coroutine;
            if (!startManually) StartCoroutine();
        }

        public void SetCoroutine(IEnumerator coroutine)
        {
            if (HasStarted)
            {
                throw new System.Exception($"Can't set coroutine because the handler has already started.");
            }

            Coroutine = coroutine;
        }

        public void StartCoroutine()
        {
            if (HasStarted)
            {
                throw new System.Exception($"Coroutine has already started.");
            }

            if (Coroutine == null)
            {
                throw new System.NullReferenceException($"Can't start coroutine, no couroutine is set.");
            }

            HandledCoroutine = Runner.StartCoroutine(Run(Coroutine));
            Current = HandledCoroutine;
            return;

            IEnumerator Run(IEnumerator coroutine)
            {
                Done = false;
                yield return coroutine;
                Done = true;
            }
        }

        /// <remarks>
        /// Important: Coroutine cannot be stopped when inside the coroutine itself. Make sure to call <see cref="StopCoroutine"/> <b>outside of the routine</b>.
        /// </remarks>
        public bool StopCoroutine()
        {
            if (!HasStarted) return false;
            if (Interrupted) return false;
            if (Done) return false;

            Runner.StopCoroutine(HandledCoroutine);
            Interrupted = true;
            Current = null;

            return true;
        }
    }
}