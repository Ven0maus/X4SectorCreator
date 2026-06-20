using X4SectorCreator.Helpers;
using Xunit;

namespace X4SectorCreator.Tests;

public sealed class ImportTranslationTextHelperTests
{
    [Fact]
    public void Normalize_StripsTranslationTokenAndKeepsInlineFallbackText()
    {
        string result = ImportTranslationTextHelper.Normalize("{20005, 1054}(Hades)");

        Assert.Equal("Hades", result);
    }

    [Theory]
    [InlineData("{20005, 6042}(Mines of Fortune)", "Mines of Fortune")]
    [InlineData("{20005, 3038}(Perpetual Sin)", "Perpetual Sin")]
    [InlineData("{20005, 3020}(Xaar's Belt)", "Xaar's Belt")]
    [InlineData("{20005,9024}(Farnham's Legend)", "Farnham's Legend")]
    [InlineData("{20202,901} {20001,201} {20401,4}(Kha'ak Sector Delta)", "Kha'ak Sector Delta")]
    [InlineData("(Mesurgu){99000,10010}", "Mesurgu")]
    [InlineData("(Xolarxi){99000,10020}", "Xolarxi")]
    public void Normalize_UsesRealModExamples(string input, string expected)
    {
        string result = ImportTranslationTextHelper.Normalize(input);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("(Taranis)(Mk1){29101,4201} {20111,101}", "Taranis Mk1")]
    [InlineData("(Prometheus) {20101,32601}", "Prometheus")]
    [InlineData("(S)(R)(2){20101,70401}{20101,71201}{20404,2}", "S R 2")]
    public void Normalize_FlattensParentheticalFallbackGroupsFromRorExamples(string input, string expected)
    {
        string result = ImportTranslationTextHelper.Normalize(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Normalize_CollapsesDuplicateNameRenderedTwice()
    {
        string result = ImportTranslationTextHelper.Normalize("(Mesurgu)Mesurgu");

        Assert.Equal("Mesurgu", result);
    }

    [Fact]
    public void Normalize_CollapsesSuffixDuplicateNameRenderedTwice()
    {
        string result = ImportTranslationTextHelper.Normalize("Hades(Hades)");

        Assert.Equal("Hades", result);
    }

    [Fact]
    public void Normalize_CollapsesExpandedPhraseRenderedTwice()
    {
        string result = ImportTranslationTextHelper.Normalize("Farnham's Legend Farnham's Legend");

        Assert.Equal("Farnham's Legend", result);
    }

    [Fact]
    public void Normalize_CollapsesExpandedFarnhamSectorPhraseRenderedTwice()
    {
        string result = ImportTranslationTextHelper.Normalize("Farnham's Legend V Farnham's Legend V");

        Assert.Equal("Farnham's Legend V", result);
    }

    [Theory]
    [InlineData(".")]
    [InlineData("-")]
    [InlineData("()")]
    public void Normalize_ReturnsNullForPunctuationOnlyFallback(string value)
    {
        string result = ImportTranslationTextHelper.Normalize(value);

        Assert.Null(result);
    }
}
