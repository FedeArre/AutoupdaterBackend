using System;
using System.Collections.Generic;
using System.Text;

namespace Objects
{
    public class TokenHandler
    {
        private static TokenHandler Instance;
        private List<TokenObject> Tokens;

        private TokenHandler()
        {
            Tokens = new List<TokenObject>();
        }

        public static TokenHandler GetInstance()
        {
            if(Instance == null)
                Instance = new TokenHandler();
            return Instance;
        }

        public string CreateTokenForUser(string Username)
        {
            foreach (TokenObject tokenObject in Tokens)
            {
                if (tokenObject.Username == Username)
                    return null;
            }

            TokenObject token = new TokenObject();
            token.Username = Username;
            token.Token = new System.Guid().ToString();
            token.ExpirationTime = DateTimeOffset.Now.ToUnixTimeSeconds() + (60 * 60 * 3); // 3 hour
            Tokens.Add(token);

            return token.Token;
        }

        public string IsUserLogged(string token)
        {
            foreach(TokenObject tokenObject in Tokens)
            {
                if (tokenObject.Token == token)
                {
                    if(tokenObject.ExpirationTime < DateTimeOffset.Now.ToUnixTimeSeconds())
                    {
                        Tokens.Remove(tokenObject);
                        return null;
                    }
                    else
                    {
                        RenewToken(tokenObject);
                        return tokenObject.Username;
                    }
                }
            }

            return null;
        }

        public bool RenewToken(TokenObject token)
        {
            if (token == null)
                return false;

            token.ExpirationTime = DateTimeOffset.Now.ToUnixTimeSeconds() + (60 * 60 * 3); // 3 hour
            return true;
        }
    }

    public class TokenObject
    {
        public string Username;
        public string Token;
        public long ExpirationTime;
    }
}
