using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeEditor : EditorWindow {

	private List<Node> nodes; 
	private List<Connection> connections; 

	private GUISkin skin; 

	private GUIStyle nodeStyle; 
	private GUIStyle decision_style; 
	private GUIStyle action_style; 
	private GUIStyle editing_style; 

	Vector2 node_dim = new Vector2(200,50); 
	Vector2 node_decal = new Vector2(250, 100); 

	private static Texture2D background_texture; 

	[MenuItem("Window/Node editor")]
	private static void OpenWindow()
	{
		NodeEditor window = GetWindow<NodeEditor>(); 
		window.titleContent = new GUIContent("Node editor"); 
		window.minSize = new Vector2(1200,1000); 
	}



	private void InitiateTree()
	{
		nodes = new List<Node>(); 
		Node n = new Node(new Vector2(500, 150) , node_dim.x, node_dim.y, true, decision_style, editing_style, maxSize); 
		nodes.Add(n); 
		AddBranch(Vector2.zero, n); 
	}

	private void OnEnable()
	{
		LoadSkin(); 
		LoadStyles(); 
		InitiateTree(); 
	}

	private void LoadSkin()
	{
		skin = Resources.Load<GUISkin>("FirstSkin"); 
	}

	private void LoadStyles()
	{
		background_texture = new Texture2D(1,1, TextureFormat.RGBA32, false); 
		background_texture.SetPixel(0,0, new Color(0.25f ,0.25f,0.25f,1f)); 
		background_texture.Apply(); 

		decision_style = new GUIStyle(); 
		decision_style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node3.png") as Texture2D;
		decision_style.border = new RectOffset(12,12,12,12); 

		action_style = new GUIStyle(); 
		action_style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node5.png") as Texture2D;
		action_style.border = new RectOffset(12,12,12,12); 

		editing_style = new GUIStyle(); 
		editing_style.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
		editing_style.border = new RectOffset(12,12,12,12); 
	}

	private void OnGUI()
	{
		DrawBackground(); 
		DrawLines(); 
		DrawNodes(skin); 
		DrawButtons(); 
		ProcessNodeEvents(Event.current); 
		ProcessEvents(Event.current); 

		if(GUI.changed)
			Repaint(); 
	}

	private void DrawButtons()
	{
		if(GUI.Button(new Rect(1100, 900, 250 ,45),"Create tree" ))
			CreateTree(); 
		if(GUI.Button(new Rect(1100, 950, 250 ,45),"Simple init" ))
			InitializeToSimple(); 
	}

	private void InitializeToSimple()
	{
		nodes[1].ChangeType(decision_style, action_style); 
		nodes[2].ChangeType(decision_style, action_style); 

		nodes[0].Name = "Decision ini"; 
		nodes[1].Name = "A1"; 
		nodes[2].Name = "A2"; 

		ProbAction [] p1 = new ProbAction[3]; 
		ProbAction [] p2 = new ProbAction[2]; 


		p1[0].decision_id = "ac1"; 
		p1[0].prob = 1; 
		p1[1].decision_id = "ac2"; 
		p1[1].prob = 1; 
		p1[2].decision_id = "ac3"; 
		p1[2].prob = 1; 

		p2[0].decision_id = "ac1"; 
		p2[0].prob = 1; 
		p2[1].decision_id = "ac2"; 
		p2[1].prob = 15; 

		nodes[1].prob_actions = p1;
		nodes[2].NbActions = 2; 
		nodes[2].prob_actions = p2; 
	}

	private void CreateTree()
	{
		DecisionTree my_dt = ScriptableObject.CreateInstance<DecisionTree>(); 
		Dictionary<Node, DTNode> mapping = new Dictionary<Node, DTNode>(); 

		Dictionary<Node, int> gen_mapping = new Dictionary<Node, int>(); 

		List<DTNode> translated_nodes = new List<DTNode>(); 

		int counter_node = 0; 
		foreach(Node n in nodes)
		{
			DTNode current_node = new DTNode(); 
			current_node.Name = n.Name; 

			if(n.decision_type)
			{
				current_node.state_value_to_test = n.IndexToConsider; 
				current_node.test_value = n.TestValue; 
				current_node.decision_type = true; 
			}
			else
			{
				current_node.actions = n.prob_actions; 
				current_node.Normalize(); 
			}
			mapping.Add(n, current_node); 
			gen_mapping.Add(n, counter_node); 
			counter_node += 1; 
		}

		foreach(Node n in nodes)
		{
			if(n.decision_type)
			{


				mapping[n].true_node = gen_mapping[n.my_true_node]; 
				mapping[n].false_node = gen_mapping[n.my_false_node]; 
			}
		}

		foreach(DTNode d in mapping.Values)
		{
			translated_nodes.Add(d); 
		}
		my_dt.nodes = translated_nodes; 


		AssetDatabase.CreateAsset(my_dt, "Assets/ProbabilisticDecisionTree/DTFromCustom.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = my_dt;

	}

	private void DrawBackground()
	{
		// GUI.color = Color.black; 
 		GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y) , background_texture, 
 			ScaleMode.StretchToFill);

 		float nb_div_x = 10f; 
 		float nb_div_y = 5f; 
 		float distance_x = Screen.width/nb_div_x; 
 		float distance_y = Screen.height/nb_div_y; 
 		for(int i = 0; i<nb_div_x; i++)
 		{
 			Vector2 x1 = new Vector2(0, distance_x*i); 
 			Vector2 x2 = new Vector2(Screen.width, distance_x*i); 

 			Vector2 y1 = new Vector2(distance_y*i,0); 
 			Vector2 y2 = new Vector2(distance_y*i, Screen.height); 
 			Vector3 [] x_pos = new Vector3 [] {x1, x2}; 
 			Vector3 [] y_pos = new Vector3 [] {y1, y2}; 
 			Handles.DrawAAPolyLine(2, x_pos);
 			Handles.DrawAAPolyLine(2, y_pos); 
 		}
	}

	private void DrawLines()
	{
		if(connections != null)
		{
			foreach(Connection c in connections)
			{
				c.Draw(); 
			}
		}
	}

	private void DrawNodes(GUISkin skin)
	{
		if(nodes != null)
		{
			foreach(Node n in nodes)
			{
				n.Draw(skin); 
			}
		}
	}
	private void ProcessEvents(Event e)
	{
		switch(e.type)
		{
			case EventType.MouseDown: 
				if(e.button == 1)
				{
					bool mouse_in_nodes; 
					Node current_node; 
					CheckMouseInNodes(e.mousePosition, out mouse_in_nodes, out current_node); 
					if(mouse_in_nodes) 
						ProcessNodeMenu(e.mousePosition, current_node); 
					else
						ProcessContextMenu(e.mousePosition); 
				}
				// else if(e.button == 0)
				// {
				// 	bool mouse_in_nodes; 
				// 	Node current_node; 
				// 	CheckMouseInNodes(e.mousePosition, out mouse_in_nodes, out current_node); 
				// 	if(mouse_in_nodes)
				// 	{
				// 		current_node.is_editing = !current_node.is_editing; 
				// 	}
				// }
				break; 
		}
	}

	private void ProcessNodeEvents(Event e)
	{
		if(nodes != null)
		{
			foreach(Node n in nodes)
			{
				bool gui_changed = n.ProcessEvents(e); 
				if(gui_changed)
					GUI.changed = true; 
			}
		}
	}

	private void ProcessContextMenu(Vector2 mousePosition)
	{
		GenericMenu generic_menu = new GenericMenu(); 
		generic_menu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
		generic_menu.ShowAsContext(); 
	}

	private void ProcessNodeMenu(Vector2 mousePosition, Node n)
	{
		GenericMenu generic_menu = new GenericMenu(); 
		// generic_menu.AddItem(new GUIContent("Add decision node"), false, () => AddDecisionNode(mousePosition, n));
		if(n.decision_type)
			generic_menu.AddItem(new GUIContent("Add branch"), false, () => AddBranch(mousePosition, n));
		generic_menu.AddItem(new GUIContent("Change node type"), false, () => ChangeNodeType(mousePosition, n));
		// generic_menu.AddItem(new GUIContent("Remove node"), false, () => RemoveNode(mousePosition, n));
		generic_menu.ShowAsContext(); 
	}

	private void OnClickAddNode(Vector2 mousePosition)
	{
		// Node n = new Node(mousePosition, node_dim.x,node_dim.y, decision_style); 
		if(nodes == null)
		{
			nodes = new List<Node>(); 
		}
		Debug.Log("Doens't do anything"); 
		// nodes.Add(n); 
	}

	private void CheckMouseInNodes(Vector2 mousePosition, out bool mouse_in_nodes, out Node current_node)
	{
		mouse_in_nodes = false; 
		current_node = null; 
		foreach(Node n in nodes)
		{
			if(n.rect.Contains(mousePosition))
			{
				mouse_in_nodes = true; 
				current_node = n; 
			}
		}

		// return current_node; 
	}

	private void AddDecisionNode(Vector2 mousePosition, Node current_node)
	{

	}

	private void ChangeNodeType(Vector2 mousePosition, Node current_node)
	{
		current_node.ChangeType(decision_style, action_style);
	}
	private void AddBranch(Vector2 mousePosition, Node current_node)
	{
		// Add two new nodes 
		
		Node n1 = new Node(current_node.rect.position + node_decal, node_dim.x, node_dim.y, true, decision_style, editing_style, maxSize); 
		Node n2 = new Node(current_node.rect.position + new Vector2(-node_decal.x, node_decal.y), node_dim.x, node_dim.y, false, decision_style, editing_style, maxSize); 

		nodes.Add(n1); 
		nodes.Add(n2); 
		current_node.my_true_node = n1; 
		current_node.my_false_node = n2; 

		if(connections == null)
		{
			connections = new List<Connection>(); 
		}

		connections.Add(new Connection(current_node, n1)); 
		connections.Add(new Connection(current_node, n2)); 

	}
	private void RemoveNode(Vector2 mousePosition, Node current_node)
	{

	}
}

