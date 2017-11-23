using System.Collections;
using System.Linq;
using Microsoft.Ajax.Utilities;
using OfficeOpenXml;
using Utils.ExpressionTree.Extensions;
using System;
using OfficeOpenXml.Style;
using Utils.Universal;
using System.Collections.Generic;

namespace Utils.UnionPay
{
	public static class ExcelExtensions
	{
		public static ExcelRange GetRange(this ExcelWorksheet sheet, string name, int colEnd = 0)
		{
			return sheet.Cells.GetRange($"{sheet.Name}_{name}", colEnd);
		}

		public static string[] GetRange(this ExcelWorksheet sheet, string[] locator, string FillOfX)
		{
			var leftTop = locator[0];
			var leftBottom = locator[1];
			var rightBottom = locator[2];

			var leftStart = leftTop.Split("#")[0];
			var leftStartOffset = leftTop.Split("#")[1];

			var leftEnd = leftBottom.Split("#")[0];
			var leftEndOffset = leftBottom.Split("#")[1];

			var rightEnd = rightBottom.Split("#")[0];
			var rightEndOffset = rightBottom.Split("#")[1];

			var result = new List<string>();

			sheet.Cells.ForEach(c =>
			{
				if (c.NotNullOrEmpty() && !(c.Style.Fill.BackgroundColor.Rgb == FillOfX))
				{
					if (c.WithValue(leftStart))
					{
						leftStart = UniversalTools.Md5Str(leftStart);
						result.AddRange(c.MoveOffset(leftStartOffset, FillOfX).Address.AddressToNumber(true).Seperate());
					}

					if (c.WithValue(leftEnd))
					{
						if(result.Count >= 2)
						{
							var a = int.Parse(result[1]) + 1;
							leftEnd = UniversalTools.Md5Str(leftEnd);
							result.Add(a.ToString());
							result.Add(c.MoveOffset(leftEndOffset, FillOfX).Address.AddressToNumber(true).Seperate().ToArray()[1]);
						}
					}
					
					if (c.WithValue(rightEnd))
					{
						if(result.Count > 0)
						{
							var a = GetColumnLetter(result[0].Select(q => q - 'A' + 2).Aggregate((sum, next) => sum * 26 + next));
							rightEnd = UniversalTools.Md5Str(rightEnd);
							result.Add(a);
							result.Add(c.MoveOffset(rightEndOffset, FillOfX).Address.AddressToNumber(true).Seperate().ToArray()[0]);
						}
					}
				}
			});

			return result.Take(6).ToArray();
		}

		public static ExcelRange GetRange(this ExcelWorksheet sheet, int[] index)
		{
			return sheet.Cells.GetRange(index);
		}

		public static int[] ExpandRow(this int[] index, int offset)
		{
			var newIndex = index;
			newIndex[2] += offset;
			return newIndex;
		}

		public static void SetStyle(this ExcelRangeBase cell, string format)
		{
			cell.Style.Numberformat.Format = format;
		}

		public static void SetWidth(this ExcelColumn column, double width)
		{
			double num1 = width >= 1.0 ? Math.Round((Math.Round(7.0 * (width - 0.0), 0) - 5.0) / 7.0, 2) : Math.Round((Math.Round(12.0 * (width - 0.0), 0) - Math.Round(5.0 * width, 0)) / 12.0, 2);
			double num2 = width - num1;
			double num3 = width >= 1.0 ? Math.Round(7.0 * num2 - 0.0, 0) / 7.0 : Math.Round(12.0 * num2 - 0.0, 0) / 12.0 + 0.0;
			if (num1 > 0.0)
				column.Width = width + num3;
			else
				column.Width = 0.0;
		}

		//
		// Summary:
		//     Expand the current range index to new index, only expand column range
		//
		// Parameters:
		//	 index:
		//		index of current range
		//   offset:
		//     positive for right expandation, negative for left expandation
		public static int[] ExpandColumn(this int[] index, int offset)
		{
			var newIndex = index;
			newIndex[3] += offset;
			return newIndex;
		}

