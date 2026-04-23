// Decompiled with JetBrains decompiler
// Type: LmpClient.Utilities.Json
// Assembly: LmpClient, Version=0.29.0.574, Culture=neutral, PublicKeyToken=null
// MVID: AF13D95E-BF4A-4E52-801E-D34E0C110EFE
// Assembly location: D:\Programming\LunaMultiplayer-master\references\0.29.0\GameData\LunaMultiplayer\Plugins\LmpClient.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LmpClient.Utilities
{
  public static class Json
  {
    public static object Deserialize(string json)
    {
      try
      {
        return string.IsNullOrEmpty(json) ? (object) null : Json.Parser.Parse(json);
      }
      catch (Exception ex)
      {
        LunaLog.LogError(ex.ToString());
        return (object) null;
      }
    }

    public static string Serialize(object obj) => Json.Serializer.Serialize(obj);

    private sealed class Parser : IDisposable
    {
      private const string WordBreak = "{}[],:\"";
      private StringReader _json;

      private Parser(string jsonString) => this._json = new StringReader(jsonString);

      private char NextChar => Convert.ToChar(this._json.Read());

      private Json.Parser.Token NextToken
      {
        get
        {
          this.EatWhitespace();
          if (this._json.Peek() == -1)
            return Json.Parser.Token.None;
          switch (this.PeekChar)
          {
            case '"':
              return Json.Parser.Token.String;
            case ',':
              this._json.Read();
              return Json.Parser.Token.Comma;
            case '-':
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
              return Json.Parser.Token.Number;
            case ':':
              return Json.Parser.Token.Colon;
            case '[':
              return Json.Parser.Token.SquaredOpen;
            case ']':
              this._json.Read();
              return Json.Parser.Token.SquaredClose;
            case '{':
              return Json.Parser.Token.CurlyOpen;
            case '}':
              this._json.Read();
              return Json.Parser.Token.CurlyClose;
            default:
              string lower = this.NextWord.ToLower();
              if (lower == "false")
                return Json.Parser.Token.False;
              if (lower == "true")
                return Json.Parser.Token.True;
              return lower == "null" ? Json.Parser.Token.Null : Json.Parser.Token.None;
          }
        }
      }

      private string NextWord
      {
        get
        {
          StringBuilder stringBuilder = new StringBuilder();
          while (!Json.Parser.IsWordBreak(this.PeekChar))
          {
            stringBuilder.Append(this.NextChar);
            if (this._json.Peek() == -1)
              break;
          }
          return stringBuilder.ToString();
        }
      }

      private char PeekChar => Convert.ToChar(this._json.Peek());

      private static bool IsWordBreak(char c) => char.IsWhiteSpace(c) || "{}[],:\"".IndexOf(c) != -1;

      public static object Parse(string jsonString)
      {
        using (Json.Parser parser = new Json.Parser(jsonString))
          return parser.ParseValue();
      }

      public void Dispose()
      {
        this._json.Dispose();
        this._json = (StringReader) null;
      }

      private void EatWhitespace()
      {
        while (char.IsWhiteSpace(this.PeekChar))
        {
          this._json.Read();
          if (this._json.Peek() == -1)
            break;
        }
      }

      private List<object> ParseArray()
      {
        List<object> array = new List<object>();
        this._json.Read();
        bool flag = true;
        while (flag)
        {
          Json.Parser.Token nextToken = this.NextToken;
          switch (nextToken)
          {
            case Json.Parser.Token.None:
              return (List<object>) null;
            case Json.Parser.Token.SquaredClose:
              flag = false;
              break;
            case Json.Parser.Token.Comma:
              continue;
            default:
              object byToken = this.ParseByToken(nextToken);
              array.Add(byToken);
              break;
          }
        }
        return array;
      }

      private object ParseByToken(Json.Parser.Token token)
      {
        switch (token)
        {
          case Json.Parser.Token.CurlyOpen:
            return (object) this.ParseObject();
          case Json.Parser.Token.SquaredOpen:
            return (object) this.ParseArray();
          case Json.Parser.Token.String:
            return (object) this.ParseString();
          case Json.Parser.Token.Number:
            return this.ParseNumber();
          case Json.Parser.Token.True:
            return (object) true;
          case Json.Parser.Token.False:
            return (object) false;
          case Json.Parser.Token.Null:
            return (object) null;
          default:
            return (object) null;
        }
      }

      private object ParseNumber()
      {
        string nextWord = this.NextWord;
        if (nextWord.IndexOf('.') == -1)
        {
          long result;
          long.TryParse(nextWord, out result);
          return (object) result;
        }
        double result1;
        double.TryParse(nextWord, out result1);
        return (object) result1;
      }

      private Dictionary<string, object> ParseObject()
      {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        this._json.Read();
        while (true)
        {
          Json.Parser.Token nextToken;
          do
          {
            nextToken = this.NextToken;
            if (nextToken != Json.Parser.Token.None)
            {
              if (nextToken == Json.Parser.Token.CurlyClose)
                goto label_4;
            }
            else
              goto label_3;
          }
          while (nextToken == Json.Parser.Token.Comma);
          string key = this.ParseString();
          if (key != null && this.NextToken == Json.Parser.Token.Colon)
          {
            this._json.Read();
            dictionary[key] = this.ParseValue();
          }
          else
            goto label_6;
        }
label_3:
        return (Dictionary<string, object>) null;
label_4:
        return dictionary;
label_6:
        return (Dictionary<string, object>) null;
      }

      private string ParseString()
      {
        StringBuilder stringBuilder = new StringBuilder();
        this._json.Read();
        bool flag = true;
        while (flag && this._json.Peek() != -1)
        {
          char nextChar1 = this.NextChar;
          switch (nextChar1)
          {
            case '"':
              flag = false;
              break;
            case '\\':
              if (this._json.Peek() == -1)
              {
                flag = false;
                break;
              }
              char nextChar2 = this.NextChar;
              switch (nextChar2)
              {
                case '"':
                case '/':
                case '\\':
                  stringBuilder.Append(nextChar2);
                  break;
                case 'b':
                  stringBuilder.Append('\b');
                  break;
                case 'f':
                  stringBuilder.Append('\f');
                  break;
                case 'n':
                  stringBuilder.Append('\n');
                  break;
                case 'r':
                  stringBuilder.Append('\r');
                  break;
                case 't':
                  stringBuilder.Append('\t');
                  break;
                case 'u':
                  char[] chArray = new char[4];
                  for (int index = 0; index < 4; ++index)
                    chArray[index] = this.NextChar;
                  stringBuilder.Append((char) Convert.ToInt32(new string(chArray), 16));
                  break;
              }
              break;
            default:
              stringBuilder.Append(nextChar1);
              break;
          }
        }
        return stringBuilder.ToString();
      }

      private object ParseValue() => this.ParseByToken(this.NextToken);

      private enum Token
      {
        None,
        CurlyOpen,
        CurlyClose,
        SquaredOpen,
        SquaredClose,
        Colon,
        Comma,
        String,
        Number,
        True,
        False,
        Null,
      }
    }

    private sealed class Serializer
    {
      private readonly StringBuilder _builder;

      private Serializer() => this._builder = new StringBuilder();

      public static string Serialize(object obj)
      {
        Json.Serializer serializer = new Json.Serializer();
        serializer.SerializeValue(obj);
        return serializer._builder.ToString();
      }

      private void SerializeArray(IList anArray)
      {
        this._builder.Append('[');
        bool flag = true;
        foreach (object an in (IEnumerable) anArray)
        {
          if (!flag)
            this._builder.Append(',');
          this.SerializeValue(an);
          flag = false;
        }
        this._builder.Append(']');
      }

      private void SerializeObject(IDictionary obj)
      {
        bool flag = true;
        this._builder.Append('{');
        foreach (object key in (IEnumerable) obj.Keys)
        {
          if (!flag)
            this._builder.Append(',');
          this.SerializeString(key.ToString());
          this._builder.Append(':');
          this.SerializeValue(obj[key]);
          flag = false;
        }
        this._builder.Append('}');
      }

      private void SerializeOther(object value)
      {
        int num1;
        switch (value)
        {
          case float num2:
            this._builder.Append(num2.ToString("R"));
            return;
          case int _:
          case uint _:
          case long _:
          case sbyte _:
          case byte _:
          case short _:
          case ushort _:
            num1 = 1;
            break;
          default:
            num1 = value is ulong ? 1 : 0;
            break;
        }
        if (num1 != 0)
          this._builder.Append(value);
        else if (value is double || value is Decimal)
          this._builder.Append(Convert.ToDouble(value).ToString("R"));
        else
          this.SerializeString(value.ToString());
      }

      private void SerializeString(string str)
      {
        this._builder.Append('"');
        foreach (char ch in str.ToCharArray())
        {
          switch (ch)
          {
            case '\b':
              this._builder.Append("\\b");
              break;
            case '\t':
              this._builder.Append("\\t");
              break;
            case '\n':
              this._builder.Append("\\n");
              break;
            case '\f':
              this._builder.Append("\\f");
              break;
            case '\r':
              this._builder.Append("\\r");
              break;
            case '"':
              this._builder.Append("\\\"");
              break;
            case '\\':
              this._builder.Append("\\\\");
              break;
            default:
              int int32 = Convert.ToInt32(ch);
              if (int32 >= 32 && int32 <= 126)
              {
                this._builder.Append(ch);
                break;
              }
              this._builder.Append("\\u");
              this._builder.Append(int32.ToString("x4"));
              break;
          }
        }
        this._builder.Append('"');
      }

      private void SerializeValue(object value)
      {
        switch (value)
        {
          case null:
            this._builder.Append("null");
            break;
          case string str:
            this.SerializeString(str);
            break;
          case bool flag:
            this._builder.Append(flag ? "true" : "false");
            break;
          case IList anArray:
            this.SerializeArray(anArray);
            break;
          case IDictionary dictionary:
            this.SerializeObject(dictionary);
            break;
          case char c:
            this.SerializeString(new string(c, 1));
            break;
          default:
            this.SerializeOther(value);
            break;
        }
      }
    }
  }
}
