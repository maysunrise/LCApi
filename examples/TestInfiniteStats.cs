using LCApi.Game;
using System;
using System.Collections.Generic;
using System.Text;

public class TestInfiniteStats : InGameScript
{
	private void Start()
	{
	}

	private void Update()
	{
		PlayerManager player = PlayerManager.GetInstance();
		player.JetpackFuel = 1f;
		player.Health = 100;
		player.SuitStatus = 100;
	}
}