using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class AIProto3 : MonoBehaviour {


	public Text debug_text; 

	public NamedDistance [] distances; 
	public AIState [] states; 

	public Transform Target; 
	public AITimer think_timer; 

	BoxerModularController controller; 
	Dictionary <string, AIState> distance_to_action; 
	
	// Use this for initialization
	void Start () {

		controller = GetComponent<BoxerModularController>(); 	
		NormalizeActions(); 
		FillDict(); 	
	}
	
	// Update is called once per frame
	void Update () {

		think_timer.timer -= Time.deltaTime; 
		if(think_timer.timer <= 0)
		{
			think_timer.timer = think_timer.CoolDown; 
			string result = Logic(); 
			ParseLogic(result); 
		}


		
	}

	void ParseLogic(string result)
	{
		if(result =="fight")
		{
			controller.IAOrder(RandomizeCommand(true)); 
		}
		else
		{
			controller.IAOrder(RandomizeCommand(false)); 
		}
	}

	string Logic()
	{

		string debug_recap = ""; 
		float current_distance = (transform.position - Target.transform.position).magnitude; 
		string current_distance_name = ""; 


		debug_recap += "Real distance: " + current_distance.ToString(); 
		foreach(NamedDistance d in distances)
		{
			if(current_distance < d.MaxDistance)
			{
				current_distance_name = d.Name; 
				break; 
			}
		}

		debug_recap += "\nDistance name: " + current_distance_name;

		AIState current_state = distance_to_action[current_distance_name]; 
		float selector = Random.Range(0f,1f); 
		string action_name = ""; 
		debug_recap += "\nselector: " + selector.ToString(); 
		foreach(AIActionAtom ac in current_state.actions)
		{
			if(selector < ac.Prob)
			{
				action_name = ac.Name; 
				break; 
			}
		}
		debug_recap += "\nAction: " + action_name;
		debug_text.text = debug_recap; 

		return action_name; 

	}

	Command RandomizeCommand(bool fight)
	{
		Command c; 
		if(fight)
		{
			int side = Random.Range(0,2); 
			int attack = Random.Range(0,4);
			c = new Command(Vector2.zero, false, side, attack); 
		}
		else
		{
			Vector2 d = new Vector2(Random.Range(-1f,1f),Random.Range(-1f,1f)); 
			c = new Command(d.normalized, true, 0, 0); 
		}
		return c; 
	}

	void NormalizeActions()
	{
		foreach(AIState s in states)
		{
			float sum_probs = 0f; 
			for(int i = 0; i<s.actions.Length; i++)
			{
				sum_probs += s.actions[i].Prob; 
			}
			float cum_sum = 0f; 
			for(int i = 0; i<s.actions.Length; i++)
			{
				s.actions[i].Prob /= sum_probs;  // normalize
				s.actions[i].Prob += cum_sum;  // set in order 
				cum_sum = s.actions[i].Prob; 
			}
			
		}
	}

	void FillDict()
	{
		distance_to_action = new Dictionary<string, AIState>(); 
		for(int i = 0; i<distances.Length; i ++)
		{
			distance_to_action.Add(distances[i].Name, states[i]); 
		}
	}
}

[System.Serializable]
public struct NamedDistance
{
	public string Name; 
	public float MaxDistance; 
}

[System.Serializable]
public struct AIState
{
	public string Name; 
	public AIActionAtom [] actions; 
}

[System.Serializable]
public struct AIActionAtom 
{
	public string Name; 
	public float Prob; 
}

public struct Command
{
	public Vector2 Direction; 
	public bool Dash; 
	public bool Left; 
	public bool Right; 
	public bool Upper; 
	public bool Direct; 
	public bool Hook; 
	public bool Dodge; 

	public Command(Vector2 v, bool d, int side, int attack)
	{
		Direction = v;
		Dash = d;
		if(!d)
		{
			Left = side == 0  ? true : false;
			Right = !Left;
			Upper = attack == 0 ? true : false;
			Direct = attack == 1 ? true : false;;
			Hook = attack == 2 ? true : false;;
			Dodge = attack == 3 ? true : false;;
		}
		else
		{
			Left = false;
			Right = false;
			Upper = false;
			Direct =false;
			Hook = false;
			Dodge = false;
		}
	}
}

[System.Serializable]
public struct AITimer
{
	public float CoolDown; 
	public float timer; 
}
