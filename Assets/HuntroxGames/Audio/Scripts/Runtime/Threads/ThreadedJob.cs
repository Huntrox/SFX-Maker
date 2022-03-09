using System.Threading;
using System.Collections;
namespace HuntroxGames.Utils
{
    /// <summary>
    /// Source:
    ///     https://gist.github.com/Chaser324/aed385042f3f923ebbfe0df9570cab17
    ///     http://answers.unity3d.com/questions/357033/unity3d-and-c-coroutines-vs-threading.html
    /// </summary>
    public class ThreadedJob
    {
        private readonly object handle = new object();
        private bool isDone;
        private Thread thread;

        public bool IsDone
        {
            get
            {
                bool tmp;
                lock (handle)
                {
                    tmp = isDone;
                }

                return tmp;
            }
            set
            {
                lock (handle)
                {
                    isDone = value;
                }
            }
        }

        public virtual void Start()
        {
            thread = new Thread(Run);
            thread.Start();
        }

        public virtual void Abort()
        {
            thread.Abort();
        }

        protected virtual void ThreadFunction()
        {
        }

        protected virtual void OnFinished()
        {
        }

        public virtual bool Update()
        {
            if (IsDone)
            {
                OnFinished();
                return true;
            }

            return false;
        }

        private IEnumerator WaitFor()
        {
            while (!Update())
            {
                yield return null;
            }
        }

        private void Run()
        {
            ThreadFunction();
            IsDone = true;
        }
    }

}