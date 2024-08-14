using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
		if (!ServiceLocator.Global.TryGet<AgentDirector>(out _))
        	ServiceLocator.Global.Register(this);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