public class Connection
{
	public Node parent; 
	public Node child; 

	public Connection(Node p, Node c)
	{
		parent = p; 
		child = c; 
	}

	public void Draw()
	{
		// Handles.DrawLine(parent.rect.center, child.rect.center); 
		Vector3 [] points = new Vector3 [] {parent.rect.center, child.rect.center}; 
		Handles.DrawAAPolyLine(5, points ); 
		Vector3 mid_point = (parent.rect.center + child.rect.center)/2f; 
		string title = child.true_node ? "True" : "False"; 
		GUI.Label(new Rect(mid_point.x, mid_point.y, 50, 50), title); 

	}
}

public class Node
{
	public string Name; 
	public Rect rect; 
	public string title = "Decision"; 
	public bool is_dragged; 
	public bool decision_type = true; 
	public bool true_node = false; 

	public Node my_true_node; 
	public Node my_false_node; 


	public bool is_editing = false; 


	// Decision parameters 
	public int IndexToConsider; 
	public float TestValue; 

	//Action parameter

	public int NbActions = 3; 
	public ProbAction [] prob_actions; 

	public Vector2 maxSize; 
	public GUIStyle editing_style; 
	public GUIStyle style; 

	public Node(Vector2 position, float width, float height, bool is_true, GUIStyle nodeStyle, GUIStyle editing, Vector2 size)
	{
		rect = new Rect(position.x, position.y, width, height); 
		style = nodeStyle; 
		editing_style = editing; 
		decision_type = true; 
		true_node = is_true; 
		maxSize = size; 

		prob_actions = new ProbAction[NbActions]; 
	}

