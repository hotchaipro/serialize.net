Serialize.NET
=============

A cross-platform library for efficiently serializing basic .NET types into JSON, XML, Bencode, and PBON (portable binary) formats.

About
=====

Serialize.NET provides a simple .NET interface to serialize and deserialize objects using a variety of wire formats.

It is implemented as a Portable Class Library, so it can be used with Mono, Xamarin.iOS, and Xamarin.Android as a cross-platform solution.

The serialization formats support transmitting objects between platforms (regardless of processor endianness, for example).

The interfaces are intentionally primitive and are suitable for extending into higher-level abstractions, for example, a messaging protocol.

The serialization interfaces also support inspection of the underlying byte stream, which enables scenarios such as digital signing.

See the wiki for more details.
