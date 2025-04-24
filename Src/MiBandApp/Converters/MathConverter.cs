
// Type: MiBandApp.Converters.MathConverter
// Assembly: MiBandApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5DE7A56E-45AD-4B21-9740-D9903F766DB3
// 

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Data;

#nullable disable
namespace MiBandApp.Converters
{
  public class MathConverter : IValueConverter
  {
    private static readonly char[] _allOperators = new char[7]
    {
      '+',
      '-',
      '*',
      '/',
      '%',
      '(',
      ')'
    };
    private static readonly List<string> _grouping = new List<string>()
    {
      "(",
      ")"
    };
    private static readonly List<string> _operators = new List<string>()
    {
      "+",
      "-",
      "*",
      "/",
      "%"
    };

    public object Convert(object value, Type targetType, object parameter, string language)
    {
      string mathEquation = (parameter as string).Replace(" ", "").Replace("@VALUE", value.ToString());
      List<double> numbers = new List<double>();
      foreach (string s in mathEquation.Split(MathConverter._allOperators))
      {
        double result;
        if (s != string.Empty && double.TryParse(s, out result))
          numbers.Add(result);
      }
      this.EvaluateMathString(ref mathEquation, ref numbers, 0);
      return (object) numbers[0];
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }

    private void EvaluateMathString(ref string mathEquation, ref List<double> numbers, int index)
    {
      for (string nextToken1 = this.GetNextToken(mathEquation); nextToken1 != string.Empty; nextToken1 = this.GetNextToken(mathEquation))
      {
        mathEquation = mathEquation.Remove(0, nextToken1.Length);
        if (MathConverter._grouping.Contains(nextToken1))
        {
          switch (nextToken1)
          {
            case "(":
              this.EvaluateMathString(ref mathEquation, ref numbers, index);
              break;
            case ")":
              return;
          }
        }
        if (MathConverter._operators.Contains(nextToken1))
        {
          try
          {
            string nextToken2 = this.GetNextToken(mathEquation);
            if (nextToken2 == "(")
              this.EvaluateMathString(ref mathEquation, ref numbers, index + 1);
            if (numbers.Count <= index + 1 || double.Parse(nextToken2) != numbers[index + 1] && !(nextToken2 == "("))
              throw new FormatException("Next token is not the expected number");
            switch (nextToken1)
            {
              case "+":
                numbers[index] = numbers[index] + numbers[index + 1];
                break;
              case "-":
                numbers[index] = numbers[index] - numbers[index + 1];
                break;
              case "*":
                numbers[index] = numbers[index] * numbers[index + 1];
                break;
              case "/":
                numbers[index] = numbers[index] / numbers[index + 1];
                break;
              case "%":
                numbers[index] = numbers[index] % numbers[index + 1];
                break;
            }
            numbers.RemoveAt(index + 1);
          }
          catch
          {
          }
        }
      }
    }

    private string GetNextToken(string mathEquation)
    {
      if (mathEquation == string.Empty)
        return string.Empty;
      string nextToken = "";
      foreach (char ch in mathEquation)
      {
        if (((IEnumerable<char>) MathConverter._allOperators).Contains<char>(ch))
          return !(nextToken == "") ? nextToken : ch.ToString();
        nextToken += ch.ToString();
      }
      return nextToken;
    }
  }
}
