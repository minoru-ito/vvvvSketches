#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes
{
	#region PluginInfo
	[PluginInfo(Name = "Leaf", Category = "Transform")]
	#endregion PluginInfo
	public class TransformLeafNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Transform Edge")]
		public ISpread<Matrix4x4> FInEdge;
		
		[Input("Transform Leaf")]
		public ISpread<Matrix4x4> FInLeaf;

		[Output("Output")]
		public ISpread<Matrix4x4> FOutput;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FOutput.SliceCount = FInEdge.SliceCount * FInLeaf.SliceCount;
			int id = 0;

			for (int i = 0; i < FInEdge.SliceCount; i++)
			{
				for(int j = 0; j < FInLeaf.SliceCount; j++)
				{
					FOutput[id++] = FInLeaf[j] * FInEdge[i];
				}
			}
		}
	}
}
