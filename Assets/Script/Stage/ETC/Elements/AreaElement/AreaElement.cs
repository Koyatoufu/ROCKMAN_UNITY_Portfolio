using UnityEngine;
using System.Collections;

public class AreaElement : ElementBase {

	protected Panel m_panel;

	protected int m_nChildCount;

	protected AreaElement()
	{
		m_panel = null;
		m_nChildCount = 0;
	}
}
