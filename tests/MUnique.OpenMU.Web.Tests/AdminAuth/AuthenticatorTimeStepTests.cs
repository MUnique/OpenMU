// <copyright file="AuthenticatorTimeStepTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.AdminAuth;

using System.Text;
using MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// Tests for <see cref="AuthenticatorTimeStep"/>, using the SHA-1 test vectors of RFC 6238, appendix B.
/// </summary>
[TestFixture]
public class AuthenticatorTimeStepTests
{
    /// <summary>
    /// The base32 encoding of the RFC 6238 SHA-1 test key "12345678901234567890".
    /// </summary>
    private const string RfcKey = "GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQ";

    /// <summary>
    /// Tests that the codes match the last six digits of the RFC 6238 test vectors.
    /// </summary>
    /// <param name="unixTime">The time of the test vector.</param>
    /// <param name="expectedCode">The expected six digit code.</param>
    [TestCase(59, "287082")]
    [TestCase(1111111109, "081804")]
    [TestCase(1111111111, "050471")]
    [TestCase(1234567890, "005924")]
    [TestCase(2000000000, "279037")]
    public void ComputesRfcTestVectors(long unixTime, string expectedCode)
    {
        var step = AuthenticatorTimeStep.GetStep(DateTimeOffset.FromUnixTimeSeconds(unixTime));

        Assert.That(AuthenticatorTimeStep.ComputeCode(Encoding.ASCII.GetBytes("12345678901234567890"), step), Is.EqualTo(expectedCode));
    }

    /// <summary>
    /// Tests that the base32 decoding returns the original key, ignoring spaces, padding and the case.
    /// </summary>
    [Test]
    public void DecodesBase32Key()
    {
        var decoded = AuthenticatorTimeStep.DecodeBase32("gezd gnbv gy3t qojq gezd gnbv gy3t qojq====");

        Assert.That(Encoding.ASCII.GetString(decoded), Is.EqualTo("12345678901234567890"));
    }

    /// <summary>
    /// Tests that the step of the code is found, also after the current step moved on - which is the
    /// case a replay check based on the current step got wrong.
    /// </summary>
    /// <param name="currentStepOffset">The offset of the current step to the step of the code.</param>
    [TestCase(-2)]
    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public void FindsStepOfCodeWithinWindow(int currentStepOffset)
    {
        const long codeStep = 37037036; // The step of the vector at 1111111109.

        Assert.That(AuthenticatorTimeStep.FindStep(RfcKey, "081804", codeStep + currentStepOffset), Is.EqualTo(codeStep));
    }

    /// <summary>
    /// Tests that a code outside of the validation window or a wrong code has no step.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <param name="currentStep">The current step.</param>
    [TestCase("081804", 37037036 + 3)]
    [TestCase("081804", 37037036 - 3)]
    [TestCase("000000", 37037036)]
    public void FindsNoStepForOtherCodes(string code, long currentStep)
    {
        Assert.That(AuthenticatorTimeStep.FindStep(RfcKey, code, currentStep), Is.Null);
    }
}