		public static ExcelRangeBase MoveOffset(this ExcelRangeBase cell, string indecator, string FillOfX)
		{
			ExcelRangeBase newCell = null;

			if (indecator[1].Equals('X'))
			{

			}
			else
			{
				var index = int.Parse(indecator[1].ToString());

				if (indecator.Contains("v", "^"))
				{
					if (indecator[0].Equals('^'))
					{
						index = -index;
					}

					var newCellIndex = cell.GetRangeIndex().MoveRow(index);
					newCell = cell.Worksheet.GetRange(newCellIndex);
				}

				if (indecator.Contains("<", ">"))
				{
					if (indecator[0].Equals('<'))
					{
						index = -index;
					}

					var newCellIndex = cell.GetRangeIndex().MoveColumn(index);
					newCell = cell.Worksheet.GetRange(newCellIndex);
				}
			}

			return newCell;
		}

		//
		// Summary:
		//     Move the current range index to another index
		//
		// Parameters:
		//	 index:
		//		index of current range
		//   offset:
		//     positive for move right, negative for move left
		public static int[] MoveColumn(this int[] index, int offset)
		{
			var newIndex = index;
			newIndex[1] += offset;
			newIndex[3] += offset;
			return newIndex;
		}

		public static ExcelRangeBase MoveColumn(this ExcelRangeBase cell, int offset)
		{
			var newCellIndex = cell.GetRangeIndex().MoveColumn(offset);
			var newCell = cell.Worksheet.GetRange(newCellIndex);
			return newCell;
		}

		//
		// Summary:
		//     Move the current range index to another index
		//
		// Parameters:
		//	 index:
		//		index of current range
		//   offset:
		//     positive for move down, negative for move up
		public static int[] MoveRow(this int[] index, int offset)
		{
			var newIndex = index;
			newIndex[0] += offset;
			newIndex[2] += offset;
			return newIndex;
		}


		//
		// Summary:
		//     copy the style of old range to new range
		//
		// Parameters:
		//	 to:
		//		the new range where style will applied to
		//   from:
		//     the old range where style will be copied
		public static ExcelRange CopyStyleFrom(this ExcelRange to, ExcelRange from, bool withValue = false)
		{
			var fromIndex = from.GetRangeIndex();
			var toIndex = to.GetRangeIndex();
			var offset = fromIndex[3] - fromIndex[1] + 1;
			var range = toIndex[3] - toIndex[1] + 1;
			var rest = range % offset;
			var loop = (range - rest) / offset;
			var index = 0;
			for (; index < loop; index++)
			{
				var tmpRange = to.GetRange(new[] { fromIndex[0], fromIndex[1] + (index + 1) * offset, fromIndex[2], fromIndex[3] + (index + 1) * offset });
				from.Copy(tmpRange);
				if (!withValue)
				{
					tmpRange.ForEach(t => t.Value = "");
				}
			}

			if (rest > 0)
			{
				var restRange = to.GetRange(new[] { fromIndex[0], fromIndex[1] + index * offset, fromIndex[2], fromIndex[1] + index * offset + rest - 1 });
				var x = from.Start.Address.AddressToNumber();
				var y = from.End.Address.AddressToNumber();
				var restFrom = from[x[0], x[1], y[0], x[1] + rest - 1];
				restFrom.Copy(restRange);
				if (!withValue)
				{
					restRange.ForEach(t => t.Value = "");
				}
			}

			return to;
		}

		public static int[] GetRangeIndex(this ExcelRange range)
		{
			#region deprecated
			//			var add = range.Address;
			//			var start = add.Split(':')[0].AddressToNumber();
			//			var end = add.Split(':')[1].AddressToNumber();
			//			return new[] { start[0], start[1], end[0], end[1] }; // RowST, ColST, RowED, ColED
			#endregion

			return new[] { range.Start.Row, range.Start.Column, range.End.Row, range.End.Column };
		}

		public static int[] GetRangeIndex(this ExcelRangeBase range)
		{
			#region deprecated
			//			var add = range.Address;
			//			var start = add.Split(':')[0].AddressToNumber();
			//			var end = add.Split(':')[1].AddressToNumber();
			//			return new[] { start[0], start[1], end[0], end[1] }; // RowST, ColST, RowED, ColED
			#endregion

			return new[] { range.Start.Row, range.Start.Column, range.End.Row, range.End.Column };
		}

