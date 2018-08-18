using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New DT", menuName = "Decision Tree")]
public class DecisionTree : ScriptableObject
{
	public List<DecisionTreeNode> nodes; 

	// public string Decide(GSR s)
	// {
	// 	Debug.Log(nodes); 
	// 	DecisionTreeNode selected = nodes[0].Decide(s); 
	// 	return selected.Sample(); 
	// }


	// public void SetNodes(List<DecisionTreeNode> n)
	// {
	// 	// Debug.Log(n[1]); 
	// 	nodes = new List<DecisionTreeNode>(); 
	// 	nodes = n; 

	// 	// Debug.Log(nodes[0]); 
	// 	// Debug.Log(nodes[1]); 
	// 	// Debug.Break(); 
	// }

	// public override string ToString()
	// {
	// 	Debug.Log(nodes); 
	// 	return "Decision tree with " + nodes.Count.ToString() + " nodes"; 
	// }

}

[System.Serializable]
public abstract class DecisionTreeNode
{
	public DecisionTreeNode false_node; 
	public DecisionTreeNode true_node; 

	abstract public DecisionTreeNode Decide(GSR s); 
	// {
		
	// }

	abstract public string Sample();
	// {
	// }
}

[System.Serializable]
public class Decision : DecisionTreeNode
{
	public DecisionTreeNode true_node; 
	public DecisionTreeNode false_node; 

	public int state_value_to_test; 
	public float test_value; 

	public override DecisionTreeNode Decide(GSR state_reprez)
	{
		float state_value = state_reprez.state[state_value_to_test]; 
		if(test_value > state_value)
		{
			Debug.Log("Selecting true node");
			return true_node; 
		}
		else
		{
			Debug.Log("Selecting false node");
			return false_node; 
		}
	}

	public override string Sample()
	{
		return "Error: Decision sample called"; 
	}

	public override string ToString()
	{
		return "Decision node testing value " + state_value_to_test.ToString() + " with " + test_value.ToString() + " as limit";
	}
}

[System.Serializable]
public class DecisionTreeAction : DecisionTreeNode
{
	public ProbAction [] actions; 

	public void Normalize()
	{
		float sum_prob = 0f; 
		foreach(ProbAction p in actions)
		{
			sum_prob += p.prob; 
		}

		float cum_sum = 0f; 
		for (int i = 0; i<actions.Length; i++) 
		{
			actions[i].prob /= sum_prob; 
			actions[i].prob += cum_sum;
			cum_sum = actions[i].prob; 
		}
	}

	public override DecisionTreeNode Decide(GSR s)
	{
		return this;
		// return null; 
	}

	public override string Sample()
	{
		float selector = Random.Range(0f,1f); 
		foreach(ProbAction p in actions)
		{
			if(selector<p.prob)
				return p.decision_id; 
		}

		return "Error"; 
	}

	public override string ToString()
	{	
		string recap = "Action node with "; 
		foreach(ProbAction pa in actions) 
		{
			recap += "\n" + pa.ToString(); 
		}
		recap += "\n Length: " + actions.Length.ToString(); 
		return recap; 
	}
}

[System.Serializable]
public struct ProbAction
{
	public string decision_id;
	public float prob;

	public override string ToString()
	{
		return "ProbAction of " + decision_id + " is " + prob.ToString(); 
	}
}

[System.Serializable]
public struct GSR
{
	public float [] state; 
}


// public class DecisionTreeAction(DecisionTreeNode)
// {
// 	public ProbAction [] actions; 

// 	public void Normalize()
// 	{
// 		float sum_prob = 0f; 
// 		foreach(ProbAction p in actions)
// 		{
// 			sum_prob += p.prob; 
// 		}
// 		float cum_sum = 0f; 
// 		foreach(ProbAction p in actions)
// 		{
// 			p.prob /= sum_prob; 
// 			p.prob += cum_sum;
// 			cum_sum = p.prob; 
// 		}
// 	}

// 	public string MakeDecision()
// 	{
// 		float selector = Random.Range(0f,1f); 
// 		foreach(ProbAction p in actions)
// 		{
// 			if(selector<p.prob)
// 				return p.decision_id; 
// 		}

// 		return "Error"; 
// 	}
// }





