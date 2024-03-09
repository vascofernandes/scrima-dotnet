using System.Collections.Generic;
using Scrima.Core;
using Scrima.Core.Model;
using Scrima.Core.Query.Expressions;

namespace Scrima.OData.Parsers;

internal static class SelectExpressionParser
{
    internal static (QueryNode selectExpression, bool isStartSelect) Parse(string selectValue, EdmComplexType model, EdmTypeProvider typeMap)
    {
        var parserImpl = new SelectExpressionParserImpl(model, typeMap);
        var queryNode = parserImpl.Parse(new SelectExpressionLexer(selectValue));

        return queryNode;
    }
    
    internal class SelectExpressionParserImpl
    {
        private readonly EdmComplexType _model;
        private readonly EdmTypeProvider _typeProvider;
        private readonly Queue<Token> _tokens = new();

        internal SelectExpressionParserImpl(EdmComplexType model, EdmTypeProvider typeProvider)
        {
            _model = model;
            _typeProvider = typeProvider;
        }

        internal (QueryNode selectExpression, bool isStartSelect) Parse(SelectExpressionLexer selectExpressionLexer)
        {
            while (selectExpressionLexer.MoveNext())
            {
                var token = selectExpressionLexer.Current;

                switch (token.TokenType)
                {
                    default:
                        _tokens.Enqueue(token);
                        break;
                }
            }

            if (_tokens.Count == 0)
            {
                throw new ODataParseException(Messages.UnableToParseSelect);
            }
            
            var properties = new List<EdmProperty>();

            switch (_tokens.Peek().TokenType)
            {
                case TokenType.Star:
                    var allPropertyAccessNode = new PropertyAccessNode(_model.Properties);
                    return (allPropertyAccessNode, true);
                case TokenType.PropertyName:
                    while (_tokens.Count > 0)
                    {
                        var token = _tokens.Dequeue();

                        switch (token.TokenType)
                        {
                            case TokenType.PropertyName:
                                
                                var props = PropertyParseHelper.ParseNestedProperties(token.Value, _model);
                                properties.AddRange(props);
                                break;
                        }
                    }
                    break;
                default:
                    throw new ODataParseException(_tokens.Peek().TokenType.ToString());
            }

            var propertyAccessNode = new PropertyAccessNode(properties);
            return (propertyAccessNode, false);
        }
    }
}
