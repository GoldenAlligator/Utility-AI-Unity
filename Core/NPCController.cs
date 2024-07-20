using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.UtilityAI;

namespace TL.Core
{
    public enum State
    {
        decide,
        move,
        execute
    }

    public class NPCController : MonoBehaviour
    {
        public MoveController mover { get; set; }
        public AIBrain aiBrain { get; set; }
        public NPCInventory Inventory { get; set; }
        public Stats stats { get; set; }

        public Context context;

        public State currentState { get; set; }
        void Start()
        {
            InitializeComponents();
        }
        void Update()
        {
            if (context == null)
            {
                InitializeComponents();  
                if (context == null) 
                {
                    return;
                }
            }

            FSMTick();
        }


        void InitializeComponents()
        {
            context = GetComponent<Context>();
            if (context == null)
            {
                return;
            }

            mover = GetComponent<MoveController>();
            if (mover == null)
            {
                mover = gameObject.AddComponent<MoveController>();
            }
            aiBrain = GetComponent<AIBrain>();
            if (aiBrain == null)
            {
                aiBrain = gameObject.AddComponent<AIBrain>();
            }
            Inventory = GetComponent<NPCInventory>();
            if (Inventory == null)
            {
                Inventory = gameObject.AddComponent<NPCInventory>();
            }

            stats = GetComponent<Stats>();
            if (stats == null)
            {
                stats = gameObject.AddComponent<Stats>();
            }
            currentState = State.decide;
        }

        public void FSMTick()
        {
            if (aiBrain == null)
            {
                return;
            }

            if (currentState == State.decide)
            {
                aiBrain.DecideBestAction();

                if (aiBrain.bestAction == null)
                {
                    return;
                }

                if (aiBrain.bestAction.RequiredDestination != null && Vector3.Distance(aiBrain.bestAction.RequiredDestination.position, this.transform.position) < 2f)
                {
                    currentState = State.execute;
                }
                else
                {
                    currentState = State.move;
                }
            }
            else if (currentState == State.move)
            {
                if (aiBrain.bestAction.RequiredDestination == null)
                {
                    currentState = State.decide;
                    return;
                }

                float distance = Vector3.Distance(aiBrain.bestAction.RequiredDestination.position, this.transform.position);
                if (distance < 2f)
                {
                    currentState = State.execute;
                }
                else
                {
                    mover.MoveTo(aiBrain.bestAction.RequiredDestination.position);
                }
            }
            else if (currentState == State.execute)
            {
                if (aiBrain.finishedExecutingBestAction == false)
                {
                    aiBrain.bestAction.Execute(this);
                }
                else if (aiBrain.finishedExecutingBestAction == true)
                {
                    currentState = State.decide;
                }
            }
        }

        #region Workhorse methods

        public void OnFinishedAction()
        {
            aiBrain.DecideBestAction();
        }

        public bool AmIAtRestDestination()
        {
            return context != null && context.home != null && Vector3.Distance(this.transform.position, context.home.transform.position) <= context.MinDistance;
        }

        #endregion

        #region Coroutine

        public void DoWork(int time)
        {
            StartCoroutine(WorkCoroutine(time));
        }

        public void DoSleep(int time)
        {
            StartCoroutine(SleepCoroutine(time));
        }

        IEnumerator WorkCoroutine(int time)
        {
            int counter = time;
            while (counter > 0)
            {
                yield return new WaitForSeconds(1);
                counter--;
            }

            Inventory.AddResource(ResourceType.wood, 10);
            aiBrain.finishedExecutingBestAction = true;
            yield break;
        }

        IEnumerator SleepCoroutine(int time)
        {
            int counter = time;
            while (counter > 0)
            {
                yield return new WaitForSeconds(1);
                counter--;
            }

            stats.energy += 5;
            aiBrain.finishedExecutingBestAction = true;
            yield break;
        }

        #endregion
    }
}
