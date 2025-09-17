using LCApi.Game;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class TestExplosiveDropped : MonoBehaviour
{
	private float _time;
	private void Update()
	{
		_time += Time.deltaTime;
		if (_time >= 3)
		{
			_time = 0;
			WorldManager.GetInstance().ChunkManager.BrownMobExplode(transform.position);
			Destroy(gameObject);
		}
	}
}