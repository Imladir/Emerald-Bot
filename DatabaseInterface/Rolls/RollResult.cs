using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Model.Rolls
{
    public class RollResult
    {
        private int _explosive;
        private int _success;
        private int _opportunity;
        private int _strife;

        public RollResult()
        {
            _explosive = 0;
            _success = 0;
            _opportunity = 0;
            _strife = 0;
        }

        public RollResult(DieFace Die)
        {
            _explosive = Die.ExplosiveSuccess() ? 1 : 0;
            _success = Die.Success() ? 1 : 0;
            _opportunity = Die.Opportunity() ? 1 : 0;
            _strife = Die.Strife() ? 1 : 0;
        }

        public int CountSuccess()
        {
            return _explosive + _success;
        }

        public static RollResult operator +(RollResult r1, RollResult r2)
        {
            RollResult res = new RollResult
            {
                _explosive = r1._explosive + r2._explosive,
                _success = r1._success + r2._success,
                _opportunity = r1._opportunity + r2._opportunity,
                _strife = r1._strife + r2._strife
            };
            return res;
        }

        public static RollResult operator -(RollResult r1, RollResult r2)
        {
            RollResult res = new RollResult
            {
                _explosive = r1._explosive - r2._explosive,
                _success = r1._success - r2._success,
                _opportunity = r1._opportunity - r2._opportunity,
                _strife = r1._strife - r2._strife
            };
            return res;
        }

        public string GetString(bool merge = false)
        {
            string res = "";
            if (!merge)
            {
                if (_explosive > 0) res += "{explosive}" + _explosive;
                if (_success > 0) res += "{success}" + _success;
            }
            else
            {
                if (_explosive + _success > 0) res += "{success}" + (_explosive + _success);
            }
            if (_opportunity > 0) res += "{opportunity}" + _opportunity;
            if (_strife > 0) res += "{strife}" + _strife;
            if (res == "") res = "Nothing...";

            return res;
        }
    }
}
