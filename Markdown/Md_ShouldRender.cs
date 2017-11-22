using FluentAssertions;
using NUnit.Framework;

namespace Markdown
{
    [TestFixture]
    public class Md_ShouldRender
    {
        private Md md;

        [SetUp]
        public void SetUp()
        {
            md = new Md();
            md.AddLexemConsumer(new EscapeConsumer('\\'));
        }

        [Timeout(1000)]
        [TestCase(@"no underscores", @"no underscores", TestName = "LeaveUntouched_TextWithoutUnderscores")]
        [TestCase(@"_text_", @"<em>text</em>", TestName = "SingleUnderscores_ShouldBeReplacedWithTagEm")]
        [TestCase(@"__text__", @"<strong>text</strong>", TestName =
            "DoubleUnderscores_ShouldBeReplacedWithTagStrong")]
        [TestCase(@"__*text*__", @"<strong><em>text</em></strong>", TestName =
            "Nesting_ShouldBeReplacedWithNestedTags")]
        [TestCase(@"____интся____", @"____интся____", TestName = "UnknownSequenceOfUnderscores_ShouldBeText")]
        [TestCase(@"__text", @"__text", TestName = "NotPairedTag_ShouldBeText")]
        [TestCase(@"*__text*", @"<em>__text</em>", TestName = "NotPairedTagInsidePair_ShouldBeText")]
        [TestCase(@"\_text_", @"_text_", TestName = "EscapeChar_ShouldPreventFormatingTagWithPair")]
        [TestCase(@"\_text\_", @"_text_", TestName = "EscapeChar_ShouldPreventFormatingBothTagsInPair")]
        [TestCase(@"_*text*_", @"<em>*text*</em>", TestName = "Tag_ShouldNotWorkInsideSelfOrAlias")]
        [TestCase(@"__н _с ием.", @"__н _с ием.", TestName = "NotPairableTags_ShouldBeText")]
        [TestCase(@"и_ подчерки_ ", @"и_ подчерки_ ", TestName =
            "OpeningTagsFolowedByWhiteSpace_ShouldBeText")]
        [TestCase(@"_подчерки _н", @"_подчерки _н", TestName = "ClosingTagsLeadByWhiteSpace_ShouldBeText")]
        [TestCase(@"_две __пары_ разных__", @"<em>две __пары</em> разных__", TestName =
            "IfTwoTagsInterceptEachOther_ShouldWorkLeadingOne")]
        [TestCase(@"цифрами_12_3", @"цифрами_12_3", TestName =
            "Undersores_ShouldBeTextInsideWordsWithDigits")]
        public void CheckMdOn(string subject, string expected)
        {
            md.RenderToHtml(subject).Should().Be(expected);
        }
    }
}