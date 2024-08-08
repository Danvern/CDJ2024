using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityServiceLocator;

public class AgentDirector : MonoBehaviour
{
	private EntityMediator targetPlayer;

	public void SetPrimaryPlayer(EntityMediator targetPlayer) => this.targetPlayer = targetPlayer;
	public EntityMediator GetPrimaryPlayer() => targetPlayer;

    // Start is called before the first frame update
    void Awake()
    {
        ServiceLocator.Global.Register(this);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
