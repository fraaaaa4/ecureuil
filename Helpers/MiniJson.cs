using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.IO;

namespace Ecureuil.Core.Helpers {
  /// <summary>
  /// Parser JSON leggero e compatto compatibile con .NET 2.0 / C# 2.0 (senza dipendenze esterne).
  /// </summary>
  public static class MiniJson {
    public static object Deserialize(string json) {
      if (json == null) return null;
      return Parser.Parse(json);
    }

    public static T Deserialize<T>(string json) {
      object parsed = Deserialize(json);
      return (T)ConvertValue(parsed, typeof(T));
    }

    public static string Serialize(object obj) {
      return Serializer.Serialize(obj);
    }

    private static object ConvertValue(object val, Type targetType) {
      if (val == null) {
        if (targetType.IsValueType) return Activator.CreateInstance(targetType);
        return null;
      }

      Type underlying = Nullable.GetUnderlyingType(targetType);
      if (underlying != null) {
        return ConvertValue(val, underlying);
      }

      if (targetType.IsAssignableFrom(val.GetType())) {
        return val;
      }

      if (targetType == typeof(string)) {
        return val.ToString();
      }

      if (targetType == typeof(int)) return Convert.ToInt32(val);
      if (targetType == typeof(long)) return Convert.ToInt64(val);
      if (targetType == typeof(double)) return Convert.ToDouble(val, CultureInfo.InvariantCulture);
      if (targetType == typeof(float)) return Convert.ToSingle(val, CultureInfo.InvariantCulture);
      if (targetType == typeof(bool)) return Convert.ToBoolean(val);
      if (targetType == typeof(DateTime)) {
        DateTime dt;
        if (DateTime.TryParse(val.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) {
          return dt;
        }
        if (DateTime.TryParse(val.ToString(), out dt)) {
          return dt;
        }
        return DateTime.MinValue;
      }

      // Se il target e una List<T> ma il JSON ha fornito un singolo valore (es: "category": "Utilities")
      if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>)) {
        Type elemType = targetType.GetGenericArguments()[0];
        IList targetList = (IList)Activator.CreateInstance(targetType);

        if (val is List<object>) {
          List<object> list = (List<object>)val;
          for (int i = 0; i < list.Count; i++) {
            targetList.Add(ConvertValue(list[i], elemType));
          }
        } else {
          targetList.Add(ConvertValue(val, elemType));
        }
        return targetList;
      }

      if (val is Dictionary<string, object>) {
        Dictionary<string, object> dict = (Dictionary<string, object>)val;
        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
          Type valType = targetType.GetGenericArguments()[1];
          IDictionary targetDict = (IDictionary)Activator.CreateInstance(targetType);
          foreach (KeyValuePair<string, object> kvp in dict) {
            targetDict.Add(kvp.Key, ConvertValue(kvp.Value, valType));
          }
          return targetDict;
        }

        object instance = Activator.CreateInstance(targetType);
        PropertyInfo[] props = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (KeyValuePair<string, object> kvp in dict) {
          string k = kvp.Key;
          bool matched = false;
          for (int i = 0; i < props.Length; i++) {
            if (string.Compare(props[i].Name, k, true) == 0 && props[i].CanWrite) {
              props[i].SetValue(instance, ConvertValue(kvp.Value, props[i].PropertyType), null);
              matched = true;
              break;
            }
          }
          if (!matched) {
            for (int i = 0; i < fields.Length; i++) {
              if (string.Compare(fields[i].Name, k, true) == 0) {
                fields[i].SetValue(instance, ConvertValue(kvp.Value, fields[i].FieldType));
                break;
              }
            }
          }
        }
        return instance;
      }

