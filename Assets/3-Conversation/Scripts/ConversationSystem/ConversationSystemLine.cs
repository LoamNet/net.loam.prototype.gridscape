using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ConversationSystem
{
    public class ConversationSystemLine
    {
        // Public
        public string text;

        // Internal
        private string _jump;
        private int _lineNumber;
        private ConversationSystem _system;

        public bool IsValid { get; private set; }

        public ConversationSystemLine(ConversationSystem system, string text, string jumpTarget, int currentLine)
        {
            this.text = text;

            this._system = system;
            this._jump = jumpTarget;
            this._lineNumber = currentLine;

            this.IsValid = true;
        }

        /// <summary>
        /// Using any jump info and existing line number info, try and step forward to the next line.
        /// </summary>
        public void Next()
        {
            if (!IsValid)
            {
                throw new System.Exception("Actionable Line has been invalidated! Cannot go to 'next'!");
            }

            int nextLine = _system.DetermineNext(_lineNumber, _jump);
            _system.LoadLine(nextLine);
        }

        /// <summary>
        /// Changes the valid flag, which provides some guards to situations 
        /// where this object is being held on to for too long.
        /// </summary>
        public void Invalidate()
        {
            IsValid = false;
        }
    }
}

