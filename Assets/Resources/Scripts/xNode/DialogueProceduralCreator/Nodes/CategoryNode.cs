using UnityEngine;
using XNode;

public class CategoryNode : BaseNode
{
	public enum Category
	{
		ALUCINATIONS, PAIN, BRAWL, FEAR
	}
	
	public Category category;
	[Range(1, 10)]
	public int stressThreshold = 10;
	
	[Space(20)]
	[Output] public ThemeNode themes;
	
	public override object GetValue(NodePort port) { return null; }
}