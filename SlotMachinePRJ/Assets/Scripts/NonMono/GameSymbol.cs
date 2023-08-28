using System;
using Godot;

public class GameSymbol{
    
    public Texture2D symbol_png;
    bool is_typeWinner = false;
    float multiplier;
    public bool SymbolInPlay = false;

    string Name;
    public GameSymbol(Texture2D symbol_png, bool is_typeWinner, float multiplier){
        this.symbol_png = symbol_png;
        this.is_typeWinner = is_typeWinner;
        this.multiplier = multiplier;
        Name = symbol_png.ResourcePath + "X" + multiplier.ToString();
    }   
    public string GetID(){
        return Name;
    }
      public float GetRewardStat(){
        return multiplier;
    }

}