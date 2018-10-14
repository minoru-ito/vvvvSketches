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
	[PluginInfo(Name = "=", Category = "Transform", Tags = "matrix")]
	#endregion PluginInfo
	public class Transform_Node : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input 1")]
		public ISpread<Matrix4x4> FInput1;
		
		[Input("Input 2")]
		public ISpread<Matrix4x4> FInput2;

		[Output("Output")]
		public ISpread<bool> FOutput;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FOutput.SliceCount = SpreadMax;

			for (int i = 0; i < SpreadMax; i++)
				FOutput[i] = (FInput1[i] == FInput2[2]);

			//FLogger.Log(LogType.Debug, "Logging to Renderer (TTY)");
		}
	}
}
