using System.CommandLine;
using System.CommandLine.Help;

namespace Messages.CLI
{
    public class DefaultHelpBuilder : HelpBuilder
    {
        public DefaultHelpBuilder() 
            : base(LocalizationResources.Instance, int.MaxValue)
        {
        }

        public override void Write(HelpContext context)
        {
            // TODO PRJ: Probably flesh this out.
            base.Write(context);
        }
    }
}
