// <copyright file="JsonChunkedStreamDeserializationTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Text;
using System.IO;
using System.Text.Json;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.Json;

/// <summary>
/// Regression test for the bot purge failure: <see cref="ReferenceResolvingConverter{T}"/> must survive
/// chunked (streaming) deserialization, where the underlying <see cref="System.Text.Json.Utf8JsonReader"/>
/// runs with <c>isFinalBlock: false</c>. Skipping values with <c>Utf8JsonReader.Skip()</c> throws
/// "Cannot skip tokens on partial JSON" there - <c>TrySkip()</c> has to be used instead.
/// The payload mimics the Postgres account JSON: a <c>null</c> adder-only collection
/// (<c>array_agg</c> over zero rows) and an unknown property (e.g. a new column), followed by
/// enough data to force multi-segment parsing like Npgsql's sequential-access stream does.
/// </summary>
[TestFixture]
public class JsonChunkedStreamDeserializationTest
{
    /// <summary>
    /// A null adder-only collection and an unknown property are skipped without error when the
    /// JSON arrives in small chunks.
    /// </summary>
    [Test]
    public void NullCollectionAndUnknownPropertySurviveChunkedStream()
    {
        var id = Guid.NewGuid();
        var json = "{\"$id\":\"" + id + "\",\"Id\":\"" + id + "\",\"LoginName\":\"bot0001\""
            + ",\"RawChildren\":null"
            + ",\"SomeFutureColumn\":{\"Nested\":[1,2,3]}"
            + ",\"Padding\":\"" + new string('x', 128 * 1024) + "\"}";
        using var stream = new ChunkedStream(Encoding.UTF8.GetBytes(json));

        var result = stream.FromJson<TestParent>();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.LoginName, Is.EqualTo("bot0001"));
        Assert.That(result.RawChildren, Is.Empty);
    }

    /// <summary>
    /// The core of the bot purge failure: when the converter runs on a partial buffer
    /// (<c>isFinalBlock: false</c>, as handed over by a chunked stream before more data arrives),
    /// skipping must not escape as "Cannot skip tokens on partial JSON".
    /// <c>Utf8JsonReader.Skip()</c> throws <see cref="InvalidOperationException"/> there, which the
    /// serializer cannot resume from; <c>TrySkip()</c> consumes complete tokens and lets the
    /// serializer retry with more data instead.
    /// </summary>
    [Test]
    public void PartialBufferSkipStaysResumable()
    {
        var id = Guid.NewGuid();

        // Complete tokens, but truncated mid-object: the null collection is fully buffered,
        // more data is still pending - exactly the state which crashed the bot purge.
        var head = "{\"$id\":\"" + id + "\",\"Id\":\"" + id + "\",\"RawChildren\":null";
        var bytes = Encoding.UTF8.GetBytes(head);
        var reader = new Utf8JsonReader(bytes, isFinalBlock: false, state: default);
        var options = new JsonSerializerOptions { ReferenceHandler = new IdReferenceHandler() };
        var converter = new ReferenceResolvingConverter<TestParent>([]);

        // Throws InvalidOperationException ("Cannot skip tokens on partial JSON") on the old code.
        var result = ReadPartial(ref reader, converter, options);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.RawChildren, Is.Empty);
    }

    /// <summary>
    /// Reads a partial buffer with the converter. A static helper because <c>ref</c> locals
    /// cannot be captured in lambdas.
    /// </summary>
    private static TestParent? ReadPartial(ref Utf8JsonReader reader, ReferenceResolvingConverter<TestParent> converter, JsonSerializerOptions options)
    {
        return converter.Read(ref reader, typeof(TestParent), options);
    }

    /// <summary>
    /// A minimal <see cref="IIdentifiable"/> item for the chunked stream test.
    /// </summary>
    private sealed class TestChild : IIdentifiable
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// A minimal <see cref="IIdentifiable"/> aggregate root shaped like the persisted account:
    /// settable scalars plus a get-only <c>Raw…</c> collection which is filled through an adder.
    /// </summary>
    private sealed class TestParent : IIdentifiable
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the login name.
        /// </summary>
        public string LoginName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the padding which forces multi-segment parsing.
        /// </summary>
        public string Padding { get; set; } = string.Empty;

        /// <summary>
        /// Gets the raw children, filled through an adder like the generated <c>Raw…</c> collections.
        /// </summary>
        public ICollection<TestChild> RawChildren { get; } = new List<TestChild>();
    }

    /// <summary>
    /// A non-seekable stream which hands out a few bytes per read, like Npgsql's
    /// sequential-access stream does.
    /// </summary>
    private sealed class ChunkedStream : Stream
    {
        private const int MaxBytesPerRead = 512;

        private readonly byte[] _data;

        private int _position;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkedStream"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public ChunkedStream(byte[] data)
        {
            this._data = data;
        }

        /// <inheritdoc />
        public override bool CanRead => true;

        /// <inheritdoc />
        public override bool CanSeek => false;

        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <inheritdoc />
        public override long Length => throw new NotSupportedException();

        /// <inheritdoc />
        public override long Position
        {
            get => this._position;
            set => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override void Flush()
        {
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.Read(new Span<byte>(buffer, offset, count));
        }

        /// <inheritdoc />
        public override int Read(Span<byte> buffer)
        {
            if (this._position >= this._data.Length)
            {
                return 0;
            }

            var count = Math.Min(Math.Min(MaxBytesPerRead, buffer.Length), this._data.Length - this._position);
            this._data.AsSpan(this._position, count).CopyTo(buffer);
            this._position += count;
            return count;
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        /// <inheritdoc />
        public override void SetLength(long value) => throw new NotSupportedException();

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
