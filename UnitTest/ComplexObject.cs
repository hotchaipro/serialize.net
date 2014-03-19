﻿#region License
// Copyright (c) 2014, David Taylor
//
// Permission to use, copy, modify, and/or distribute this software for any 
// purpose with or without fee is hereby granted, provided that the above 
// copyright notice and this permission notice appear in all copies, unless 
// such copies are solely in the form of machine-executable object code 
// generated by a source language processor.
//
// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES 
// WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF 
// MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR 
// ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES 
// WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN 
// ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF 
// OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
#endregion License
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HotChai.Serialization.UnitTest
{
    public sealed class ComplexObject
    {
        private const int MaximumValueLengthInBytes = 1024;

        private static class MemberKey
        {
            public const int NestedObject = 1;
            public const int WriteOnlyNestedObject = 2;
            public const int NestedEmptyObject = 3;
            public const int WriteOnlyNestedEmptyObject = 4;
            public const int NestedNullObject = 5;
            public const int WriteOnlyNestedNullObject = 6;

            public const int ArrayOfObjects = 7;
            public const int WriteOnlyArrayOfObjects = 8;
            public const int EmptyArrayOfObjects = 9;
            public const int WriteOnlyEmptyArrayOfObjects = 10;
            public const int NullArrayOfObjects = 11;
            public const int WriteOnlyNullArrayOfObjects = 12;

            public const int NestedArrayOfInts = 13;
            public const int WriteOnlyNestedArrayOfInts = 14;
            public const int NestedEmptyArrayOfInts = 15;
            public const int WriteOnlyNestedEmptyArrayOfInts = 16;
            public const int NestedNullArrayOfInts = 17;
            public const int WriteOnlyNestedNullArrayOfInts = 18;

            public const int NestedArrayOfStrings = 19;
            public const int WriteOnlyNestedArrayOfStrings = 20;
            public const int NestedEmptyArrayOfStrings = 21;
            public const int WriteOnlyNestedEmptyArrayOfStrings = 22;
            public const int NestedNullArrayOfStrings = 23;
            public const int WriteOnlyNestedNullArrayOfStrings = 24;

            public const int NestedArrayOfObjects = 25;
            public const int WriteOnlyNestedArrayOfObjects = 26;
            public const int NestedEmptyArrayOfObjects = 27;
            public const int WriteOnlyNestedEmptyArrayOfObjects = 28;
            public const int NestedNullArrayOfObjects = 29;
            public const int WriteOnlyNestedNullArrayOfObjects = 30;
        }

        private ComplexObject()
        {
            // Ensure default null values won't match during validation
            this.NestedNullObject = SimpleObject.Invalid;
            this.NullArrayOfObjects = new SimpleObject[] { };
            this.NestedNullArrayOfInts = new int[][] { };
            this.NestedNullArrayOfStrings = new string[][] { };
            this.NestedNullArrayOfObjects = new SimpleObject[][] { };
        }

        public static ComplexObject Create()
        {
            ComplexObject simpleObject = new ComplexObject()
            {
                NestedObject = SimpleObject.Create(),
                NestedEmptyObject = EmptyObject.Create(),
                NestedNullObject = null,

                ArrayOfObjects = new SimpleObject[] { SimpleObject.Create(), null, SimpleObject.Create() },
                EmptyArrayOfObjects = new SimpleObject[] { },
                NullArrayOfObjects = null,

                NestedArrayOfInts = new int[][]
                    {
                        new int[] { 1, 2, 3 },
                        null,
                        new int[] { },
                        new int[] { -1, -2, -3 }
                    },
                NestedEmptyArrayOfInts = new int[][] { },
                NestedNullArrayOfInts = null,

                NestedArrayOfStrings = new string[][]
                    {
                        new string[] { "String1", "String2", "String3" },
                        new string[] {},
                        null,
                    },
                NestedEmptyArrayOfStrings = new string[][] { },
                NestedNullArrayOfStrings = null,

                NestedArrayOfObjects = new SimpleObject[][]
                    {
                        new SimpleObject[] { SimpleObject.Create(), null, SimpleObject.Create() },
                        new SimpleObject[] { },
                        null,
                    },
                NestedEmptyArrayOfObjects = new SimpleObject[][] { },
                NestedNullArrayOfObjects = null,
            };

            return simpleObject;
        }

        private SimpleObject NestedObject { get; set; }
        private EmptyObject NestedEmptyObject { get; set; }
        private SimpleObject NestedNullObject { get; set; }

        private SimpleObject[] ArrayOfObjects { get; set; }
        private SimpleObject[] EmptyArrayOfObjects { get; set; }
        private SimpleObject[] NullArrayOfObjects { get; set; }

        private int[][] NestedArrayOfInts { get; set; }
        private int[][] NestedEmptyArrayOfInts { get; set; }
        private int[][] NestedNullArrayOfInts { get; set; }

        private string[][] NestedArrayOfStrings { get; set; }
        private string[][] NestedEmptyArrayOfStrings { get; set; }
        private string[][] NestedNullArrayOfStrings { get; set; }

        private SimpleObject[][] NestedArrayOfObjects { get; set; }
        private SimpleObject[][] NestedEmptyArrayOfObjects { get; set; }
        private SimpleObject[][] NestedNullArrayOfObjects { get; set; }

        public static void WriteTo(
            IObjectWriter writer,
            ComplexObject complexObject)
        {
            if (null == complexObject)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();

            // Nested object
            writer.WriteStartMember(MemberKey.NestedObject);
            SimpleObject.WriteTo(writer, complexObject.NestedObject);
            writer.WriteEndMember();

            // Nested object (write-only)
            writer.WriteStartMember(MemberKey.WriteOnlyNestedObject);
            SimpleObject.WriteTo(writer, complexObject.NestedObject);
            writer.WriteEndMember();

            // Nested empty object
            writer.WriteStartMember(MemberKey.NestedEmptyObject);
            EmptyObject.WriteTo(writer, complexObject.NestedEmptyObject);
            writer.WriteEndMember();

            // Nested empty object (write-only)
            writer.WriteStartMember(MemberKey.WriteOnlyNestedEmptyObject);
            SimpleObject.WriteTo(writer, complexObject.NestedNullObject);
            writer.WriteEndMember();

            // Nested null object
            writer.WriteStartMember(MemberKey.NestedNullObject);
            SimpleObject.WriteTo(writer, complexObject.NestedNullObject);
            writer.WriteEndMember();

            // Nested null object (write-only)
            writer.WriteStartMember(MemberKey.WriteOnlyNestedNullObject);
            SimpleObject.WriteTo(writer, complexObject.NestedNullObject);
            writer.WriteEndMember();

            // Array of objects
            WriteMember(writer, MemberKey.ArrayOfObjects, complexObject.ArrayOfObjects);
            WriteMember(writer, MemberKey.WriteOnlyArrayOfObjects, complexObject.ArrayOfObjects);

            // Empty array of objects
            WriteMember(writer, MemberKey.EmptyArrayOfObjects, complexObject.EmptyArrayOfObjects);
            WriteMember(writer, MemberKey.WriteOnlyEmptyArrayOfObjects, complexObject.EmptyArrayOfObjects);

            // Null array of objects
            WriteMember(writer, MemberKey.NullArrayOfObjects, complexObject.NullArrayOfObjects);
            WriteMember(writer, MemberKey.WriteOnlyNullArrayOfObjects, complexObject.NullArrayOfObjects);

            // Nested array of ints
            WriteMember(writer, MemberKey.NestedArrayOfInts, complexObject.NestedArrayOfInts);
            WriteMember(writer, MemberKey.WriteOnlyNestedArrayOfInts, complexObject.NestedArrayOfInts);

            // Empty nested array of ints
            WriteMember(writer, MemberKey.NestedEmptyArrayOfInts, complexObject.NestedEmptyArrayOfInts);
            WriteMember(writer, MemberKey.WriteOnlyNestedEmptyArrayOfInts, complexObject.NestedEmptyArrayOfInts);

            // Null nested array of ints
            WriteMember(writer, MemberKey.NestedNullArrayOfInts, complexObject.NestedNullArrayOfInts);
            WriteMember(writer, MemberKey.WriteOnlyNestedNullArrayOfInts, complexObject.NestedNullArrayOfInts);

            // Nested array of strings
            WriteMember(writer, MemberKey.NestedArrayOfStrings, complexObject.NestedArrayOfStrings);
            WriteMember(writer, MemberKey.WriteOnlyNestedArrayOfStrings, complexObject.NestedArrayOfStrings);

            // Empty nested array of strings
            WriteMember(writer, MemberKey.NestedEmptyArrayOfStrings, complexObject.NestedEmptyArrayOfStrings);
            WriteMember(writer, MemberKey.WriteOnlyNestedEmptyArrayOfStrings, complexObject.NestedEmptyArrayOfStrings);

            // Null nested array of strings
            WriteMember(writer, MemberKey.NestedNullArrayOfStrings, complexObject.NestedNullArrayOfStrings);
            WriteMember(writer, MemberKey.WriteOnlyNestedNullArrayOfStrings, complexObject.NestedNullArrayOfStrings);

            // Nested array of objects
            WriteMember(writer, MemberKey.NestedArrayOfObjects, complexObject.NestedArrayOfObjects);
            WriteMember(writer, MemberKey.WriteOnlyNestedArrayOfObjects, complexObject.NestedArrayOfObjects);

            // Empty nested array of objects
            WriteMember(writer, MemberKey.NestedEmptyArrayOfObjects, complexObject.NestedEmptyArrayOfObjects);
            WriteMember(writer, MemberKey.WriteOnlyNestedEmptyArrayOfObjects, complexObject.NestedEmptyArrayOfObjects);

            // Null nested array of objects
            WriteMember(writer, MemberKey.NestedNullArrayOfObjects, complexObject.NestedNullArrayOfObjects);
            WriteMember(writer, MemberKey.WriteOnlyNestedNullArrayOfObjects, complexObject.NestedNullArrayOfObjects);

            writer.WriteEndObject();
        }

        public static ComplexObject ReadFrom(
            IObjectReader reader)
        {
            ComplexObject complexObject = null;

            if (reader.ReadStartObject())
            {
                complexObject = new ComplexObject();

                int memberKey;

                while (reader.ReadNextMemberKey())
                {
                    memberKey = reader.MemberKey;

                    if (memberKey == MemberKey.NestedObject)
                    {
                        complexObject.NestedObject = SimpleObject.ReadFrom(reader);
                    }
                    if (memberKey == MemberKey.NestedEmptyObject)
                    {
                        complexObject.NestedEmptyObject = EmptyObject.ReadFrom(reader);
                    }
                    if (memberKey == MemberKey.NestedNullObject)
                    {
                        complexObject.NestedNullObject = SimpleObject.ReadFrom(reader);
                    }
                    else if (memberKey == MemberKey.ArrayOfObjects)
                    {
                        complexObject.ArrayOfObjects = ReadArrayOfSimpleObjects(reader);
                    }
                    else if (memberKey == MemberKey.EmptyArrayOfObjects)
                    {
                        complexObject.EmptyArrayOfObjects = ReadArrayOfSimpleObjects(reader);
                    }
                    else if (memberKey == MemberKey.NullArrayOfObjects)
                    {
                        complexObject.NullArrayOfObjects = ReadArrayOfSimpleObjects(reader);
                    }
                    else if (memberKey == MemberKey.NestedArrayOfInts)
                    {
                        complexObject.NestedArrayOfInts = ReadNestedArrayOfInts(reader);
                    }
                    else if (memberKey == MemberKey.NestedEmptyArrayOfInts)
                    {
                        complexObject.NestedEmptyArrayOfInts = ReadNestedArrayOfInts(reader);
                    }
                    else if (memberKey == MemberKey.NestedNullArrayOfInts)
                    {
                        complexObject.NestedNullArrayOfInts = ReadNestedArrayOfInts(reader);
                    }
                    else if (memberKey == MemberKey.NestedArrayOfStrings)
                    {
                        complexObject.NestedArrayOfStrings = ReadNestedArrayOfStrings(reader);
                    }
                    else if (memberKey == MemberKey.NestedEmptyArrayOfStrings)
                    {
                        complexObject.NestedEmptyArrayOfStrings = ReadNestedArrayOfStrings(reader);
                    }
                    else if (memberKey == MemberKey.NestedNullArrayOfStrings)
                    {
                        complexObject.NestedNullArrayOfStrings = ReadNestedArrayOfStrings(reader);
                    }
                    else if (memberKey == MemberKey.NestedArrayOfObjects)
                    {
                        complexObject.NestedArrayOfObjects = ReadNestedArrayOfSimpleObjects(reader);
                    }
                    else if (memberKey == MemberKey.NestedEmptyArrayOfObjects)
                    {
                        complexObject.NestedEmptyArrayOfObjects = ReadNestedArrayOfSimpleObjects(reader);
                    }
                    else if (memberKey == MemberKey.NestedNullArrayOfObjects)
                    {
                        complexObject.NestedNullArrayOfObjects = ReadNestedArrayOfSimpleObjects(reader);
                    }
                    //else
                    //{
                    //    Debug.WriteLine("Skipping member key {0}", memberKey);
                    //}
                }

                reader.ReadEndObject();
            }

            return complexObject;
        }

        public void VerifyIsEqual(
            ComplexObject other)
        {
            // TODO
        }

        private static void WriteMember(
            IObjectWriter writer,
            int memberKey,
            SimpleObject[] array)
        {
            writer.WriteStartMember(memberKey);

            if (null == array)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStartArray();
                foreach (SimpleObject simpleObject in array)
                {
                    SimpleObject.WriteTo(writer, simpleObject);
                }
                writer.WriteEndArray();
            }
            writer.WriteEndMember();
        }

        private static SimpleObject[] ReadArrayOfSimpleObjects(
            IObjectReader reader)
        {
            SimpleObject[] array = null;

            if (reader.ReadStartArray())
            {
                List<SimpleObject> list = new List<SimpleObject>();

                while (reader.MoveToNextArrayValue())
                {
                    list.Add(SimpleObject.ReadFrom(reader));
                }

                reader.ReadEndArray();

                array = list.ToArray();
            }

            return array;
        }

        private static void WriteMember(
            IObjectWriter writer,
            int memberKey,
            int[][] array)
        {
            writer.WriteStartMember(memberKey);
            if (array == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStartArray();
                foreach (var nestedArray in array)
                {
                    if (nestedArray == null)
                    {
                        writer.WriteNullValue();
                    }
                    else
                    {
                        writer.WriteStartArray();
                        foreach (var value in nestedArray)
                        {
                            writer.WriteValue(value);
                        }
                        writer.WriteEndArray();
                    }
                }
                writer.WriteEndArray();
            }
            writer.WriteEndMember();
        }

        private static int[][] ReadNestedArrayOfInts(
            IObjectReader reader)
        {
            int[][] array = null;

            if (reader.ReadStartArray())
            {
                List<int[]> list = new List<int[]>();

                while (reader.MoveToNextArrayValue())
                {
                    if (reader.ReadStartArray())
                    {
                        List<int> innerList = new List<int>();

                        while (reader.MoveToNextArrayValue())
                        {
                            innerList.Add(reader.ReadValueAsInt32());
                        }

                        reader.ReadEndArray();

                        list.Add(innerList.ToArray());
                    }
                    else
                    {
                        list.Add(null);
                    }
                }

                reader.ReadEndArray();

                array = list.ToArray();
            }

            return array;
        }

        private static void WriteMember(
            IObjectWriter writer,
            int memberKey,
            string[][] array)
        {
            writer.WriteStartMember(memberKey);
            if (array == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStartArray();
                foreach (var nestedArray in array)
                {
                    if (nestedArray == null)
                    {
                        writer.WriteNullValue();
                    }
                    else
                    {
                        writer.WriteStartArray();
                        foreach (var value in nestedArray)
                        {
                            writer.WriteValue(value);
                        }
                        writer.WriteEndArray();
                    }
                }
                writer.WriteEndArray();
            }
            writer.WriteEndMember();
        }

        private static string[][] ReadNestedArrayOfStrings(
            IObjectReader reader)
        {
            string[][] array = null;

            if (reader.ReadStartArray())
            {
                List<string[]> list = new List<string[]>();

                while (reader.MoveToNextArrayValue())
                {
                    if (reader.ReadStartArray())
                    {
                        List<string> innerList = new List<string>();

                        while (reader.MoveToNextArrayValue())
                        {
                            innerList.Add(reader.ReadValueAsString(MaximumValueLengthInBytes));
                        }

                        reader.ReadEndArray();

                        list.Add(innerList.ToArray());
                    }
                    else
                    {
                        list.Add(null);
                    }
                }

                reader.ReadEndArray();

                array = list.ToArray();
            }

            return array;
        }

        private static void WriteMember(
            IObjectWriter writer,
            int memberKey,
            SimpleObject[][] array)
        {
            writer.WriteStartMember(memberKey);
            if (array == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStartArray();
                foreach (var nestedArray in array)
                {
                    if (null == nestedArray)
                    {
                        writer.WriteNullValue();
                    }
                    else
                    {
                        writer.WriteStartArray();
                        foreach (var value in nestedArray)
                        {
                            SimpleObject.WriteTo(writer, value);
                        }
                        writer.WriteEndArray();
                    }
                }
                writer.WriteEndArray();
            }
            writer.WriteEndMember();
        }

        private static SimpleObject[][] ReadNestedArrayOfSimpleObjects(
            IObjectReader reader)
        {
            SimpleObject[][] array = null;

            if (reader.ReadStartArray())
            {
                List<SimpleObject[]> list = new List<SimpleObject[]>();

                while (reader.MoveToNextArrayValue())
                {
                    if (reader.ReadStartArray())
                    {
                        List<SimpleObject> innerList = new List<SimpleObject>();

                        while (reader.MoveToNextArrayValue())
                        {
                            SimpleObject innerObject = SimpleObject.ReadFrom(reader);
                            innerList.Add(innerObject);
                        }

                        reader.ReadEndArray();

                        list.Add(innerList.ToArray());
                    }
                    else
                    {
                        list.Add(null);
                    }
                }

                reader.ReadEndArray();

                array = list.ToArray();
            }

            return array;
        }
    }
}
