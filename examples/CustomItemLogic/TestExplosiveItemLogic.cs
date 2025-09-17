using LCApi.Game;
using LCApi.Items;
using LCApi.LCData;
using System;
using System.Collections.Generic;
using System.Text;

public class TestExplosiveItemLogic : ItemLogicBase
{
	public override void Holding()
	{

	}

	public override void PrimaryAction(PlayerItemAction state)
	{
		if (state == PlayerItemAction.EndAction)
		{
			Dropped dropped = PlayerManager.GetInstance().ThrowStack(1);
			if (dropped != null)
			{
				dropped.gameObject.AddComponent<TestExplosiveDropped>();
			}
		}
	}

	public override void SecondaryAction(PlayerItemAction state)
	{

	}
}