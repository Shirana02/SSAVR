
using System.Text.RegularExpressions;

namespace sas_avr_dnet.AsmCode
{
	internal class AsmPreProcessor {
		/// <summary>
		/// すべての行の、'#'から行末までを削除する。
		/// </summary>
		/// <param name="srcCode"></param>
		/// <returns></returns>
		static internal string RemoveComment(string srcCode) {
			return Regex.Replace(srcCode, @"#.+$", "");
		}

		/// <summary>
		/// すべての行の、行頭で連続するタブ文字を削除する。
		/// </summary>
		/// <param name="srcCode"></param>
		/// <returns></returns>
		static internal string RemoveIndent(string srcCode){
			return Regex.Replace(srcCode,@"^[\t]+","");
		}

		/// <summary>
		/// インデント削除＋コメント削除
		/// </summary>
		/// <param name="srcCode"></param>
		/// <returns></returns>
		static internal string ExecAllPreProcess(string srcCode){
			string ret = srcCode;
			ret = RemoveComment(srcCode);
			ret = RemoveIndent(srcCode);
			return ret;
		}
	}
}
