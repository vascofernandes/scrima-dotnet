using System;

namespace Scrima.OData.Parsers;

internal class SelectExpressionLexer
{
    private static readonly TokenDefinition[] TokenDefinitions = new[]
    {
        new TokenDefinition(TokenType.Star,                @"*(?=\s?)"),
        new TokenDefinition(TokenType.Comma,                @",(?=\s?)"),
        new TokenDefinition(TokenType.PropertyName,         @"[\w\/]+"),
        new TokenDefinition(TokenType.String,               @"'(?:''|[\w\s-.~!$&()*+,;=@%\\\/]*)*'"),
        new TokenDefinition(TokenType.Whitespace,           @"\s", ignore: true),
    };

    private readonly string _content;
    private int _position;

    internal SelectExpressionLexer(string content)
    {
        _content = content;
        Current = default(Token);

        _position = _content.StartsWith("$select=", StringComparison.Ordinal)
            ? content.IndexOf('=') + 1
            : 0;
    }

    internal Token Current { get; private set; }

    internal bool MoveNext()
    {
        if (_content.Length == _position)
        {
            return false;
        }

        for (int i = 0; i < TokenDefinitions.Length; i++)
        {
            var tokenDefinition = TokenDefinitions[i];

            var match = tokenDefinition.Regex.Match(_content, _position);

            if (match.Success)
            {
                if (tokenDefinition.Ignore)
                {
                    _position += match.Length;
                    i = -1;
                    continue;
                }

                Current = tokenDefinition.CreateToken(match);
                _position += match.Length;

                return true;
            }
        }

        if (_content.Length != _position)
        {
            throw new ODataParseException(Messages.UnableToParseSelect);
        }

        return false;
    }
}
