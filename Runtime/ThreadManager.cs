using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
namespace UDPNetwork
{
    public class ThreadManager : MonoBehaviour
    {
        private static List<Action> executeOnThread = new List<Action>();
        private static List<Action> afterExecution = new List<Action>();
        private static List<Action> oneTimeExecution = new List<Action>();
        private Dictionary<int, List<Action>> threadLists = new Dictionary<int, List<Action>>();
        private Dictionary<int, List<Action>> threadCopyLists = new Dictionary<int, List<Action>>();
        private List<Thread> workingThreads = new List<Thread>();
        [SerializeField]
        int threadCount;
        [SerializeField]
        int sleepInMsAfterAction = 15;
        private bool work = false;
        // Start is called before the first frame update
        void Start()
        {
            work = true;
            for (int i = 0; i < threadCount; i++)
            {
                int a = i;
                workingThreads.Add(new Thread(() => Work(a)));
                threadLists.Add(a, new List<Action>());
                threadCopyLists.Add(a, new List<Action>());
            }

            for (int i = threadCount; i < (threadCount + threadCount / 2); i++)
            {
                int a = i;
                workingThreads.Add(new Thread(() => OneTimeWork(a)));
                threadLists.Add(a, new List<Action>());
                threadCopyLists.Add(a, new List<Action>());
            }

            for (int i = 0; i < workingThreads.Count; i++)
            {
                workingThreads[i].Name = "worker" + i;
                workingThreads[i].Start();
            }

        }

        // Update is called once per frame
        void Update()
        {

        }

        public static void AddAction(Action action)
        {
            lock (executeOnThread)
            {
                executeOnThread.Add(action);
            }
        }

        public static void AddOneTimeAction(Action action)
        {
            lock (oneTimeExecution)
            {
                oneTimeExecution.Add(action);
            }
        }


        void OneTimeWork(int index)
        {
            try
            {
                while (work)
                {
                    threadLists[index].Clear();
                    lock (oneTimeExecution)
                    {
                        if (oneTimeExecution.Count > 0 && oneTimeExecution.Count < 5)
                        {

                            threadLists[index].AddRange(oneTimeExecution);
                            oneTimeExecution.Clear();
                        }
                        else if (oneTimeExecution.Count >= 5)
                        {
                            threadLists[index].Add(oneTimeExecution[oneTimeExecution.Count - 1]);
                            threadLists[index].Add(oneTimeExecution[oneTimeExecution.Count - 2]);
                            threadLists[index].Add(oneTimeExecution[oneTimeExecution.Count - 3]);
                            threadLists[index].Add(oneTimeExecution[oneTimeExecution.Count - 4]);
                            threadLists[index].Add(oneTimeExecution[oneTimeExecution.Count - 5]);
                            oneTimeExecution.Remove(threadLists[index][0]);
                            oneTimeExecution.Remove(threadLists[index][1]);
                            oneTimeExecution.Remove(threadLists[index][2]);
                            oneTimeExecution.Remove(threadLists[index][3]);
                            oneTimeExecution.Remove(threadLists[index][4]);
                        }

                    }

                    lock (threadLists[index])
                    {
                        for (int i = threadLists[index].Count - 1; i >= 0; i--)
                        {
                            threadLists[index][i]();

                        }
                    }
                    Thread.Sleep(sleepInMsAfterAction);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("index: " + index + " " + ex.ToString());
            }
        }

        void Work(int index)
        {
            try
            {
                while (work)
                {
                    //Debug.Log("work " + index);
                    threadLists[index].Clear();

                    lock (executeOnThread)
                    {
                        if (executeOnThread.Count < 5)
                        {
                            lock (afterExecution)
                            {
                                executeOnThread.AddRange(afterExecution);
                                afterExecution.Clear();
                            }
                        }
                        else
                        {
                            threadLists[index].Add(executeOnThread[executeOnThread.Count - 1]);
                            threadLists[index].Add(executeOnThread[executeOnThread.Count - 2]);
                            threadLists[index].Add(executeOnThread[executeOnThread.Count - 3]);
                            threadLists[index].Add(executeOnThread[executeOnThread.Count - 4]);
                            threadLists[index].Add(executeOnThread[executeOnThread.Count - 5]);
                            executeOnThread.Remove(threadLists[index][0]);
                            executeOnThread.Remove(threadLists[index][1]);
                            executeOnThread.Remove(threadLists[index][2]);
                            executeOnThread.Remove(threadLists[index][3]);
                            executeOnThread.Remove(threadLists[index][4]);
                        }

                    }
                    threadCopyLists[index].Clear();
                    threadCopyLists[index].AddRange(threadLists[index]);

                    lock (threadLists[index])
                    {
                        for (int i = threadLists[index].Count - 1; i >= 0; i--)
                        {
                            threadLists[index][i]();
                        }
                    }

                    lock (afterExecution)
                    {
                        afterExecution.AddRange(threadCopyLists[index]);
                    }

                    Thread.Sleep(sleepInMsAfterAction);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("index: " + index + " " + ex.ToString());
            }
        }

        private void StopWork()
        {
            work = false;
            for (int i = 0; i < threadCount; i++)
            {
                workingThreads[i].Join();
            }
        }

        private void OnApplicationQuit()
        {
            StopWork();
        }
    }
}