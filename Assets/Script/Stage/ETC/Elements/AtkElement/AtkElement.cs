using UnityEngine;
using System.Collections;

public class AtkElement : ElementBase {

	protected float m_fSpeed;

	protected AtkElement()
	{
		m_fSpeed = 0.0f;
		m_bAllive = false;
	}

	public void SetSpeed(float fSpeed)
	{
		m_fSpeed = fSpeed;
	}
		

}
