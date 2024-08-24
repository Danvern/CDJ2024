using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityServiceLocator;

public class WinWhenDead : MonoBehaviour, IEntityObserver
{
    // Start is called before the first frame update
    void OnEnable()
    {
		GetComponent<Entity>().AddObserver(this);
    }

	public void OnNotify(EntityData data)
	{
		switch (data.Prompt)
		{
			case VoicePrompt.Lose:
				ServiceLocator.ForSceneOf(this).Get<UIController>().Win();
				break;
		}
	}

    // Update is called once per frame
    void OnDisable()
    {
		GetComponent<Entity>().RemoveObserver(this);
    }
}