      return val;
    }

    private sealed class Parser : IDisposable {
      const string WORD_BREAK = "{}[],:\"";
      public static bool IsWordBreak(char c) {
        return char.IsWhiteSpace(c) || WORD_BREAK.IndexOf(c) != -1;
      }

      private StringReader json;

      private Parser(string jsonString) {
        json = new StringReader(jsonString);
      }

      public static object Parse(string jsonString) {
        using (Parser instance = new Parser(jsonString)) {
          return instance.ParseValue();
        }
      }

      public void Dispose() {
        if (json != null) {
          json.Dispose();
          json = null;
        }
      }

      private Dictionary<string, object> ParseObject() {
        Dictionary<string, object> table = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        json.Read(); // consume '{'

        while (true) {
          switch (NextToken) {
            case TOKEN.NONE:
              return null;
            case TOKEN.COMMA:
              continue;
            case TOKEN.CURLY_CLOSE:
              return table;
            default:
              string name = ParseString();
              if (name == null) return null;
              if (NextToken != TOKEN.COLON) return null;
              json.Read(); // consume ':'
              table[name] = ParseValue();
              break;
          }
        }
      }

      private List<object> ParseArray() {
        List<object> array = new List<object>();
        json.Read(); // consume '['
        bool parsing = true;
        while (parsing) {
          TOKEN nextToken = NextToken;
          switch (nextToken) {
            case TOKEN.NONE:
              return null;
            case TOKEN.COMMA:
              continue;
            case TOKEN.SQUARED_CLOSE:
              parsing = false;
              break;
            default:
              object val = ParseByToken(nextToken);
              array.Add(val);
              break;
          }
        }
        return array;
      }

      private object ParseValue() {
        TOKEN nextToken = NextToken;
        return ParseByToken(nextToken);
      }

      private object ParseByToken(TOKEN token) {
        switch (token) {
          case TOKEN.STRING:
            return ParseString();
          case TOKEN.NUMBER:
            return ParseNumber();
          case TOKEN.CURLY_OPEN:
            return ParseObject();
          case TOKEN.SQUARED_OPEN:
            return ParseArray();
          case TOKEN.TRUE:
            return true;
          case TOKEN.FALSE:
            return false;
          case TOKEN.NULL:
            return null;
          default:
            return null;
        }
      }

      private string ParseString() {
        StringBuilder s = new StringBuilder();
        char c;
        json.Read(); // consume '"'

        bool parsing = true;
        while (parsing) {
          if (json.Peek() == -1) break;
          c = NextChar;
          if (c == '"') {
            parsing = false;
          } else if (c == '\\') {
            if (json.Peek() == -1) break;
            c = NextChar;
            switch (c) {
              case '"':
              case '\\':
              case '/':
                s.Append(c);
                break;
              case 'b':
                s.Append('\b');
                break;
              case 'f':
                s.Append('\f');
                break;
              case 'n':
                s.Append('\n');
                break;
              case 'r':
                s.Append('\r');
                break;
              case 't':
                s.Append('\t');
                break;
              case 'u':
                char[] hex = new char[4];
                for (int i = 0; i < 4; i++) hex[i] = NextChar;
                s.Append((char)Convert.ToInt32(new string(hex), 16));
                break;
            }
          } else {
            s.Append(c);
          }
        }
        return s.ToString();
      }

      private object ParseNumber() {
        string number = NextWord;
        if (number.IndexOf('.') == -1) {
          long parsedLong;
          if (long.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedLong)) {
            return parsedLong;
          }
        }
        double parsedDouble;
        if (double.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedDouble)) {
          return parsedDouble;
        }
        return 0;
      }

      private void EatWhitespace() {
        while (char.IsWhiteSpace(PeekChar)) {
          json.Read();
          if (json.Peek() == -1) break;
        }
      }

      private char PeekChar {
        get {
          int p = json.Peek();
          return p == -1 ? '\0' : Convert.ToChar(p);
        }
      }

      private char NextChar {
        get { return Convert.ToChar(json.Read()); }
      }

      private string NextWord {
        get {
          StringBuilder word = new StringBuilder();
          while (!IsWordBreak(PeekChar)) {
            word.Append(NextChar);
            if (json.Peek() == -1) break;
          }
          return word.ToString();
        }
      }

      private TOKEN NextToken {
        get {
          EatWhitespace();
          if (json.Peek() == -1) return TOKEN.NONE;

          switch (PeekChar) {
            case '{':
              return TOKEN.CURLY_OPEN;
            case '}':
              json.Read();
              return TOKEN.CURLY_CLOSE;
            case '[':
              return TOKEN.SQUARED_OPEN;
            case ']':
              json.Read();
              return TOKEN.SQUARED_CLOSE;
            case ',':
              json.Read();
              return TOKEN.COMMA;
            case '"':
              return TOKEN.STRING;
            case ':':
              return TOKEN.COLON;
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
            case '-':
              return TOKEN.NUMBER;
          }

          string word = NextWord;
          switch (word) {
            case "false":
              return TOKEN.FALSE;
            case "true":
              return TOKEN.TRUE;
            case "null":
              return TOKEN.NULL;
          }

          return TOKEN.NONE;
        }
      }

      private enum TOKEN {
        NONE,
        CURLY_OPEN,
        CURLY_CLOSE,
        SQUARED_OPEN,
        SQUARED_CLOSE,
        COLON,
        COMMA,
        STRING,
        NUMBER,
        TRUE,
        FALSE,
        NULL
      }
    }

    private sealed class Serializer {
      private StringBuilder builder = new StringBuilder();

      public static string Serialize(object obj) {
        Serializer s = new Serializer();
        s.SerializeValue(obj);
        return s.builder.ToString();
      }

      private void SerializeValue(object val) {
        if (val == null) {
          builder.Append("null");
        } else if (val is string) {
          SerializeString((string)val);
        } else if (val is bool) {
          builder.Append(((bool)val) ? "true" : "false");
        } else if (val is IList) {
          SerializeArray((IList)val);
        } else if (val is IDictionary) {
          SerializeDictionary((IDictionary)val);
        } else if (val is char) {
          SerializeString(new string((char)val, 1));
        } else if (val is DateTime) {
          SerializeString(((DateTime)val).ToString("yyyy-MM-ddTHH:mm:ssZ"));
        } else if (val.GetType().IsPrimitive || val is decimal) {
          builder.Append(Convert.ToString(val, CultureInfo.InvariantCulture));
        } else {
          SerializeObject(val);
        }
      }

      private void SerializeObject(object obj) {
        builder.Append("{");
        bool first = true;
        PropertyInfo[] props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        for (int i = 0; i < props.Length; i++) {
          PropertyInfo p = props[i];
          if (!p.CanRead) continue;
          object val = p.GetValue(obj, null);
          if (val == null) continue;
          if (!first) builder.Append(",");
          SerializeString(p.Name);
          builder.Append(":");
          SerializeValue(val);
          first = false;
        }
        builder.Append("}");
      }

      private void SerializeDictionary(IDictionary dict) {
        builder.Append("{");
        bool first = true;
        foreach (DictionaryEntry kvp in dict) {
          if (!first) builder.Append(",");
          SerializeString(kvp.Key.ToString());
          builder.Append(":");
          SerializeValue(kvp.Value);
          first = false;
        }
        builder.Append("}");
      }

      private void SerializeArray(IList array) {
        builder.Append("[");
        bool first = true;
        for (int i = 0; i < array.Count; i++) {
          if (!first) builder.Append(",");
          SerializeValue(array[i]);
          first = false;
        }
        builder.Append("]");
      }

      private void SerializeString(string str) {
        builder.Append('"');
        for (int i = 0; i < str.Length; i++) {
          char c = str[i];
          switch (c) {
            case '"':
              builder.Append("\\\"");
              break;
            case '\\':
              builder.Append("\\\\");
              break;
            case '\b':
              builder.Append("\\b");
              break;
            case '\f':
              builder.Append("\\f");
              break;
            case '\n':
              builder.Append("\\n");
              break;
            case '\r':
              builder.Append("\\r");
              break;
            case '\t':
              builder.Append("\\t");
              break;
            default:
              int codepoint = Convert.ToInt32(c);
              if ((codepoint >= 32) && (codepoint <= 126)) {
                builder.Append(c);
              } else {
                builder.Append("\\u" + codepoint.ToString("x4"));
              }
              break;
          }
        }
        builder.Append('"');
      }
    }
  }
}
