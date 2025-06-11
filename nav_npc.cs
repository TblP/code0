using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Text.RegularExpressions;
public class nav_npc : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform destination;

    public Animator npc_anim;
    public Animator npc_anim_face;
    private bool reachedDestination = false; // Флаг достиг ли NPC точки
    private bool isPlayingGreeting = false;  // Флаг для Greeting
    private bool isPlayingIdleSequence = false; // Флаг для Idle 
    public float idleTimer = 0f;            // Таймер для отслеживания времени в Idle
    public const float idleThreshold = 15f; // Время для переключения на случайный Idle2/Idle3

    public Transform Head_npc;

    public GameObject Dialogue_ico;

    public Vector3 temporaryDestination; // Временная точка для очереди

    public bool get_order = false;
    public bool order_done = false;
    public bool goaway = true;

    public List<GameObject> group1;
    public List<GameObject> group2;
    public List<GameObject> group3;

    public string coffee_name;

    public Vidacha vid;

    public Transform arm;

    void Start()
    {

        if (group1.Count > 0)
        {
            int randomIndex1 = Random.Range(0, group1.Count);
            group1[randomIndex1].SetActive(true);
            coffee_name += group1[randomIndex1].name;
        }


        if (group2.Count > 0)
        {
            int randomIndex2 = Random.Range(0, group2.Count);
            group2[randomIndex2].SetActive(true);
            coffee_name += group2[randomIndex2].name;
        }

        
        if (group3.Count > 0)
        {
            int randomChoice = Random.Range(0, 2); 
            if (randomChoice == 1)
            {
                group3[0].SetActive(true); 
                coffee_name += group3[0].name;
            }
        }
        vid = GameObject.Find("Vidacha").GetComponent<Vidacha>();
        npc_anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        temporaryDestination = GameObject.Find("SpawnPoint").transform.position;
    }
    public void startMove()
    {
        agent.SetDestination(destination.position);
        agent.isStopped = false;
        reachedDestination = false;
        isPlayingGreeting = false; 
        isPlayingIdleSequence = false;
        npc_anim.SetBool("go", true);
        npc_anim.Play("walk");
        npc_anim.SetBool("queidle", false);
        npc_anim.SetBool("greeting", false);
        npc_anim.SetBool("idle", false);
        
    }

    void Update()
    {
        if (order_done)
        {
            npc_anim.SetBool("idle", false);
            npc_anim.SetBool("grab", true);
            Transform cup = vid.cup.transform;
            CupOfCoffee coc = cup.GetComponent<CupOfCoffee>();

            bool isCoffeeMatch = Regex.IsMatch(coffee_name, $@"\b{Regex.Escape(coc.typeCoffe)}(?=[^a-zA-Z0-9])");
            bool isMilkMatch = Regex.IsMatch(coffee_name, $@"(?<=[a-zA-Z]){Regex.Escape(coc.typeMilk)}(?=[_])");


            Debug.Log(isMilkMatch + " " + coffee_name + " " + coc.typeMilk);
            Debug.Log(isCoffeeMatch + " " + coffee_name + " " + coc.typeCoffe);
            if (isCoffeeMatch && isMilkMatch)
            {
                npc_anim_face.SetBool("stand", false);
                npc_anim_face.SetBool("smile", true);
                npc_anim_face.Play("smile_anim_1");
            }
            if (isCoffeeMatch ^ isMilkMatch)
            {
                npc_anim_face.SetBool("stand", false);
                npc_anim_face.SetBool("quest", true);
                npc_anim_face.Play("quest_anim");
            }
            if (!isCoffeeMatch && !isMilkMatch)
            {
                npc_anim_face.SetBool("stand", false);
                npc_anim_face.SetBool("angry", true);
                npc_anim_face.Play("angry_anim");
            }
            Rigidbody rb_cup = vid.cup.GetComponent<Rigidbody>();
            
            
            if (IsAnimationHalfFinished("pickup"))
            {
                cup.SetParent(arm);
                cup.position = Vector3.zero;
                rb_cup.useGravity = false;
            }
            
            if (IsAnimationFinished("pickup"))
            {
                get_order = true;
                order_done = false;
                
                npc_anim_face.SetBool("smile", false);
                npc_anim_face.SetBool("quest", false);
                npc_anim_face.SetBool("angry", false);
                npc_anim_face.SetBool("stand", true);
                
            }
            
        }
        if (get_order & goaway)
        {
            Dialogue_ico.SetActive(false);
            npc_anim.Play("walk");
            goaway = false;
            agent.SetDestination(temporaryDestination);
            agent.isStopped = false;

            spawn_npc spawn = GameObject.Find("pawnManager").GetComponent<spawn_npc>();

            // Удаляем текущий NPC из списка активных NPC
            spawn.activeNPCs.Remove(gameObject);
            // Запускаем обновление позиций в очереди
            spawn.UpdateQueuePositions();
        }
        if (!get_order)
        {
            if (isPlayingGreeting || isPlayingIdleSequence) return; // Избегаем повторного запуска анимаций

            float distance = Vector3.Distance(transform.position, destination.position);

            if (!reachedDestination && distance <= 0.5f)
            {
                reachedDestination = true;
                agent.isStopped = true;
                npc_anim.SetBool("go", false);
                if (destination.name == "NavMeshPoint")
                {
                    StartCoroutine(PlayGreetingAndIdle());
                }
                else
                {
                    npc_anim.SetBool("queidle", true);
                } 
            }

            if (reachedDestination && npc_anim.GetBool("idle") && !isPlayingIdleSequence)
            {
                Dialogue_ico.SetActive(true);
                idleTimer += Time.deltaTime;
                order_done = vid.order;
                if (idleTimer >= idleThreshold)
                {
                    StartCoroutine(PlayRandomIdle());
                }
            }
        }
    }

    private IEnumerator PlayGreetingAndIdle()
    {
        isPlayingGreeting = true;

        // Останавливаем движение
        agent.isStopped = true;

        // Переход к анимации Greeting
        npc_anim.SetBool("greeting", true);
        yield return new WaitForSeconds(2f);

        // Переход к анимации Idle
        npc_anim.SetBool("greeting", false);
        npc_anim.SetBool("idle", true);

        isPlayingGreeting = false;
    }

    private IEnumerator PlayRandomIdle()
    {
        idleTimer = 0f;
        isPlayingIdleSequence = true;

        // Выбираем случайную анимацию (Idle2 или Idle3)
        int randomIdle = Random.Range(0, 2);
        string idleState = randomIdle == 0 ? "idle2" : "idle3";

        npc_anim.SetBool("idle", false);
        npc_anim.SetBool(idleState, true);

        yield return new WaitUntil(() => IsAnimationFinished(idleState));

        // Возвращаемся в Idle
        npc_anim.SetBool("idle", true);
        npc_anim.SetBool("idle2", false);
        npc_anim.SetBool("idle3", false);

        isPlayingIdleSequence = false;
    }

    private bool IsAnimationFinished(string animationName)
    {
        AnimatorStateInfo stateInfo = npc_anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 0.95f;
    }
    private bool IsAnimationHalfFinished(string animationName)
    {
        AnimatorStateInfo stateInfo = npc_anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 0.5f;
    }
}
