using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gun 
{
	public Transform transform; 
	public LayerMask Mask;
	public float Range = 15;

	public LineRenderer line; 
	public ParticleSystem effect;

	public Gun(Transform t, LayerMask l, LineRenderer ll)
	{
		transform = t; 
		Mask = l; 
		line = ll; 
		line.enabled = false; 


	}

	public void Shoot()
	{
		Ray ray = new Ray (transform.position, transform.forward); 
		RaycastHit hit; 

		line.enabled = true; 
		ParticleSystem e = Object.Instantiate(effect, transform.position + 0.1f*transform.forward, Quaternion.identity) as ParticleSystem; 
		Object.Destroy(e,0.3f); 

		if (Physics.Raycast(ray, out hit, Range, Mask))
		{
			GameObject go = hit.transform.gameObject;
			go.GetComponent<EnnemiControl>().Touched(); 
			line.SetPosition(0, transform.position + 0.1f*transform.forward); 
			line.SetPosition(1, go.transform.position);
			ParticleSystem ee = Object.Instantiate(effect, hit.point, Quaternion.identity) as ParticleSystem;
			Object.Destroy(ee,0.3f); 
		}
		else
		{
			line.SetPosition(0, transform.position + 0.1f*transform.forward); 
			line.SetPosition(1, transform.position + Range*transform.forward);	
		}

		

	}

	public void StopEffects()
	{
		line.enabled = false;
	}

}