using Godot;
public static class IO{
    public static string assetPath = "res://Assets/Models/Play Area Assets/";
	public static string SymbolTexturePath = "res://Assets/Textures/SymbolTextures/";
	public static string texturePath = "res://Assets/Textures/";

	public static string textureArtPath = "res://Assets/Textures/Art/";
	public static string textureHeadboardPath = "res://Assets/Textures/Headboard/";
	public static string textureMachineFramesPath = "res://Assets/Textures/MachineFrames/";
	public static string textureBackgroundPath = "res://Assets/Textures/Background/";

    public static class slot_machine_assets{
    public static string play_area_asset = "PlayArea001.tscn";
    public static string output_asset= "VerticalOutput001.tscn";
    }

    public static Texture2D LoadArtTexture(string texture){
        return GD.Load<Texture2D>(textureArtPath + texture);
    }
    public static Texture2D LoadHeadBoardTexture(string texture){
        return GD.Load<Texture2D>(textureHeadboardPath + texture);
    }public static Texture2D LoadMachineFrameTexture(string texture){
        return GD.Load<Texture2D>(textureMachineFramesPath + texture);
    }public static Texture2D LoadBackgroundTexture(string texture){
        return GD.Load<Texture2D>(textureBackgroundPath + texture);
    }
    public static Texture2D LoadSymbolTexture(string texture){
        return GD.Load<Texture2D>(SymbolTexturePath + texture);
    }
}