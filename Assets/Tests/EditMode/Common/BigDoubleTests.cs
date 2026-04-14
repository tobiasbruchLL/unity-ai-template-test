using NUnit.Framework;
using BreakInfinity;

namespace LL.Common.Tests
{
    [TestFixture]
    public class BigDoubleTests
    {
        // ── Constants ────────────────────────────────────────────────────────

        [Test]
        public void Tolerance_HasExpectedValue()
        {
            Assert.AreEqual(1e-18, BigDouble.Tolerance);
        }

        [Test]
        public void Zero_MantissaAndExponentAreZero()
        {
            Assert.AreEqual(0.0, BigDouble.Zero.Mantissa);
            Assert.AreEqual(0L,  BigDouble.Zero.Exponent);
        }

        [Test]
        public void One_MantissaIsOneAndExponentIsZero()
        {
            Assert.AreEqual(1.0, BigDouble.One.Mantissa);
            Assert.AreEqual(0L,  BigDouble.One.Exponent);
        }

        // ── Construction & Normalization ──────────────────────────────────────

        [Test]
        public void Constructor_FromDouble_NormalizesCorrectly()
        {
            var value = new BigDouble(150.0);
            Assert.AreEqual(2L,  value.Exponent);
            Assert.AreEqual(1.5, value.Mantissa, 1e-12);
        }

        [Test]
        public void Constructor_FromMantissaExponent_NormalizesWhenMantissaOutOfRange()
        {
            var value = new BigDouble(15.0, 0);
            Assert.AreEqual(1L,  value.Exponent);
            Assert.AreEqual(1.5, value.Mantissa, 1e-12);
        }

        [Test]
        public void Constructor_FromZeroDouble_ReturnsZero()
        {
            var value = new BigDouble(0.0);
            Assert.IsTrue(value == BigDouble.Zero);
        }

        [Test]
        public void Constructor_FromNaN_ReturnsNaN()
        {
            var value = new BigDouble(double.NaN);
            Assert.IsTrue(BigDouble.IsNaN(value));
        }

        [Test]
        public void Constructor_FromPositiveInfinity_ReturnsPositiveInfinity()
        {
            var value = new BigDouble(double.PositiveInfinity);
            Assert.IsTrue(BigDouble.IsPositiveInfinity(value));
        }

        // ── Arithmetic operators ─────────────────────────────────────────────

        [Test]
        public void Addition_TwoSmallValues_ReturnsCorrectSum()
        {
            var result = new BigDouble(3.0) + new BigDouble(4.0);
            Assert.AreEqual(new BigDouble(7.0), result);
        }

        [Test]
        public void Subtraction_LargerMinusSmaller_ReturnsCorrectDifference()
        {
            var result = new BigDouble(10.0) - new BigDouble(3.0);
            Assert.AreEqual(new BigDouble(7.0), result);
        }

        [Test]
        public void Multiplication_TwoValues_ReturnsCorrectProduct()
        {
            var result = new BigDouble(2.0) * new BigDouble(3.0);
            Assert.AreEqual(new BigDouble(6.0), result);
        }

        [Test]
        public void Division_DividendByDivisor_ReturnsCorrectQuotient()
        {
            var result = new BigDouble(10.0) / new BigDouble(2.0);
            Assert.AreEqual(new BigDouble(5.0), result);
        }

        [Test]
        public void UnaryNegation_PositiveValue_ReturnsNegative()
        {
            var result = -new BigDouble(5.0);
            Assert.IsTrue(result < BigDouble.Zero);
        }

        [Test]
        public void Addition_VeryLargeExponents_MantissaStaysNormalized()
        {
            var a = new BigDouble(1.0, 100);
            var b = new BigDouble(1.0, 100);
            var result = a + b;
            Assert.AreEqual(100L, result.Exponent);
            Assert.AreEqual(2.0,  result.Mantissa, 1e-12);
        }

        // ── Comparison operators ──────────────────────────────────────────────

        [Test]
        public void LessThan_SmallerValue_ReturnsTrue()
        {
            Assert.IsTrue(new BigDouble(1.0) < new BigDouble(2.0));
        }

        [Test]
        public void GreaterThan_LargerValue_ReturnsTrue()
        {
            Assert.IsTrue(new BigDouble(5.0) > new BigDouble(3.0));
        }

