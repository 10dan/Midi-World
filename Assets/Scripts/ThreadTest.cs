using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ThreadTest : MonoBehaviour {
    // The ThreadProc method is called when the thread starts.
    // It loops ten times, writing to the console and yielding
    // the rest of its time slice each time, and then ends.
    public static void ThreadProc() {
        for (int i = 0; i < 10; i++) {
            print(i);
            // Yield the rest of the time slice.
            Thread.Sleep(0);
        }
    }

    private void Start() {
        print("Main thread: Start a second thread.");
        // The constructor for the Thread class requires a ThreadStart
        // delegate that represents the method to be executed on the
        // thread.  C# simplifies the creation of this delegate.
        Thread t = new Thread(new ThreadStart(ThreadProc));

        // Start ThreadProc.  Note that on a uniprocessor, the new
        // thread does not get any processor time until the main thread
        // is preempted or yields.  Uncomment the Thread.Sleep that
        // follows t.Start() to see the difference.
        t.Start();
        //Thread.Sleep(0);

        for (int i = 0; i < 4; i++) {
            print("Main thread: Do some work.");
            Thread.Sleep(0);
        }

        print("Main thread: Call Join(), to wait until ThreadProc ends.");
        t.Join();
        print("Main thread: ThreadProc.Join has returned.  Press Enter to end program.");
    }
}