		public static ExcelRange GetRange(this ExcelRange range, int[] index)
		{
			return range[index[0], index[1], index[2], index[3]];
		}

		public static ExcelRange GetRange(this ExcelRange range, string type, int offset = 0)
		{
			int[] index; // RowST, ColST, RowED, ColED
			switch (type)
			{
				case "Revenue Estimates_Exp":
					index = new[] { 4, 2, 13, 5 };
					break;
				case "Summary_Exp":
					index = new[] { 4, 2, 12, 2 };
					break;
				case "Value by city_Exp":
					index = new[] { 3, 2, offset, 7 };
					break;
				case "Consumer Profile_Exp":
					index = new[] { 4, 2, 37, 6 };
					break;
				default:
					index = new[] { 1, 1, 1, 1 };
					break;
			}
			return range.GetRange(index);
		}

		public static void ReplaceValue(this ExcelRange range, string target, string src)
		{
			range.Value = range.Value.ToString().Replace(target, src);
		}

		public static void ReplaceValue(this ExcelRangeBase range, string target, string src)
		{
			if (range.Value == null) return;
			range.Value = range.Value.ToString().Replace(target, src);
		}

		public static void SetValue(this ExcelRange range, string src)
		{
			range.Value = src;
		}

		public static void SetValue(this ExcelRangeBase range, object src)
		{
			range.Value = src;
		}

		public static bool WithValue(this ExcelRangeBase range, string src)
		{
			return range.Value != null && range.Value.ToString().Contains(src);
		}

		public static bool NotNullOrEmpty(this ExcelRangeBase range)
		{
			return range.Value != null && !range.Value.Equals("");
		}

		public static bool WithValue(this ExcelRangeBase range, params string[] src)
		{
			return range.Value != null && range.Value.ToString().In(src);
		}

		public static void InsertRowsBelow(this ExcelRangeBase range, IList YIndex, int XSize = 0)
		{
			var rcs = range.Start.Address.AddressToNumber();
			var rce = range.End.Address.AddressToNumber();
			range.Worksheet.InsertRow(rcs[0] + 1, YIndex.Count, 2);
			for (var i = 0; i < YIndex.Count; i++)
			{
				range.Worksheet.SetValue(i + rcs[0] + 1, rcs[1], YIndex[i]);
				for (var j = 0; j < XSize; j++)
				{
					range.Worksheet.SetValue(i + rcs[0] + 1, rcs[1] + 1 + j, "1"); // You must set value to newly inserted cells, otherwise the sheet cant get its range address
				}
			}
		}

		public static bool MultiArrayEquals<T>(this T[,] left, T[,] right)
		{
			var equal =
				left.Rank == right.Rank &&
				Enumerable.Range(0, left.Rank).All(dimension => left.GetLength(dimension) == right.GetLength(dimension)) &&
				left.Cast<T>().SequenceEqual(right.Cast<T>());
			return equal;
		}

		public static void Remove(this ExcelRangeBase range)
		{
			range.Worksheet.DeleteRow(range.Start.Address.AddressToNumber()[0]);
		}

		public static int[] AddressToNumber(this string text, bool reverse = false)
		{
			var col = text.Where(char.IsLetter).Select(c => c - 'A' + 1).Aggregate((sum, next) => sum * 26 + next);
			var row = int.Parse(string.Join("", text.Where(char.IsDigit)));
			return reverse ? new[] { col, row } : new[] { row, col };
		}

		public static string GetColumnLetter(int column)
		{
			if (column < 1) return string.Empty;
			return GetColumnLetter((column - 1) / 26) + (char)('A' + (column - 1) % 26);
		}

		public static IEnumerable<string> Seperate(this int[] addr)
		{
			return addr.Select((a, index) => index == 0 ? GetColumnLetter(a) : a.ToString());
		}

		public static string NumberToAddress(this int[] index)
		{
			var CS = GetColumnLetter(index[1]);
			var CE = GetColumnLetter(index[3]);
			return $"{CS}{index[0]}:{CE}{index[2]}";
		}
	}
}