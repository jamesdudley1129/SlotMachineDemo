using Godot;   
using System;
using System.Collections.Generic;
public class SlotMachineSampler
	{
		public static int indexChange = 0;
		Button Left;
		Button Right;
		public static float Speed = 2.5f;//must be positive
		public static int snapDistance = 8;//must be atleast twice the speed
		public static float MaxScale = 1f;
		public static float minScale = .7f;
		public static Vector2 spaceing = new Vector2();
		public static Vector2[] OriginalPos = new Vector2[3];
		//left = 0,middle = 1, right = 2
		public static Vector2[] OriginalScale = new Vector2[3];
		//min = 0, max = 1
		public SlotSample[] Samples = new SlotSample[3];
		public static List<API.GameDataObj> games = new List<API.GameDataObj>();
        public int GameSampleIndex;
		public void FirstRender(){
			if(games.Count >= 3){
                GameSampleIndex = (int)Mathf.Round(games.Count/2);
            }
            Samples[0].AssignedObject = games[GameSampleIndex -1];
            Samples[1].AssignedObject = games[GameSampleIndex];
            Samples[2].AssignedObject = games[GameSampleIndex +1];
            foreach(SlotSample smp in Samples){
                smp.DrawGame();
            }
        }
        public void UpdatePos(){
			if(Left.ButtonPressed){
				indexChange = -1;
			}
			if(Right.ButtonPressed){
				indexChange = 1;
			}
			foreach(SlotSample smp in Samples){
				if(smp.InPos == true)
				{
					//change target
					int index = smp.GetIndexOfPosition(OriginalPos);
					if(indexChange > 0){
						if(index < OriginalPos.Length -1 ){
							index += indexChange;							
						}
						else{
							index = 0;
						}
					}
					if(indexChange < 0)
					{
						if(index > 0){
							index += indexChange;							
						}
						else{
							index = 2;
						}
					}
					smp.UpdateTarget(OriginalPos[index],OriginalScale[index]);
				}
			}
			if(indexChange >= 1){
				foreach(SlotSample smp in Samples){
					smp.UpdatePos(new Vector2( 1 * Speed,0));
				}
			}
			if(indexChange <= -1){
				foreach(SlotSample smp in Samples){
					smp.UpdatePos(new Vector2( -1 * Speed,0));
				}
			}
		}
		public SlotMachineSampler(ReferenceRect root){
			
			Samples[0] = new SlotSample(root.GetNode<ReferenceRect>("Node1"));
			OriginalPos[0] = Samples[0].rect.Position;
			Samples[0].rect.Scale = Vector2.One * minScale;//overides the scale from script
			OriginalScale[0] = Samples[0].rect.Scale;
			
			Samples[1] = new SlotSample(root.GetNode<ReferenceRect>("Node2"));
			OriginalPos[1] = Samples[1].rect.Position;
			Samples[1].rect.Scale = Vector2.One * MaxScale;//overides the scale from script
			OriginalScale[1] = Samples[1].rect.Scale;
			
			Samples[2] = new SlotSample(root.GetNode<ReferenceRect>("Node3"));
			OriginalPos[2] = Samples[2].rect.Position;
			Samples[2].rect.Scale = Vector2.One * minScale;//overides the scale from script
			OriginalScale[2] = Samples[2].rect.Scale;
			
			spaceing = new Vector2((OriginalPos[0] - OriginalPos[1]).Abs().X,0);
			Left = root.GetNode<Button>("leftbutton");
			Right = root.GetNode<Button>("rightbutton");
			
		}
		public API.GameDataObj GetCenterSample(){
			API.GameDataObj slot_smp = null;
			foreach(SlotSample sample in Samples){
				if(sample.Targetpos == OriginalPos[1]){
					slot_smp = sample.AssignedObject;
				}
			}
			return slot_smp;
		}
		public class SlotSample{
			public ReferenceRect rect;
			Vector2 StartingPos;		
			Vector2 StartingScale;	
			public Vector2 Targetpos;
			Vector2 TargetScale;
			Vector2 Dir;
            public API.GameDataObj AssignedObject;
			public bool InPos = true;
			public SlotSample(ReferenceRect x){
				rect = x;
				StartingPos = x.Position;
				StartingScale = x.Scale;
			}
			public void UpdateTarget(Vector2 pos,Vector2 scale){
				Targetpos = pos;
				TargetScale = scale;
				InPos = false;
			}
			public void UpdatePos(Vector2 dir){
				if((rect.Position - Targetpos).Abs().X < snapDistance){
					Snap();
				}else{
					CycleNextSample(dir);
				}
			}
			public void Snap(){
				rect.Position = Targetpos;
				//rect.Scale = TargetScale;
				StartingPos = Targetpos;
				StartingScale = TargetScale;
				Targetpos = Vector2.Zero;
				TargetScale = Vector2.Zero;
				indexChange = 0;
				InPos = true;
			}
			public void CycleNextSample(Vector2 dir){
                CheckAndUpdateBounds(dir);
				rect.Position += dir * Speed;
				if (StartingPos == OriginalPos[1] || Targetpos == OriginalPos[1]){
						//slowly adjust the scale based on how close it is to its target
						float MaxDistance = (OriginalPos[1] - spaceing).Abs().X;
						float MinDistance = 0;

						float currentDistance = (rect.Position - OriginalPos[1]).Abs().X; 
						
						if(currentDistance <= MaxDistance){
							float value_XModifier = Mathf.Abs((currentDistance/MaxDistance)-1);

							float scale = MathF.Abs(value_XModifier * (minScale-1)) + minScale;
							GD.PrintErr("scale =" +scale+"| current distance =" + currentDistance + "Max Distance =" + MaxDistance);
							Vector2 sc  = Vector2.One *scale;
							rect.Scale = sc; 
						}
						
					}
			}
            public void CheckAndUpdateBounds(Vector2 dir){
					// detection for going out of screen >>
				if(dir.X > 0){
					if((rect.Position + (dir * Speed)) > OriginalPos[2] + spaceing/2) {
                        GetNextGameSample(1);
						rect.Position = OriginalPos[0] - spaceing /2;
					}
				}// detection for going out of screen <<
				if(dir.X <0) 
				{
					if((rect.Position + (dir * Speed)) < OriginalPos[0] - spaceing/2){
                        GetNextGameSample(-1);
						rect.Position = OriginalPos[2] + spaceing /2;
					}	
						
				}	
            }
			public void GetNextGameSample(int index_change){
                
                int currnet_index = SlotMachineSampler.games.IndexOf(AssignedObject);
                
                if(index_change == -1){
                    if(currnet_index + index_change > 0){
                        currnet_index += index_change;
                    }else{
                        currnet_index = SlotMachineSampler.games.Count-1;
                    }
                }
                else if(index_change == 1){
                    if(currnet_index + index_change < SlotMachineSampler.games.Count){
                        currnet_index += index_change;
                    }else{
                        currnet_index = 0;
                    }
                }
                AssignedObject = SlotMachineSampler.games[currnet_index];
                DrawGame();
            }
            public int GetIndexOfPosition(Vector2[] Position_Source){
				int index = new int();
				int counter = 0;
				foreach(Vector2 vec in Position_Source){
					if(vec <= rect.Position + (Vector2.One * snapDistance) && vec >= rect.Position - (Vector2.One * snapDistance)){
						index = counter;
						break;
					}else{
						counter++;
					}
				}
				return index;
			}
			public void DrawGame(){
				rect.GetNode<TextureRect>("Art").Texture = IO.LoadArtTexture(AssignedObject.artTexture);
				rect.GetNode<TextureRect>("Headboard").Texture = IO.LoadHeadBoardTexture(AssignedObject.headBoardTexture);
				rect.GetNode<TextureRect>("Screen").Texture = IO.LoadBackgroundTexture(AssignedObject.backgroundTexture);
				rect.GetNode<TextureRect>("Frame2").Texture = IO.LoadMachineFrameTexture(AssignedObject.frameTexture);
			}
		}

	}