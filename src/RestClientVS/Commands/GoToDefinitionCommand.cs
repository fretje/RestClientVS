﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;
using RestClient;
using RestClientVS.Parsing;

namespace RestClientVS.Commands
{
    [Export(typeof(ICommandHandler))]
    [Name(nameof(CommentCommand))]
    [ContentType(RestLanguage.LanguageName)]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    public class GoToDefinitionCommand : ICommandHandler<GoToDefinitionCommandArgs>
    {
        public string DisplayName => nameof(GoToDefinitionCommand);

        public bool ExecuteCommand(GoToDefinitionCommandArgs args, CommandExecutionContext executionContext)
        {
            var position = args.TextView.Caret.Position.BufferPosition.Position;

            Document document = args.TextView.TextBuffer.GetDocument();
            Token token = document.GetTokenFromPosition(position);
            IEnumerable<Variable> variables = document.Tokens.OfType<Variable>();

            if (token is RestClient.Reference reference)
            {
                Variable definition = variables.FirstOrDefault(v => v.Name.Text.Equals(reference.Value.Text, StringComparison.OrdinalIgnoreCase));

                if (definition != null)
                {
                    args.TextView.Caret.MoveTo(new SnapshotPoint(args.TextView.TextBuffer.CurrentSnapshot, definition.Start));
                }

                return true;
            }

            return false;
        }

        public CommandState GetCommandState(GoToDefinitionCommandArgs args)
        {
            return CommandState.Available;
        }
    }
}