	public void Drag(Vector2 delta)
	{
		rect.position += delta; 
	}

	public void ChangeType(GUIStyle decision, GUIStyle action)
	{
		decision_type = !decision_type; 
		if(decision_type)
		{
			title = "Decision";
			style = decision; 
		}
		else
		{
			title = "Action"; 
			style = action; 
		}
	}

	public void Draw(GUISkin skin)
	{

		float handles_line_width = 5f; 
		GUI.Box(rect, "", style); 
		GUILayout.BeginArea(rect); 
		title = GUILayout.TextField(title, skin.GetStyle("st1")); 

		
		GUILayout.EndArea(); 
		DrawEditingToggle(); 
		if(is_editing)
		{
			// Debug.Log(IndexToConsider); 
			Handles.color = Color.red; 
			if(decision_type)
				DrawDecision(skin, handles_line_width); 
			else
				DrawAction(skin, handles_line_width); 
		}

		

	}	


	public void DrawEditingToggle()
	{
		Color c = GUI.color; 
		if(is_editing)
			GUI.color = Color.red; 
		else 
			GUI.color = Color.green; 

		string button_text = is_editing ? "X" : "O"; 
		if(GUI.Button(new Rect(rect.position.x + 180, rect.position.y + 10, 15, 15), button_text))
		{
			Debug.Log("Pressed"); 
			is_editing = !is_editing; 
		}

		GUI.color = c; 
	}

