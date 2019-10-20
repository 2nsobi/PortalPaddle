<<<<<<< HEAD
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Unity.Appodeal.Xcode.PBX {
    enum TokenType {
=======
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System;

namespace Unity.Appodeal.Xcode.PBX
{
    enum TokenType
    {
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        EOF,
        Invalid,
        String,
        QuotedString,
        Comment,
<<<<<<< HEAD

        Semicolon, // ;
        Comma, // ,
        Eq, // =
        LParen, // (
        RParen, // )
        LBrace, // {
        RBrace, // }      
    }

    class Token {
        public TokenType type;

        // the line of the input stream the token starts in (0-based)
        public int line;

        // start and past-the-end positions of the token in the input stream
        public int begin, end;
    }

    class TokenList : List<Token> { }

    class Lexer {
=======
        
        Semicolon,  // ;
        Comma,      // ,
        Eq,         // =
        LParen,     // (
        RParen,     // )
        LBrace,     // {
        RBrace,     // }      
    }
    
    class Token
    {
        public TokenType type;
        
        // the line of the input stream the token starts in (0-based)
        public int line;
        
        // start and past-the-end positions of the token in the input stream
        public int begin, end;
    }
    
    class TokenList : List<Token>
    {
    }
    
    class Lexer
    {
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
        string text;
        int pos;
        int length;
        int line;

<<<<<<< HEAD
        public static TokenList Tokenize (string text) {
            var lexer = new Lexer ();
            lexer.SetText (text);
            return lexer.ScanAll ();
        }

        public void SetText (string text) {
=======
        public static TokenList Tokenize(string text)
        {
            var lexer = new Lexer();
            lexer.SetText(text);
            return lexer.ScanAll();
        }
        
        public void SetText(string text)
        {
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            this.text = text + "    "; // to prevent out-of-bounds access during look ahead
            pos = 0;
            length = text.Length;
            line = 0;
        }
<<<<<<< HEAD

        public TokenList ScanAll () {
            var tokens = new TokenList ();

            while (true) {
                var tok = new Token ();
                ScanOne (tok);
                tokens.Add (tok);
=======
        
        public TokenList ScanAll()
        {
            var tokens = new TokenList();
            
            while (true)
            {
                var tok = new Token();
                ScanOne(tok);
                tokens.Add(tok);
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                if (tok.type == TokenType.EOF)
                    break;
            }
            return tokens;
        }
<<<<<<< HEAD

        void UpdateNewlineStats (char ch) {
            if (ch == '\n')
                line++;
        }

        // tokens list is modified in the case when we add BrokenLine token and need to remove already
        // added tokens for the current line
        void ScanOne (Token tok) {
            while (true) {
                while (pos < length && Char.IsWhiteSpace (text[pos])) {
                    UpdateNewlineStats (text[pos]);
                    pos++;
                }

                if (pos >= length) {
                    tok.type = TokenType.EOF;
                    break;
                }

                char ch = text[pos];
                char ch2 = text[pos + 1];

                if (ch == '\"')
                    ScanQuotedString (tok);
                else if (ch == '/' && ch2 == '*')
                    ScanMultilineComment (tok);
                else if (ch == '/' && ch2 == '/')
                    ScanComment (tok);
                else if (IsOperator (ch))
                    ScanOperator (tok);
                else
                    ScanString (tok); // be more robust and accept whatever is left
                return;
            }
        }

        void ScanString (Token tok) {
            tok.type = TokenType.String;
            tok.begin = pos;
            while (pos < length) {
                char ch = text[pos];
                char ch2 = text[pos + 1];

                if (Char.IsWhiteSpace (ch))
=======
        
        void UpdateNewlineStats(char ch)
        {
            if (ch == '\n')
                line++;
        }
        
        // tokens list is modified in the case when we add BrokenLine token and need to remove already
        // added tokens for the current line
        void ScanOne(Token tok)
        {
            while (true)
            {
                while (pos < length && Char.IsWhiteSpace(text[pos]))
                {
                    UpdateNewlineStats(text[pos]);
                    pos++;
                }
                
                if (pos >= length)
                {
                    tok.type = TokenType.EOF;
                    break;
                }
                
                char ch = text[pos];
                char ch2 = text[pos+1];
                
                if (ch == '\"')
                    ScanQuotedString(tok);
                else if (ch == '/' && ch2 == '*')
                    ScanMultilineComment(tok);
                else if (ch == '/' && ch2 == '/')
                    ScanComment(tok);
                else if (IsOperator(ch))
                    ScanOperator(tok);
                else
                    ScanString(tok); // be more robust and accept whatever is left
                return;
            }    
        }
        
        void ScanString(Token tok)
        {
            tok.type = TokenType.String;
            tok.begin = pos;
            while (pos < length)
            {
                char ch = text[pos];
                char ch2 = text[pos+1];
                
                if (Char.IsWhiteSpace(ch))
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                    break;
                else if (ch == '\"')
                    break;
                else if (ch == '/' && ch2 == '*')
                    break;
                else if (ch == '/' && ch2 == '/')
                    break;
<<<<<<< HEAD
                else if (IsOperator (ch))
=======
                else if (IsOperator(ch))
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                    break;
                pos++;
            }
            tok.end = pos;
            tok.line = line;
        }
<<<<<<< HEAD

        void ScanQuotedString (Token tok) {
            tok.type = TokenType.QuotedString;
            tok.begin = pos;
            pos++;

            while (pos < length) {
                // ignore escaped quotes
                if (text[pos] == '\\' && text[pos + 1] == '\"') {
                    pos += 2;
                    continue;
                }

                // note that we close unclosed quotes
                if (text[pos] == '\"')
                    break;

                UpdateNewlineStats (text[pos]);
=======
        
        void ScanQuotedString(Token tok)
        {
            tok.type = TokenType.QuotedString;
            tok.begin = pos;
            pos++;
            
            while (pos < length)
            {
                // ignore escaped quotes
                if (text[pos] == '\\' && text[pos+1] == '\"')
                {
                    pos += 2;
                    continue;
                }
            
                // note that we close unclosed quotes
                if (text[pos] == '\"')
                    break;
                
                UpdateNewlineStats(text[pos]);
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                pos++;
            }
            pos++;
            tok.end = pos;
            tok.line = line;
        }

<<<<<<< HEAD
        void ScanMultilineComment (Token tok) {
            tok.type = TokenType.Comment;
            tok.begin = pos;
            pos += 2;

            while (pos < length) {
                if (text[pos] == '*' && text[pos + 1] == '/')
                    break;

                // we support multiline comments
                UpdateNewlineStats (text[pos]);
=======
        void ScanMultilineComment(Token tok)
        {
            tok.type = TokenType.Comment;
            tok.begin = pos;
            pos += 2;
            
            while (pos < length)
            {
                if (text[pos] == '*' && text[pos+1] == '/')
                    break;
                
                // we support multiline comments
                UpdateNewlineStats(text[pos]);
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                pos++;
            }
            pos += 2;
            tok.end = pos;
            tok.line = line;
        }

<<<<<<< HEAD
        void ScanComment (Token tok) {
=======
        void ScanComment(Token tok)
        {
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            tok.type = TokenType.Comment;
            tok.begin = pos;
            pos += 2;

<<<<<<< HEAD
            while (pos < length) {
=======
            while (pos < length)
            {
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                if (text[pos] == '\n')
                    break;
                pos++;
            }
<<<<<<< HEAD
            UpdateNewlineStats (text[pos]);
=======
            UpdateNewlineStats(text[pos]);
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            pos++;
            tok.end = pos;
            tok.line = line;
        }
<<<<<<< HEAD

        bool IsOperator (char ch) {
=======
        
        bool IsOperator(char ch)
        {
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            if (ch == ';' || ch == ',' || ch == '=' || ch == '(' || ch == ')' || ch == '{' || ch == '}')
                return true;
            return false;
        }

<<<<<<< HEAD
        void ScanOperator (Token tok) {
            switch (text[pos]) {
                case ';':
                    ScanOperatorSpecific (tok, TokenType.Semicolon);
                    return;
                case ',':
                    ScanOperatorSpecific (tok, TokenType.Comma);
                    return;
                case '=':
                    ScanOperatorSpecific (tok, TokenType.Eq);
                    return;
                case '(':
                    ScanOperatorSpecific (tok, TokenType.LParen);
                    return;
                case ')':
                    ScanOperatorSpecific (tok, TokenType.RParen);
                    return;
                case '{':
                    ScanOperatorSpecific (tok, TokenType.LBrace);
                    return;
                case '}':
                    ScanOperatorSpecific (tok, TokenType.RBrace);
                    return;
                default:
                    return;
            }
        }

        void ScanOperatorSpecific (Token tok, TokenType type) {
=======
        void ScanOperator(Token tok)
        {
            switch (text[pos])
            {
                case ';': ScanOperatorSpecific(tok, TokenType.Semicolon); return;
                case ',': ScanOperatorSpecific(tok, TokenType.Comma); return;
                case '=': ScanOperatorSpecific(tok, TokenType.Eq); return;
                case '(': ScanOperatorSpecific(tok, TokenType.LParen); return;
                case ')': ScanOperatorSpecific(tok, TokenType.RParen); return;
                case '{': ScanOperatorSpecific(tok, TokenType.LBrace); return;
                case '}': ScanOperatorSpecific(tok, TokenType.RBrace); return;
                default: return;
            }
        }
        
        void ScanOperatorSpecific(Token tok, TokenType type)
        {
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            tok.type = type;
            tok.begin = pos;
            pos++;
            tok.end = pos;
            tok.line = line;
        }
    }
<<<<<<< HEAD
}
=======
    

} // namespace UnityEditor.iOS.Xcode
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
