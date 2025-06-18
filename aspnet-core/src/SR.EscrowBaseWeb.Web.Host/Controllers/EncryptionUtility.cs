using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace SR.EscrowWeb.Enterprise.utility
{
    public static class EncryptionUtility
    {
        public const int MIN_ASC = 32;
        public const int MAX_ASC = 126;
        public const int NO_OF_CHARS = MAX_ASC - MIN_ASC + 1;

        private static int MoveAsc(int A, int mLvl)
        {
            mLvl = mLvl % NO_OF_CHARS;
            A = A + mLvl;
            if (A < MIN_ASC)
            {
                A = A + NO_OF_CHARS;
            }
            else if(A > MAX_ASC) 
            {
                 
                A = A - NO_OF_CHARS;
            }

            return A;
        }

        public static string Mid(string s, int a, int b)

        {

            string temp = s.Substring(a - 1, b);

            return temp;

        }

        public static string encrypt(string s, string key)
        {
            int P;
            int keypos = 0;
            int C;
            string E = "";
            int K;
            int chkSum = 0;

            if (key == "")
            {
                return s;
            }

            for (P =1; P <= s.Length; P++)
            {
                if (Strings.Asc(Strings.Mid(s, P, 1)) < MIN_ASC || Strings.Asc(Strings.Mid(s, P, 1)) > MAX_ASC)
                {
                    return null; throw new Exception("Char at position " + P + " is invalid!");

                }
            }

            for (keypos = 1; keypos <= key.Length; keypos++)
            {
                chkSum = chkSum + Strings.Asc(Strings.Mid(key, keypos, 1)) * keypos;
            }

            keypos = 0;

            for (P = 1; P <= s.Length; P++)
            {
                C = Strings.Asc(Strings.Mid(s, P, 1));
                keypos = keypos + 1;

                if (keypos > key.Length)
                {
                    keypos = 1;
                }

                K = Strings.Asc(Strings.Mid(key, keypos, 1));
                C = MoveAsc(C, K);
                C = MoveAsc(C, K * key.Length);
                C = MoveAsc(C, chkSum * K);
                C = MoveAsc(C, P * K);
                C = MoveAsc(C, s.Length * P);
                E = E + Strings.Chr(C);
            }

            return E;
        }

        public static string decrypt(string s, string key)
        {
            int P;
            int keypos = 0;
            int C;
            string D = "";
            int K;
            int chkSum = 0;

            if (key == "")
            {
                return s;
            }

            for (keypos = 1; keypos <= key.Length; keypos++)
            {
                chkSum = chkSum + Strings.Asc(Strings.Mid(key, keypos, 1)) * keypos;
            }

            keypos = 0;

            for (P = 1; P <= s.Length; P++)
            {
                C = Strings.Asc(Strings.Mid(s, P, 1));
                keypos = keypos + 1;

                if (keypos > key.Length) {
                    keypos = 1;
                }

                K = Strings.Asc(Strings.Mid(key, keypos, 1));
                C = MoveAsc(C, -(s.Length * P));
                C = MoveAsc(C, -(P * K));
                C = MoveAsc(C, -(chkSum * K));
                C = MoveAsc(C, -(K * key.Length));
                C = MoveAsc(C, -K);
                D = D + Strings.Chr(C);
            }

            return D;
        }
    }
}
