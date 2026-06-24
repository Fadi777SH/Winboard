using System;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using System.Windows.Media.TextFormatting;

namespace C_Panel.UserString
{
    public class BasicsInput
    {
        public string GetFocuedElement(AutomationElement element)
        {

            if (!element.TryGetCurrentPattern(TextPattern.Pattern, out object pattern)) {
                return string.Empty;
                
            }
            TextPattern textPattern = (TextPattern)pattern;
            TextPatternRange[] TheWholeLine = textPattern.GetSelection();
            if(TheWholeLine.Length == 0 || TheWholeLine == null)
            {
                return "";
            }
            TextPatternRange TakeTheFirstElement = TheWholeLine[0].Clone();
            TakeTheFirstElement.ExpandToEnclosingUnit(TextUnit.Word);
            return TakeTheFirstElement.GetText(-1).Trim();

            
        }

    }

}
