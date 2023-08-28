using Godot;
using System;
public static class SlotFunctions{

	public static Texture2D[] RNGOutput(Texture2D[] inputs, int NumOfOutputs){
		Texture2D[] output = new Texture2D[NumOfOutputs];
		var rng = new Godot.RandomNumberGenerator();
		while(NumOfOutputs > 0){
			NumOfOutputs--;
			rng.Randomize();
			int index = (int)MathF.Round(rng.RandfRange(0,inputs.Length-1));
			output[NumOfOutputs] = inputs[index];
		}

			return output;
	}
	public static GameSymbol[] PickThree(GameSymbol[] AllSymbols){
		var rng = new Godot.RandomNumberGenerator();
		
		int size = 3;
		GameSymbol[] selected = new GameSymbol[size];
		while (size > 0){
			rng.Randomize();
			GameSymbol attemptedSelection = AllSymbols[rng.RandiRange(0,AllSymbols.Length-1)];
			if(attemptedSelection.SymbolInPlay == false){
				size--;
				attemptedSelection.SymbolInPlay = true;
				selected[size] = attemptedSelection;
			}
			
		}

		return selected;
	}
	

}
