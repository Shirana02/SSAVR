
using System.Text;

namespace sas_avr_dnet {
	internal class InstructionList {
		internal List<Instruction> Instructions;

		internal InstructionList(List<Instruction> instList){
			Instructions = instList;
		}


		internal string ToHexText(){
			StringBuilder hexTextBld = new StringBuilder(256);
			foreach(Instruction inst in Instructions){
				hexTextBld.Append(inst.ToHexText());
			}
			return hexTextBld.ToString();
		}

		/// <summary>
		/// Instructionを名前検索してディープコピー。存在しない場合はArgumentExceptionが発生する。
		/// </summary>
		/// <param name="mnimonic"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		internal Instruction GetCopy(string mnimonic){
			foreach(Instruction inst in Instructions){
				if(inst.Mnimonic == mnimonic){
					return new Instruction(inst);
				}
			}
			throw new ArgumentException("コピーしたいMnimonicがInstructionListに存在しません。");
		}
	}
}
