using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MiBand3
{
    public class Helpers
    {
        public static string TimeSpanToText(DateTime value)
        {
            TimeSpan timeRemaining = DateTime.Now - value;

            if (timeRemaining.Days > 365)
            {
                return $"Before {Math.Round((double)timeRemaining.Days / 365)} years";
            }
            else if (timeRemaining.Days > 31)
            {
                return $"Before {Math.Round((double)timeRemaining.Days / 31)} month";
            }
            else if (timeRemaining.Days != 0)
            {
                return $"Before {timeRemaining.Days} days";
            }
            else if (timeRemaining.Hours != 0)
            {
                return $"Before {timeRemaining.Hours} hours";
            }
            else if (timeRemaining.Minutes != 0)
            {
                return $"Before {timeRemaining.Minutes} minutes";
            }
            else
            {
                return $"Before {timeRemaining.Seconds} seconds";
            }
        }

        public static string ToXml(object value, Type type)
        {
            var serializer = new XmlSerializer(type);
            var stringBuilder = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };

            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                serializer.Serialize(xmlWriter, value);
            }
            return stringBuilder.ToString();
        }

        public static object FromXml(string xml, Type type)
        {
            var serializer = new XmlSerializer(type);
            object deserialized;
            using (var stringReader = new StringReader(xml))
            {
                deserialized = serializer.Deserialize(stringReader);
            }

            return deserialized;
        }

        public static List<string> SplitByLength(string inputString, int splitAt, int maxListLength = 9999)
        {
            var stringList = new List<string>();
            try
            {
                if (inputString.Length <= splitAt)
                {
                    stringList.Add(inputString);
                    return stringList;
                }

                for (int i = 0; i < inputString.Length; i += splitAt)
                {
                    var element = inputString.Substring(i, Math.Min(splitAt, inputString.Length - i));
                    if (!string.IsNullOrEmpty(element))
                    {
                        if (stringList.Count <= maxListLength)
                        {
                            stringList.Add(element);
                        }
                    }
                }

                return stringList;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public static void DebugWriter(Type sender, string message, string type = "ERROR")
        {
            Debug.WriteLine($"{type} in {sender.FullName}: {message}");
        }
    }
}

