#region usings
using System;
using System.ComponentModel.Composition;
using System.Collections;
using System.Collections.Generic;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;

using VVVV.Nodes.Lindenmayer;
#endregion usings

namespace VVVV.Nodes
{
	#region PluginInfo
	[PluginInfo(Name = "Lindenmayer", Category = "Spreads", Version = "mino")]
	#endregion PluginInfo
	public class minoSpreadsLindenmayerNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Axiom", DefaultString = "F")]
        protected IDiffSpread<string> FAxiom;
        
        [Input("Productions F", DefaultString = "F+F")]
        protected IDiffSpread<string> FProductionsF;
        
        [Input("Productions G", DefaultString = "")]
        protected IDiffSpread<string> FProductionsG;
        
        [Input("Depth", DefaultValue = 1, MinValue = 0, MaxValue = 10)]
        protected IDiffSpread<int> FDepth;
        
        [Input("Seed", DefaultValue = 1, MinValue = 0)]
        protected IDiffSpread<int> FSeed;
        
        [Input("Branch Length", DefaultValue = 0.2, MinValue = 0)]
        protected IDiffSpread<double> FBranchLength;
        
        [Input("Length Deviation", DefaultValue = 0.5, MinValue = 0)]
        protected IDiffSpread<double> FBranchLengthDeviation;
        
        [Input("Angle", DefaultValue = 0.1, MinValue = 0, MaxValue = 1)]
        protected IDiffSpread<double> FAngle;
        
        [Input("Angle Deviation", DefaultValue = 0, MinValue = 0, MaxValue = 1)]
        protected IDiffSpread<double> FAngleDeviation;
        
        [Output("Level")]
        protected ISpread<int> FLevel;
        
        [Output("G at F Slice")]
        protected ISpread<int> FGAtFSlice;
        
        [Output("Bin Sizes")]
        protected ISpread<int> FBinSizes;
        
        [Output("Bin Sizes G")]
        protected ISpread<int> FBinSizesG;
        
        [Output("Transform")]
        protected ISpread<Matrix4x4> FTransform;
        
        [Output("Transform G")]
        protected ISpread<Matrix4x4> FTransformG;
		
		[Output("Transform Slim")]
		protected ISpread<Matrix4x4> FTransformSlim;

		private List<TLindenmayer> FLindenmayers = new List<TLindenmayer>();
		private int FOldSpreadMax = 0;
		
		[Import()]
		public ILogger FLogger;
		
		#endregion field declaration
		
		public void Evaluate(int SpreadMax)
		{
			bool revaluate = false;
			
			if (FOldSpreadMax != SpreadMax)
			{
				//FLogger.Log(LogType.Debug, "spreadcount changed: " + SpreadMax.ToString());
				FLindenmayers.Clear();
				
				for (int i=0; i<SpreadMax; i++)
					FLindenmayers.Add(new TLindenmayer());
				
				FOldSpreadMax = SpreadMax;
				revaluate = true;
			}
			
			if (FAxiom.IsChanged || revaluate)
			{
				for (int i=0; i<SpreadMax; i++)
					FLindenmayers[i].Axiom = FAxiom[i];
				
				revaluate = true;
			}

			if (FProductionsF.IsChanged || revaluate)
			{
				for (int i=0; i<SpreadMax; i++)
				{
					FLindenmayers[i].ProductionsF.Clear();
					
					string s = FProductionsF[i].Trim(); 
					if (!string.IsNullOrEmpty(s))
					{
						char[] split = {','};
						string[] sa = s.Split(split);
						
						for (int j=0; j<sa.Length; j++)
							FLindenmayers[i].ProductionsF.Add(sa[j]);
					}
				}
				
				revaluate = true;
			}
			
			if (FProductionsG.IsChanged || revaluate)
			{
				for (int i=0; i<SpreadMax; i++)
				{
					FLindenmayers[i].ProductionsG.Clear();
					
					string s = FProductionsG[i].Trim(); 
					if (!string.IsNullOrEmpty(s))
					{
						char[] split = {','};
						string[] sa = s.Split(split);
						
						for (int j=0; j<sa.Length; j++)
							FLindenmayers[i].ProductionsG.Add(sa[j]);
					}
				}
				
				revaluate = true;
			}
			
			if (FDepth.IsChanged || revaluate)
			{
				for (int i=0; i<SpreadMax; i++)
					FLindenmayers[i].Depth = FDepth[i];
				
				revaluate = true;
			}
			
			if (FSeed.IsChanged || revaluate)
			{
				for (int i=0; i<SpreadMax; i++)
					FLindenmayers[i].Seed = FSeed[i];
				
				revaluate = true;
			}

			if (FBranchLength.IsChanged || revaluate)
			{
				for (int i=0; i<SpreadMax; i++)
					FLindenmayers[i].BranchLength = FBranchLength[i];
				
				revaluate = true;
			}
			
			if (FBranchLengthDeviation.IsChanged || revaluate)
			{
				for (int i=0; i<SpreadMax; i++)
					FLindenmayers[i].BranchLengthDeviation = FBranchLengthDeviation[i];
				
				revaluate = true;
			}
			
			if (FAngle.IsChanged || revaluate)
			{
				for (int i=0; i<SpreadMax; i++)
					FLindenmayers[i].Angle = FAngle[i];
				
				revaluate = true;
			}
			
			if (FAngleDeviation.IsChanged || revaluate)
			{
				for (int i=0; i<SpreadMax; i++)
					FLindenmayers[i].AngleDeviation = FAngleDeviation[i];
				
				revaluate = true;
			}
			
			if (revaluate)
			{
				int c = 0;
				int gs = 0;
				foreach (TLindenmayer lm in FLindenmayers)
				{
					lm.Evaluate();
					c += lm.BranchCount;
					gs += lm.TransformsG.Count;
				}

				FTransform.SliceCount = c;
				FTransformG.SliceCount = gs;
				FGAtFSlice.SliceCount = gs;
				FLevel.SliceCount = c;
				FBinSizes.SliceCount = FLindenmayers.Count;
				FBinSizesG.SliceCount = FLindenmayers.Count;

				int sliceF = 0;
				int sliceG = 0;
				for (int i=0; i<SpreadMax; i++)
				{
				    FBinSizes[i] = FLindenmayers[i].BranchCount;
					for (int j=0; j<FLindenmayers[i].BranchCount; j++)
					{
					    FTransform[sliceF] = (Matrix4x4) FLindenmayers[i].Transforms[j];
					    FLevel[sliceF] = (int) FLindenmayers[i].Level[j];
						sliceF++;
					}
					
					FBinSizesG[i] = FLindenmayers[i].TransformsG.Count;
					for (int j=0; j<FLindenmayers[i].TransformsG.Count; j++)
					{
					    FTransformG[sliceG] = (Matrix4x4) FLindenmayers[i].TransformsG[j];
					    FGAtFSlice[sliceG] = (int) FLindenmayers[i].GAtFSlice[j];
						sliceG++;
					}
				}
				
				if(c > 0)
				{
					List<Matrix4x4> list = new List<Matrix4x4>();
					
					for (int i=0; i<SpreadMax; i++)
					{
						for (int j=0; j<FLindenmayers[i].BranchCount; j++)
						{
							Matrix4x4 m = (Matrix4x4) FLindenmayers[i].Transforms[j];
							if(!list.Contains(m))
							{
								list.Add(m);
							}
						}
					}
					
					FTransformSlim.SliceCount = list.Count;
					for(int i = 0; i<list.Count; i++)
					{
						FTransformSlim[i] = list[i];
					}
				}
			}
		}
	}
}
