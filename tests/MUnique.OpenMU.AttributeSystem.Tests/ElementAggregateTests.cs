// <copyright file="ElementAggregateTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem.Tests;

/// <summary>
/// Tests for the <see cref="ElementAggregate"/>.
/// </summary>
[TestFixture]
public class ElementAggregateTests
{
    private static readonly AggregateType[] KnownTypes = [AggregateType.AddRaw, AggregateType.Multiplicate, AggregateType.AddFinal, AggregateType.Maximum];

    /// <summary>
    /// An aggregate without values gives 0 and contains no kind.
    /// </summary>
    [Test]
    public void EmptyGivesZero()
    {
        var aggregate = default(ElementAggregate);
        var value = aggregate.GetValue();
        var containsAny = KnownTypes.Any(t => Contains(default, t));

        Assert.Multiple(() =>
        {
            Assert.That(value, Is.Zero);
            Assert.That(containsAny, Is.False);
        });
    }

    /// <summary>
    /// The aggregate contains the kind of an added value, and no other.
    /// </summary>
    /// <param name="type">The aggregate type of the added value.</param>
    [TestCase(AggregateType.AddRaw)]
    [TestCase(AggregateType.Multiplicate)]
    [TestCase(AggregateType.AddFinal)]
    [TestCase(AggregateType.Maximum)]
    public void ContainsTheKindOfAnAddedValue(AggregateType type)
    {
        var aggregate = default(ElementAggregate);
        aggregate.Add(type, 2);
        var contained = new List<AggregateType>();
        foreach (var knownType in KnownTypes)
        {
            if (aggregate.Contains(knownType))
            {
                contained.Add(knownType);
            }
        }

        Assert.That(contained, Is.EqualTo(new[] { type }));
    }

    /// <summary>
    /// The values are combined as a <see cref="ComposableAttribute"/> does: (added values + highest maximum) * factors + finally added values.
    /// </summary>
    [Test]
    public void CombinesTheKinds()
    {
        var aggregate = default(ElementAggregate);
        aggregate.Add(new ConstantElement(10));
        aggregate.Add(new ConstantElement(5));
        aggregate.Add(AggregateType.Maximum, 3);
        aggregate.Add(AggregateType.Maximum, 7);
        aggregate.Add(AggregateType.Multiplicate, 2);
        aggregate.Add(AggregateType.AddFinal, 1);

        Assert.That(aggregate.GetValue(), Is.EqualTo(((10 + 5 + 7) * 2) + 1));
    }

    /// <summary>
    /// Only factors give the product of the factors.
    /// </summary>
    [Test]
    public void OnlyFactorsGiveTheirProduct()
    {
        var aggregate = default(ElementAggregate);
        aggregate.Add(AggregateType.Multiplicate, 2);
        aggregate.Add(AggregateType.Multiplicate, 1.5f);

        Assert.That(aggregate.GetValue(), Is.EqualTo(3));
    }

    /// <summary>
    /// A value of an unknown type adds nothing, but the values are no longer only factors:
    /// the factor then multiplies the added values, which are none.
    /// </summary>
    [Test]
    public void UnknownTypeEndsOnlyFactors()
    {
        const AggregateType unknown = (AggregateType)99;
        var aggregate = default(ElementAggregate);
        aggregate.Add(AggregateType.Multiplicate, 2);
        aggregate.Add(unknown, 5);
        var value = aggregate.GetValue();
        var containsUnknown = aggregate.Contains(unknown);

        Assert.Multiple(() =>
        {
            Assert.That(value, Is.Zero);
            Assert.That(containsUnknown, Is.True);
        });
    }

    /// <summary>
    /// The value is limited by the passed maximum value and by the maximum value of the attribute definition, whichever is lower.
    /// </summary>
    /// <param name="maximumValue">The passed maximum value.</param>
    /// <param name="definitionMaximum">The maximum value of the attribute definition.</param>
    /// <param name="expected">The expected value.</param>
    [TestCase(5f, 8f, 5f)]
    [TestCase(9f, 8f, 8f)]
    [TestCase(null, 8f, 8f)]
    [TestCase(5f, null, 5f)]
    [TestCase(null, null, 10f)]
    public void ValueIsLimitedByBothMaximums(float? maximumValue, float? definitionMaximum, float expected)
    {
        var definition = new AttributeDefinition(Guid.NewGuid(), "Test", string.Empty) { MaximumValue = definitionMaximum };
        var aggregate = default(ElementAggregate);
        aggregate.Add(new ConstantElement(10));

        Assert.That(aggregate.GetValue(definition, maximumValue), Is.EqualTo(expected));
    }

    private static bool Contains(ElementAggregate aggregate, AggregateType type) => aggregate.Contains(type);
}