        [Test]
        public void LessThanOrEqual_EqualValues_ReturnsTrue()
        {
            Assert.IsTrue(new BigDouble(4.0) <= new BigDouble(4.0));
        }

        [Test]
        public void GreaterThanOrEqual_EqualValues_ReturnsTrue()
        {
            Assert.IsTrue(new BigDouble(4.0) >= new BigDouble(4.0));
        }

        [Test]
        public void Equality_SameValue_ReturnsTrue()
        {
            Assert.IsTrue(new BigDouble(7.0) == new BigDouble(7.0));
        }

        [Test]
        public void Inequality_DifferentValues_ReturnsTrue()
        {
            Assert.IsTrue(new BigDouble(1.0) != new BigDouble(2.0));
        }

        // ── IEquatable / IComparable ──────────────────────────────────────────

        [Test]
        public void Equals_SameValue_ReturnsTrue()
        {
            Assert.IsTrue(new BigDouble(42.0).Equals(new BigDouble(42.0)));
        }

        [Test]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            Assert.IsFalse(new BigDouble(1.0).Equals(new BigDouble(2.0)));
        }

        [Test]
        public void Equals_WithTolerance_TreatsNearbyValuesAsEqual()
        {
            var a = new BigDouble(1.0);
            // 1e-15 is representable and well within the 1e-18 tolerance band
            // when measured as a relative difference at this exponent scale
            var b = new BigDouble(1.0 + 1e-15);
            Assert.IsTrue(a.Equals(b, 1e-14));
        }

        [Test]
        public void CompareTo_SmallerValue_ReturnsPositive()
        {
            Assert.Greater(new BigDouble(10.0).CompareTo(new BigDouble(5.0)), 0);
        }

        [Test]
        public void CompareTo_LargerValue_ReturnsNegative()
        {
            Assert.Less(new BigDouble(5.0).CompareTo(new BigDouble(10.0)), 0);
        }

        [Test]
        public void CompareTo_EqualValues_ReturnsZero()
        {
            Assert.AreEqual(0, new BigDouble(7.0).CompareTo(new BigDouble(7.0)));
        }

        [Test]
        public void GetHashCode_EqualValues_ReturnSameHash()
        {
            Assert.AreEqual(new BigDouble(99.0).GetHashCode(), new BigDouble(99.0).GetHashCode());
        }

        // ── IFormattable / ToString ───────────────────────────────────────────

        [Test]
        public void ToString_NoFormat_ReturnsNonEmptyString()
        {
            var str = new BigDouble(1234.0).ToString();
            Assert.IsNotNull(str);
            Assert.IsNotEmpty(str);
        }

        [Test]
        public void ToString_ExponentialFormat_ContainsE()
        {
            var str = new BigDouble(1.5, 10).ToString("E");
            StringAssert.Contains("e", str.ToLower());
        }

        // ── Implicit conversions ──────────────────────────────────────────────

        [Test]
        public void ImplicitConversion_FromDouble_ProducesEquivalentValue()
        {
            BigDouble value = 25.0;
            Assert.AreEqual(new BigDouble(25.0), value);
        }

        [Test]
        public void ImplicitConversion_FromInt_ProducesEquivalentValue()
        {
            BigDouble value = 10;
            Assert.AreEqual(new BigDouble(10.0), value);
        }

        // ── Static utilities ──────────────────────────────────────────────────

        [Test]
        public void Max_ReturnsLargerOfTwoValues()
        {
            var result = BigDouble.Max(new BigDouble(3.0), new BigDouble(7.0));
            Assert.AreEqual(new BigDouble(7.0), result);
        }

        [Test]
        public void Min_ReturnsSmallerOfTwoValues()
        {
            var result = BigDouble.Min(new BigDouble(3.0), new BigDouble(7.0));
            Assert.AreEqual(new BigDouble(3.0), result);
        }

        [Test]
        public void Abs_NegativeValue_ReturnsPositive()
        {
            var result = BigDouble.Abs(new BigDouble(-5.0));
            Assert.IsTrue(result > BigDouble.Zero);
        }

        [Test]
        public void Normalize_AlreadyNormalMantissa_IsUnchanged()
        {
            var result = BigDouble.Normalize(1.5, 3);
            Assert.AreEqual(1.5, result.Mantissa, 1e-12);
            Assert.AreEqual(3L,  result.Exponent);
        }
    }
}
