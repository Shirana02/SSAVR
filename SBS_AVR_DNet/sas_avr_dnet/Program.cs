using sas_avr_dnet.AsmCode;
using sas_avr_dnet.InnerLogic_Instruction;
using sas_avr_dnet.IntelHex;
using System.Diagnostics;

InstructionList instructionDefinition;
try
{
	instructionDefinition = new InstructionList(InstructionLoader.FromCsv_All(@".\InstructionFormatFile.csv"));
}catch(Exception ex){
	Console.WriteLine("命令後の定義読み込みに失敗しました。");
	Console.WriteLine(ex.Message);
	return;
}

string srcPath_asmCodeFile = @".\SrcCode.sac";
string dstPath_primitiveAsmCodeFile = @".\SrcCode.psac";
string dstPath_machineCodeFile = @".\out.hex";
switch(args.Length){
	case 1:
		srcPath_asmCodeFile = args[0];
		break;
	case 2:
		srcPath_asmCodeFile = args[0];
		dstPath_primitiveAsmCodeFile = args[1];
		break;
	case 3:
		srcPath_asmCodeFile = args[0];
		dstPath_primitiveAsmCodeFile = args[1];
		dstPath_machineCodeFile = args[2];
		break;
}
if(args.Length == 0){ 
}else if(args.Length ==1){
}

try{
	AsmCodeLoader.MakePrimitiveAsmCodeFile(srcPath_asmCodeFile,dstPath_primitiveAsmCodeFile);
}catch(Exception ex){
	Console.WriteLine("ソースコードのプリプロセスに失敗しました。");
	Console.WriteLine(ex.Message);
	return;
}

InstructionList codeInstructions;
try
{
	codeInstructions = AsmCodeLoader.FromPrimitiveAsmCode(dstPath_primitiveAsmCodeFile,instructionDefinition);
}catch(Exception ex){
	Console.WriteLine("PrimitiveAsmCodeの翻訳に失敗しました。");
	Console.WriteLine(ex.Message);
	return;
}

Console.WriteLine("ーーーアセンブル結果ーーー");
uint address = 0;
foreach(Instruction inst in codeInstructions.Instructions){
	Console.WriteLine(Convert.ToString(address,16).PadLeft(4,' ') + ":" + inst.Mnimonic + ":" + inst.ToBinText());
	address += (uint)inst.ToHexText().Length / 4;
}

string intelHex = IntelHex.ToIntelHex(codeInstructions.ToHexText());

using(StreamWriter sr = new StreamWriter(dstPath_machineCodeFile, false)){
	try
	{
		sr.Write(intelHex);
		Console.WriteLine();
		Console.Write("output -> ./program.hex");
		Console.WriteLine(dstPath_machineCodeFile);
	}catch(Exception){
		Console.WriteLine("hexファイルの書き込みに失敗しました。");
	}
}