	public void DrawDecision(GUISkin skin, float lines_width)
	{
		Vector2 small_box_dim = new Vector2(80, 30); 
		Vector2 small_box_decal = new Vector2(80, 60); 

		Rect box_rect = Rect.zero; 
		box_rect.center = rect.center + new Vector2(150, 50); 
		box_rect.size = new Vector2(300,150); 

		Vector2 inter = new Vector2(rect.center.x, box_rect.center.y); 
		Handles.DrawAAPolyLine(lines_width, new Vector3 []{box_rect.center, inter, rect.center});  

		GUI.Box(box_rect, "", editing_style); 		

		Rect index_rect = Rect.zero; 
		index_rect.center = box_rect.center + new Vector2(small_box_decal.x/2, -3*small_box_decal.y/4); 
		index_rect.size = small_box_dim; 
		IndexToConsider = EditorGUI.IntField(index_rect, IndexToConsider); 

		Rect testvalue_rect = index_rect; 
		testvalue_rect.position += new Vector2(0, small_box_decal.y); 
		TestValue = EditorGUI.FloatField(testvalue_rect, TestValue); 

		Rect label_index_rect = Rect.zero; 
		label_index_rect.center = index_rect.center + new Vector2(-150, 0); 
		label_index_rect.size = small_box_dim; 
		EditorGUI.LabelField(label_index_rect, "Index to consider"); 

		Rect label_text_value_rect = label_index_rect; 
		label_text_value_rect.center +=  new Vector2(0, small_box_decal.y); 
		EditorGUI.LabelField(label_text_value_rect, "Test value"); 

		


	}


	public void DrawAction(GUISkin skin, float lines_width)
	{

		Vector2 small_box_dim = new Vector2(80, 30); 
		Vector2 small_box_decal = new Vector2(80, 60); 

		Rect box_rect = Rect.zero; 
		box_rect.center = rect.center + new Vector2(150, 50); 
		box_rect.size = new Vector2(300,300); 

		Vector2 inter = new Vector2(rect.center.x, box_rect.center.y); 
		Handles.DrawAAPolyLine(lines_width, new Vector3 []{box_rect.center, inter, rect.center});

		GUI.Box(box_rect, "", editing_style); 

		Rect nbaction_rect = Rect.zero; 
		nbaction_rect.center = box_rect.center + new Vector2(-small_box_dim.x/2, -135); 
		nbaction_rect.size = small_box_dim; 
		NbActions = EditorGUI.IntField(nbaction_rect, NbActions); 

		NbActions = NbActions < 1 ? 1 : NbActions; 
		NbActions = Mathf.Min(NbActions, 5); 

		if(prob_actions == null)
		{
			prob_actions = new ProbAction [NbActions]; 
			Debug.Log("Creating prob actions of size " + NbActions.ToString()); 
		}

		if(prob_actions.Length != NbActions)
		{
			prob_actions = new ProbAction[NbActions]; 
			Debug.Log("Adapting prob actions of size " + NbActions.ToString()); 
		}

		if(prob_actions != null)
		{
			for(int i = 0; i < NbActions; i++)
			{
				Rect prob_name_box = Rect.zero;
				prob_name_box.center = nbaction_rect.center + new Vector2(-150 + small_box_dim.x/2, i*45 + 30);  
				prob_name_box.size = small_box_dim; 
				Rect prob_prob_box = prob_name_box; 
				prob_prob_box.center += new Vector2(150,0); 
				prob_name_box.size = small_box_dim;  

				prob_actions[i].decision_id = EditorGUI.TextField(prob_name_box, prob_actions[i].decision_id); 
				prob_actions[i].prob = EditorGUI.FloatField(prob_prob_box, prob_actions[i].prob); 
			}
		}

	}	


	public bool ProcessEvents(Event e)
	{
		switch(e.type)
		{
			case EventType.MouseDown: 
			if(e.button == 0)
			{
				if(rect.Contains(e.mousePosition))
				{
					is_dragged = true; 
					GUI.changed = true; 
				}
				else
				{
					GUI.changed = true; 
				}
			}
			break; 

			case EventType.MouseUp: 
				is_dragged = false; 
				break; 

			case EventType.MouseDrag: 
				if(e.button == 0 && is_dragged)
				{
					Drag(e.delta); 
					e.Use(); 
					return true; 
				}
				break; 
		}

		return false; 
	}

}
