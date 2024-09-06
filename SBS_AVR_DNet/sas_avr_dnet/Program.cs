using sas_avr_dnet;
using System.Reflection.Metadata.Ecma335;

InstructionList instructionDefinition;
try
{
	instructionDefinition = new InstructionList(InstructionLoader.FromCsv_All(@".\InstructionFormatFile.csv"));
}catch(Exception ex){
	Console.WriteLine("命令後の定義読み込みに失敗しました。");
	Console.WriteLine(ex.Message);
	return;
}

InstructionList codeInstructions;
try
{
	codeInstructions = AsmCodeLoader.FromPrimitiveAsmCode_All(@".\sample_PrimitiveAsmCode.pac",instructionDefinition);
}catch(Exception ex){
	Console.WriteLine("PrimitiveAsmCodeの翻訳に失敗しました。");
	Console.WriteLine(ex.Message);
	return;
}

Console.WriteLine("ーーーアセンブル結果：命令語毎ーーー");
uint address = 0;
foreach(Instruction inst in codeInstructions.Instructions){
	Console.WriteLine(Convert.ToString(address,16).PadLeft(4,' ') + ":" + inst.Mnimonic + ":" + inst.ToBinText());
	address += (uint)inst.ToHexText().Length / 4;
}

string intelHex = IntelHex.ToIntelHex(codeInstructions.ToHexText());

using(StreamWriter sr = new StreamWriter("./program.hex", false)){
	try
	{
		sr.Write(intelHex);
		Console.WriteLine();
		Console.WriteLine("output -> ./program.hex");
	}catch(Exception){
		Console.WriteLine("hexファイルの書き込みに失敗しました。");
	}
}
