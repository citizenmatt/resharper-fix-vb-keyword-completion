using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Feature.Services.VB.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.VB.CodeCompletion.LookupItems;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.VB;
using JetBrains.ReSharper.Psi.VB.Parsing;

namespace CitizenMatt.ReSharper.VBKeywordCompletionFix
{
    [Language(typeof(VBLanguage))]
    public class ThenKeywordItemsProvider : ItemsProviderOfSpecificContext<VBCodeCompletionContextSimple>
    {
        protected override bool IsAvailable(VBCodeCompletionContextSimple context)
        {
            var basicContext = context.BasicContext;
            var codeCompletionType = basicContext.CodeCompletionType;
            if (codeCompletionType == CodeCompletionType.AutomaticCompletion)
                return true;
            if (codeCompletionType == CodeCompletionType.BasicCompletion)
                return basicContext.KeywordCompletionEnabled;
            return false;
        }

        protected override LookupFocusBehaviour GetLookupFocusBehaviour(VBCodeCompletionContextSimple context)
        {
            return LookupFocusBehaviour.Hard;
        }

        protected override bool AddLookupItems(VBCodeCompletionContextSimple context, GroupedItemsCollector collector)
        {
            var file = context.BasicContext.File;
            var caretDocumentRange = context.BasicContext.CaretDocumentRange;
            var token = file.FindNodeAt(caretDocumentRange) as ITokenNode;
            if (token == null || token.GetTokenType() == VBTokenType.THEN_KEYWORD)
                return false;

            var prevToken = VBTokenUtil.GetPrevMeaningfulToken(token);
            if (prevToken.IsIdentifier())
            {
                prevToken = VBTokenUtil.GetPrevMeaningfulToken(prevToken);
                if (prevToken.GetTokenType() == VBTokenType.ELSEIF_KEYWORD)
                {
                    var tokenRange = token.GetDocumentRange().TextRange;
                    var ranges = new TextLookupRanges(tokenRange, false, caretDocumentRange.TextRange);
                    collector.AddRanges(ranges);

                    var lookupItem = new VBKeywordLookupItem(VBTokenType.THEN_KEYWORD.TokenRepresentation)
                    {
                        VisualReplaceRangeMarker = ranges.ReplaceRange.CreateRangeMarker(context.BasicContext.Document)
                    };
                    lookupItem.InitializeRanges(ranges, context.BasicContext);
                    collector.AddAtDefaultPlace(lookupItem);
                    return true;
                }
            }
            return false;
        }
    }
}