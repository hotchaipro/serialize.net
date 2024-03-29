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
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace HotChai.Serialization.UnitTest
{
    [TestClass]
    public sealed class JsonNetPerformanceComparisonTests
    {
        public sealed class JsonNetSimpleObject
        {
            private const int StringQuotaInBytes = 1024;

            public int Id { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public int[] Scores { get; set; }

            public static void WriteTo(
                IObjectWriter writer,
                JsonNetSimpleObject obj)
            {
                if (null == obj)
                {
                    writer.WriteNullValue();
                    return;
                }

                writer.WriteStartObject();

                writer.WriteMember(1, obj.Id);

                writer.WriteMember(2, obj.Name);

                writer.WriteMember(3, obj.Address);

                writer.WriteMember(4, obj.Scores);

                writer.WriteEndObject();
            }

            public static JsonNetSimpleObject ReadFrom(
                IObjectReader reader)
            {
                JsonNetSimpleObject obj = null;

                if (reader.ReadStartObject())
                {
                    obj = new JsonNetSimpleObject();

                    int memberKey;

                    while (reader.MoveToNextMember())
                    {
                        memberKey = reader.MemberKey;

                        if (memberKey == 1)
                        {
                            obj.Id = reader.ReadValueAsInt32();
                        }
                        else if (memberKey == 2)
                        {
                            obj.Name = reader.ReadValueAsString(StringQuotaInBytes);
                        }
                        else if (memberKey == 3)
                        {
                            obj.Address = reader.ReadValueAsString(StringQuotaInBytes);
                        }
                        else if (memberKey == 4)
                        {
                            obj.Scores = null;

                            if (reader.ReadStartArray())
                            {
                                List<int> list = new List<int>();

                                while (reader.MoveToNextArrayValue())
                                {
                                    list.Add(reader.ReadValueAsInt32());
                                }

                                reader.ReadEndArray();

                                obj.Scores = list.ToArray();
                            }
                        }
                    }

                    reader.ReadEndObject();
                }

                return obj;
            }

            public void VerifyIsEqual(
                JsonNetSimpleObject other)
            {
                if (this.Id != other.Id)
                {
                    throw new Exception("Id does not match.");
                }

                if (this.Name != other.Name)
                {
                    throw new Exception("Name does not match.");
                }

                if (this.Address != other.Address)
                {
                    throw new Exception("Address does not match.");
                }

                if (!ArrayComparer<int>.Equals(this.Scores, other.Scores))
                {
                    throw new Exception("Scores does not match.");
                }
            }
        }

        [TestMethod]
        [TestCategory("Performance")]
        public void JsonNetSimpleObjectTest()
        {
            const int WarmupIterations = 10;
            const int Iterations = 5000;

            Stopwatch writeStopWatch = new Stopwatch();

            byte[] buffer = new byte[1024];
            byte[] serialized;

            JsonNetSimpleObject writeObject = new JsonNetSimpleObject()
            {
                Name = "Simple-1",
                Id = 2311,
                Address = "Planet Earth",
                Scores = new[] { 82, 96, 49, 40, 38, 38, 78, 96, 2, 39 },
            };

            ObjectReaderWriterFactory factory = new PortableBinaryObjectReaderWriterFactory();

            // Serialize the test object to a byte array
            using (var stream = new MemoryStream(buffer))
            {
                IObjectWriter writer;

                GC.Collect(3, GCCollectionMode.Forced, true);

                // Warm-up
                for (int i = 0; i < WarmupIterations; i += 1)
                {
                    stream.SetLength(0);
                    writer = factory.CreateWriter(stream);
                    JsonNetSimpleObject.WriteTo(writer, writeObject);
                }

                writer = factory.CreateWriter(Stream.Null);

                writeStopWatch.Start();

                for (int i = 0; i < Iterations; i += 1)
                {
                    JsonNetSimpleObject.WriteTo(writer, writeObject);
                }

                writeStopWatch.Stop();

                serialized = stream.ToArray();
            }

            string writeDump = serialized.ToAsciiDumpString();
            Debug.WriteLine(writeDump);

            Test.Output("{0} writer serialized {1} simple objects ({2:#,###} bytes each) in {3} ms ({4} ticks @ {5}). Average is {6} ticks per object.",
                factory.Name, Iterations, serialized.Length, writeStopWatch.Elapsed.TotalMilliseconds, writeStopWatch.ElapsedTicks, Stopwatch.Frequency, writeStopWatch.ElapsedTicks / Iterations);

            Stopwatch readStopWatch = new Stopwatch();

            JsonNetSimpleObject readObject = null;

            // Deserialize the test object from a byte array
            using (var stream = new MemoryStream(serialized))
            {
                IObjectReader reader;

                // Warm-up
                for (int i = 0; i < WarmupIterations; i += 1)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    reader = factory.CreateReader(stream);
                    readObject = JsonNetSimpleObject.ReadFrom(reader);
                }

                readStopWatch.Start();

                for (int i = 0; i < Iterations; i += 1)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    reader = factory.CreateReader(stream);
                    readObject = JsonNetSimpleObject.ReadFrom(reader);
                }

                readStopWatch.Stop();
            }

            Test.Output("{0} reader deserialized {1} simple objects ({2:#,###} bytes each) in {3} ms ({4} ticks @ {5}). Average is {6} ticks per object.",
                factory.Name, Iterations, serialized.Length, readStopWatch.Elapsed.TotalMilliseconds, readStopWatch.ElapsedTicks, Stopwatch.Frequency, readStopWatch.ElapsedTicks / Iterations);

            // Compare objects
            writeObject.VerifyIsEqual(readObject);
        }
    }
}
